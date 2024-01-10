namespace PlannerCRM.Shared.DTOs.EmployeeProfilePictureDto;

public class ProfilePictureDto
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }
    public string FileName { get; set; }

    public string ImageType { get; set; }

    public string? Thumbnail { get; set; }

    public virtual EmployeeSelectDto? EmployeeInfo { get; set; }
}