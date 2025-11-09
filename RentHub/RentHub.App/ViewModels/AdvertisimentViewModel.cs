namespace RentHub.App.ViewModels
{
    public class AdvertisimentViewModel
    {
        public int AdvertisementId { get; set; }

        public int FlatId { get; set; }

        public int PlatformId { get; set; }

        public string RentType { get; set; } = null!;

        public double PriceForPeriod { get; set; }

        public double IncomeForPeriod { get; set; }

        public string LinkToAdvertisement { get; set; } = null!;
    }
}
