using accounting.src.Core.IRepository;
using accounting.src.Data;
using accounting.src.Entity;
using accounting.src.Entity.Request;
using accounting.src.Entity.Response;
using accounting.src.Models;
using accounting.src.Utility;
using Microsoft.EntityFrameworkCore;

namespace accounting.src.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AddAsync(SignUpBody body)
        {
            var user = await GetAsync(body.Email);
            if(user == null)
            {
                var newUser = new User
                {
                    FirstName = body.FirstName,
                    LastName = body.LastName,
                    Patronymic = body.Patronymic,
                    Phone = body.Phone,
                    Email = body.Email,
                    Role = Enum.GetName(typeof(UserRole), UserRole.None)!,
                    Password = body.Password,
                };

                var resultAdded = await _context.Users.AddAsync(newUser);
                _context.SaveChanges();

                return resultAdded.Entity;
            }
            return null;
        }

        public async Task<string?> GenerateRecoveryCode(string email, TimeSpan? interval = null)
        {
            interval ??= TimeSpan.FromMinutes(10.0);
            var user = await GetAsync(email);
            if (user == null)
                return null;

            user.RecoveryCode = RecoveryCodeGenerator.Generate();
            user.RecoveryCodeValidBefore = DateTime.UtcNow.Add(interval.Value);
            user.WasPasswordResetRequest = true;
            _context.SaveChanges();

            return user.RecoveryCode;
        }

        public Task<User?> GetAsync(Guid id)
            => _context.Users.FirstOrDefaultAsync(u => u.Id == id);

        public Task<User?> GetAsync(string email)
        {
            var encryptedEmail = Aes256Provider.Encrypt(email);
            return _context.Users.FirstOrDefaultAsync(u => u.Email == encryptedEmail);
        }

        public async Task<User?> GetByToken(string token)
        {
            //var tokenHash = Hmac512Provider.Compute(token);
            return await _context.Users.FirstOrDefaultAsync(u => u.Token.Equals(token));
        }

        public List<UserBody> GetUsers(Guid id)
            => _context.Users
            .Where(user => user.Id != id)
            .Select(user => user.ToUserBody()).ToList();

        public async Task<TokenPair> ResetPassword(User user, string newPassword)
        {
            user.Password = Hmac512Provider.Compute(newPassword);
            user.WasPasswordResetRequest = false;
            user.RecoveryCodeValidBefore = null;

            await _context.SaveChangesAsync();
            return JwtManager.GenerateTokenPair(user.Id, user.Role);
        }

        public async Task<User?> UpdateAsync(Guid id, UpdateUserBody body)
        {
            var user = await GetAsync(id);
            if (user == null)
                return null;

            user.FirstName = body.Firstname;
            user.LastName = body.Lastname;
            user.Patronymic = body.Patronymic;
            user.Phone = body.Phone;
            _context.SaveChanges();
            return user;
        }

        public async Task<bool> UpdateImage(Guid id, string filename)
        {
            var user = await GetAsync(id);
            if (user == null)
                return false;

            user.Image = filename;
            _context.SaveChanges();
            return true;
        }

        public async Task<User?> UpdateRole(string email, UserRole role)
        {
            var user = await GetAsync(email);
            if(user == null) 
                return null;

            user.Role = Enum.GetName(typeof(UserRole), role)!;
            _context.SaveChanges();
            return user;
        }

        public async Task UpdateToken(Guid id, string token)
        {
            var user = await GetAsync(id);
            if(user != null)
            {
                var hashToken = token;
                user.Token = hashToken;
                _context.SaveChanges();
            }
        }
    }
}
