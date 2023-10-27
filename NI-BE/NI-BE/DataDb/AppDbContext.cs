using Microsoft.EntityFrameworkCore;
using NI_BE.DataDb.Models;

namespace NI_BE.DataDb
{
    public class AppDbContext :DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<SOEM1_TN>();
        }

        public DbSet<SOEM1_TN> SOEM1_TNs { get; set; }
    }
}
