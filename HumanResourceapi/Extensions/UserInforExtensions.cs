using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResoureapi.Models;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceapi.Extensions
{
    public static class UserInforExtensions
    {
        public static IQueryable<UserInfor> ProjectUserInforToUserInfor(this IQueryable<UserInfor> query)
        {
            return query.Select(userInfor => new UserInfor
            {
                //StaffId = userInfor.StaffId,
                //UserId = userInfor.Id,
                LastName = userInfor.LastName,
                FirstName = userInfor.FirstName,
                Dob = userInfor.Dob,
                Phone = userInfor.Phone,
                //Gender = userInfor.Gender,
                Address = userInfor.Address,
                Country = userInfor.Country,
                CitizenId = userInfor.CitizenId,
                DepartmentId = userInfor.DepartmentId,
                //Position = userInfor.Position,
                //HireDate = userInfor.HireDate,
                BankAccount = userInfor.BankAccount,
                BankAccountName = userInfor.BankAccountName,
                Bank = userInfor.Bank,
                WorkTimeByYear = userInfor.WorkTimeByYear,
                AccountStatus = userInfor.AccountStatus
            }).AsNoTracking();
        }

        public static IQueryable<UserInfor> Search(
            this IQueryable<UserInfor> query,
            string searchTerm
            )
        {
            if (string.IsNullOrEmpty(searchTerm)) { return query; }

            var lowCaseSearchItem = searchTerm.Trim().ToLower();
            return query.Where(
                c => c.FirstName.ToLower().Contains(lowCaseSearchItem) ||
                c.LastName.ToLower().Contains(lowCaseSearchItem) ||
                (c.LastName + " " + c.FirstName.ToLower()).Contains(lowCaseSearchItem));
        }

        public static IQueryable<UserInfor> Filter(
            this IQueryable<UserInfor> query,
            string departments)
        {
            var departmentsList = new List<string>();

            if (!string.IsNullOrEmpty(departments))
            {
                departmentsList.AddRange(departments.ToLower().Split(",").ToList());
            }

            query = query.Where(c => departmentsList.Count == 0 ||
            departmentsList.Contains(c.Department.DepartmentName.ToLower()));
            return query;
        }
    }
}