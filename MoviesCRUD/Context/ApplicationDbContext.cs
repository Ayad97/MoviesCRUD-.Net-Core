using Microsoft.EntityFrameworkCore;
using MoviesCRUD.Models;
using MoviesCRUD.MoviesViewModel;

namespace MoviesCRUD.Context
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { 
           
        }
        public DbSet<Movies> Movies { get; set; }
        public DbSet<Genre> Genres { get; set; }
    
    }
}
