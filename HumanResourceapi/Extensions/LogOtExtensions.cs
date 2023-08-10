using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HumanResourceapi.Models;

namespace HumanResourceapi.Extensions
{
    public static class LogOtExtensions
    {
        public static IQueryable<LogOt> Search(
            this IQueryable<LogOt> query,
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

        public static IQueryable<LogOt> Filter(
            this IQueryable<LogOt> query,
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