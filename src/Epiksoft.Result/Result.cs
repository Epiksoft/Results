using Microsoft.AspNetCore.Mvc;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Epiksoft.Results;

public class Result
{
    public ResultStatus Status { get; }
    public string Message { get; protected set; }
    public string Code { get; protected set; }
    public bool Succeeded { get; }
    public IReadOnlyCollection<ResultError> Errors { get => _errors; }
    public IReadOnlyDictionary<string, object> MetaData { get => _metaData; }

    [JsonIgnore]
    public HttpStatusCode HttpStatusCode { get; private set; }

    protected List<ResultError> _errors;
    protected Dictionary<string, object> _metaData;

    protected Result(ResultStatus status, string message, string code)
    {
        Status = status;
        Message = message;
        Code = code;

        _errors = new();
        _metaData = new();

        HttpStatusCode = status is ResultStatus.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest;
        Succeeded = status is ResultStatus.Success;
    }

    #region Result builders

    /// <summary>
    /// Creates a success result with message and code
    /// </summary>
    /// <param name="message">The message you want to provide e.g. "Operation was successful"</param>
    /// <param name="code">The code you want to provide e.g. "successful_operation"</param>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result Success(string message, string code)
        => new(ResultStatus.Success, message, code);

    /// <summary>
    /// Creates a success result
    /// </summary>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result Success()
        => Success(string.Empty, string.Empty);

    /// <summary>
    /// Creates a success result with data, message and code
    /// </summary>
    /// <typeparam name="TData">Type of the data that you want to provide</typeparam>
    /// <param name="data">The data that you want to provide e.g. list of view models</param>
    /// <param name="message">The message you want to provide e.g. "Operation was successful"</param>
    /// <param name="code">The code you want to provide e.g. "successful_operation"</param>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result<TData> Success<TData>(TData data, string message, string code)
        => new(data, ResultStatus.Success, message, code);


    /// <summary>
    /// Creates a success result with data, message and code
    /// </summary>
    /// <typeparam name="TData">Type of the data that you want to provide</typeparam>
    /// <param name="data">The data that you want to provide e.g. list of view models</param>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result<TData> Success<TData>(TData data)
        => Success(data, string.Empty, string.Empty);

    /// <summary>
    /// Creates a failure result with message and code
    /// </summary>
    /// <param name="message">The message you want to provide e.g. "Operation failed"</param>
    /// <param name="code">The code you want to provide e.g. "failed_operation"</param>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result Failure(string message, string code)
        => new(ResultStatus.Failure, message, code);

    /// <summary>
    /// Creates a failure result
    /// </summary>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result Failure()
        => Failure(string.Empty, string.Empty);

    /// <summary>
    /// Creates a failure result with data, message and code
    /// </summary>
    /// <typeparam name="TData">Type of the data that you want to provide</typeparam>
    /// <param name="data">The data that you want to provide e.g. list of view models</param>
    /// <param name="message">The message you want to provide e.g. "Operation failed"</param>
    /// <param name="code">The code you want to provide e.g. "failed_operation"</param>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result<TData> Failure<TData>(TData data, string message, string code)
        => new(data, ResultStatus.Failure, message, code);

    /// <summary>
    /// Creates a failure result with data, message and code
    /// </summary>
    /// <typeparam name="TData">Type of the data that you want to provide</typeparam>
    /// <param name="data">The data that you want to provide e.g. list of view models</param>
    /// <returns>Created <c>Result</c> object</returns>
    public static Result<TData> Failure<TData>(TData data)
        => Failure(data, string.Empty, string.Empty);

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
    public virtual Result WithError(string message, string code)
    {
        return WithError(new ResultError(message, code));
    }

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

    public override string ToString()
    {
        return ToJson();
    }

}

public class Result<TData> : Result
{
    public TData Data { get; }

    internal Result(TData data, ResultStatus status, string message, string code) : base(status, message, code)
    {
        Data = data;
    }

    /// <inheritdoc />
    public override Result<TData> WithCode(string code)
    {
        base.WithCode(code);

        return this;
    }

    /// <inheritdoc />
    public override Result<TData> WithMetaData(string key, object value)
    {
        base.WithMetaData(key, value);

        return this;
    }

    /// <inheritdoc />
    public override Result<TData> WithMetaData(Dictionary<string, object> metaData)
    {
        base.WithMetaData(metaData);

        return this;
    }

    /// <inheritdoc />
    public override Result<TData> WithHttpStatusCode(HttpStatusCode httpStatusCode)
    {
        base.WithHttpStatusCode(httpStatusCode);

        return this;
    }

    /// <inheritdoc />
    public override string ToJson()
    {
        return JsonSerializer.Serialize(this);
    }

    /// <inheritdoc />
    public override IActionResult ToResponse()
    {
        return new ObjectResult(this) { StatusCode = (int)HttpStatusCode };
    }

    /// <inheritdoc />
    public override Result<TData> WithError(params ResultError[] resultErrors)
    {
        base.WithError(resultErrors);

        return this;
    }

    /// <inheritdoc />
    public override Result<TData> WithError(string message, string code)
    {
        base.WithError(message, code);

        return this;
    }

    /// <inheritdoc />
    public override Result<TData> WithMessage(string message)
    {
        base.WithMessage(message);

        return this;
    }

    /// <inheritdoc />
    public override Task<IActionResult> ToResponseAsync()
    {
        return Task.FromResult(ToResponse());
    }

    public override string ToString()
    {
        return ToJson();
    }
}