using MessMeApi.DbContexts;
using MessMeApi.Entities.Models;
using MessMeApi.Repositories.Interfaces;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Dapper;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
namespace MessMeApi.Repositories.Implementations
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;
        private readonly string _connectionString;

        public UserRepository(AppDbContext context,
                                IConfiguration configuration) 
        { 
            _context = context;
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.Users.ToListAsync();
        }
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }
        public async Task<User> AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
        public async Task<User?> UpdateAsync(User user)
        {
            var existingUser = await _context.Users.FindAsync(user.Id);
            if (existingUser == null) 
            { 
                return null;
            }
            existingUser.Username = user.Username;
            existingUser.Email = user.Email;
            await _context.SaveChangesAsync();
            return existingUser;
        }
        public async Task<bool> DeleteAsync(int id)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return false;
            }
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
            return true;
        }
        public async Task<User> GetUserByUsernameAsync(string username)
        {
            await using var connection = new SqlConnection(_connectionString);
            var item = connection.QueryFirstOrDefault<User>(
                "SELECT * FROM Users WHERE Username = @Username",
                new { Username = username });
            return item;
        }

        public async Task<bool> RegisterUser(User user)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                await connection.ExecuteAsync(
                    "INSERT INTO Users (Username, PasswordHash, Salt, Email, Role) " +
                    "VALUES (@Username, @PasswordHash, @Salt, @Email, @Role)",
                    user
                );
                return true;
            }catch (Exception e)
            {
                return false;
            }
           
        }
        public async Task<bool> EmailExists(string email)
        {
            using var connection = new SqlConnection(_connectionString);
            var result = await connection.QueryFirstOrDefaultAsync<int>(
                "SELECT COUNT(1) FROM Users WHERE Email = @Email",
                new { Email = email }
            );
            return result > 0;
        }

    }
}
