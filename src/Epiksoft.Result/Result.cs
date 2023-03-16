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
	public HttpStatusCode HttpStatusCode { get; }

	protected List<ResultError> _errors;
	protected Dictionary<string, object> _metaData;

	protected Result(ResultStatus status, string message, string code, HttpStatusCode? httpStatusCode = null)
	{
		Status = status;
		Message = message;
		Code = code;

		_errors = new();
		_metaData = new();

		HttpStatusCode = httpStatusCode ?? (status is ResultStatus.Success ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
		Succeeded = status is ResultStatus.Success;
	}

	#region Result builders

	/// <summary>
	/// Creates a success result with message, code and http status code (optional)
	/// </summary>
	/// <param name="message">The message you want to provide e.g. "Operation was successful"</param>
	/// <param name="code">The code you want to provide e.g. "successful_operation"</param>
	/// <param name="httpStatusCode">Http status code that you want to return. If <paramref name="httpStatusCode"/>
	/// is <c>null</c>, default value will be <c>HttpStatusCode.OK</c></param>
	/// <returns>Created <c>Result</c> object</returns>
	public static Result Success(string message, string code, HttpStatusCode? httpStatusCode = null)
		=> new(ResultStatus.Success, message, code, httpStatusCode);

	/// <summary>
	/// Creates a success result with http status code (optional)
	/// </summary>
	/// <param name="httpStatusCode">Http status code that you want to return. If <paramref name="httpStatusCode"/>
	/// is <c>null</c>, default value will be <c>HttpStatusCode.OK</c></param>
	/// <returns>Created <c>Result</c> object</returns>
	public static Result Success(HttpStatusCode? httpStatusCode = null)
		=> Success(string.Empty, string.Empty, httpStatusCode);

	/// <summary>
	/// Creates a success result with data, message, code and http status code (optional)
	/// </summary>
	/// <typeparam name="TData">Type of the data that you want to provide</typeparam>
	/// <param name="data">The data that you want to provide e.g. list of view models</param>
	/// <param name="message">The message you want to provide e.g. "Operation was successful"</param>
	/// <param name="code">The code you want to provide e.g. "successful_operation"</param>
	/// <param name="httpStatusCode">Http status code that you want to return. If <paramref name="httpStatusCode"/>
	/// is <c>null</c>, default value will be <c>HttpStatusCode.OK</c></param>
	/// <returns>Created <c>Result</c> object</returns>
	public static Result<TData> Success<TData>(TData data, string message, string code, HttpStatusCode? httpStatusCode = null)
		=> new(data, ResultStatus.Success, message, code, httpStatusCode);


	/// <summary>
	/// Creates a success result with data, message, code and http status code (optional)
	/// </summary>
	/// <typeparam name="TData">Type of the data that you want to provide</typeparam>
	/// <param name="data">The data that you want to provide e.g. list of view models</param>
	/// <param name="httpStatusCode">Http status code that you want to return. If <paramref name="httpStatusCode"/>
	/// is <c>null</c>, default value will be <c>HttpStatusCode.OK</c></param>
	/// <returns>Created <c>Result</c> object</returns>
	public static Result<TData> Success<TData>(TData data, HttpStatusCode? httpStatusCode = null)
		=> Success(data, string.Empty, string.Empty, httpStatusCode);

	/// <summary>
	/// Creates a failure result with message, code and http status code (optional)
	/// </summary>
	/// <param name="message">The message you want to provide e.g. "Operation failed"</param>
	/// <param name="code">The code you want to provide e.g. "failed_operation"</param>
	/// <param name="httpStatusCode">Http status code that you want to return. If <paramref name="httpStatusCode"/>
	/// is <c>null</c>, default value will be <c>HttpStatusCode.BadRequest</c></param>
	/// <returns>Created <c>Result</c> object</returns>
	public static Result Failure(string message, string code, HttpStatusCode? httpStatusCode = null)
		=> new(ResultStatus.Failure, message, code, httpStatusCode);

	/// <summary>
	/// Creates a failure result with http status code (optional)
	/// </summary>
	/// <param name="httpStatusCode">Http status code that you want to return. If <paramref name="httpStatusCode"/>
	/// is <c>null</c>, default value will be <c>HttpStatusCode.BadRequest</c></param>
	/// <returns>Created <c>Result</c> object</returns>
	public static Result Failure(HttpStatusCode? httpStatusCode = null)
		=> Failure(string.Empty, string.Empty, httpStatusCode);

	/// <summary>
	/// Creates a failure result with data, message, code and http status code (optional)
	/// </summary>
	/// <typeparam name="TData">Type of the data that you want to provide</typeparam>
	/// <param name="data">The data that you want to provide e.g. list of view models</param>
	/// <param name="message">The message you want to provide e.g. "Operation failed"</param>
	/// <param name="code">The code you want to provide e.g. "failed_operation"</param>
	/// <param name="httpStatusCode">Http status code that you want to return. If <paramref name="httpStatusCode"/>
	/// is <c>null</c>, default value will be <c>HttpStatusCode.OK</c></param>
	/// <returns>Created <c>Result</c> object</returns>
	public static Result<TData> Failure<TData>(TData data, string message, string code, HttpStatusCode? httpStatusCode = null)
		=> new(data, ResultStatus.Failure, message, code, httpStatusCode);

	/// <summary>
	/// Creates a failure result with data, message, code and http status code (optional)
	/// </summary>
	/// <typeparam name="TData">Type of the data that you want to provide</typeparam>
	/// <param name="data">The data that you want to provide e.g. list of view models</param>
	/// <param name="httpStatusCode">Http status code that you want to return. If <paramref name="httpStatusCode"/>
	/// is <c>null</c>, default value will be <c>HttpStatusCode.OK</c></param>
	/// <returns>Created <c>Result</c> object</returns>
	public static Result<TData> Failure<TData>(TData data, HttpStatusCode? httpStatusCode = null)
		=> Failure(data, string.Empty, string.Empty, httpStatusCode);

	#endregion

	#region Result extensions

	/// <summary>
	/// Adds errors to result
	/// </summary>
	/// <param name="resultErrors">The errors that you want to add to result</param>
	/// <returns>The object itself</returns>
	public Result WithError(params ResultError[] resultErrors)
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
	public Result WithError(string message, string code)
	{
		return WithError(new ResultError(message, code));
	}

	/// <summary>
	/// Adds a message to the result
	/// </summary>
	/// <param name="message">The message that you want to provide e.g. "Account successfully registered"</param>
	/// <returns>The object itself</returns>
	public Result WithMessage(string message)
	{
		Message = message;

		return this;
	}

	/// <summary>
	/// Adds a code to the result
	/// </summary>
	/// <param name="code">The code that you want to provide e.g. "account_created"</param>
	/// <returns>The object itself</returns>
	public Result WithCode(string code)
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
	public Result WithMetaData(string key, object value)
	{
		_metaData.Add(key, value);

		return this;
	}

	/// <summary>
	/// Adds metadata dictionary directly to the result
	/// </summary>
	/// <param name="metaData">The dictionary that contains keys and values</param>
	/// <returns>The object itself</returns>
	public Result WithMetaData(Dictionary<string, object> metaData)
	{
		_metaData = metaData;

		return this;
	}
	#endregion

	#region Result converting

	/// <summary>
	/// Serializes the result
	/// </summary>
	/// <returns>Serialized Json string</returns>
	public string ToJson() => JsonSerializer.Serialize(this);

	/// <summary>
	/// Converts the result into <c>IActionResult</c> by http status code
	/// </summary>
	/// <returns>Converted action result</returns>
	public IActionResult ToResponse() => new ObjectResult(this) { StatusCode = (int)HttpStatusCode };

	/// <summary>
	/// Converts the result into <c>IActionResult</c> by http status code (async)
	/// </summary>
	/// <returns>Converted action result (Task)</returns>
	public Task<IActionResult> ToResponseAsync() => Task.FromResult(ToResponse());
	#endregion

}

public class Result<TData> : Result
{
	public TData Data { get; }

	internal Result(TData data, ResultStatus status, string message, string code, HttpStatusCode? httpStatusCode = null) : base(status, message, code, httpStatusCode)
	{
		Data = data;
	}

}