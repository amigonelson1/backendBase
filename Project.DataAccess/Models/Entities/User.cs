using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DataAccess
{
    public class User : IdentityUser
    {
        [NotMapped]
        public string UserNameEmail { get => $"{UserName}-{Email}";}
    }
}