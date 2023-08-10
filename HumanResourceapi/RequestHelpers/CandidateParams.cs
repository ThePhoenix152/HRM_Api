namespace API.RequestHelpers
{
    public class CandidateParams : PaginationPrams
    {
        public string? SearchTerm { get; set; }
        public string? Departments { get; set; }
    }
}
