using MessMeApi.Entities.Dtos;
using MessMeApi.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MessMeApi.Controllers;

[Authorize]
[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly UserService _service;
    public UserController(UserService service) { 
        _service = service; }

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
        return Ok(await _service.GetAllUsersAsync());
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
        var user = await _service.GetUserByIdAsync(id);
        return user == null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<IActionResult> AddUser([FromBody] UserDto userDto)
    {
        var newUser = await _service.AddUserAsync(userDto);
        return CreatedAtAction(nameof(GetUser), new { id = newUser }, newUser);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, [FromBody] UserDto userDto)
    {
        var updatedUser = await _service.UpdateUserAsync(id, userDto);
        return updatedUser == null ? NotFound() : Ok(updatedUser);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var isSuccess = await _service.DeleteUserAsync(id);
        if (isSuccess)
        {
            return NoContent();
        }
        else
        {
            return NotFound();
        }
    }
}
