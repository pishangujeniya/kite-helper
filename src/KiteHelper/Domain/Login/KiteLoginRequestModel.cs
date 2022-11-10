using System.Text.Json.Serialization;

namespace KiteHelper.Domain.Login
{
    public class KiteLoginRequestModel
    {
        [JsonPropertyName("UserName")]
        public string UserName { get; set; }

        [JsonPropertyName("Password")]
        public string Password { get; set; }

        [JsonPropertyName("AppCode")]
        public int AppCode { get; set; }
    }
}
