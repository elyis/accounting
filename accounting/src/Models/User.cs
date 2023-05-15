using accounting.src.Entity.Response;
using accounting.src.Utility;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace accounting.src.Models
{
    [Index("Email", IsUnique = true)]
    public class User
    {
        private string email;
        private string password;


        public Guid Id { get; set; }
        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Patronymic { get; set; }

        public string Phone { get; set; }

        public string Email
        {
            get => Aes256Provider.Decrypt(email);
            set => email = Aes256Provider.Encrypt(value);
        }

        public string Password
        {
            get => password;
            set => password = Hmac512Provider.Compute(value);
        }

        public string? Token { get; set; }

        public string? Image { get; set; } = null;
        public string Role { get; set; }
        public DateTime? RecoveryCodeValidBefore { get; set; }
        public string? RecoveryCode { get; set; }
        public bool WasPasswordResetRequest { get; set; } = false;

        public List<MaterialAccounting> MaterialAccountings { get; set; } = new();
        public List<ProductAccounting> ProductAccountings { get; set; } = new();

        public UserBody ToUserBody()
        {
            UserBody body = new UserBody
            {
                FirstName = FirstName,
                LastName = LastName,
                Patronymic = Patronymic,
                Phone = Phone,
                Email = Email,
                Role = (UserRole)Enum.Parse(typeof(UserRole), Role),
                ImageUrl = Image == null ? null : $"{Constants.webPathToProfileIcons}{Image}"
            };

            return body;
        }
    }

    [JsonConverter(typeof(StringEnumConverter))]
    public enum UserRole
    {
        None,
        Seller,
        Manager,
        Administrator
    }
}
