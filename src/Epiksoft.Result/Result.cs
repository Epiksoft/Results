using System.Net;

namespace Epiksoft.Result;

public class Result
{
	public int Status { get; }
	public string Message { get; }
	public string Code { get; }
	public bool Succeeded { get; }
	public ICollection<ResultError> Errors { get; set; }
	public HttpStatusCode HttpStatusCode { get; }

	public Result(int status, string message, string code, HttpStatusCode? httpStatusCode = null)
	{
		Status = status;
		Message = message;
		Code = code;

		HttpStatusCode = httpStatusCode ?? (status is 1 ? HttpStatusCode.OK : HttpStatusCode.BadRequest);
		Succeeded = status is 1;
	}

	public static Result Success(string message, string code, HttpStatusCode? httpStatusCode = null) => new(1, message, code, httpStatusCode);

	public static Result Success(HttpStatusCode? httpStatusCode = null) => new(1, string.Empty, string.Empty, httpStatusCode);

	public static Result<TData> Success<TData>(TData data, string message, string code, HttpStatusCode? httpStatusCode = null) => new(data, 1, message, code, httpStatusCode);

	public static Result<TData> Success<TData>(TData data, HttpStatusCode? httpStatusCode = null) => new(data, 1, string.Empty, string.Empty, httpStatusCode);

	public static Result Failure(string message, string code, HttpStatusCode? httpStatusCode = null) => new(0, message, code, httpStatusCode);

	public static Result Failure(HttpStatusCode? httpStatusCode = null) => new(0, string.Empty, string.Empty, httpStatusCode);

	public static Result<TData> Failure<TData>(TData data, string message, string code, HttpStatusCode? httpStatusCode = null) => new(data, 0, message, code, httpStatusCode);

	public static Result<TData> Failure<TData>(TData data, HttpStatusCode? httpStatusCode = null) => new(data, 1, string.Empty, string.Empty, httpStatusCode);

}

public class Result<TData> : Result
{
	public TData Data { get; }

	public Result(TData data, int status, string message, string code, HttpStatusCode? httpStatusCode = null) : base(status, message, code, httpStatusCode)
	{
		Data = data;
	}

}



