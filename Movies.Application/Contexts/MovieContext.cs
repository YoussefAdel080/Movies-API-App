using Microsoft.EntityFrameworkCore;
using Movies.Application.Models;

namespace Movies.Application.Contexts;

public class MovieContext : DbContext
{
    public MovieContext(DbContextOptions<MovieContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Rating>()
            .HasKey(r => new { r.UserId, r.MovieId });

        modelBuilder.Entity<Rating>()
            .HasOne(r => r.Movie)
            .WithMany()
            .HasForeignKey(r => r.MovieId);
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Genre> Genres { get; set; }
    public DbSet<Rating> Ratings { get; set; }
}
