using APW.Api.ViewModels;
using APW.Business;
using Microsoft.AspNetCore.Mvc;

namespace APW.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubscriptionApiController : ControllerBase
{
    private readonly ISubscriptionBusiness _subscriptionBusiness;

    public SubscriptionApiController(ISubscriptionBusiness subscriptionBusiness)
    {
        _subscriptionBusiness = subscriptionBusiness;
    }

    // GET api/SubscriptionApi/mine/5
    [HttpGet("mine/{userId}")]
    public async Task<ActionResult<IEnumerable<int>>> GetMine(int userId)
    {
        var sourceIds = await _subscriptionBusiness.GetSubscribedSourceIdsAsync(userId);
        return Ok(sourceIds);
    }

    // POST api/SubscriptionApi
    [HttpPost]
    public async Task<ActionResult<ToggleSubscriptionResultViewModel>> Toggle(ToggleSubscriptionViewModel request)
    {
        var subscribed = await _subscriptionBusiness.ToggleSubscriptionAsync(request.UserId, request.SourceId);
        return Ok(new ToggleSubscriptionResultViewModel { Subscribed = subscribed });
    }
}