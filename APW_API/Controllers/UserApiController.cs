using Microsoft.AspNetCore.Mvc;
using APW.Api.ViewModels;
using APW.Business;
using APW.Models;

namespace APW.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserApiController : ControllerBase
{
    private readonly IUserBusiness _userBusiness;

    public UserApiController(IUserBusiness userBusiness)
    {
        _userBusiness = userBusiness;
    }

    // GET api/UserApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserViewModel>>> Get()
    {
        var users = await _userBusiness.ReadUsersAsync();
        var result = users.Select(ToViewModel);
        return Ok(result);
    }

    // GET api/UserApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<UserViewModel>> Get(int id)
    {
        var user = await _userBusiness.FindUserAsync(id);
        if (user is null) return NotFound();
        return Ok(ToViewModel(user));
    }

    // GET api/UserApi/by-token/3f2a9c1e-...
    [HttpGet("by-token/{token:guid}")]
    public async Task<ActionResult<UserViewModel>> GetByToken(Guid token)
    {
        var user = await _userBusiness.FindByTokenAsync(token);
        if (user is null) return NotFound();
        return Ok(ToViewModel(user));
    }

    // POST api/UserApi
    [HttpPost]
    public async Task<ActionResult> Post(UserViewModel viewModel)
    {
        var user = new User
        {
            Username = viewModel.Username,
            Email = viewModel.Email,
            PasswordHash = viewModel.Password,
            RoleId = viewModel.RoleId,
            IsActive = viewModel.IsActive,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _userBusiness.CreateUserAsync(user);
        return created ? Ok() : BadRequest();
    }

    // PUT api/UserApi/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, UserViewModel viewModel)
    {
        var user = await _userBusiness.FindUserAsync(id);
        if (user is null) return NotFound();

        user.Username = viewModel.Username;
        user.Email = viewModel.Email;
        user.RoleId = viewModel.RoleId;
        user.IsActive = viewModel.IsActive;

        var updated = await _userBusiness.UpdateUserAsync(user);
        return updated ? Ok() : BadRequest();
    }

    // DELETE api/UserApi/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var user = await _userBusiness.FindUserAsync(id);
        if (user is null) return NotFound();

        var deleted = await _userBusiness.DeleteUserAsync(user);
        return deleted ? Ok() : BadRequest();
    }

    // Convierte el Model de EF a su ViewModel publico, sin exponer la password
    private static UserViewModel ToViewModel(User user)
    {
        return new UserViewModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            RoleId = user.RoleId,
            IsActive = user.IsActive,
            FeedToken = user.FeedToken
        };
    }
}