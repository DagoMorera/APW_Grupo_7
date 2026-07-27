using Microsoft.AspNetCore.Mvc;
using APW.Api.ViewModels;
using APW.Business;

namespace APW.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthApiController : ControllerBase
{
    private readonly IUserBusiness _userBusiness;
    private readonly IRoleBusiness _roleBusiness;

    public AuthApiController(IUserBusiness userBusiness, IRoleBusiness roleBusiness)
    {
        _userBusiness = userBusiness;
        _roleBusiness = roleBusiness;
    }

    // POST api/AuthApi/login
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseViewModel>> Login(LoginRequestViewModel request)
    {
        var user = await _userBusiness.ValidateCredentialsAsync(request.Username, request.Password);
        if (user is null) return Unauthorized();

        var role = await _roleBusiness.FindRoleAsync(user.RoleId);

        var response = new LoginResponseViewModel
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            RoleName = role?.Name ?? string.Empty
        };

        return Ok(response);
    }
}