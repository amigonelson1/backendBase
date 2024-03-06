using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Business.DTOs;
using Business.Interfaces;
using DataAccess;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace Business.Services
{
  public class TokenService : ITokenService
  {
    private readonly IConfiguration _configuration;
    private readonly UserManager<User> _userManager;

    public TokenService(IConfiguration configuration, UserManager<User> userManager)
    {
        _configuration = configuration;
        _userManager = userManager;
    }
    public async Task<AuthenticationResponseDTO> TokenBuilder (UserCredentialsDTO model)
    {
      var user = await _userManager.FindByNameAsync(model.UserName) ?? throw new ArgumentException("The user is not register in the app");
      var rol = await _userManager.GetRolesAsync(user);
      var claims = new List<Claim>()
      {
          new("Id", user.Id.ToString()),
          new("UserName", user.UserName!.ToString()),
          new(ClaimTypes.Email, user.Email!.ToString()),
          new(ClaimTypes.Role, rol.Count > 0 ? rol.First().ToString() : ""),
      };

      var claimsDB = await _userManager.GetClaimsAsync(user);
      claims.AddRange(claimsDB);

      var secret = _configuration["JWT_Secret1"] ?? throw new ArgumentException("Secret hash is null");
      var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
      var expired = DateTime.UtcNow.AddHours(5);
      var securityToken = new JwtSecurityToken(issuer: null, audience: null, claims: claims, expires: expired, signingCredentials: creds);

      return new AuthenticationResponseDTO()
      {
          Token = new JwtSecurityTokenHandler().WriteToken(securityToken),
          Expire = expired
      };
    }
  }
}