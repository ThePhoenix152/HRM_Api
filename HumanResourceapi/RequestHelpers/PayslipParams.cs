namespace HumanResourceapi.RequestHelpers
{
    public class PayslipParams : PaginationPrams
    {
        public string? OrderBy { get; set; }
        public string? SearchTerm { get; set; }

        public string? Departments { get; set; }

    }
}
