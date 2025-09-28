using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Pomelo.EntityFrameworkCore.MySql.Scaffolding.Internal;

namespace RentHub.Core.Model;

public partial class RenthubContext : DbContext
{
    public RenthubContext()
    {
    }

    public RenthubContext(DbContextOptions<RenthubContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ad> Ads { get; set; }

    public virtual DbSet<Flat> Flats { get; set; }

    public virtual DbSet<PlacementPlatform> PlacementPlatforms { get; set; }

    public virtual DbSet<Renter> Renters { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

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

        modelBuilder.Entity<Ad>(entity =>
        {
            entity.HasKey(e => e.IdAd).HasName("PRIMARY");

            entity.ToTable("ads");

            entity.HasIndex(e => e.IdFlat, "fk_id_flat_idx");

            entity.HasIndex(e => e.IdPlatform, "fk_id_platform_idx");

            entity.Property(e => e.IdAd).HasColumnName("id_ad");
            entity.Property(e => e.IdFlat).HasColumnName("id_flat");
            entity.Property(e => e.IdPlatform).HasColumnName("id_platform");
            entity.Property(e => e.IncomeForPeriod)
                .HasPrecision(10, 2)
                .HasColumnName("income_for_period");
            entity.Property(e => e.LinkToAd).HasColumnName("link_to_ad");
            entity.Property(e => e.PriceForPeriod)
                .HasPrecision(10, 2)
                .HasColumnName("price_for_period");
            entity.Property(e => e.RentType)
                .HasColumnType("enum('Посуточно','Длительный период')")
                .HasColumnName("rent_type");

            entity.HasOne(d => d.IdFlatNavigation).WithMany(p => p.Ads)
                .HasForeignKey(d => d.IdFlat)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_id_flat");

            entity.HasOne(d => d.IdPlatformNavigation).WithMany(p => p.Ads)
                .HasForeignKey(d => d.IdPlatform)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_id_platform");
        });

        modelBuilder.Entity<Flat>(entity =>
        {
            entity.HasKey(e => e.IdFlat).HasName("PRIMARY");

            entity.ToTable("flats");

            entity.Property(e => e.IdFlat).HasColumnName("id_flat");
            entity.Property(e => e.City)
                .HasMaxLength(100)
                .HasColumnName("city");
            entity.Property(e => e.Country)
                .HasMaxLength(100)
                .HasColumnName("country");
            entity.Property(e => e.Description).HasColumnName("description");
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
        });

        modelBuilder.Entity<PlacementPlatform>(entity =>
        {
            entity.HasKey(e => e.IdPlatform).HasName("PRIMARY");

            entity.ToTable("placement_platforms");

            entity.Property(e => e.IdPlatform).HasColumnName("id_platform");
            entity.Property(e => e.PlatformName)
                .HasMaxLength(100)
                .HasColumnName("platform_name");
        });

        modelBuilder.Entity<Renter>(entity =>
        {
            entity.HasKey(e => e.IdRenter).HasName("PRIMARY");

            entity.ToTable("renters");

            entity.Property(e => e.IdRenter).HasColumnName("id_renter");
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
            entity.HasKey(e => e.IdReservation).HasName("PRIMARY");

            entity.ToTable("reservations");

            entity.HasIndex(e => e.IdAd, "fk_ad_id_idx");

            entity.HasIndex(e => e.IdRenter, "fk_renter_id_idx");

            entity.Property(e => e.IdReservation).HasColumnName("id_reservation");
            entity.Property(e => e.DateOfEndReservation).HasColumnName("date_of_end_reservation");
            entity.Property(e => e.DateOfStartReservation).HasColumnName("date_of_start_reservation");
            entity.Property(e => e.IdAd).HasColumnName("id_ad");
            entity.Property(e => e.IdRenter).HasColumnName("id_renter");
            entity.Property(e => e.Income)
                .HasPrecision(10, 2)
                .HasColumnName("income");
            entity.Property(e => e.Summ)
                .HasPrecision(10, 2)
                .HasColumnName("summ");

            entity.HasOne(d => d.IdAdNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.IdAd)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_ad_id");

            entity.HasOne(d => d.IdRenterNavigation).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.IdRenter)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("fk_renter_id");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
