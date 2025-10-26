using System.ComponentModel;
namespace RentHub.API.RequestModels.Avito
{
    public class AvitoLoginRequest
    {
        [DefaultValue("client_credentials")]
        public string GrantType { get; set; } = null!;
        public string ClientId { get; set; } = null!;
        public string ClientSecret { get; set; } = null!;
    }
}
