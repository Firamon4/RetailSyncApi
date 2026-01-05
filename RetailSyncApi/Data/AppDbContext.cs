using Microsoft.EntityFrameworkCore;
using RetailSyncApi.Models;

namespace RetailSyncApi.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<SyncPackage> SyncPackages { get; set; }
    }
}