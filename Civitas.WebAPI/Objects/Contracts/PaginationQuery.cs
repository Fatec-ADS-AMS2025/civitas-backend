namespace Civitas.WebAPI.Objects.Contracts
{
    public class PaginationQuery
    {
        public const int DefaultPage = 1;
        public const int DefaultPageSize = 20;
        public const int MaxPageSize = 100;

        public int Page { get; set; } = DefaultPage;
        public int Size { get; set; } = DefaultPageSize;
        public string? SortBy { get; set; }
        public string? SortDirection { get; set; }

        public int NormalizedPage => Page < DefaultPage ? DefaultPage : Page;

        public int NormalizedSize
        {
            get
            {
                if (Size < 1)
                {
                    return DefaultPageSize;
                }

                return Size > MaxPageSize ? MaxPageSize : Size;
            }
        }

        public bool IsDescending =>
            string.Equals(SortDirection, "desc", StringComparison.OrdinalIgnoreCase);
    }
}
