using HumanResoureapi.Models;
using Microsoft.EntityFrameworkCore;
using System.Net.NetworkInformation;

namespace HumanResourceapi.Extensions
{
    public static class PayslipExtesion
    {
        public static IQueryable<Payslip> Sort(this IQueryable<Payslip> query, string orderBy) 
        {
            if (string.IsNullOrWhiteSpace(orderBy)) return query.OrderByDescending(c => c.PayslipId);

            query = orderBy switch
            {
                "payslipId" => query.OrderBy(c => c.PayslipId),
                "payslipIdDesc" => query.OrderByDescending(c => c.PayslipId),
                _ => query.OrderByDescending(c => c.PayslipId)
            };

            return query;
        }

        public static  IQueryable<Payslip> Search(
            this IQueryable<Payslip> query, 
            string searchTerm)
        {
            if (string.IsNullOrEmpty(searchTerm)) return query;

            var lowerCaseSearchItem = searchTerm
                .Trim()
                .ToLower();

            return  query.Where(
                c => c.Staff.FirstName.ToLower().Contains(lowerCaseSearchItem) ||
                c.Staff.LastName.ToLower().Contains(lowerCaseSearchItem) ||
                (c.Staff.LastName + " " + c.Staff.FirstName).ToLower().Contains(lowerCaseSearchItem));
        }

        public static IQueryable<Payslip> Filter(this IQueryable<Payslip> query, string departments)
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
