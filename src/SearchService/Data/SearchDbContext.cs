using Microsoft.EntityFrameworkCore;
using SearchService.Entities;

namespace SearchService.Data
{
    public class SearchDbContext : DbContext
    {
        public SearchDbContext(DbContextOptions dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Item> Items { get; set; }
    }
}
