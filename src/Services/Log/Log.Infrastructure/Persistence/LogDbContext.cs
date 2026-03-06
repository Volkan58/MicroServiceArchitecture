using Log.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace Log.Infrastructure.Persistence
{
    public class LogDbContext : DbContext
    {
        public LogDbContext(DbContextOptions<LogDbContext> options) : base(options)
        {
        }

        public DbSet<LogEntry> LogEntries { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.Property(e => e.ServiceName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.HasIndex(e => e.ServiceName);

                entity.Property(e => e.Level)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.HasIndex(e => e.Level);

                entity.Property(e => e.Message)
                    .IsRequired();

                entity.Property(e => e.Exception);

                entity.Property(e => e.StackTrace);

                entity.Property(e => e.Properties)
                    .HasConversion(
                        v => JsonConvert.SerializeObject(v),
                        v => JsonConvert.DeserializeObject<Dictionary<string, string>>(v) ?? new Dictionary<string, string>()
                    );

                entity.Property(e => e.Timestamp)
                    .IsRequired();

                entity.HasIndex(e => e.Timestamp);
            });
        }
    }
}
