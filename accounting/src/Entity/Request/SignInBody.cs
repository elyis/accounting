using System.ComponentModel.DataAnnotations;


namespace accounting.src.Entity.Request
{
    public class SignInBody
    {
        [EmailAddress]
        public string Email { get; set; }

        public string Password { get; set; }
    }
}
