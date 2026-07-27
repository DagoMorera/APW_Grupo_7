using APW.Api.ViewModels;
using APW.Business;
using APW.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace APW.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoleApiController : ControllerBase
{
    private readonly IRoleBusiness _roleBusiness;

    public RoleApiController(IRoleBusiness roleBusiness)
    {
        _roleBusiness = roleBusiness;
    }

    // GET api/RoleApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<RoleViewModel>>> Get()
    {
        var roles = await _roleBusiness.ReadRolesAsync();
        var result = roles.Select(ToViewModel);
        return Ok(result);
    }

    // GET api/RoleApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<RoleViewModel>> Get(int id)
    {
        var role = await _roleBusiness.FindRoleAsync(id);
        if (role is null) return NotFound();
        return Ok(ToViewModel(role));
    }

    // POST api/RoleApi
    [HttpPost]
    public async Task<ActionResult> Post(RoleViewModel viewModel)
    {
        var role = new Role { Name = viewModel.Name };
        var created = await _roleBusiness.CreateRoleAsync(role);
        return created ? Ok() : BadRequest();
    }

    // PUT api/RoleApi/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, RoleViewModel viewModel)
    {
        var role = new Role { Id = id, Name = viewModel.Name };
        var updated = await _roleBusiness.UpdateRoleAsync(role);
        return updated ? Ok() : BadRequest();
    }

    // DELETE api/RoleApi/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var role = await _roleBusiness.FindRoleAsync(id);
        if (role is null) return NotFound();

        var deleted = await _roleBusiness.DeleteRoleAsync(role);
        return deleted ? Ok() : BadRequest();
    }

    // Convierte el Model de EF a su ViewModel publico
    private static RoleViewModel ToViewModel(Role role)
    {
        return new RoleViewModel { Id = role.Id, Name = role.Name };
    }
}