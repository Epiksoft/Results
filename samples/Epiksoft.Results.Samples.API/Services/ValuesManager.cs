using Epiksoft.Results.Samples.API.Models;
using System.Net;

namespace Epiksoft.Results.Samples.API.Services;

public class ValuesManager : IValuesService
{
    public Result CheckValue(int value)
    {
        if (value < 0)
            return Result.Failure()
                .WithError("The value cannot be lesser than zero", "invalid_value")
                .WithError(new ResultError("example message", "example_code"));

        if (value == 0)
            return Result.Success()
                .WithMetaData("traceId", Guid.NewGuid())
                .WithHttpStatusCode(HttpStatusCode.Accepted);

        if (value > 5)
            return Result.Success()
                .WithMessage("The value is greater than five")
                .WithCode("value_is_greater")
                .WithHttpStatusCode(HttpStatusCode.Accepted);

        return Result.Success(Enumerable.Range(1, 25).ToList())
            .WithHttpStatusCode(HttpStatusCode.OK)
            .WithMetaData("id", Guid.NewGuid());


    }

	public Result CheckValueData(int value)
	{
        if(value > 100)
            return Result.Success(new List<DataModel> { new DataModel
			{
				InformationId = 23435,
				Information = "It is a message from model"
			}}).WithMessage("Base result message").WithCode("base_result_code")
            .WithMetaData("traceId", Guid.NewGuid())
            .WithMetaData("date", DateTime.UtcNow)
            .WithHttpStatusCode(HttpStatusCode.Created);

        return Result.Failure(new DataModel
        {
            InformationId = 84534,
            Information = "It is a failure message from model"
        }).WithError("Value is lesser than 100", "incorrect_value");
	}
}
