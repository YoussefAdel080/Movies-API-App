using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Movies.Application.Contexts;

public class MovieContextFactory : IDesignTimeDbContextFactory<MovieContext>
{
    public MovieContext CreateDbContext(string[] args)
    {
        var hostAppBase = ResolveMoviesHostDirectory();
        var configuration = BuildConfiguration(hostAppBase);
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? "Server=(localdb)\\MSSQLLocalDB;Database=MoviesDB;Trusted_Connection=True;";

        var options = new DbContextOptionsBuilder<MovieContext>()
            .UseSqlServer(connectionString)
            .Options;

        return new MovieContext(options);
    }

    static IConfiguration BuildConfiguration(string basePath)
    {
        return new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();
    }

    /// <summary>
    /// Resolves the ASP.NET Core host directory so appsettings.json is found when running
    /// <c>dotnet ef</c> from the class library folder or the solution root.
    /// </summary>
    static string ResolveMoviesHostDirectory()
    {
        var cwd = Directory.GetCurrentDirectory();

        foreach (var candidate in new[] { cwd, Path.Combine(cwd, "Movies") })
        {
            if (File.Exists(Path.Combine(candidate, "appsettings.json")))
                return Path.GetFullPath(candidate);
        }

        var dir = new DirectoryInfo(cwd);
        for (var depth = 0; depth < 12 && dir is not null; depth++, dir = dir.Parent!)
        {
            var moviesAppsettings = Path.Combine(dir.FullName, "Movies", "appsettings.json");
            if (File.Exists(moviesAppsettings))
                return Path.Combine(dir.FullName, "Movies");
        }

        return cwd;
    }
}
