using System.Text.RegularExpressions;

namespace Movies.Application.Models;

public partial class Movie
{
    public required Guid Id { get; init; }

    public required string Title { get; set; }

    public string Slug => GenerateSlug();

    public required int YearOfRelease { get; set; }

    private string GenerateSlug()
    {
        var sluggedTitle = SlugRegex()
            .Replace(Title, string.Empty)
            .ToLower()
            .Replace(' ', '-');

        return $"{sluggedTitle}-{YearOfRelease}";
    }

    [GeneratedRegex("[^0-9A-Za-z _-]", RegexOptions.NonBacktracking, 10)]
    public static partial Regex SlugRegex();
}
