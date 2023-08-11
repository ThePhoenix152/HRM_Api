using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HumanResourceapi.Models;
using Microsoft.EntityFrameworkCore;

namespace HumanResourceapi.Extensions
{
    public static class TicketEextensions
    {
        public static IQueryable<Ticket> ProjectTicketToTicket(this IQueryable<Ticket> query)
        {
            return query
                    .Select(ticket => new Ticket
                    {
                        TicketId = ticket.TicketId,
                        StaffId = ticket.StaffId,
                        TicketTypeId = ticket.TicketTypeId,
                        TicketFile = ticket.TicketFile,
                        TicketStatus = ticket.TicketStatus,
                        ProcessNote = ticket.ProcessNote,
                        RespondencesId = ticket.RespondencesId,
                        CreateAt = ticket.CreateAt,
                        ChangeStatusTime = ticket.ChangeStatusTime
                    }).AsNoTracking();
        }

        public static IQueryable<Ticket> Search(
            this IQueryable<Ticket> query,
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

        public static IQueryable<Ticket> Filter(
            this IQueryable<Ticket> query,
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