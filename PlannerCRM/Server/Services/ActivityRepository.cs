using PlannerCRM.Server.DataAccess;
using PlannerCRM.Server.Models;
using Microsoft.EntityFrameworkCore;
using PlannerCRM.Shared.DTOs.ActivityDto.Forms;
using PlannerCRM.Shared.DTOs.ActivityDto.Views;
using PlannerCRM.Shared.DTOs.EmployeeDto.Views;
using PlannerCRM.Shared.CustomExceptions;
using static PlannerCRM.Shared.Constants.ExceptionsMessages;

namespace PlannerCRM.Server.Services;

public class ActivityRepository
{
    private readonly AppDbContext _db;

    public ActivityRepository(AppDbContext db) {
        _db = db;
    }

    public async Task AddAsync(ActivityAddFormDto dto) {
        if (dto.GetType() == null) {
            throw new NullReferenceException(NULL_OBJECT);
        }

        var innerPropertiesAreNull = dto.GetType().GetProperties()
            .Any(prop => prop.GetValue(dto) == null);
        if (innerPropertiesAreNull) {
            throw new ArgumentNullException(NULL_PARAM);
        }
        
        var isAlreadyPresent = await _db.Activities
            .SingleOrDefaultAsync(ac => ac.Id == dto.Id);
        if (isAlreadyPresent != null) {
            throw new DuplicateElementException(OBJECT_ALREADY_PRESENT);
        }

        await _db.Activities.AddAsync(new Activity {
            Id = dto.Id,
            Name = dto.Name,
            StartDate = dto.StartDate ?? throw new NullReferenceException(NULL_PROP),
            FinishDate = dto.FinishDate ?? throw new NullReferenceException(NULL_PROP),
            WorkOrderId = dto.WorkOrderId ?? throw new NullReferenceException(NULL_PROP),
            EmployeeActivity = dto.EmployeesActivities
                .Select(ea => new EmployeeActivity {
                    Id = ea.Id,
                    ActivityId = ea.ActivityId,
                    EmployeeId = ea.EmployeeId
                })
               .ToList(),
        });

        var rowsAffected = await _db.SaveChangesAsync();
        if (rowsAffected == 0) {
            throw new DbUpdateException(IMPOSSIBLE_SAVE_CHANGES);
        }
    }

    public async Task DeleteAsync(int id) {
        var activityDelete = await _db.Activities
            .SingleOrDefaultAsync(ac => ac.Id == id);

        if (activityDelete == null) {
            throw new InvalidOperationException(IMPOSSIBLE_DELETE);
        }
        _db.Activities.Remove(activityDelete);

        var rowsAffected = await _db.SaveChangesAsync();
        if (rowsAffected == 0) {
            throw new DbUpdateException(IMPOSSIBLE_SAVE_CHANGES);
        }
    }

    public async Task EditAsync(ActivityEditFormDto dto) {
        if (dto == null) {
            throw new NullReferenceException(NULL_OBJECT);
        }

        var HasPropertiesNull = dto.GetType().GetProperties()  //exception when updating activity
            .Any(prop => prop.GetValue(dto) == null);
        if (HasPropertiesNull) {
            throw new ArgumentNullException(NULL_PARAM);
        }
        
        var model = await _db.Activities
            .SingleOrDefaultAsync(ac => ac.Id == dto.Id);

        if (model == null) {
            throw new KeyNotFoundException(OBJECT_NOT_FOUND);
        }

        model.Id = dto.Id;
        model.Name = dto.Name;
        model.StartDate = dto.StartDate ?? throw new NullReferenceException(NULL_PROP);
        model.FinishDate = dto.FinishDate ?? throw new NullReferenceException(NULL_PROP);
        model.WorkOrderId = dto.WorkOrderId ?? throw new NullReferenceException(NULL_PROP);
        model.EmployeeActivity = dto.EmployeesActivities
            .Where(ea => 
                model.EmployeeActivity
                    .Any(ea => ea.EmployeeId != ea.Id))
            .Select(ea => new EmployeeActivity {
                EmployeeId = ea.Id,
                ActivityId = dto.Id
            })
            .ToList();
        
        var rowsAffected = await _db.SaveChangesAsync();
        if (rowsAffected == 0) {
            throw new DbUpdateException(IMPOSSIBLE_SAVE_CHANGES);
        }
    }

    public async Task<ActivityViewDto> GetForViewAsync(int id) {
        return await _db.Activities
            .Select(ac => new ActivityViewDto {
                Id = ac.Id,
                Name = ac.Name,
                StartDate = ac.StartDate,
                FinishDate = ac.FinishDate,
                WorkOrderId = ac.WorkOrderId,
            })
            .SingleOrDefaultAsync(ac => ac.Id == id);
    }

    public async Task<ActivityEditFormDto> GetForEditAsync(int id) {
        return await _db.Activities
            .Select(ac => new ActivityEditFormDto {
                Id = ac.Id,
                Name = ac.Name,
                StartDate = ac.StartDate,
                FinishDate = ac.FinishDate,
                WorkOrderId = ac.WorkOrderId,
                EmployeesActivities = ac.EmployeeActivity
                    .Select(ea => new EmployeeActivityDto {
                        Id = ea.Id,
                        EmployeeId = ea.EmployeeId,
                        Employee = _db.Employees
                            .Select(em => new EmployeeSelectDto {
                                Id = em.Id,
                                Email = em.Email,
                                FirstName = em.FirstName,
                                LastName = em.LastName,
                                Role = em.Role
                            })
                            .Single(em => em.Id == ea.EmployeeId),
                        ActivityId = ea.ActivityId,
                        Activity = _db.Activities
                            .Select(ac => new ActivitySelectDto {
                            Id = ac.Id,
                            Name = ac.Name,
                            StartDate = ac.StartDate,
                            FinishDate = ac.FinishDate,
                            WorkOrderId = ac.WorkOrderId
                        })
                        .Single(ac => ac.Id == ea.ActivityId)
                    })
                    .ToList()
            })
            .SingleAsync(ac => ac.Id == id);
    }

    public async Task<ActivityDeleteDto> GetForDeleteAsync(int id) {
        return await _db.Activities
            .Select(ac => new ActivityDeleteDto {
                Id = ac.Id,
                Name = ac.Name,
                StartDate = ac.StartDate,
                FinishDate = ac.FinishDate
            })
            .SingleOrDefaultAsync(ac => ac.Id == id);
    }

    public async Task<List<ActivityEditFormDto>> GetActivityByEmployeeId(int employeeId) {
         return await _db.Activities
            .Select(ac => new ActivityEditFormDto {
                Id = ac.Id,
                Name = ac.Name,
                StartDate = ac.StartDate,
                FinishDate = ac.FinishDate,
                WorkOrderId = ac.WorkOrderId,
                EmployeesActivities = ac.EmployeeActivity
                    .Select(ea => new EmployeeActivityDto {
                        Id = ea.EmployeeId,
                        EmployeeId = ea.EmployeeId,
                        Employee = _db.Employees
                            .Select(em => new EmployeeSelectDto {
                                Id = em.Id,
                                Email = em.Email,
                                FirstName = em.FirstName,
                                LastName = em.LastName,
                                Role = em.Role
                            })
                            .Single(em => em.Id == ea.EmployeeId),
                        Activity = _db.Activities
                            .Select(ac => new ActivitySelectDto {
                            Id = ac.Id,
                            Name = ac.Name,
                            StartDate = ac.StartDate,
                            FinishDate = ac.FinishDate,
                            WorkOrderId = ac.WorkOrderId
                        })
                        .Single(ac => ac.Id == ea.ActivityId)
                    })
                    .ToList()   
            })
            .Where(ac => _db.EmployeeActivity
                .Any(ea => ea.EmployeeId == employeeId && ac.Id == ea.ActivityId))
            .ToListAsync();
    }

    public async Task<List<ActivityEditFormDto>> GetActivitiesPerWorkOrderAsync(int workOrderId) {
        return await _db.Activities
            .Select(ac => new ActivityEditFormDto {
                Id = ac.Id,
                Name = ac.Name,
                StartDate = ac.StartDate,
                FinishDate = ac.FinishDate,
                WorkOrderId = ac.WorkOrderId,
                EmployeesActivities = ac.EmployeeActivity
                    .Select(ea => new EmployeeActivityDto {
                        Id = ea.Id,
                        EmployeeId = ea.EmployeeId,
                        Employee = _db.Employees
                            .Select(em => new EmployeeSelectDto {
                                Id = em.Id,
                                Email = em.Email,
                                FirstName = em.FirstName,
                                LastName = em.LastName,
                                Role = em.Role
                            })
                            .Single(em => em.Id == ea.EmployeeId),
                        ActivityId = ea.ActivityId,
                        Activity = _db.Activities
                            .Select(ac => new ActivitySelectDto {
                            Id = ac.Id,
                            Name = ac.Name,
                            StartDate = ac.StartDate,
                            FinishDate = ac.FinishDate,
                            WorkOrderId = ac.WorkOrderId
                        })
                        .Single(ac => ac.Id == ea.ActivityId)
                    })
                    .ToList()   
            })
            .Where(ac => ac.WorkOrderId == workOrderId)
            .ToListAsync();
    }

    public async Task<List<ActivityViewDto>> GetAllAsync() {
        return await _db.Activities
            .Select(ac => new ActivityViewDto {
                Id = ac.Id,
                Name = ac.Name,
                StartDate = ac.StartDate,
                FinishDate = ac.FinishDate,
                WorkOrderId = ac.WorkOrderId
            })
            .ToListAsync();
    }
}
