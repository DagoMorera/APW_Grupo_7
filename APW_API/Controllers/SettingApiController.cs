using APW.Api.ViewModels;
using APW.Business;
using APW.Models;
using Microsoft.AspNetCore.Mvc;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APW.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingApiController : ControllerBase
{
    private readonly ISettingBusiness _settingBusiness;

    public SettingApiController(ISettingBusiness settingBusiness)
    {
        _settingBusiness = settingBusiness;
    }

    // GET api/SettingApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SettingViewModel>>> Get()
    {
        var settings = await _settingBusiness.ReadSettingsAsync();
        var result = settings.Select(ToViewModel);
        return Ok(result);
    }

    // GET api/SettingApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SettingViewModel>> Get(int id)
    {
        var setting = await _settingBusiness.FindSettingAsync(id);
        if (setting is null) return NotFound();
        return Ok(ToViewModel(setting));
    }

    // POST api/SettingApi
    [HttpPost]
    public async Task<ActionResult> Post(SettingViewModel viewModel)
    {
        var setting = new Setting
        {
            SourceId = viewModel.SourceId,
            KeyName = viewModel.KeyName,
            KeyValue = viewModel.KeyValue,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _settingBusiness.CreateSettingAsync(setting);
        return created ? Ok() : BadRequest();
    }

    // PUT api/SettingApi/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, SettingViewModel viewModel)
    {
        var setting = await _settingBusiness.FindSettingAsync(id);
        if (setting is null) return NotFound();

        setting.SourceId = viewModel.SourceId;
        setting.KeyName = viewModel.KeyName;
        setting.KeyValue = viewModel.KeyValue;

        var updated = await _settingBusiness.UpdateSettingAsync(setting);
        return updated ? Ok() : BadRequest();
    }

    // DELETE api/SettingApi/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var setting = await _settingBusiness.FindSettingAsync(id);
        if (setting is null) return NotFound();

        var deleted = await _settingBusiness.DeleteSettingAsync(setting);
        return deleted ? Ok() : BadRequest();
    }

    // Convierte el Model de EF a su ViewModel publico
    private static SettingViewModel ToViewModel(Setting setting)
    {
        return new SettingViewModel
        {
            Id = setting.Id,
            SourceId = setting.SourceId,
            KeyName = setting.KeyName,
            KeyValue = setting.KeyValue
        };
    }
}