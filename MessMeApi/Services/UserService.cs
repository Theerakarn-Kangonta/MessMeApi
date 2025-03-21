using AutoMapper;
using MessMeApi.Entities.Dtos;
using MessMeApi.Entities.Models;
using MessMeApi.Repositories.Interfaces;

namespace MessMeApi.Services
{
    public class UserService
    {
        private readonly IUserRepository _repository;
        private readonly IMapper _mapper;
        public UserService(
            IUserRepository repository,
            IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
        {
            var users = await _repository.GetAllAsync();
            return _mapper.Map<IEnumerable<UserDto>>(users);
        }
        public async Task<UserDto?> GetUserByIdAsync(int id)
        {
            var user = await _repository.GetByIdAsync(id);
            return user == null ? null : _mapper.Map<UserDto>(user);
        }
        public async Task<UserDto> AddUserAsync(UserDto userDto)
        {
            var user = _mapper.Map<User>(userDto);
            var newUser = await _repository.AddAsync(user);
            return _mapper.Map<UserDto>(newUser);
        }
        public async Task<UserDto?> UpdateUserAsync(int id, UserDto userDto)
        {
            var user = await _repository.GetByIdAsync(id);
            if (user == null) return null;
            _mapper.Map(userDto, user);
            var updatedUser = await _repository.UpdateAsync(user);
            return updatedUser == null ? null : _mapper.Map<UserDto>(updatedUser);
        }
        public Task<bool> DeleteUserAsync(int id)
        {
            return _repository.DeleteAsync(id);
        }
    }
}
