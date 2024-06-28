namespace PlannerCRM.Shared.DTOs.EmployeeDto.Views;

public class CurrentEmployeeDto
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string FullName { get; set; }
    public Roles Role { get; set; }

}