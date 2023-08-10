namespace API.RequestHelpers
{
    public class UserInforParams : PaginationPrams
    {
        public string? SearchTerm { get; set; }
        public string? Departments { get; set; }
    }
}
