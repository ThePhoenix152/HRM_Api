namespace API.RequestHelpers
{
    public class ContractParams : PaginationPrams
    {
        public string? SearchTerm { get; set; }

        public string? Departments { get; set; }
    }
}
