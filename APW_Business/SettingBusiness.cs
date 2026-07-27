using APW.Models;
using APW.Repositories;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace APW.Business;

public interface ISettingBusiness
{
    Task<IEnumerable<Setting>> ReadSettingsAsync();
    Task<Setting> FindSettingAsync(int id);
    Task<bool> CreateSettingAsync(Setting setting);
    Task<bool> UpdateSettingAsync(Setting setting);
    Task<bool> DeleteSettingAsync(Setting setting);
}

// Logica de negocio de Settings
public class SettingBusiness(ISettingRepository settingRepository) : ISettingBusiness
{
    private readonly ISettingRepository _settingRepository = settingRepository;

    public async Task<IEnumerable<Setting>> ReadSettingsAsync()
    {
        return await _settingRepository.ReadAsync();
    }

    public async Task<Setting> FindSettingAsync(int id)
    {
        return await _settingRepository.FindAsync(id);
    }

    public async Task<bool> CreateSettingAsync(Setting setting)
    {
        return await _settingRepository.CreateAsync(setting);
    }

    public async Task<bool> UpdateSettingAsync(Setting setting)
    {
        return await _settingRepository.UpdateAsync(setting);
    }

    public async Task<bool> DeleteSettingAsync(Setting setting)
    {
        return await _settingRepository.DeleteAsync(setting);
    }
}