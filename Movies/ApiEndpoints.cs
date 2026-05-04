namespace Movies
{
    public static class ApiEndpoints
    {
        private const string ApiBase = "api";

        public static class Movies
        {
            private const string Base = $"{ApiBase}/movies";
                    
            public const string Create = Base;
            public const string Get = $"{Base}/{{idOrSlug}}";
            public const string GetAll = Base;
            public const string Update = $"{Base}/{{id:Guid}}";
            public const string Delete = $"{Base}/{{id:Guid}}";
            public const string Rate = $"{Base}/{{id:Guid}}/ratings";
            public const string DeleteRating = $"{Base}/{{id:Guid}}/ratings";
        }

        public class Ratings
        {
            private const string Base = $"{ApiBase}/ratings";
            private const string GetUserRatings = $"{Base}/me";

        }
    }
}
