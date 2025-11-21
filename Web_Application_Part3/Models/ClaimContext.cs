
using Microsoft.EntityFrameworkCore;
using Web_Application_Part3.Models;

namespace Web_Application_Part3.Data
{
    public class ClaimContext : DbContext
    {
        public ClaimContext(DbContextOptions<ClaimContext> options) : base(options)
        {
        }

        public DbSet<Claim> Claims { get; set; }
    }
}

