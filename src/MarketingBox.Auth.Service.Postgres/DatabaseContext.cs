using MarketingBox.Auth.Service.Domain.Models;
using Microsoft.EntityFrameworkCore;
using MyJetWallet.Sdk.Postgres;

namespace MarketingBox.Auth.Service.Postgres
{
    public class DatabaseContext : MyDbContext
    {
        public const string Schema = "auth-service";

        private const string UserTableName = "user";
        private const string UserLogTableName = "user-log";

        public DbSet<User> Users { get; set; }
        public DbSet<UserLog> UserLogs { get; set; }


        public DatabaseContext(DbContextOptions options) : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (LoggerFactory != null)
            {
                optionsBuilder.UseLoggerFactory(LoggerFactory).EnableSensitiveDataLogging();
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema(Schema);

            SetUserEntity(modelBuilder);
            SetUserLogEntity(modelBuilder);

            base.OnModelCreating(modelBuilder);
        }

        private void SetUserLogEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserLog>().ToTable(UserLogTableName);
            modelBuilder.Entity<UserLog>().HasKey(x => x.Id);
        }

        private void SetUserEntity(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable(UserTableName);
            
            modelBuilder.Entity<User>()
                .HasKey(e => new {e.TenantId, Email = e.ExternalUserId });
            modelBuilder.Entity<User>()
                .HasIndex(e => new { e.TenantId, e.Username })
                .IsUnique();
            modelBuilder.Entity<User>()
                .HasIndex(e => new { e.TenantId, e.EmailEncrypted })
                .IsUnique();
        }
    }
}
