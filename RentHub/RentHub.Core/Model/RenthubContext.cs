using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace RentHub.Core.Model;

public partial class RentHubContext : DbContext
{
    public static RentHubContext Instance { get; private set; } = new();

    public RentHubContext()
    {
    }

    public RentHubContext(DbContextOptions<RentHubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Advertisement> Advertisements { get; set; }

    public virtual DbSet<Flat> Flats { get; set; }

    public virtual DbSet<PlacementPlatform> PlacementPlatforms { get; set; }

    public virtual DbSet<Renter> Renters { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var filePath = Path.GetFullPath(
            Path.Combine("..", "RentHub.Core", "AppSettings", "appsettings.Development.json"));

        var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory).AddJsonFile(filePath, optional: true).Build();

        var smtpSection = config.GetSection("MySqlConnectionSettings");
        string server = smtpSection["server"]!;
        string user = smtpSection["user"]!;
        string password = smtpSection["password"]!;
        string database = smtpSection["database"]!;

        optionsBuilder.UseMySql(
            $"server={server};user={user};password={password};database={database}",
            ServerVersion.Parse("8.0.41-mysql"));
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Advertisement>(entity =>
        {
            entity.HasKey(e => e.AdvertisementId).HasName("PRIMARY");

            entity.ToTable("advertisements");

            entity.HasIndex(e => e.FlatId, "fk_flat_id_idx");

            entity.HasIndex(e => e.PlatformId, "fk_platform_id_idx");

            entity.Property(e => e.AdvertisementId).HasColumnName("advertisement_id");
            entity.Property(e => e.FlatId).HasColumnName("flat_id");
            entity.Property(e => e.IncomeForPeriod).HasColumnName("income_for_period");
            entity.Property(e => e.LinkToAdvertisement).HasColumnName("link_to_advertisement");
            entity.Property(e => e.PlatformId).HasColumnName("platform_id");
            entity.Property(e => e.PriceForPeriod).HasColumnName("price_for_period");
            entity.Property(e => e.RentType)
                .HasColumnType("enum('Посуточно','Длительный период')")
                .HasColumnName("rent_type");

            entity.HasOne(d => d.Flat).WithMany(p => p.Advertisements)
                .HasForeignKey(d => d.FlatId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_flat_id");

            entity.HasOne(d => d.Platform).WithMany(p => p.Advertisements)
                .HasForeignKey(d => d.PlatformId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_platform_id");
        });

        modelBuilder.Entity<Flat>(entity =>
        {
            entity.HasKey(e => e.FlatId).HasName("PRIMARY");

            entity.ToTable("flats");

            entity.HasIndex(e => e.UserId, "user_id_fk_idx");

            entity.Property(e => e.FlatId).HasColumnName("flat_id");
            entity.Property(e => e.ApartmentNumber)
                .HasMaxLength(5)
                .HasColumnName("apartment_number");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.District)
                .HasMaxLength(100)
                .HasColumnName("district");
            entity.Property(e => e.FloorNumber).HasColumnName("floor_number");
            entity.Property(e => e.FloorsNumber).HasColumnName("floors_number");
            entity.Property(e => e.HouseNumber)
                .HasMaxLength(10)
                .HasColumnName("house_number");
            entity.Property(e => e.Photo).HasColumnName("photo");
            entity.Property(e => e.RoomCount).HasColumnName("room_count");
            entity.Property(e => e.Size)
                .HasPrecision(6, 2)
                .HasColumnName("size");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.Flats)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("user_id_fk");
        });

        modelBuilder.Entity<PlacementPlatform>(entity =>
        {
            entity.HasKey(e => e.PlatformId).HasName("PRIMARY");

            entity.ToTable("placement_platforms");

            entity.Property(e => e.PlatformId).HasColumnName("platform_id");
            entity.Property(e => e.PlatformName)
                .HasMaxLength(100)
                .HasColumnName("platform_name");
        });

        modelBuilder.Entity<Renter>(entity =>
        {
            entity.HasKey(e => e.RenterId).HasName("PRIMARY");

            entity.ToTable("renters");

            entity.Property(e => e.RenterId).HasColumnName("renter_id");
            entity.Property(e => e.Lastname)
                .HasMaxLength(60)
                .HasColumnName("lastname");
            entity.Property(e => e.Name)
                .HasMaxLength(60)
                .HasColumnName("name");
            entity.Property(e => e.Patronymic)
                .HasMaxLength(60)
                .HasColumnName("patronymic");
            entity.Property(e => e.PhoneNumber)
                .HasMaxLength(16)
                .HasComment("+7 900 304 93 12")
                .HasColumnName("phone_number");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PRIMARY");

            entity.ToTable("reservations");

            entity.HasIndex(e => e.AdvertisementId, "fk_ad_id_idx");

            entity.HasIndex(e => e.RenterId, "fk_renter_id_idx");

            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.AdvertisementId).HasColumnName("advertisement_id");
            entity.Property(e => e.DateOfEndReservation).HasColumnName("date_of_end_reservation");
            entity.Property(e => e.DateOfStartReservation).HasColumnName("date_of_start_reservation");
            entity.Property(e => e.Income)
                .HasPrecision(10, 2)
                .HasColumnName("income");
            entity.Property(e => e.RenterId).HasColumnName("renter_id");
            entity.Property(e => e.Summ)
                .HasPrecision(10, 2)
                .HasColumnName("summ");

            entity.HasOne(d => d.Advertisement).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.AdvertisementId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ad_id");

            entity.HasOne(d => d.Renter).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.RenterId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_renter_id");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PRIMARY");

            entity.ToTable("users");

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .HasColumnName("email");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .HasColumnName("password");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
