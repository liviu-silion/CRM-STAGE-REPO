using PlannerCRM.Server.DataAccess;
using PlannerCRM.Server.Models;
using Microsoft.EntityFrameworkCore;
using PlannerCRM.Shared.DTOs.Workorder.Forms;
using PlannerCRM.Shared.DTOs.Workorder.Views;

namespace PlannerCRM.Server.Services.ConcreteClasses;

public class WorkOrderRepository
{
    private readonly AppDbContext _db;

    public WorkOrderRepository(AppDbContext db) {
        _db = db;
    }
    public async Task AddAsync(WorkorderAddFormDTO entity) {
        _db.WorkOrders.Add(new WorkOrder {
            Name = entity.Name,
            StartDate = DateTime.Parse(entity.StartDate),
            FinishDate = DateTime.Parse(entity.FinishDate)
        });

        await _db.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id) {
        var entity = await _db.WorkOrders.SingleOrDefaultAsync(w => w.Id == id);
        
        if (entity == null) {
            return;
        }
        _db.WorkOrders.Remove(entity);
        
        await _db.SaveChangesAsync();
    }

    public async Task<bool> EditAsync(WorkorderEditFormDTO entity) {
        var model = await _db.WorkOrders.SingleOrDefaultAsync(w => w.Id == entity.Id);
        
        if (model == null) {
            return false;
        }

        model.Id = entity.Id;
        model.Name = entity.Name;
        model.StartDate = DateTime.Parse(entity.StartDate);
        model.FinishDate = DateTime.Parse(entity.FinishDate);

        await _db.SaveChangesAsync();
        return true;
    }

    public async Task<WorkorderViewDTO> GetAsync(int id) {
        return await _db.WorkOrders
            .Select(e => new WorkorderViewDTO {
                Id = e.Id,
                Name = e.Name,
                StartDate = e.StartDate.ToShortDateString(),
                FinishDate = e.FinishDate.ToShortDateString()
            })
            .SingleOrDefaultAsync(e => e.Id == id);
    }

    public async Task<List<WorkorderViewDTO>> GetAllAsync() {
        return await _db.WorkOrders
            .Select(e => new WorkorderViewDTO {
                Id = e.Id,
                Name = e.Name,
                StartDate = e.StartDate.ToShortDateString(),
                FinishDate = e.FinishDate.ToShortDateString()
            })
            .ToListAsync();
    }
}