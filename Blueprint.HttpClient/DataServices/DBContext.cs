using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Blueprint.HttpClient1.DataServices
{
    public partial class DBContext : DbContext
    {
        private readonly ILoggerFactory _loggerFactory;

        public DBContext()
        {
        }

        public DBContext(DbContextOptions<DBContext> options, ILoggerFactory loggerFactory)
            : base(options)
        {
            _loggerFactory = loggerFactory;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
            optionsBuilder.UseLoggerFactory(_loggerFactory);
        }
    }
}
