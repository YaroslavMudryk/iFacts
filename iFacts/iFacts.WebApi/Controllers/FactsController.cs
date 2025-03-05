using iFacts.Shared.Api;
using iFacts.WebApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace iFacts.WebApi.Controllers;

[Route("api/v1/facts")]
[ApiController]
public class FactsController(FactsService factsService) : ControllerBase
{
    [HttpGet("/random")]
    public async Task<IActionResult> GetRandomFactAsync()
    {
        return Ok((await factsService.GetRandomFactAsync()).MapToResponse());
    }

    [HttpGet("/{factId:int}")]
    public async Task<IActionResult> GetFactByIdAsync(int factId)
    {
        return Ok((await factsService.GetFactByIdAsync(factId)).MapToResponse());
    }
}
