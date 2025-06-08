using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace api_hotel.Models;

public partial class HotelContext : DbContext
{
    public HotelContext()
    {
    }

    public HotelContext(DbContextOptions<HotelContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Guest> Guests { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Room> Rooms { get; set; }

    public virtual DbSet<Service> Services { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("bookings_pkey");

            entity.ToTable("bookings");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.EndDate).HasColumnName("end_date");
            entity.Property(e => e.GuestId).HasColumnName("guest_id");
            entity.Property(e => e.RoomNumber).HasColumnName("room_number");
            entity.Property(e => e.Services)
                .HasDefaultValueSql("ARRAY[]::integer[]")
                .HasColumnName("services");
            entity.Property(e => e.StartDate).HasColumnName("start_date");
            entity.Property(e => e.TotalPrice)
                .HasPrecision(10, 2)
                .HasDefaultValueSql("0.00")
                .HasColumnName("total_price");

            entity.HasOne(d => d.Guest).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.GuestId)
                .HasConstraintName("bookings_guest_id_fkey");

            entity.HasOne(d => d.RoomNumberNavigation).WithMany(p => p.Bookings)
                .HasForeignKey(d => d.RoomNumber)
                .HasConstraintName("bookings_room_number_fkey");
        });

        modelBuilder.Entity<Guest>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("guests_pkey");

            entity.ToTable("guests");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Discount)
                .HasDefaultValueSql("'None'::text")
                .HasColumnName("discount");
            entity.Property(e => e.FullNameOrOrganization).HasColumnName("full_name_or_organization");
            entity.Property(e => e.IsOrganization)
                .HasDefaultValue(false)
                .HasColumnName("is_organization");
            entity.Property(e => e.Phone).HasColumnName("phone");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("roles_pkey");

            entity.ToTable("roles");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Position).HasColumnName("position");
            entity.Property(e => e.RoleName).HasColumnName("role_name");
        });

        modelBuilder.Entity<Room>(entity =>
        {
            entity.HasKey(e => e.RoomNumber).HasName("rooms_pkey");

            entity.ToTable("rooms");

            entity.Property(e => e.RoomNumber)
                .ValueGeneratedNever()
                .HasColumnName("room_number");
            entity.Property(e => e.Capacity).HasColumnName("capacity");
            entity.Property(e => e.Category).HasColumnName("category");
            entity.Property(e => e.Floor).HasColumnName("floor");
            entity.Property(e => e.PricePerDay)
                .HasPrecision(10, 2)
                .HasColumnName("price_per_day");
            entity.Property(e => e.RequestDetails)
                .HasDefaultValueSql("''::text")
                .HasColumnName("request_details");
        });

        modelBuilder.Entity<Service>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("services_pkey");

            entity.ToTable("services");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Price)
                .HasPrecision(10, 2)
                .HasColumnName("price");
            entity.Property(e => e.ServiceName).HasColumnName("service_name");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Login, "users_login_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.FullName).HasColumnName("full_name");
            entity.Property(e => e.Login).HasColumnName("login");
            entity.Property(e => e.Password).HasColumnName("password");
            entity.Property(e => e.RoleId).HasColumnName("role_id");

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("users_role_id_fkey");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
