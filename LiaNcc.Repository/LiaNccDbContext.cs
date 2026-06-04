using LiaNcc.Models.Entities;
using Microsoft.EntityFrameworkCore;

namespace LiaNcc.Repository
{
    public class LiaNccDbContext : DbContext
    {
        public LiaNccDbContext(DbContextOptions<LiaNccDbContext> options) : base(options)
        {
        }

        public DbSet<User> Users { get; set; } = null!;
        public DbSet<Role> Roles { get; set; } = null!;
        public DbSet<UserRole> UserRoles { get; set; } = null!;
        public DbSet<Language> Languages { get; set; } = null!;
        public DbSet<LocalizedContent> LocalizedContents { get; set; } = null!;
        public DbSet<SitePage> SitePages { get; set; } = null!;
        public DbSet<PageSection> PageSections { get; set; } = null!;
        public DbSet<CallToAction> CallToActions { get; set; } = null!;
        public DbSet<MediaAsset> MediaAssets { get; set; } = null!;
        public DbSet<EntityMedia> EntityMedia { get; set; } = null!;
        public DbSet<Service> Services { get; set; } = null!;
        public DbSet<VehicleCategory> VehicleCategories { get; set; } = null!;
        public DbSet<Vehicle> Vehicles { get; set; } = null!;
        public DbSet<VehicleFeature> VehicleFeatures { get; set; } = null!;
        public DbSet<TourCategory> TourCategories { get; set; } = null!;
        public DbSet<Tour> Tours { get; set; } = null!;
        public DbSet<TourSection> TourSections { get; set; } = null!;
        public DbSet<TourGalleryImage> TourGalleryImages { get; set; } = null!;
        public DbSet<TourInfoItem> TourInfoItems { get; set; } = null!;
        public DbSet<BookingServiceType> BookingServiceTypes { get; set; } = null!;
        public DbSet<BookingPassengerOption> BookingPassengerOptions { get; set; } = null!;
        public DbSet<Booking> Bookings { get; set; } = null!;
        public DbSet<ContactMessage> ContactMessages { get; set; } = null!;
        public DbSet<CompanyProfile> CompanyProfiles { get; set; } = null!;
        public DbSet<CompanyContact> CompanyContacts { get; set; } = null!;
        public DbSet<Partner> Partners { get; set; } = null!;
        public DbSet<ApplicationLog> ApplicationLogs { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Users
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.Property(e => e.PasswordHash).IsRequired().HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.HasIndex(e => e.Email).IsUnique();
            });

            // Roles
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Description).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");
                entity.HasIndex(e => e.Name).IsUnique();
            });

            // UserRoles
            modelBuilder.Entity<UserRole>(entity =>
            {
                entity.HasKey(e => new { e.UserId, e.RoleId });
                entity.Property(e => e.AssignedAt).HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasOne(e => e.User).WithMany(u => u.UserRoles).HasForeignKey(e => e.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.HasOne(e => e.Role).WithMany(r => r.UserRoles).HasForeignKey(e => e.RoleId).OnDelete(DeleteBehavior.Cascade);
            });

            // Localization
            modelBuilder.Entity<Language>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Code).IsRequired().HasMaxLength(10);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Code).IsUnique();
            });

            modelBuilder.Entity<LocalizedContent>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
                entity.Property(e => e.ContentKey).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Language).WithMany(l => l.LocalizedContents).HasForeignKey(e => e.LanguageCode).HasPrincipalKey(l => l.Code).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.EntityName, e.EntityId, e.LanguageCode, e.ContentKey });
            });

            // CMS
            modelBuilder.Entity<SitePage>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.MetaTitle).HasMaxLength(200);
                entity.Property(e => e.MetaDescription).HasMaxLength(500);
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            modelBuilder.Entity<PageSection>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.ImageUrl).HasMaxLength(1000);
                entity.HasOne(e => e.SitePage).WithMany(p => p.PageSections).HasForeignKey(e => e.PageId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<CallToAction>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Label).HasMaxLength(100);
                entity.Property(e => e.Title).HasMaxLength(200);
                entity.Property(e => e.Url).HasMaxLength(500);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.HasOne(e => e.SitePage).WithMany(p => p.CallToActions).HasForeignKey(e => e.PageId).OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(e => e.PageSection).WithMany(s => s.CallToActions).HasForeignKey(e => e.SectionId).OnDelete(DeleteBehavior.ClientSetNull);
            });

            // Media
            modelBuilder.Entity<MediaAsset>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.FileName).IsRequired().HasMaxLength(255);
                entity.Property(e => e.FileUrl).IsRequired().HasMaxLength(1000);
            });

            modelBuilder.Entity<EntityMedia>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.EntityName).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.MediaAsset).WithMany(m => m.EntityMedias).HasForeignKey(e => e.MediaAssetId).OnDelete(DeleteBehavior.Cascade);
                entity.HasIndex(e => new { e.EntityName, e.EntityId });
            });

            // Services
            modelBuilder.Entity<Service>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.CoverImageUrl).HasMaxLength(1000);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsBookable).HasDefaultValue(true);
            });

            // Vehicles
            modelBuilder.Entity<VehicleCategory>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Vehicle>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsBookable).HasDefaultValue(true);
                entity.HasOne(e => e.VehicleCategory).WithMany(c => c.Vehicles).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.Cascade);
            });

            modelBuilder.Entity<VehicleFeature>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.HasOne(e => e.Vehicle).WithMany(v => v.VehicleFeatures).HasForeignKey(e => e.VehicleId).OnDelete(DeleteBehavior.Cascade);
            });

            // Tours
            modelBuilder.Entity<TourCategory>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            });

            modelBuilder.Entity<Tour>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Slug).IsRequired().HasMaxLength(150);
                entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
                entity.Property(e => e.IsActive).HasDefaultValue(true);
                entity.Property(e => e.IsBookable).HasDefaultValue(true);
                entity.HasOne(e => e.TourCategory).WithMany(c => c.Tours).HasForeignKey(e => e.CategoryId).OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasIndex(e => e.Slug).IsUnique();
            });

            modelBuilder.Entity<TourSection>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.HasOne(e => e.Tour).WithMany(t => t.TourSections).HasForeignKey(e => e.TourId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<TourGalleryImage>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.HasOne(e => e.Tour).WithMany(t => t.TourGalleryImages).HasForeignKey(e => e.TourId).OnDelete(DeleteBehavior.Cascade);
            });
            modelBuilder.Entity<TourInfoItem>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.HasOne(e => e.Tour).WithMany(t => t.TourInfoItems).HasForeignKey(e => e.TourId).OnDelete(DeleteBehavior.Cascade);
            });

            // Bookings
            modelBuilder.Entity<BookingServiceType>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Code).IsUnique();
            });
            modelBuilder.Entity<BookingPassengerOption>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
            });
            modelBuilder.Entity<Booking>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(256);
                entity.HasOne(e => e.ServiceType).WithMany(s => s.Bookings).HasForeignKey(e => e.ServiceTypeId).OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(e => e.PassengerOption).WithMany(p => p.Bookings).HasForeignKey(e => e.PassengerOptionId).OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(e => e.Tour).WithMany(t => t.Bookings).HasForeignKey(e => e.TourId).OnDelete(DeleteBehavior.ClientSetNull);
                entity.HasOne(e => e.Vehicle).WithMany(v => v.Bookings).HasForeignKey(e => e.VehicleId).OnDelete(DeleteBehavior.ClientSetNull);
            });

            // ContactMessages
            modelBuilder.Entity<ContactMessage>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.FullName).IsRequired().HasMaxLength(200);
            });

            // Company
            modelBuilder.Entity<CompanyProfile>(entity => {
                entity.ToTable("CompanyProfile");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(200);
                entity.Property(e => e.Latitude).HasPrecision(10, 7);
                entity.Property(e => e.Longitude).HasPrecision(10, 7);
                entity.Property(e => e.GoogleMapsUrl).HasMaxLength(1000);
                entity.Property(e => e.AboutTitle).HasMaxLength(200);
                entity.Property(e => e.AboutDescription).HasColumnType("nvarchar(max)");
                entity.Property(e => e.AboutImageUrl).HasMaxLength(1000);
            });
            modelBuilder.Entity<CompanyContact>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.HasOne(e => e.CompanyProfile).WithMany(c => c.CompanyContacts).HasForeignKey(e => e.CompanyId).OnDelete(DeleteBehavior.Cascade);
            });

            // Partners
            modelBuilder.Entity<Partner>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Name).IsRequired().HasMaxLength(150);
            });

            // ApplicationLogs
            modelBuilder.Entity<ApplicationLog>(entity => {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Id).HasDefaultValueSql("NEWID()");
                entity.Property(e => e.Source).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Level).IsRequired().HasMaxLength(50);
                entity.Property(e => e.CreatedAt).HasDefaultValueSql("SYSUTCDATETIME()");

                entity.HasIndex(e => e.CreatedAt);
                entity.HasIndex(e => e.Level);
                entity.HasIndex(e => e.Source);
                entity.HasIndex(e => e.Area);
                entity.HasIndex(e => e.CorrelationId);
            });
        }
    }
}
