using System.IO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Database.Shared
{
    public class ContextFactory : IDesignTimeDbContextFactory<Context>
    {
        public ContextFactory ()
        {

        }

        private IConfiguration Configuration =>  new ConfigurationBuilder().SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

            public Context CreateDbContext(string[] args)
        {
            
            var builder = new DbContextOptionsBuilder<Context>();
            builder.UseNpgsql(Configuration.GetConnectionString("farmaowl"));

            return new Context(builder.Options);
        }
    }
}