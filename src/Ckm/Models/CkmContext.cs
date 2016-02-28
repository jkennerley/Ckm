namespace Ckm.Models
{
    using System;
    using Microsoft.AspNet.Identity.EntityFramework;
    using Microsoft.Data.Entity;

    //public class CkmContext : DbContext
    public class CkmContext : IdentityDbContext<CkmUser>
    {
        public CkmContext()
        {
            Database.EnsureCreated();
        }

        public DbSet<Trip> Trips { get; set; }
        public DbSet<Stop> Stops { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connString = Startup.Configuration["Data:CkmContextConnection"];

            optionsBuilder.UseSqlServer(connString);

            base.OnConfiguring(optionsBuilder);
        }
    }
}

