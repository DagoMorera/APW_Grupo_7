using Microsoft.AspNetCore.Mvc;
using APW.Api.ViewModels;
using APW.Business;
using APW.Models;

namespace APW.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SourceItemApiController : ControllerBase
{
    private readonly ISourceItemBusiness _sourceItemBusiness;

    public SourceItemApiController(ISourceItemBusiness sourceItemBusiness)
    {
        _sourceItemBusiness = sourceItemBusiness;
    }

    // GET api/SourceItemApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SourceItemViewModel>>> Get()
    {
        var items = await _sourceItemBusiness.ReadSourceItemsAsync();
        var result = items.Select(ToViewModel);
        return Ok(result);
    }

    // GET api/SourceItemApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SourceItemViewModel>> Get(int id)
    {
        var item = await _sourceItemBusiness.FindSourceItemAsync(id);
        if (item is null) return NotFound();
        return Ok(ToViewModel(item));
    }

    // POST api/SourceItemApi
    [HttpPost]
    public async Task<ActionResult> Post(SourceItemViewModel viewModel)
    {
        var item = new SourceItem
        {
            SourceId = viewModel.SourceId,
            Json = viewModel.Json,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _sourceItemBusiness.CreateSourceItemAsync(item);
        return created ? Ok() : BadRequest();
    }

    // PUT api/SourceItemApi/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, SourceItemViewModel viewModel)
    {
        var item = await _sourceItemBusiness.FindSourceItemAsync(id);
        if (item is null) return NotFound();

        item.SourceId = viewModel.SourceId;
        item.Json = viewModel.Json;

        var updated = await _sourceItemBusiness.UpdateSourceItemAsync(item);
        return updated ? Ok() : BadRequest();
    }

    // DELETE api/SourceItemApi/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var item = await _sourceItemBusiness.FindSourceItemAsync(id);
        if (item is null) return NotFound();

        var deleted = await _sourceItemBusiness.DeleteSourceItemAsync(item);
        return deleted ? Ok() : BadRequest();
    }

    // Convierte el Model de EF a su ViewModel publico
    private static SourceItemViewModel ToViewModel(SourceItem item)
    {
        return new SourceItemViewModel
        {
            Id = item.Id,
            SourceId = item.SourceId,
            Json = item.Json,
            CreatedAt = item.CreatedAt
        };
    }
}