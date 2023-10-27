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
            modelBuilder.Entity<TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER>();
        }

        public DbSet<TRANS_MW_ERC_PM_TN_RADIO_LINK_POWER> TRANS_MW_ERC_PM_TN_RADIO_LINK_POWERs { get; set; }
    }
}
