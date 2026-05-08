namespace Movies.Contracts.Responses
{
    public class PagedResponse<T>
    {
        public required IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
        public required int Page { get; init; }
        public required int PageSize { get; init; }
        public required int Count { get; init; }
        public bool HasNextPage => Count > Page * PageSize;

    }
}
