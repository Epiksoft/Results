using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Text.Json;

namespace Epiksoft.Results.Tests;

public class ResultTests
{

    [Fact]
    public void SuccessWithMessageAndCode_ShouldReturnHttpStatusCodeOK_WhenConvertingActionResult()
    {
        string message = "test message";
        string code = "test_message_code";
        var result = Result.Success()
            .WithCode(code)
            .WithMessage(message);
        var objResult = result.ToResponse() is ObjectResult ? (ObjectResult)result.ToResponse() : null;

        Assert.Equal(message, result.Message);
        Assert.Equal(code, result.Code);
        Assert.True(result.Succeeded);
        Assert.NotNull(objResult);
        Assert.Equal((int)HttpStatusCode.OK, objResult.StatusCode);
    }

    [Fact]
    public void FailureWithMessageAndCode_ShouldReturnHttpStatusCodeBadRequest_WhenConvertingActionResult()
    {
        string message = "test message";
        string code = "test_message_code";
        var result = Result.Failure()
            .WithCode(code)
            .WithMessage(message);
        var objResult = result.ToResponse() is ObjectResult ? (ObjectResult)result.ToResponse() : null;

        Assert.Equal(message, result.Message);
        Assert.Equal(code, result.Code);
        Assert.False(result.Succeeded);
        Assert.True(result.ToResponse() is ObjectResult);
        Assert.Equal((int)HttpStatusCode.BadRequest, objResult.StatusCode);
    }


    [Fact]
    public void SuccessWithMessageAndCodeAndData_ShouldReturnHttpStatusCodeCreated_WhenConvertingActionResult()
    {
        string message = "test message";
        string code = "test_message_code";
        string data = "Ali";

        var result = Result.Success(data)
            .WithMessage(message)
            .WithCode(code)
            .WithHttpStatusCode(HttpStatusCode.Created);

        var objResult = result.ToResponse() is ObjectResult ? (ObjectResult)result.ToResponse() : null;

        Assert.Equal(message, result.Message);
        Assert.Equal(code, result.Code);
        Assert.True(result.Succeeded);
        Assert.True(result.ToResponse() is ObjectResult);
        Assert.Equal((int)HttpStatusCode.Created, objResult.StatusCode);
        Assert.Equal(data, result.Data);
    }

    [Fact]
    public void ResultWithMetaData_ShouldIncludeMetaData()
    {
        var dateTime = DateTime.Now;
        string key = "dateTime";
        object value = dateTime;

        var result = Result.Success()
            .WithMetaData(key, value);

        Assert.Equal(dateTime, result.Metadata[key]);
    }

    [Fact]
    public void ResultWithErrors_ShouldIncludeErrors()
    {
        string code = "test_code";
        string message = "test";
        string otherMessage = "test1";

        var result = Result.Failure()
            .WithError(message, code)
            .WithError(new ResultError(message, code), new ResultError(message, code))
            .WithError(message, code)
            .WithMessage(otherMessage)
            .WithCode(code);


        Assert.Equal(otherMessage, result.Message);
        Assert.Equal(code, result.Code);
        Assert.False(result.Succeeded);
        Assert.True(result.Errors?.Any(x => x.Code == code && x.Message == message));
        Assert.Equal(4, result.Errors?.Count);
    }

    [Fact]
    public void FailureWithoutData_ShouldIncludeErrors()
    {
        string message = "error message";
        string code = "error_code";

        var result = Result.Failure<object>(new ResultError(message, code));

        Assert.False(result.Succeeded);
        Assert.True(result.Failed);
        Assert.Null(result.Data);
        Assert.True(result.Errors.Count > 0);
        Assert.Equal(message, result.Errors.First().Message);
        Assert.Equal(code, result.Errors.First().Code);
    }

    [Fact]
    public void ResultWithNullData_ShouldNotIncludeData()
    {
        var result = Result.Failure<object>();

        Assert.Null(result.Data);
    }

    [Fact]
    public void Result_ShouldIncludeMetaData_WhenMetaDataFactoryProvided()
    {
        IServiceCollection services = new ServiceCollection();

        var serverTime = DateTime.UtcNow;
        var requestId = Guid.NewGuid();

        services.AddResultOptions(o =>
        {
            o.MetadataFactory = () => {
                var dictionary = new Dictionary<string, object>
                {
                    { nameof(serverTime), serverTime },
                    { nameof(requestId), requestId }
                };

                return dictionary;
            };

            o.JsonSerializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
        });

        var result = Result.Success();

        Assert.True(result.Metadata.ContainsKey(nameof(serverTime)));
        Assert.Equal(result.Metadata[nameof(serverTime)], serverTime);
        Assert.True(result.Metadata.ContainsKey(nameof(requestId)));
        Assert.Equal(result.Metadata[nameof(requestId)], requestId);
    }

    [Fact]
    public void Result_ShouldReturnNotFound_WhenDataIsNull()
    {
        var result = Result.Failure<object>();
        var response = result.ToResponse();

        Assert.True(response is ObjectResult);
        Assert.Equal(((ObjectResult)response).StatusCode, (int)HttpStatusCode.NotFound);
    }

    [Fact]
    public void Result_ShouldReturnOK_WhenDataIsNotNull()
    {
        var result = Result.Success<object>(new List<int> { 1,3,5,7});
        var response = result.ToResponse();

        Assert.True(response is ObjectResult);
        Assert.Equal(((ObjectResult)response).StatusCode, (int)HttpStatusCode.OK);
    }
}
