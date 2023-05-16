using PlannerCRM.Server.DataAccess;
using PlannerCRM.Server.Models;
using Microsoft.EntityFrameworkCore;
using PlannerCRM.Shared.DTOs.Workorder.Forms;
using PlannerCRM.Shared.DTOs.Workorder.Views;
using PlannerCRM.Server.CustomExceptions;

namespace PlannerCRM.Server.Services;

public class WorkOrderRepository
{
	private readonly AppDbContext _db;

	public WorkOrderRepository(AppDbContext db) {
		_db = db;
	}

	public async Task AddAsync(WorkOrderFormDto dto) {
		if (dto.GetType() == null) {
            throw new NullReferenceException("Oggetto null.");
        }
        var isNull = dto.GetType().GetProperties()
            .Any(prop => prop.GetValue(dto) == null);
        
        if (isNull) {
            throw new ArgumentNullException("Parametri null");
        }
        
        var isAlreadyPresent = await _db.WorkOrders
            .SingleOrDefaultAsync(em => em.Id == dto.Id);
        if (isAlreadyPresent != null) {
            throw new DuplicateElementException("Oggetto già presente");
        }

		await _db.WorkOrders.AddAsync(new WorkOrder {
			Name = dto.Name,
			StartDate = dto.StartDate ?? throw new NullReferenceException(),
			FinishDate = dto.FinishDate ?? throw new NullReferenceException()
		});

		var rowsAffected = await _db.SaveChangesAsync();
        if (rowsAffected == 0) {
            throw new DbUpdateException("Impossibile proseguire.");
        }
	}

	public async Task DeleteAsync(int id) {
		var workOrderDelete = await _db.WorkOrders
			.SingleOrDefaultAsync(w => w.Id == id);
		
		if (workOrderDelete == null) {
			throw new KeyNotFoundException("Impossibile eliminare l'elemento");
		}
		_db.WorkOrders.Remove(workOrderDelete);
		
		var rowsAffected = await _db.SaveChangesAsync();
        if (rowsAffected == 0) {
            throw new DbUpdateException("Impossibile salvare i dati.");
        }
	}

	public async Task EditAsync(WorkOrderFormDto dto) {
		if (dto == null) {
            throw new NullReferenceException("Oggetto null.");
        }
        var isNull = dto.GetType().GetProperties()
            .Any(prop => prop.GetValue(dto) == null);
        if (isNull) {
            throw new ArgumentNullException("Parametri null");
        }
        
        var model = await _db.WorkOrders
			.SingleOrDefaultAsync(wo => wo.Id == dto.Id);

        if (model == null) {
            throw new KeyNotFoundException("Oggetto non trovato");
        }

		model.Id = dto.Id;
		model.Name = dto.Name;
		model.StartDate = dto.StartDate ?? throw new NullReferenceException();
		model.FinishDate = dto.FinishDate ?? throw new NullReferenceException();

		var rowsAffected = await _db.SaveChangesAsync();
        if (rowsAffected == 0) {
            throw new DbUpdateException("Impossibile salvare i dati.");
        }
	}
	
	public async Task<WorkOrderDeleteDto> GetForDeleteAsync(int id) {
		return await _db.WorkOrders
			.Select(wo => new WorkOrderDeleteDto {
				Id = wo.Id,
				Name = wo.Name,
				StartDate = wo.StartDate,
				FinishDate = wo.FinishDate})
			.SingleOrDefaultAsync(wo => wo.Id == id);
	}

	public async Task<WorkOrderViewDto> GetForViewAsync(int id) {
		return await _db.WorkOrders
			.Select(wo => new WorkOrderViewDto {
				Id = wo.Id,
				Name = wo.Name,
				StartDate = wo.StartDate,
				FinishDate = wo.FinishDate})
			.SingleOrDefaultAsync(wo => wo.Id == id);
	}
	
	public async Task<WorkOrderFormDto> GetForEditAsync(int id) {
		return await _db.WorkOrders
			.Select(wo => new WorkOrderFormDto {
				Id = wo.Id,
				Name = wo.Name,
				StartDate = wo.StartDate,
				FinishDate = wo.FinishDate})
			.SingleOrDefaultAsync(wo => wo.Id == id);
	}
	
	public async Task<List<WorkOrderViewDto>> GetAllAsync() {
		return await _db.WorkOrders
			.Select(wo => new WorkOrderViewDto {
				Id = wo.Id,
				Name = wo.Name,
				StartDate = wo.StartDate,
				FinishDate = wo.FinishDate})
			.ToListAsync();
	}

    public async Task<List<WorkOrderSelectDto>> SearchWorkOrderAsync(string workOrder) {
        return await _db.WorkOrders
			.Select(wo => new WorkOrderSelectDto{
				Id = wo.Id,
				Name = wo.Name})
			.Where(e => EF.Functions.Like(e.Name , $"%{workOrder}%"))
			.ToListAsync();
    }
}

