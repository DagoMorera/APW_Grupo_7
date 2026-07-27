using APW.Models;
using APW.Repositories;

namespace APW.Business;

public interface ISourceItemBusiness
{
    Task<IEnumerable<SourceItem>> ReadSourceItemsAsync();
    Task<SourceItem> FindSourceItemAsync(int id);
    Task<bool> CreateSourceItemAsync(SourceItem sourceItem);
    Task<bool> UpdateSourceItemAsync(SourceItem sourceItem);
    Task<bool> DeleteSourceItemAsync(SourceItem sourceItem);
}

// Logica de negocio de SourceItems
public class SourceItemBusiness(ISourceItemRepository sourceItemRepository) : ISourceItemBusiness
{
    private readonly ISourceItemRepository _sourceItemRepository = sourceItemRepository;

    public async Task<IEnumerable<SourceItem>> ReadSourceItemsAsync()
    {
        return await _sourceItemRepository.ReadAsync();
    }

    public async Task<SourceItem> FindSourceItemAsync(int id)
    {
        return await _sourceItemRepository.FindAsync(id);
    }

    public async Task<bool> CreateSourceItemAsync(SourceItem sourceItem)
    {
        return await _sourceItemRepository.CreateAsync(sourceItem);
    }

    public async Task<bool> UpdateSourceItemAsync(SourceItem sourceItem)
    {
        return await _sourceItemRepository.UpdateAsync(sourceItem);
    }

    public async Task<bool> DeleteSourceItemAsync(SourceItem sourceItem)
    {
        return await _sourceItemRepository.DeleteAsync(sourceItem);
    }
}