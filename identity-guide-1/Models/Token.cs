using Newtonsoft.Json;
using System.Text.Json.Serialization;

namespace identity_guide_1.Models
{
    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }
        [JsonProperty("expires_at")]
        public DateTime ExpiresAt { get; set; }
    }
}
