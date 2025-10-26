using System.Text.Json.Serialization;

namespace RentHub.API.ResponceModels.Avito
{
    public class AvitoUserResponse
    {
        [JsonPropertyName("email")]
        public string Email { get; set; } = null!;

        [JsonPropertyName("id")]
        public long Id { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; } = null!;

        [JsonPropertyName("phone")]
        public string Phone { get; set; } = null!;

        [JsonPropertyName("phones")]
        public string[] Phones { get; set; } = null!;

        [JsonPropertyName("profile_url")]
        public string ProfileUrl { get; set; } = null!;
    }
}
