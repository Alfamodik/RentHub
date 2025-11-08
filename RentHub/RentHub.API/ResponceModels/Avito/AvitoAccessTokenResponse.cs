namespace RentHub.API.ResponceModels.Avito
{
    public class AvitoAccessTokenResponse
    {
        public string AccessToken { get; set; } = null!;

        public int ExpiresIn { get; set; }

        public string RefreshToken { get; set; } = null!;

        public string Scope { get; set; } = null!;

        public string TokenType { get; set; } = null!;
    }
}
