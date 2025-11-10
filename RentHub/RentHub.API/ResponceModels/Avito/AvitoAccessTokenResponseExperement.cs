using System.Text.Json.Serialization;

namespace RentHub.API.ResponceModels.Avito
{
    public class AvitoAccessTokenResponseExperement
    {
        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        [JsonPropertyName("token_type")]
        public string TokenType { get; set; }

        [JsonPropertyName("expires_in")]
        public int ExpiresIn { get; set; }
    }
}
