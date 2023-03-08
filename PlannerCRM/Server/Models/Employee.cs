using PlannerCRM.Shared.Models;

namespace PlannerCRM.Server.Models;

public class Employee
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string Password { get; set; }
    public DateTime StartDate { get; set; }
    public DateTime Birthday { get; set; }
    public string NumericCode { get; set; }
    public Roles Role { get; set; }
    public List<EmployeeSalaries> Salaries { get; set; }
    public List<EmployeeActivity> EmployeeActivity { get; set; }
}