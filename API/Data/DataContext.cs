using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data;

public class DataContext : DbContext
{
     // configure database connection
    public DataContext(DbContextOptions options) : base(options){ }

    // entity (table) in the database
    public DbSet<AppUser> Users { get; set; } 
}