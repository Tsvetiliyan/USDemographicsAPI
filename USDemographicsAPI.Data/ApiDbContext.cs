using Microsoft.EntityFrameworkCore;
using USDemographicsAPI.Core.DomainModels;

namespace USDemographicsAPI.Data;

public class ApiDbContext : DbContext
{
    public ApiDbContext(DbContextOptions<ApiDbContext> options) : base(options)
    {
        
    }

    public DbSet<County> Counties { get; set; }
    public DbSet<State> States { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);


        //The state's name and state abbreviation has to be unique, as well as fips (same as county fip)

        modelBuilder.Entity<State>().HasIndex(c => c.StateName).IsUnique();
        modelBuilder.Entity<State>().HasIndex(c => c.StateAbbreviation).IsUnique();



    }

}
