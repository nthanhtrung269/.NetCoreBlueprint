using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blueprint.CustomExceptionHandlerMiddleware.Project.DataModels
{
    public partial class BlueprintContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public BlueprintContext()
        {
        }

        public BlueprintContext(DbContextOptions<BlueprintContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        public virtual DbSet<TransactionLog> TransactionLog { get; set; }
        public virtual DbSet<Setting> Setting { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<TransactionLog>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("TransactionLogId");

                entity.HasKey(e => e.Id).HasName("PK_TransactionLog");

                entity.ToTable("TransactionLog");

                entity.Property(e => e.TransactionName).HasMaxLength(50);

                entity.Property(e => e.Status).HasMaxLength(20);

                entity.Property(e => e.ExceptionMessage).HasMaxLength(200);

                entity.Property(e => e.HostName).HasMaxLength(50);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);
            });

            modelBuilder.Entity<Setting>(entity =>
            {
                entity.ToTable("Setting");

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(500);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.SettingName).HasMaxLength(100);
            });
        }
    }
}
