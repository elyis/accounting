using accounting.src.Entity;
using accounting.src.Entity.Request;
using accounting.src.Entity.Response;
using accounting.src.Models;

namespace accounting.src.Core.IRepository
{
    public interface IUserRepository
    {
        Task<User?> GetAsync(Guid id);
        Task<User?> GetAsync(string email);
        Task<User?> AddAsync(SignUpBody body);
        Task<User?> GetByToken(string token);
        Task UpdateToken(Guid id, string token);
        Task<bool> UpdateImage(Guid id, string filename);
        Task<string?> GenerateRecoveryCode(string email, TimeSpan? interval = null);
        Task<TokenPair> ResetPassword(User user, string newPassword);
        Task<User?> UpdateAsync(Guid id, UpdateUserBody body);
        List<UserBody> GetUsers(Guid id);
        Task<User?> UpdateRole(string email, UserRole role);
    }
}
