using ExhibitionApp.Models;
using Microsoft.DotNet.Scaffolding.Shared.Project;
using Microsoft.EntityFrameworkCore;

namespace ExhibitionApp.Data
{
    public sealed class ExhibitionAppDbContext : DbContext
    {
        public DbSet<Exhibition> Exhibitions { get; set; }
        public DbSet<Exhibit> Exhibits { get; set; }
        public DbSet<ExhibitType> ExhibitTypes { get; set; }
        public DbSet<Address> Addresses { get; set; }
        public DbSet<Author> Authors { get; set; }
        public DbSet<Sex> Sexes { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Country> Countries { get; set; }
        public DbSet<Street> Streets { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<User> Users { get; set; }

        public ExhibitionAppDbContext(DbContextOptions<ExhibitionAppDbContext> options) : base(options) 
        {
            //Database.EnsureDeleted();
            //Database.EnsureCreated();

            //DbInitializer.Initialize(this);
        }
    }
}
