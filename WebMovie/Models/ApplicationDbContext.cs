using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
namespace WebMovie.Models
{
        public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
        {
            public ApplicationDbContext(DbContextOptions<ApplicationDbContext>
            options) : base(options)
            { }
            public DbSet<Movie> Movies { get; set; }
            public DbSet<Genre> Genres { get; set; }
            public DbSet<Country> Countries { get; set; }
        }
}
