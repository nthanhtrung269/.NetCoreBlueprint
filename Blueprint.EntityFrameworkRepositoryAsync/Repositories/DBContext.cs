using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blueprint.EntityFrameworkRepositoryAsync
{
    public partial class DBContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly IDomainEventService _domainEventService;

        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options,
            ILoggerFactory loggerFactory,
            IDomainEventService domainEventService)
            : base(options)
        {
            _loggerFactory = loggerFactory;
            _domainEventService = domainEventService;
        }

        public virtual DbSet<BlueprintFile> BlueprintFile { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            var entities = ChangeTracker.Entries().Where(t => t.State == EntityState.Modified || t.State == EntityState.Added).ToArray();

            foreach (var entity in entities)
            {
                switch (entity.State)
                {
                    case EntityState.Added:
                        entity.CurrentValues[nameof(AuditableEntity<object>.CreatedBy)] = "Admin";
                        entity.CurrentValues[nameof(AuditableEntity<object>.CreatedDate)] = DateTime.UtcNow;
                        break;

                    case EntityState.Modified:
                        entity.CurrentValues[nameof(AuditableEntity<object>.ModifiedBy)] = "Admin";
                        entity.CurrentValues[nameof(AuditableEntity<object>.ModifiedDate)] = DateTime.UtcNow;
                        break;
                }
            }

            int result = await base.SaveChangesAsync(cancellationToken);

            await DispatchEvents();

            return result;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<BlueprintFile>(entity =>
            {
                entity.ToTable("BlueprintFile");

                entity.Property(e => e.CloudUrl).HasMaxLength(500);

                entity.Property(e => e.CompanyId)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.CreatedBy).HasMaxLength(50);

                entity.Property(e => e.CreatedDate).HasColumnType("datetime");

                entity.Property(e => e.Extension).HasMaxLength(10);

                entity.Property(e => e.FilePath).HasMaxLength(500);

                entity.Property(e => e.FileType)
                    .HasMaxLength(50)
                    .IsUnicode(false);

                entity.Property(e => e.ModifiedBy).HasMaxLength(50);

                entity.Property(e => e.ModifiedDate).HasColumnType("datetime");

                entity.Property(e => e.OriginalFileName).HasMaxLength(256);

                entity.Property(e => e.Source)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .HasDefaultValueSql("((0))");

                entity.Property(e => e.ThumbnailPath).HasMaxLength(500);
            });
        }

        private async Task DispatchEvents()
        {
            var domainEventEntities = ChangeTracker.Entries<IHasDomainEvent>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .ToArray();

            foreach (var domainEvent in domainEventEntities)
            {
                await _domainEventService.Publish(domainEvent);
            }
        }
    }
}
