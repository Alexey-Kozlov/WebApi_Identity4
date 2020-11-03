using Newtonsoft.Json;
using WebApi.Logger;

namespace WebApi.Models
{

	public class PostResult<T>
	{
		public PostResult(long id, T request, string userMessage = "")
		{
			Id = id;
			Request = request;
			UserMessage = userMessage;
		}

		/// <summary>
		/// Идентификатор созданной сущности
		/// </summary>
		[JsonProperty("createId")]
		public long Id { get; set; }

		/// <summary>
		/// Запрос на основе которого создавалась сущность
		/// </summary>
		[JsonProperty("request")]
		public T Request { get; set; }

		/// <summary>
		/// Сообщение для пользователя
		/// </summary>
		[JsonProperty("userMessage")]
		public string UserMessage { get; set; }
	}
}
