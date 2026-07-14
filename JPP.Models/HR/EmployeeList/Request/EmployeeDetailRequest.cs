using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace JPP.Models.HR.EmployeeList.Request
{
    public class EmployeeDetailRequest : IValidatableObject
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Emp No. is required.")]
        [StringLength(50, ErrorMessage = "Emp No. maximum length is 50 characters.")]
        public string Number { get; set; } = string.Empty;

        [Required(ErrorMessage = "Firstname is required.")]
        [StringLength(100, ErrorMessage = "Firstname maximum length is 100 characters.")]
        public string FirstName { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Lastname maximum length is 100 characters.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Username is required.")]
        [StringLength(100, ErrorMessage = "Username maximum length is 100 characters.")]
        public string Username { get; set; } = string.Empty;

        public string Password { get; set; } = string.Empty;

        public string ConfirmPassword { get; set; } = string.Empty;

        [Required(ErrorMessage = "PassCode is required.")]
        [StringLength(4, MinimumLength = 4, ErrorMessage = "PassCode must be 4 digits.")]
        public string Passcode { get; set; } = string.Empty;

        public int RoleId { get; set; }

        public int EmployeeDepartmentId { get; set; }

        public int SupervisorId { get; set; }

        public string HPNum { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Email Address is not valid.")]
        public string EmailAddress { get; set; } = string.Empty;

        public int HrTimeCategoryId { get; set; }

        public bool AllowToChangePrice { get; set; }

        public bool AllowToManageServicePackage { get; set; }

        public bool AllowToManageRedemption { get; set; }

        public bool AllowToVoid { get; set; }

        public bool Inactive { get; set; }

        public DateTime? DateHired { get; set; }

        public bool HasContractEnd { get; set; }

        public DateTime? DateContractEnd { get; set; }

        public string Nric { get; set; } = string.Empty;

        public string Race { get; set; } = string.Empty;

        public string CountryOfBirth { get; set; } = string.Empty;

        public DateTime? Dob { get; set; }

        public string MaritalStatus { get; set; } = string.Empty;

        public string SpouseName { get; set; } = string.Empty;

        public string SpouseContactNo { get; set; } = string.Empty;

        public string PayMethod { get; set; } = string.Empty;

        public string PayFrequency { get; set; } = string.Empty;

        public decimal RateUsedToBillCustomer { get; set; }

        public decimal RateSalary { get; set; }

        public decimal RateSalaryHourly { get; set; }

        public decimal DirectorRemuneration { get; set; }

        public decimal CpfEmployeePercent { get; set; }

        public decimal CpfEmployerPercent { get; set; }

        public string PaymentMode { get; set; } = string.Empty;

        public string BankAccNo { get; set; } = string.Empty;

        public decimal RateOvertimeWeekday { get; set; }

        public decimal RateOvertimeWeekend { get; set; }

        public decimal RateOvertimePublicHoliday { get; set; }

        public decimal SalesCommission { get; set; }

        public decimal DeductionUnpaidLeave { get; set; }

        public decimal AllowanceTransport { get; set; }

        public decimal DeductionOthers { get; set; }

        public decimal AllowancePaidLeave { get; set; }

        public decimal ForeignLevy { get; set; }

        public decimal AllowanceOther { get; set; }

        public decimal Bonus { get; set; }

        public decimal Donation { get; set; }

        public string DonationType { get; set; } = string.Empty;

        public decimal Sdl { get; set; }

        public decimal Reimbursement { get; set; }

        public decimal DeductionWithoutCpf { get; set; }

        public string AllowanceRemarks { get; set; } = string.Empty;

        public string Notice { get; set; } = string.Empty;

        public bool ModHome { get; set; }
        public bool ModAdmin { get; set; }
        public bool ModOutlet { get; set; }
        public bool ModDental { get; set; }
        public bool ModPos { get; set; }
        public bool ModAms { get; set; }
        public bool ModCrm { get; set; }
        public bool ModHr { get; set; }
        public bool ModInv { get; set; }
        public bool ModSp { get; set; }
        public bool ModMed { get; set; }
        public bool ModFin { get; set; }
        public bool ModBi { get; set; }
        public bool ModEcom { get; set; }
        public bool ModLogistic { get; set; }

        public int RoasterYear { get; set; } = DateTime.Today.Year;

        public int RoasterMonth { get; set; } = DateTime.Today.Month;

        public string SubmitMode { get; set; } = "Save";

        public List<EmployeeTimeTableDayRequest> TimeTableDays { get; set; } = new();

        public List<EmployeeDutyRoasterDayRequest> DutyRoasterDays { get; set; } = new();

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (Id <= 0 && string.IsNullOrWhiteSpace(Password))
            {
                yield return new ValidationResult("Password is required for new employee.", new[] { nameof(Password) });
            }

            if (!string.IsNullOrWhiteSpace(Password) || !string.IsNullOrWhiteSpace(ConfirmPassword))
            {
                if (!string.Equals(Password, ConfirmPassword, StringComparison.Ordinal))
                {
                    yield return new ValidationResult("Please reconfirm password.", new[] { nameof(ConfirmPassword) });
                }
            }

            if (!string.IsNullOrWhiteSpace(Passcode) && Passcode.Trim().Length != 4)
            {
                yield return new ValidationResult("PassCode must be 4 digits.", new[] { nameof(Passcode) });
            }

            if (HasContractEnd && !DateContractEnd.HasValue)
            {
                yield return new ValidationResult("Contract end date is required when Contract End is checked.", new[] { nameof(DateContractEnd) });
            }
        }
    }
}
