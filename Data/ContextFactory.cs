using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Data
{
    public class ContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            const Environment.SpecialFolder
                folder = Environment.SpecialFolder.LocalApplicationData;
            var path = Environment.GetFolderPath(folder);
            var dbPath = Path.Join(path, "app.db");

            var builder = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite($"Data Source={dbPath}");

            return new ApplicationDbContext(builder.Options);
        }
    }
}