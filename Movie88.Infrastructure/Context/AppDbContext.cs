using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Movie88.Infrastructure.Entities;

namespace Movie88.Infrastructure.Context;

public partial class AppDbContext : DbContext
{
    public AppDbContext()
    {
    }

    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Auditorium> Auditoriums { get; set; }

    public virtual DbSet<Booking> Bookings { get; set; }

    public virtual DbSet<Bookingcombo> Bookingcombos { get; set; }

    public virtual DbSet<Bookingpromotion> Bookingpromotions { get; set; }

    public virtual DbSet<Bookingseat> Bookingseats { get; set; }

    public virtual DbSet<Cinema> Cinemas { get; set; }

    public virtual DbSet<Combo> Combos { get; set; }

    public virtual DbSet<Customer> Customers { get; set; }

    public virtual DbSet<Movie> Movies { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Paymentmethod> Paymentmethods { get; set; }

    public virtual DbSet<Promotion> Promotions { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<UserRefreshToken> UserRefreshTokens { get; set; }

    public virtual DbSet<Review> Reviews { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<Seat> Seats { get; set; }

    public virtual DbSet<Showtime> Showtimes { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<OtpToken> OtpTokens { get; set; }

    public virtual DbSet<Voucher> Vouchers { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Configuration from appsettings.json via ServiceExtensions
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .HasPostgresEnum("auth", "aal_level", new[] { "aal1", "aal2", "aal3" })
            .HasPostgresEnum("auth", "code_challenge_method", new[] { "s256", "plain" })
            .HasPostgresEnum("auth", "factor_status", new[] { "unverified", "verified" })
            .HasPostgresEnum("auth", "factor_type", new[] { "totp", "webauthn", "phone" })
            .HasPostgresEnum("auth", "oauth_authorization_status", new[] { "pending", "approved", "denied", "expired" })
            .HasPostgresEnum("auth", "oauth_client_type", new[] { "public", "confidential" })
            .HasPostgresEnum("auth", "oauth_registration_type", new[] { "dynamic", "manual" })
            .HasPostgresEnum("auth", "oauth_response_type", new[] { "code" })
            .HasPostgresEnum("auth", "one_time_token_type", new[] { "confirmation_token", "reauthentication_token", "recovery_token", "email_change_token_new", "email_change_token_current", "phone_change_token" })
            .HasPostgresEnum("realtime", "action", new[] { "INSERT", "UPDATE", "DELETE", "TRUNCATE", "ERROR" })
            .HasPostgresEnum("realtime", "equality_op", new[] { "eq", "neq", "lt", "lte", "gt", "gte", "in" })
            .HasPostgresEnum("storage", "buckettype", new[] { "STANDARD", "ANALYTICS" })
            .HasPostgresExtension("extensions", "pg_stat_statements")
            .HasPostgresExtension("extensions", "pgcrypto")
            .HasPostgresExtension("extensions", "uuid-ossp")
            .HasPostgresExtension("graphql", "pg_graphql")
            .HasPostgresExtension("vault", "supabase_vault");

        modelBuilder.Entity<Auditorium>(entity =>
        {
            entity.HasKey(e => e.Auditoriumid).HasName("auditoriums_pkey");

            entity.HasOne(d => d.Cinema).WithMany(p => p.Auditoria)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("auditoriums_cinemaid_fkey");
        });

        modelBuilder.Entity<Booking>(entity =>
        {
            entity.HasKey(e => e.Bookingid).HasName("bookings_pkey");

            entity.Property(e => e.Bookingtime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValueSql("'Pending'::character varying");

            entity.HasOne(d => d.Customer).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookings_customerid_fkey");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Bookings)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookings_showtimeid_fkey");

            entity.HasOne(d => d.Voucher).WithMany(p => p.Bookings).HasConstraintName("bookings_voucherid_fkey");

            entity.HasOne(d => d.CheckedInByUser).WithMany(p => p.BookingsCheckedInBy)
                .HasForeignKey(d => d.Checkedinby)
                .OnDelete(DeleteBehavior.SetNull)
                .HasConstraintName("fk_bookings_checkedinby_users");
        });

        modelBuilder.Entity<Bookingcombo>(entity =>
        {
            entity.HasKey(e => e.Bookingcomboid).HasName("bookingcombos_pkey");

            entity.Property(e => e.Quantity).HasDefaultValue(1);

            entity.HasOne(d => d.Booking).WithMany(p => p.Bookingcombos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingcombos_bookingid_fkey");

            entity.HasOne(d => d.Combo).WithMany(p => p.Bookingcombos)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingcombos_comboid_fkey");
        });

        modelBuilder.Entity<Bookingpromotion>(entity =>
        {
            entity.HasKey(e => e.Bookingpromotionid).HasName("bookingpromotions_pkey");

            entity.HasOne(d => d.Booking).WithMany(p => p.Bookingpromotions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingpromotions_bookingid_fkey");

            entity.HasOne(d => d.Promotion).WithMany(p => p.Bookingpromotions)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingpromotions_promotionid_fkey");
        });

        modelBuilder.Entity<Bookingseat>(entity =>
        {
            entity.HasKey(e => e.Bookingseatid).HasName("bookingseats_pkey");

            entity.HasOne(d => d.Booking).WithMany(p => p.Bookingseats)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingseats_bookingid_fkey");

            entity.HasOne(d => d.Seat).WithMany(p => p.Bookingseats)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingseats_seatid_fkey");

            entity.HasOne(d => d.Showtime).WithMany(p => p.Bookingseats)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("bookingseats_showtimeid_fkey");
        });

        modelBuilder.Entity<Cinema>(entity =>
        {
            entity.HasKey(e => e.Cinemaid).HasName("cinemas_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Combo>(entity =>
        {
            entity.HasKey(e => e.Comboid).HasName("combos_pkey");
        });

        modelBuilder.Entity<Customer>(entity =>
        {
            entity.HasKey(e => e.Customerid).HasName("customers_pkey");

            entity.HasOne(d => d.User).WithOne(p => p.Customer)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("customers_userid_fkey");
        });

        modelBuilder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Movieid).HasName("movies_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Rating).HasDefaultValueSql("'P'::character varying");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.Paymentid).HasName("payments_pkey");

            entity.Property(e => e.Paymenttime).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Status).HasDefaultValueSql("'Pending'::character varying");

            entity.HasOne(d => d.Booking).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_bookingid_fkey");

            entity.HasOne(d => d.Customer).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_customerid_fkey");

            entity.HasOne(d => d.Method).WithMany(p => p.Payments)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("payments_methodid_fkey");
        });

        modelBuilder.Entity<Paymentmethod>(entity =>
        {
            entity.HasKey(e => e.Methodid).HasName("paymentmethods_pkey");
        });

        modelBuilder.Entity<Promotion>(entity =>
        {
            entity.HasKey(e => e.Promotionid).HasName("promotions_pkey");
        });

        modelBuilder.Entity<RefreshToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("refresh_tokens_pkey");

            entity.ToTable("refresh_tokens", "auth", tb => tb.HasComment("Auth: Store of tokens used to refresh JWT tokens once they expire."));
        });

        modelBuilder.Entity<Review>(entity =>
        {
            entity.HasKey(e => e.Reviewid).HasName("reviews_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");

            entity.HasOne(d => d.Customer).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviews_customerid_fkey");

            entity.HasOne(d => d.Movie).WithMany(p => p.Reviews)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("reviews_movieid_fkey");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.Roleid).HasName("roles_pkey");
        });

        modelBuilder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.Seatid).HasName("seats_pkey");

            entity.Property(e => e.Isavailable).HasDefaultValue(true);
            entity.Property(e => e.Row).IsFixedLength();
            entity.Property(e => e.Type).HasDefaultValueSql("'Standard'::character varying");

            entity.HasOne(d => d.Auditorium).WithMany(p => p.Seats)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("seats_auditoriumid_fkey");
        });

        modelBuilder.Entity<Showtime>(entity =>
        {
            entity.HasKey(e => e.Showtimeid).HasName("showtimes_pkey");

            entity.Property(e => e.Format).HasDefaultValueSql("'2D'::character varying");
            entity.Property(e => e.Languagetype).HasDefaultValueSql("'Original - Vietsub'::character varying");

            entity.HasOne(d => d.Auditorium).WithMany(p => p.Showtimes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("showtimes_auditoriumid_fkey");

            entity.HasOne(d => d.Movie).WithMany(p => p.Showtimes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("showtimes_movieid_fkey");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Userid).HasName("User_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Isverified).HasDefaultValue(false);
            entity.Property(e => e.Isactive).HasDefaultValue(true);

            entity.HasOne(d => d.Role).WithMany(p => p.Users)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("User_roleid_fkey");
        });

        modelBuilder.Entity<OtpToken>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("otp_tokens_pkey");

            entity.Property(e => e.Createdat).HasDefaultValueSql("CURRENT_TIMESTAMP");
            entity.Property(e => e.Isused).HasDefaultValue(false);

            entity.HasOne(d => d.User).WithMany(p => p.OtpTokens)
                .OnDelete(DeleteBehavior.Cascade)
                .HasConstraintName("fk_otp_userid");

            // Unique constraint on (otpcode, otptype, email)
            entity.HasIndex(e => new { e.Otpcode, e.Otptype, e.Email })
                .IsUnique()
                .HasDatabaseName("idx_otp_code_type");

            // Check constraints
            entity.ToTable(t => t.HasCheckConstraint("chk_otp_code_length", "LENGTH(otpcode) = 6"));
            entity.ToTable(t => t.HasCheckConstraint("chk_otp_type", "otptype IN ('EmailVerification', 'PasswordReset', 'Login')"));
        });

        modelBuilder.Entity<Voucher>(entity =>
        {
            entity.HasKey(e => e.Voucherid).HasName("vouchers_pkey");

            entity.Property(e => e.Isactive).HasDefaultValue(true);
            entity.Property(e => e.Usedcount).HasDefaultValue(0);
        });

        // Apply custom configurations
        modelBuilder.ApplyConfiguration(new EntitiesConfiguration.BookingConfiguration());

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
