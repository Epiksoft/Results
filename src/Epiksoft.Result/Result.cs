using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epiksoft.Result;

public class Result
{
    public int Status { get; }
    public string Message { get; protected set; }
    public string Code { get; protected set; }
    public bool Succeeded { get; }
    public IReadOnlyCollection<ResultError> Errors { get => _errors; }
    public IReadOnlyDictionary<string, object> MetaData { get => _metaData; }

    [JsonIgnore]
    public HttpStatusCode HttpStatusCode { get; }

    protected List<ResultError> _errors;
    protected Dictionary<string, object> _metaData;

    public Result(int status, string message, string code, HttpStatusCode? httpStatusCode = null)
    {
        Status = status;
        Message = message;
        Code = code;

        _errors = new List<ResultError>();
        _metaData = new Dictionary<string, object>();

        HttpStatusCode = httpStatusCode ?? (status is 1 ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
        Succeeded = status is 1;
    }

    #region Result builders
    public static Result Success(string message, string code, HttpStatusCode? httpStatusCode = null) => new(1, message, code, httpStatusCode);

    public static Result Success(HttpStatusCode? httpStatusCode = null) => Success(string.Empty, string.Empty, httpStatusCode);

    public static Result<TData> Success<TData>(TData data, string message, string code, HttpStatusCode? httpStatusCode = null) where TData : class, new() => new(data, 1, message, code, httpStatusCode);

    public static Result<TData> Success<TData>(TData data, HttpStatusCode? httpStatusCode = null) where TData : class, new() => Success<TData>(data, string.Empty, string.Empty, httpStatusCode);

    public static Result Failure(string message, string code, HttpStatusCode? httpStatusCode = null) => new(0, message, code, httpStatusCode);

    public static Result Failure(HttpStatusCode? httpStatusCode = null) => Failure(string.Empty, string.Empty, httpStatusCode);

    public static Result<TData> Failure<TData>(TData data, string message, string code, HttpStatusCode? httpStatusCode = null) where TData : class, new() => new(data, 0, message, code, httpStatusCode);

    public static Result<TData> Failure<TData>(TData data, HttpStatusCode? httpStatusCode = null) where TData : class, new() => Failure<TData>(data, string.Empty, string.Empty, httpStatusCode);
    #endregion

    #region Result extensions
    public Result WithError(params ResultError[] resultErrors)
    {
        if (resultErrors.Any())
        {
            _errors.AddRange(resultErrors);
        }

        return this;
    }

    public Result WithError(string message, string code)
    {
        return WithError(new ResultError(message, code));
    }

    public Result WithMessage(string message)
    {
        Message = message;

        return this;
    }

    public Result WithCode(string code)
    {
        Code = code;

        return this;
    }

    public Result WithMetaData(string key, object value)
    {
        _metaData.Add(key, value);

        return this;
    }

    public Result WithMetaData(Dictionary<string, object> metaData)
    {
        _metaData = metaData;

        return this;
    }
    #endregion

    #region Result converting
    public string ToJson() => JsonSerializer.Serialize(this);

    public IActionResult ToResponse() => new ObjectResult(this) { StatusCode = (int)HttpStatusCode };

    public Task<IActionResult> ToResponseAsync() => Task.FromResult(ToResponse());
    #endregion

}

public class Result<TData> : Result where TData : class, new()
{
    public TData Data { get; }

    public Result(TData data, int status, string message, string code, HttpStatusCode? httpStatusCode = null) : base(status, message, code, httpStatusCode)
    {
        Data = data;
    }

}