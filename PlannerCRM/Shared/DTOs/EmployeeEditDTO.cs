using System.ComponentModel.DataAnnotations;
using PlannerCRM.Shared.Models;

namespace PlannerCRM.Shared.DTOs;

public class EmployeeEditDTO
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }

    [Required]
    public string StartDate { get; set; }

    [Required]
    public string BirthDay { get; set; }

    [Required]
    public string NumericCode { get; set; }

    [EnumDataType(typeof(Roles))]
    public Roles Role { get; set; }

    [Required]
    public decimal HourPay { get; set; }
}