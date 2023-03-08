using PlannerCRM.Server.Services.Interfaces;
using PlannerCRM.Server.DataAccess;
using PlannerCRM.Server.Models;
using Microsoft.EntityFrameworkCore;

namespace PlannerCRM.Server.Services.ConcreteClasses;

class ActivityRepository : IRepository<Activity>
{
    private readonly AppDbContext _db;

    public ActivityRepository(AppDbContext db) {
        _db = db;
    }
    public async Task Add(Activity entity) {
        _db.Activities.Add(entity);
        
        await _db.SaveChangesAsync();
    }

    public async Task Delete(int id) {
        var entity = await _db.Activities.SingleOrDefaultAsync(a => a.Id == id);

        if (entity == null) {
            return;
        }
        _db.Activities.Remove(entity);

        await _db.SaveChangesAsync();
    }

    public async Task<bool> Edit(int id, Activity entity) {
        var model = await _db.Activities.SingleOrDefaultAsync(a => a.Id == id);
        if (model == null) {
            return false;
        }

        model.Id = entity.Id;
        model.Name = entity.Name;
        model.StartDate = entity.StartDate;
        model.FinishDate = entity.FinishDate;
        model.EmployeeActivity = entity.EmployeeActivity;
        model.WorkOrderId = entity.WorkOrderId;

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<Activity> Get(int id) {
        return await _db.Activities.SingleOrDefaultAsync(a => a.Id == id);
    }

    public async Task<List<Activity>> GetAll() {
        return await _db.Activities.ToListAsync();
    }
}
