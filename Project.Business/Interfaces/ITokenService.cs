using Business.DTOs;

namespace Business.Interfaces
{
    public interface ITokenService
    {
       Task<AuthenticationResponseDTO> TokenBuilder (UserCredentialsDTO model);
    }
}