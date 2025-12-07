using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace gentech_services.Data
{
    public class GentechDbContextFactory : IDesignTimeDbContextFactory<GentechDbContext>
    {
        public GentechDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<GentechDbContext>();

            // Use SQLite database file in the application directory
            optionsBuilder.UseSqlite("Data Source=gentech.db");

            return new GentechDbContext(optionsBuilder.Options);
        }
    }
}
