using Newtonsoft.Json;

namespace WebApi.Models
{
	public class ErrorData
	{
		private const string InternalErrorText = "По техническим причинам операция не была исполнена. Попробуйте повторить позднее.";

		public ErrorData()
		{
			Message = InternalErrorText;
		}

		public ErrorData(string userMessage)
		{
			Message = string.IsNullOrEmpty(userMessage)
				? InternalErrorText
				: userMessage;
		}

		/// <summary>
		/// 
		/// </summary>
		[JsonProperty("userMessage")]
		public string Message { get; set; }
	}
}
