using System.ComponentModel.DataAnnotations;

namespace accounting.src.Entity.Request
{
    public class SignUpBody
    {

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Patronymic { get; set; }

        [Phone]
        public string Phone { get; set; }

        [EmailAddress]
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
