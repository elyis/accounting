using accounting.src.Models;

namespace accounting.src.Entity.Response
{
    public class UserBody
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string? ImageUrl { get; set; }
        public UserRole Role { get; set; }
    }
}
