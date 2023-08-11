namespace API.DTOs.PersonnelContractDTO
{
    public class AllowanceDTO
    {
        public int? ContractId { get; set; }
        public int AllowanceId { get; set; }
        public int? AllowanceTypeId { get; set; }

        public int? AllowanceSalary { get; set; }

        public virtual AllowanceTypeDTO? AllowanceType { get; set; }
    }
}