namespace RentHub.API.ModelsDTO
{
    public class RenterDTO
    {
        public int RenterId { get; set; }

        public string Name { get; set; } = null!;

        public string Lastname { get; set; } = null!;

        public string? Patronymic { get; set; }

        public string PhoneNumber { get; set; } = null!;
    }
}
