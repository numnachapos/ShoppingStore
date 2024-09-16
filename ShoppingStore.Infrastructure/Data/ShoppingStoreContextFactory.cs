using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ShoppingStore.Infrastructure.Data
{
    public class ShoppingStoreContextFactory : IDesignTimeDbContextFactory<ShoppingStoreContext>
    {
        public ShoppingStoreContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ShoppingStoreContext>();

            // Get configuration from appsettings.json in the ShoppingStore.Presentation project
            IConfigurationRoot configuration = new ConfigurationBuilder()
                .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "../ShoppingStore.Presentation"))
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = configuration.GetConnectionString("DefaultConnection");

            optionsBuilder.UseSqlServer(connectionString);

            return new ShoppingStoreContext(optionsBuilder.Options);
        }
    }
}