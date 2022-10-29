using System.Text.Json.Serialization;

namespace KiteHelper.Domain.Login
{
    public class KiteLoginRequestModel
    {
        [JsonPropertyName("userName")]
        public string UserName { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; }

        [JsonPropertyName("appCode")]
        public int AppCode { get; set; }
    }
}
