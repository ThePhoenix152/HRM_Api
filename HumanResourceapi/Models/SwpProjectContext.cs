using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace HumanResoureapi.Models;

public partial class SwpProjectContext : DbContext
{
    public SwpProjectContext()
    {
    }

    public SwpProjectContext(DbContextOptions<SwpProjectContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Allowance> Allowances { get; set; }

    public virtual DbSet<AllowanceType> AllowanceTypes { get; set; }

    public virtual DbSet<ContractType> ContractTypes { get; set; }

    public virtual DbSet<Department> Departments { get; set; }

    public virtual DbSet<LeaveApplication> LeaveApplications { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<OtType> OtTypes { get; set; }

    public virtual DbSet<Otapplication> Otapplications { get; set; }

    public virtual DbSet<Payslip> Payslips { get; set; }

    public virtual DbSet<PersonnelContract> PersonnelContracts { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    public virtual DbSet<TaxList> TaxLists { get; set; }

    public virtual DbSet<TheCalendar> TheCalendars { get; set; }

    public virtual DbSet<UserInfor> UserInfors { get; set; }

    public virtual DbSet<UserTax> UserTaxes { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Name=DefaultConnection");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Allowance>(entity =>
        {
            entity.HasKey(e => e.AllowanceId).HasName("PK_Allowance_allowanceId");

            entity.ToTable("Allowance");

            entity.HasIndex(e => e.AllowanceTypeId, "IX_Allowance_allowanceTypeId");

            entity.HasIndex(e => e.ContractId, "IX_Allowance_contractId");

            entity.Property(e => e.AllowanceId).HasColumnName("allowanceId");
            entity.Property(e => e.AllowanceSalary).HasColumnName("allowanceSalary");
            entity.Property(e => e.AllowanceTypeId).HasColumnName("allowanceTypeId");
            entity.Property(e => e.ContractId).HasColumnName("contractId");

            entity.HasOne(d => d.AllowanceType).WithMany(p => p.Allowances)
                .HasForeignKey(d => d.AllowanceTypeId)
                .HasConstraintName("FK_Allowance_allowanceTypeId_allowanceTypeId");

            entity.HasOne(d => d.Contract).WithMany(p => p.Allowances)
                .HasForeignKey(d => d.ContractId)
                .HasConstraintName("FK_Allowance_contractId_contractId");
        });

        modelBuilder.Entity<AllowanceType>(entity =>
        {
            entity.HasKey(e => e.AllowanceTypeId).HasName("PK_AllowanceType_allowanceTypeId");

            entity.ToTable("AllowanceType");

            entity.Property(e => e.AllowanceTypeId).HasColumnName("allowanceTypeId");
            entity.Property(e => e.AllowanceDetailSalary)
                .HasMaxLength(50)
                .HasColumnName("allowanceDetailSalary");
            entity.Property(e => e.AllowanceName)
                .HasMaxLength(50)
                .HasColumnName("allowanceName");
        });

        modelBuilder.Entity<ContractType>(entity =>
        {
            entity.HasKey(e => e.ContractTypeId).HasName("PK_ContractType_contractTypeId");

            entity.ToTable("ContractType");

            entity.Property(e => e.ContractTypeId).HasColumnName("contractTypeId");
            entity.Property(e => e.Description)
                .HasMaxLength(50)
                .HasColumnName("description");
            entity.Property(e => e.Name)
                .HasMaxLength(30)
                .HasColumnName("name");
        });

        modelBuilder.Entity<Department>(entity =>
        {
            entity.HasKey(e => e.DepartmentId).HasName("PK_Department_departmentId");

            entity.ToTable("Department");

            entity.Property(e => e.DepartmentId).HasColumnName("departmentId");
            entity.Property(e => e.DepartmentName)
                .HasMaxLength(35)
                .HasColumnName("departmentName");
            entity.Property(e => e.Status).HasColumnName("status");
        });

        modelBuilder.Entity<LeaveApplication>(entity =>
        {
            entity.HasKey(e => e.LeaveLogId).HasName("PK_LogLeave_leaveLogId");

            entity.ToTable("LeaveApplication");

            entity.HasIndex(e => e.LeaveTypeId, "IX_LogLeave_leaveTypeId");

            entity.HasIndex(e => e.StaffId, "IX_LogLeave_staffId");

            entity.Property(e => e.LeaveLogId).HasColumnName("leaveLogId");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.ChangeStatusTime)
                .HasColumnType("datetime")
                .HasColumnName("changeStatusTime");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.CreatorId).HasColumnName("creatorId");
            entity.Property(e => e.Description)
                .HasMaxLength(70)
                .HasColumnName("description");
            entity.Property(e => e.Enable).HasColumnName("enable");
            entity.Property(e => e.LeaveDays).HasColumnName("leaveDays");
            entity.Property(e => e.LeaveEnd)
                .HasColumnType("date")
                .HasColumnName("leaveEnd");
            entity.Property(e => e.LeaveHours).HasColumnName("leaveHours");
            entity.Property(e => e.LeaveStart)
                .HasColumnType("date")
                .HasColumnName("leaveStart");
            entity.Property(e => e.LeaveTypeId).HasColumnName("leaveTypeId");
            entity.Property(e => e.ProcessNote)
                .HasMaxLength(120)
                .HasColumnName("processNote");
            entity.Property(e => e.RespondencesId).HasColumnName("respondencesId");
            entity.Property(e => e.SalaryPerDay).HasColumnName("salaryPerDay");
            entity.Property(e => e.StaffId).HasColumnName("staffId");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.LeaveType).WithMany(p => p.LeaveApplications)
                .HasForeignKey(d => d.LeaveTypeId)
                .HasConstraintName("FK_LeaveApplication_LeaveType");

            entity.HasOne(d => d.Staff).WithMany(p => p.LeaveApplications)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_LeaveApplication_UserInfor");
        });

        modelBuilder.Entity<LeaveType>(entity =>
        {
            entity.HasKey(e => e.LeaveTypeId).HasName("PK_LeaveType_leaveTypeId");

            entity.ToTable("LeaveType");

            entity.Property(e => e.LeaveTypeId).HasColumnName("leaveTypeId");
            entity.Property(e => e.IsSalary).HasColumnName("isSalary");
            entity.Property(e => e.LeaveTypeDay).HasColumnName("leaveTypeDay");
            entity.Property(e => e.LeaveTypeDetail)
                .HasMaxLength(100)
                .HasColumnName("leaveTypeDetail");
            entity.Property(e => e.LeaveTypeName)
                .HasMaxLength(50)
                .HasColumnName("leaveTypeName");
        });

        modelBuilder.Entity<OtType>(entity =>
        {
            entity.HasKey(e => e.OtTypeId).HasName("PK_OtType_otTypeId");

            entity.ToTable("OtType");

            entity.Property(e => e.OtTypeId).HasColumnName("otTypeId");
            entity.Property(e => e.TypeName)
                .HasMaxLength(35)
                .HasColumnName("typeName");
            entity.Property(e => e.TypePercentage).HasColumnName("typePercentage");
        });

        modelBuilder.Entity<Otapplication>(entity =>
        {
            entity.HasKey(e => e.OtLogId).HasName("PK_LogOT_otLogId");

            entity.ToTable("OTApplication");

            entity.HasIndex(e => e.OtTypeId, "IX_LogOT_otTypeId");

            entity.HasIndex(e => e.StaffId, "IX_LogOT_staffId");

            entity.Property(e => e.OtLogId).HasColumnName("otLogId");
            entity.Property(e => e.Amount).HasColumnName("amount");
            entity.Property(e => e.ChangeStatusTime)
                .HasColumnType("datetime")
                .HasColumnName("changeStatusTime");
            entity.Property(e => e.CreateAt)
                .HasColumnType("datetime")
                .HasColumnName("createAt");
            entity.Property(e => e.CreatorId).HasColumnName("creatorId");
            entity.Property(e => e.Days).HasColumnName("days");
            entity.Property(e => e.Enable).HasColumnName("enable");
            entity.Property(e => e.LogEnd)
                .HasColumnType("datetime")
                .HasColumnName("logEnd");
            entity.Property(e => e.LogHours).HasColumnName("logHours");
            entity.Property(e => e.LogStart)
                .HasColumnType("datetime")
                .HasColumnName("logStart");
            entity.Property(e => e.OtTypeId).HasColumnName("otTypeId");
            entity.Property(e => e.ProcessNote)
                .HasMaxLength(120)
                .HasColumnName("processNote");
            entity.Property(e => e.Reason)
                .HasMaxLength(25)
                .HasColumnName("reason");
            entity.Property(e => e.RespondencesId).HasColumnName("respondencesId");
            entity.Property(e => e.SalaryPerDay).HasColumnName("salaryPerDay");
            entity.Property(e => e.StaffId).HasColumnName("staffId");
            entity.Property(e => e.Status)
                .HasMaxLength(20)
                .HasColumnName("status");

            entity.HasOne(d => d.OtType).WithMany(p => p.Otapplications)
                .HasForeignKey(d => d.OtTypeId)
                .HasConstraintName("FK_OTApplication_OtType");

            entity.HasOne(d => d.Staff).WithMany(p => p.Otapplications)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OTApplication_UserInfor");
        });

        modelBuilder.Entity<Payslip>(entity =>
        {
            entity.HasKey(e => e.PayslipId).HasName("PK_Payslip_payslipId");

            entity.ToTable("Payslip");

            entity.HasIndex(e => e.StaffId, "IX_Payslip_staffId");

            entity.Property(e => e.PayslipId).HasColumnName("payslipId");
            entity.Property(e => e.ActualWorkDays).HasColumnName("actualWorkDays");
            entity.Property(e => e.ChangeAt)
                .HasColumnType("date")
                .HasColumnName("changeAt");
            entity.Property(e => e.ChangerId).HasColumnName("changerId");
            entity.Property(e => e.CreateAt)
                .HasColumnType("date")
                .HasColumnName("createAt");
            entity.Property(e => e.CreatorId).HasColumnName("creatorId");
            entity.Property(e => e.Enable).HasColumnName("enable");
            entity.Property(e => e.FamilyDeduction).HasColumnName("familyDeduction");
            entity.Property(e => e.GrossActualSalary).HasColumnName("grossActualSalary");
            entity.Property(e => e.GrossStandardSalary).HasColumnName("grossStandardSalary");
            entity.Property(e => e.LeaveDays).HasColumnName("leaveDays");
            entity.Property(e => e.LeaveHours).HasColumnName("leaveHours");
            entity.Property(e => e.NetActualSalary).HasColumnName("netActualSalary");
            entity.Property(e => e.NetStandardSalary).HasColumnName("netStandardSalary");
            entity.Property(e => e.OtTotal).HasColumnName("otTotal");
            entity.Property(e => e.Payday)
                .HasColumnType("datetime")
                .HasColumnName("payday");
            entity.Property(e => e.SalaryRecieved).HasColumnName("salaryRecieved");
            entity.Property(e => e.SelfDeduction).HasColumnName("selfDeduction");
            entity.Property(e => e.StaffId).HasColumnName("staffId");
            entity.Property(e => e.StandardWorkDays).HasColumnName("standardWorkDays");
            entity.Property(e => e.Status)
                .HasMaxLength(25)
                .HasColumnName("status");
            entity.Property(e => e.TotalAllowance).HasColumnName("totalAllowance");
            entity.Property(e => e.TotalCompInsured).HasColumnName("totalCompInsured");
            entity.Property(e => e.TotalCompPaid).HasColumnName("totalCompPaid");

            entity.HasOne(d => d.Staff).WithMany(p => p.Payslips)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Payslip_staffId_staffId");
        });

        modelBuilder.Entity<PersonnelContract>(entity =>
        {
            entity.HasKey(e => e.ContractId).HasName("PK_PersonnelContract_contractId");

            entity.ToTable("PersonnelContract");

            entity.HasIndex(e => e.ContractTypeId, "IX_PersonnelContract_contractTypeId");

            entity.HasIndex(e => e.StaffId, "IX_PersonnelContract_staffId");

            entity.Property(e => e.ContractId).HasColumnName("contractId");
            entity.Property(e => e.ChangeAt)
                .HasColumnType("date")
                .HasColumnName("changeAt");
            entity.Property(e => e.ContractFile)
                .HasMaxLength(150)
                .IsUnicode(false)
                .HasColumnName("contractFile");
            entity.Property(e => e.ContractStatus).HasColumnName("contractStatus");
            entity.Property(e => e.ContractTypeId).HasColumnName("contractTypeId");
            entity.Property(e => e.CreateAt)
                .HasColumnType("date")
                .HasColumnName("createAt");
            entity.Property(e => e.EndDate)
                .HasColumnType("date")
                .HasColumnName("endDate");
            entity.Property(e => e.NoOfDependences).HasColumnName("noOfDependences");
            entity.Property(e => e.Note)
                .HasMaxLength(50)
                .HasColumnName("note");
            entity.Property(e => e.ResponseId).HasColumnName("responseId");
            entity.Property(e => e.Salary).HasColumnName("salary");
            entity.Property(e => e.SalaryType)
                .HasMaxLength(20)
                .HasColumnName("salaryType");
            entity.Property(e => e.StaffId).HasColumnName("staffId");
            entity.Property(e => e.StartDate)
                .HasColumnType("date")
                .HasColumnName("startDate");
            entity.Property(e => e.TaxableSalary).HasColumnName("taxableSalary");
            entity.Property(e => e.WorkDatePerWeek).HasColumnName("workDatePerWeek");

            entity.HasOne(d => d.ContractType).WithMany(p => p.PersonnelContracts)
                .HasForeignKey(d => d.ContractTypeId)
                .HasConstraintName("FK_PersonnelContract_contractTypeId_contractTypeId");

            entity.HasOne(d => d.Staff).WithMany(p => p.PersonnelContracts)
                .HasForeignKey(d => d.StaffId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PersonnelContract_staffId_staffId");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK_Role_roleId");

            entity.ToTable("Role");

            entity.Property(e => e.RoleId)
                .ValueGeneratedNever()
                .HasColumnName("roleId");
            entity.Property(e => e.RoleName)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasColumnName("roleName");
        });

        modelBuilder.Entity<TaxList>(entity =>
        {
            entity.HasKey(e => e.TaxLevel).HasName("PK_TaxList_taxLevel");

            entity.ToTable("TaxList");

            entity.Property(e => e.TaxLevel).HasColumnName("taxLevel");
            entity.Property(e => e.Description)
                .HasMaxLength(40)
                .HasColumnName("description");
            entity.Property(e => e.TaxPercentage).HasColumnName("taxPercentage");
            entity.Property(e => e.TaxRange).HasColumnName("taxRange");
        });

        modelBuilder.Entity<TheCalendar>(entity =>
        {
            entity
                .HasNoKey()
                .ToView("TheCalendar");

            entity.Property(e => e.HolidayText).HasMaxLength(255);
            entity.Property(e => e.IsWorking).HasColumnName("isWorking");
            entity.Property(e => e.Percent).HasColumnType("numeric(2, 1)");
            entity.Property(e => e.Style101)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength();
            entity.Property(e => e.TheDate).HasColumnType("date");
            entity.Property(e => e.TheDayName).HasMaxLength(30);
            entity.Property(e => e.TheFirstOfMonth).HasColumnType("date");
            entity.Property(e => e.TheFirstOfYear).HasColumnType("date");
            entity.Property(e => e.TheMonthName).HasMaxLength(30);
        });

        modelBuilder.Entity<UserInfor>(entity =>
        {
            entity.HasKey(e => e.StaffId).HasName("PK_UserInfor_staffId");

            entity.ToTable("UserInfor");

            entity.HasIndex(e => e.UserAccountUserId, "IX_UserInfor_UserAccountUserId");

            entity.HasIndex(e => e.DepartmentId, "IX_UserInfor_departmentId");

            entity.HasIndex(e => e.CitizenId, "UQ_UserInfor_citizenId")
                .IsUnique()
                .HasFilter("([citizenId] IS NOT NULL)");

            entity.Property(e => e.StaffId).HasColumnName("staffId");
            entity.Property(e => e.AccountStatus).HasColumnName("accountStatus");
            entity.Property(e => e.Address)
                .HasMaxLength(80)
                .HasColumnName("address");
            entity.Property(e => e.Bank)
                .HasMaxLength(35)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("bank");
            entity.Property(e => e.BankAccount)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("bankAccount");
            entity.Property(e => e.BankAccountName)
                .HasMaxLength(50)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("bankAccountName");
            entity.Property(e => e.CitizenId)
                .HasMaxLength(12)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("citizenId");
            entity.Property(e => e.Country)
                .HasMaxLength(20)
                .HasColumnName("country");
            entity.Property(e => e.DepartmentId).HasColumnName("departmentId");
            entity.Property(e => e.Dob)
                .HasColumnType("date")
                .HasColumnName("dob");
            entity.Property(e => e.Email)
                .HasMaxLength(50)
                .HasColumnName("email");
            entity.Property(e => e.FirstName)
                .HasMaxLength(25)
                .HasColumnName("firstName");
            entity.Property(e => e.Gender).HasColumnName("gender");
            entity.Property(e => e.HireDate)
                .HasColumnType("date")
                .HasColumnName("hireDate");
            entity.Property(e => e.ImageFile)
                .HasMaxLength(120)
                .IsUnicode(false)
                .HasColumnName("imageFile");
            entity.Property(e => e.IsManager).HasColumnName("isManager");
            entity.Property(e => e.LastName)
                .HasMaxLength(25)
                .HasColumnName("lastName");
            entity.Property(e => e.Password)
                .HasMaxLength(50)
                .HasColumnName("password");
            entity.Property(e => e.Phone)
                .HasMaxLength(10)
                .IsUnicode(false)
                .IsFixedLength()
                .HasColumnName("phone");
            entity.Property(e => e.Roleid).HasColumnName("roleid");
            entity.Property(e => e.WorkTimeByYear).HasColumnName("workTimeByYear");

            entity.HasOne(d => d.Department).WithMany(p => p.UserInfors)
                .HasForeignKey(d => d.DepartmentId)
                .HasConstraintName("FK_UserInfor_departmentId_departmentId");

            entity.HasOne(d => d.Role).WithMany(p => p.UserInfors)
                .HasForeignKey(d => d.Roleid)
                .HasConstraintName("FK_UserInfor_Role");
        });

        modelBuilder.Entity<UserTax>(entity =>
        {
            entity
                .HasNoKey()
                .ToTable("UserTax");

            entity.HasIndex(e => e.PayslipId, "IX_TaxDetail_payslipId");

            entity.HasIndex(e => e.TaxLevel, "IX_TaxDetail_taxLevel");

            entity.Property(e => e.PayslipId).HasColumnName("payslipId");
            entity.Property(e => e.TaxLevel).HasColumnName("taxLevel");

            entity.HasOne(d => d.Payslip).WithMany()
                .HasForeignKey(d => d.PayslipId)
                .HasConstraintName("FK_TaxDetail_payslipId_payslipId");

            entity.HasOne(d => d.TaxLevelNavigation).WithMany()
                .HasForeignKey(d => d.TaxLevel)
                .HasConstraintName("FK_TaxDetail_taxLevel_taxLevel");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
