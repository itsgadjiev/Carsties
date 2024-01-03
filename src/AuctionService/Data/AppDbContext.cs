using AuctionService.Entities;
using Microsoft.EntityFrameworkCore;

namespace AuctionService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions dbContextOptions) :base(dbContextOptions) 
        {
            
        }

        public DbSet<Auction> Auctions { get; set; }
        public DbSet<Item> Items { get; set; }
    }
}
