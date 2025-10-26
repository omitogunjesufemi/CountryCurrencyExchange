using Microsoft.EntityFrameworkCore;

namespace WebAPI.Models
{
    public class CountryDBContext : DbContext
    {
        public CountryDBContext(DbContextOptions<CountryDBContext> options) : base(options)
        {
        }

        public DbSet<Country> Countries { get; set; }
    }
}
