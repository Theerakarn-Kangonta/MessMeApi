using MessMeApi.Entities.Models;

namespace MessMeApi.Repositories.Interfaces
{
    public interface IUserRepository
    {
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByIdAsync(int id);
        Task<User> AddAsync(User user);
        Task<User?> UpdateAsync(User user);
        Task<bool> DeleteAsync(int id);
        Task<User> GetUserByUsernameAsync(string username);
        Task<bool> RegisterUser(User user);
        Task<bool> EmailExists(string email);
    }
}
