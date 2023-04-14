using accounting.src.Models;
using System.ComponentModel.DataAnnotations;

namespace accounting.src.Entity.Request
{
    public class RoleChangeBody
    {
        [EmailAddress]
        public string Email { get; set; }
        public UserRole NewRoleLevel { get; set; }
    }
}
