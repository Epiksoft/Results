using Epiksoft.Results.Samples.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Epiksoft.Results.Samples.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class ValuesController : ControllerBase
{
    private readonly IValuesService _valuesService;

    public ValuesController(IValuesService valuesService)
    {
        _valuesService = valuesService;
    }

    [HttpGet("{value}")]
    public async Task<IActionResult> Get(int value)
    {
        return await _valuesService.CheckValue(value).ToResponseAsync();
    }
}
