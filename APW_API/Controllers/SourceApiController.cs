using Microsoft.AspNetCore.Mvc;
using APW.Api.ViewModels;
using APW.Business;
using APW.Models;

namespace APW.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SourceApiController : ControllerBase
{
    private readonly ISourceBusiness _sourceBusiness;

    public SourceApiController(ISourceBusiness sourceBusiness)
    {
        _sourceBusiness = sourceBusiness;
    }

    // GET api/SourceApi
    [HttpGet]
    public async Task<ActionResult<IEnumerable<SourceViewModel>>> Get()
    {
        var sources = await _sourceBusiness.ReadSourcesAsync();
        var result = sources.Select(ToViewModel);
        return Ok(result);
    }

    // GET api/SourceApi/5
    [HttpGet("{id}")]
    public async Task<ActionResult<SourceViewModel>> Get(int id)
    {
        var source = await _sourceBusiness.FindSourceAsync(id);
        if (source is null) return NotFound();
        return Ok(ToViewModel(source));
    }

    // GET api/SourceApi/5/items
    [HttpGet("{id}/items")]
    public async Task<ActionResult<IEnumerable<ParsedSourceItemViewModel>>> GetItems(int id)
    {
        var items = await _sourceBusiness.GetParsedItemsAsync(id);
        var result = items.Select(item => new ParsedSourceItemViewModel
        {
            Title = item.Title,
            Description = item.Description,
            Link = item.Link,
            ImageUrl = item.ImageUrl,
            RawJson = item.RawJson
        });

        return Ok(result);
    }

    // POST api/SourceApi
    [HttpPost]
    public async Task<ActionResult> Post(SourceViewModel viewModel)
    {
        var source = new Source
        {
            Url = viewModel.Url,
            Name = viewModel.Name,
            Description = viewModel.Description,
            ComponentType = viewModel.ComponentType,
            RequiresSecret = viewModel.RequiresSecret,
            CreatedAt = DateTime.UtcNow
        };

        var created = await _sourceBusiness.CreateSourceAsync(source);
        return created ? Ok() : BadRequest();
    }

    // PUT api/SourceApi/5
    [HttpPut("{id}")]
    public async Task<ActionResult> Put(int id, SourceViewModel viewModel)
    {
        var source = await _sourceBusiness.FindSourceAsync(id);
        if (source is null) return NotFound();

        source.Url = viewModel.Url;
        source.Name = viewModel.Name;
        source.Description = viewModel.Description;
        source.ComponentType = viewModel.ComponentType;
        source.RequiresSecret = viewModel.RequiresSecret;

        var updated = await _sourceBusiness.UpdateSourceAsync(source);
        return updated ? Ok() : BadRequest();
    }

    // DELETE api/SourceApi/5
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id)
    {
        var source = await _sourceBusiness.FindSourceAsync(id);
        if (source is null) return NotFound();

        var deleted = await _sourceBusiness.DeleteSourceAsync(source);
        return deleted ? Ok() : BadRequest();
    }

    // Convierte el Model de EF a su ViewModel publico
    private static SourceViewModel ToViewModel(Source source)
    {
        return new SourceViewModel
        {
            Id = source.Id,
            Url = source.Url,
            Name = source.Name,
            Description = source.Description,
            ComponentType = source.ComponentType,
            RequiresSecret = source.RequiresSecret
        };
    }
}