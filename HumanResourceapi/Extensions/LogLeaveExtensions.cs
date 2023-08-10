using HumanResourceapi.Models;

namespace HumanResourceapi.Extensions;

public static class LogLeaveExtensions
{
    public static IQueryable<LogLeave> Search(
       this IQueryable<LogLeave> query,
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

    public static IQueryable<LogLeave> Filter(
        this IQueryable<LogLeave> query,
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
