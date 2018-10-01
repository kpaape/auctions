using Microsoft.EntityFrameworkCore;
 
namespace FinalExam.Models
{
    public class FinalContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public FinalContext(DbContextOptions<FinalContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Auction> Auctions { get; set; }

    }
}