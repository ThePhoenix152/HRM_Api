using HumanResourceapi.Models;

namespace HumanResourceapi.Extensions
{
    public static class CandidateExtensions
    {
        public static IQueryable<Candidate> Search(
            this IQueryable<Candidate> query,
            string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;
            var lowerCaseSearchItem = searchTerm
                .Trim()
                .ToLower();

            return query.Where(
               c => c.Name.ToLower().Contains(lowerCaseSearchItem) ||
               c.Name.ToLower().Contains(lowerCaseSearchItem));

        }

        public static IQueryable<Candidate> Filter(
            this IQueryable<Candidate> query,
            string departments)
        {
            var departmentList = new List<string>();

            if (!string.IsNullOrEmpty(departments))
            {
                departmentList.AddRange(departments.ToLower().Split(",").ToList());
            }
            query = query.Where(c => departmentList.Count == 0 ||
               departmentList.Contains(c.Department.ToLower()));

            return query;
        }

    }
}
