﻿namespace PlannerCRM.Server.Models.Entities;

public class WorkTime
{
    public int Id { get; set; }
    public DateTime CreationDate { get; set; }

    public double WorkedHours { get; set; }
    
    [NotMapped]
    public int WorkOrderId { get; set; }

    [NotMapped]
    public int EmployeeId { get; set; }

    [NotMapped]
    public int ActivityId { get; set; }

    // Navigation properties
    public WorkOrder WorkOrder { get; set; }
    public Employee Employee { get; set; }
    public Activity Activity { get; set; }
    public ICollection<ActivityWorkTime> ActivityWorkTimes { get; set; }
}
