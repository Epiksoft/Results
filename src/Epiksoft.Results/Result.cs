using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epiksoft.Results;

public class Result
{
    [JsonIgnore]
    public ResultStatus Status { get; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Message { get; protected set; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string Code { get; protected set; }

    public bool Succeeded { get; }

    [JsonIgnore]
    public bool Failed { get => !Succeeded; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyCollection<ResultError> Errors { get => _errors; }

    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public IReadOnlyDictionary<string, object> MetaData { get => _metaData; }

    [JsonIgnore]
    public HttpStatusCode HttpStatusCode { get; private set; }

    protected List<ResultError> _errors;

    protected Dictionary<string, object> _metaData;

    protected Result(ResultStatus status)
    {
        Status = status;

        HttpStatusCode = status is ResultStatus.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
        Succeeded = status is ResultStatus.Success;
    }

    #region Result builders

    /// <summary>
    /// Creates a success result
    /// </summary>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result Success() => new(ResultStatus.Success);

    /// <summary>
    /// Creates a success result with data
    /// </summary>
    /// <typeparam name="TData">Type of the data that you want to provide</typeparam>
    /// <param name="data">The data that you want to provide e.g. list of view models</param>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result<TData> Success<TData>(TData data) => new(ResultStatus.Success, data);

    /// <summary>
    /// Creates a failure result
    /// </summary>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result Failure() => new(ResultStatus.Failure);

    /// <summary>
    /// Creates a failure result with data
    /// </summary>
    /// <typeparam name="TData">Type of the data that you want to provide</typeparam>
    /// <param name="data">The data that you want to provide e.g. list of view models</param>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result<TData> Failure<TData>(TData data) => new(ResultStatus.Failure, data);

    /// <summary>
    /// Creates a failure result with an error
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    /// <param name="error"></param>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result<TData> Failure<TData>(ResultError error) => new(ResultStatus.Failure, error);

    #endregion

    #region Result extensions

    /// <summary>
    /// Adds errors to result
    /// </summary>
    /// <param name="resultErrors">The errors that you want to add to result</param>
    /// <returns>The object itself</returns>
    public virtual Result WithError(params ResultError[] resultErrors)
    {
        if (resultErrors.Any())
        {
            _errors ??= new();
            _errors.AddRange(resultErrors);
        }

        return this;
    }

    /// <summary>
    /// Adds an error to the result
    /// </summary>
    /// <param name="message">The error message that you want to provide e.g. "Object is not valid"</param>
    /// <param name="code">The error code that you want to provide e.g. "validation_error"</param>
    /// <returns>The object itself</returns>
    public virtual Result WithError(string message, string code) => WithError(new ResultError(message, code));

    /// <summary>
    /// Adds a message to the result
    /// </summary>
    /// <param name="message">The message that you want to provide e.g. "Account successfully registered"</param>
    /// <returns>The object itself</returns>
    public virtual Result WithMessage(string message)
    {
        Message = message;

        return this;
    }

    /// <summary>
    /// Adds a code to the result
    /// </summary>
    /// <param name="code">The code that you want to provide e.g. "account_created"</param>
    /// <returns>The object itself</returns>
    public virtual Result WithCode(string code)
    {
        Code = code;

        return this;
    }

    /// <summary>
    /// Adds a metadata to the result
    /// </summary>
    /// <param name="key">The key of the value e.g. "traceId"</param>
    /// <param name="value">The value e.g. <c>Guid.NewGuid()</c></param>
    /// <returns>The object itself</returns>
    public virtual Result WithMetaData(string key, object value)
    {
        _metaData ??= new();
        _metaData.Add(key, value);

        return this;
    }

    /// <summary>
    /// Adds metadata dictionary directly to the result
    /// </summary>
    /// <param name="metaData">The dictionary that contains keys and values</param>
    /// <returns>The object itself</returns>
    public virtual Result WithMetaData(Dictionary<string, object> metaData)
    {
        _metaData = metaData;

        return this;
    }

    /// <summary>
    /// Sets the http status code of the result
    /// </summary>
    /// <param name="httpStatusCode">Http status code</param>
    /// <returns>The object itself</returns>
    public virtual Result WithHttpStatusCode(HttpStatusCode httpStatusCode)
    {
        HttpStatusCode = httpStatusCode;

        return this;
    }

    #endregion

    #region Result converting

    /// <summary>
    /// Serializes the result
    /// </summary>
    /// <returns>Serialized Json string</returns>
    public virtual string ToJson() => JsonSerializer.Serialize(this);

    /// <summary>
    /// Converts the result into <c>IActionResult</c> by http status code
    /// </summary>
    /// <returns>Converted action result</returns>
    public virtual IActionResult ToResponse() => new ObjectResult(this) { StatusCode = (int)HttpStatusCode };

    /// <summary>
    /// Converts the result into <c>IActionResult</c> by http status code (async)
    /// </summary>
    /// <returns>Converted action result (Task)</returns>
    public virtual Task<IActionResult> ToResponseAsync() => Task.FromResult(ToResponse());

    #endregion

    public override string ToString() => ToJson();


}

public class Result<TData> : Result
{
    public TData Data { get; private set; } = default;

    internal Result(ResultStatus status, ResultError error) : base(status)
    {
        _errors ??= new();
        _errors.Add(error);
    }

    internal Result(ResultStatus status, TData data) : base(status)
    {
        Data = data;
    }

    /// <summary>
    /// Adds data to result
    /// </summary>
    /// <param name="data">The data that you want to provide</param>
    /// <returns>The object itself</returns>
    public Result<TData> WithData(TData data)
    {
        Data = data;

        return this;
    }

    /// <inheritdoc />
    public override Result<TData> WithCode(string code) => base.WithCode(code) as Result<TData>;

    /// <inheritdoc />
    public override Result<TData> WithMetaData(string key, object value) => base.WithMetaData(key, value) as Result<TData>;

    /// <inheritdoc />
    public override Result<TData> WithMetaData(Dictionary<string, object> metaData) => base.WithMetaData(metaData) as Result<TData>;

    /// <inheritdoc/>
    public override Result<TData> WithHttpStatusCode(HttpStatusCode httpStatusCode) => base.WithHttpStatusCode(httpStatusCode) as Result<TData>;

    /// <inheritdoc/>
    public override string ToJson() => JsonSerializer.Serialize(this);

    /// <inheritdoc/>
    public override IActionResult ToResponse() => new ObjectResult(this) { StatusCode = (int)HttpStatusCode };

    /// <inheritdoc/>
    public override Result<TData> WithError(params ResultError[] resultErrors) => base.WithError(resultErrors) as Result<TData>;

    /// <inheritdoc/>
    public override Result<TData> WithError(string message, string code) => base.WithError(message, code) as Result<TData>;

    /// <inheritdoc/>
    public override Result<TData> WithMessage(string message) => base.WithMessage(message) as Result<TData>;

    /// <inheritdoc/>
    public override Task<IActionResult> ToResponseAsync() => Task.FromResult(ToResponse());

    public override string ToString() => ToJson();
}