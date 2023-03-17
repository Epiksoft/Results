namespace Epiksoft.Results;

public sealed class ResultError
{
	public string Message { get; }
	public string Code { get; }

	/// <summary>
	/// ResultError constructor
	/// </summary>
	/// <param name="message">The error message that you want to provide e.g. "Email already in use"</param>
	/// <param name="code">The error code that you want to provide e.g. "email_already_in_use"</param>
	public ResultError(string message, string code)
	{
		Message = message;
		Code = code;
	}
}
