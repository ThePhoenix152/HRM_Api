using HumanResoureapi.Models;

namespace API.Extensions
{
    public static class LogLeaveExtensions
    {
        public static IQueryable<LeaveApplication> Search(
           this IQueryable<LeaveApplication> query,
           string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;
            var lowerCaseSearchItem = searchTerm
                .Trim()
                .ToLower();

            return query.Where(
               c => c.Staff.FirstName.ToLower().Contains(lowerCaseSearchItem) ||
               c.Staff.LastName.ToLower().Contains(lowerCaseSearchItem) ||
               (c.Staff.LastName + " " + c.Staff.FirstName).ToLower().Contains(lowerCaseSearchItem));

        }

        public static IQueryable<LeaveApplication> Filter(
            this IQueryable<LeaveApplication> query,
            string departments)
        {
            var departmentList = new List<string>();

            if (!string.IsNullOrEmpty(departments))
            {
                departmentList.AddRange(departments.ToLower().Split(",").ToList());
            }
            query = query.Where(c => departmentList.Count == 0 ||
               departmentList.Contains(c.Staff.Department.DepartmentName.ToLower()));

            return query;
        }

    }
}
