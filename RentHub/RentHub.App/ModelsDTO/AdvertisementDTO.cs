namespace RentHub.API.ModelsDTO
{
    public class AdvertisementDTO
    {
        public int AdvertisementId { get; set; }

        public int FlatId { get; set; }

        public int PlatformId { get; set; }

        public string RentType { get; set; } = null!;

        public decimal PriceForPeriod { get; set; }

        public decimal IncomeForPeriod { get; set; }

        public string LinkToAdvertisement { get; set; } = null!;
    }
}
