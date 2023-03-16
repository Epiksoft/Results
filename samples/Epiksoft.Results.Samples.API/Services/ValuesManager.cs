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
}
