namespace API.RequestHelpers
{
    public class LogLeaveParams : PaginationPrams
    {
        public string? SearchTerm { get; set; }
        public string? Departments { get; set; }
    }
}
