using System.ComponentModel.DataAnnotations;
using PlannerCRM.Shared.Attributes;
using PlannerCRM.Shared.DTOs.ActivityDto.Forms;
using PlannerCRM.Shared.Models;
using System.ComponentModel;
using static PlannerCRM.Shared.Constants.ConstantValues;

namespace PlannerCRM.Shared.DTOs.EmployeeDto.Forms;

public class EmployeeAddFormDto
{
    public int Id { get; set; }

    [Required(ErrorMessage = """Campo "Nome" richiesto""")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = """Campo "Cognome" richiesto""")]
    public string LastName { get; set; }

    [RangeValidator(PASS_MIN_LENGTH, PASS_MAX_LENGTH, 
        ErrorMessage="La password deve avere tra 8 e 16 caratteri.")]
    [Required(ErrorMessage = """Campo "Password" richiesto""")]
    public string Password { get; set; }

    [EmailAddress(ErrorMessage = """ Campo "Email" non valido. """)]
    [Required(ErrorMessage = """Campo "Email" richiesto""")]
    public string Email { get; set; }

    [Required(ErrorMessage = """ Campo "Data d'inizio" richiesto. """)]
    public DateTime? StartDate { get; set; }
    
    [Required(ErrorMessage = """ Campo "Data di nascita" richiesto. """)]
    public DateTime? BirthDay { get; set; } 

    [Required(ErrorMessage = """Campo "Codice fiscale" richiesto""")]
    public string NumericCode { get; set; }

    [IsNotAdminRole(ADMIN_ROLE)]
    [Required(ErrorMessage = """Campo "Ruolo" richiesto""")]
    [EnumDataType(typeof(Roles))]
    public Roles? Role { get; set; }

    [Required(ErrorMessage = """ Campo "Tariffa oraria" richiesto """)]
    public float? HourlyRate { get; set; }

    [Required(ErrorMessage = """ Campo "Data d'inizio tariffa oraria" richiesto """)]
    public DateTime? StartDateHourlyRate { get; set; }

    [Required(ErrorMessage = """ Campo "Data di fine tariffa oraria" richiesto """)]
    public DateTime? FinishDateHourlyRate { get; set; }

    public List<EmployeeSalaryDto> EmployeeSalaries { get; set; }
}