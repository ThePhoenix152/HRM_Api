namespace API.RequestHelpers
{
    public class OtherTicketParams: PaginationPrams
    {
        public string? SearchTerm { get; set; }

        public string? Departments { get; set; }
    }
}
