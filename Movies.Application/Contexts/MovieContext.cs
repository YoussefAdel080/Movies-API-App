using Microsoft.EntityFrameworkCore;
using Movies.Application.Models;

namespace Movies.Application.Contexts;

public class MovieContext : DbContext
{
    public MovieContext(DbContextOptions<MovieContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
}
