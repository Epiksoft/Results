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
    public IActionResult Get(int value)
    {
        return _valuesService.CheckValue(value).ToResponse();
    }

    [HttpGet("data/{value}")]
    public IActionResult GetData(int value)
    {
		return _valuesService.CheckValueData(value).ToResponse();
	}
}
