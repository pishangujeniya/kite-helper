using System.Text.Json.Serialization;

namespace KiteHelper.Domain.Login
{
    public class KiteLoginResponseModel
    {
        [JsonPropertyName("SessionId")]
        public string SessionId { get; set; }    
    }
}
