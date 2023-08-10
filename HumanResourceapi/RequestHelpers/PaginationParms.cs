namespace API.RequestHelpers
{
    public class PaginationPrams
    {
        private const int MaxPageSize = 50;

        public int PageNumber { get; set; } = 1;
        private int _pageSize = 30;
        public int PageSize
        {
            get => _pageSize;
            set => _pageSize = value > MaxPageSize ? MaxPageSize : value;
        }
    }

}
