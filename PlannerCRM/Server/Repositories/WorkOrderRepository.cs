namespace PlannerCRM.Server.Repositories;

public class WorkOrderRepository
{
	private readonly AppDbContext _dbContext;
	private readonly DtoValidatorUtillity _validator;
	private readonly ILogger<DtoValidatorUtillity> _logger;

	public WorkOrderRepository(AppDbContext dbContext, DtoValidatorUtillity validator, Logger<DtoValidatorUtillity> logger) {
		_dbContext = dbContext;
		_validator = validator;
		_logger = logger;
	}

	public async Task AddAsync(WorkOrderFormDto dto) {
		try	{
			var isValid = await _validator.ValidateWorkOrderAsync(dto, OperationType.ADD);
			
			if (isValid) {
				await _dbContext.WorkOrders.AddAsync(new WorkOrder {
					Id = dto.Id,
					Name = dto.Name,
					StartDate = dto.StartDate ?? throw new NullReferenceException(ExceptionsMessages.NULL_ARG),
					FinishDate = dto.FinishDate ?? throw new NullReferenceException(ExceptionsMessages.NULL_ARG),
					IsDeleted = false,
					IsCompleted = false,
					ClientId = dto.ClientId,
				});

				if (await _dbContext.SaveChangesAsync() == 0) {
					throw new DbUpdateException(ExceptionsMessages.IMPOSSIBLE_SAVE_CHANGES);
				}

				await SetForeignKeyToClientAsync(dto.ClientId);
			} else {
				throw new DbUpdateException(ExceptionsMessages.IMPOSSIBLE_ADD);
			}
		} catch (Exception exc) {
			_logger.LogError("Error: { } Message: { }", exc.StackTrace, exc.Message);

			throw;
		}
	}

	private async Task SetForeignKeyToClientAsync(int clientId) {
		try {
			var workOrder = await _dbContext.WorkOrders
				.SingleAsync(wo=> wo.ClientId == clientId);

			var client = await _dbContext.Clients
				.SingleAsync(cl => cl.Id == clientId);

			client.WorkOrderId = workOrder.Id;

			_dbContext.Update(client);

			if (await _dbContext.SaveChangesAsync() == 0) {
				throw new DbUpdateException(ExceptionsMessages.IMPOSSIBLE_SAVE_CHANGES);
			}
		} catch(Exception exc) {
			_logger.LogError("Error: { } Message: { }", exc.StackTrace, exc.Message);

			throw;
		}
	}

	public async Task DeleteAsync(int id) {
		try {
			var workOrderDelete = await _validator.ValidateDeleteWorkOrderAsync(id);

			workOrderDelete.IsDeleted = true;
			
			if (await _dbContext.SaveChangesAsync() == 0) {
				throw new DbUpdateException(ExceptionsMessages.IMPOSSIBLE_SAVE_CHANGES);
			}
		} catch (Exception exc) {
			_logger.LogError("Error: { } Message: { }", exc.StackTrace, exc.Message);

			throw;
		}
	}

	public async Task EditAsync(WorkOrderFormDto dto) {
        try {
			var isValid = await _validator.ValidateWorkOrderAsync(dto, OperationType.EDIT);

			if (isValid) {
				var model = await _dbContext.WorkOrders
											.SingleAsync(wo => (!wo.IsDeleted) && (!wo.IsCompleted) && wo.Id == dto.Id);
				
				model.Id = dto.Id;
				model.Name = dto.Name;
				model.StartDate = dto.StartDate ?? throw new NullReferenceException(ExceptionsMessages.NULL_ARG);
				model.FinishDate = dto.FinishDate ?? throw new NullReferenceException(ExceptionsMessages.NULL_ARG);

				if (await _dbContext.SaveChangesAsync() == 0) {
					throw new DbUpdateException(ExceptionsMessages.IMPOSSIBLE_SAVE_CHANGES);
				}

				await SetForeignKeyToClientAsync(dto.ClientId);
			} else {
				throw new DbUpdateException(ExceptionsMessages.IMPOSSIBLE_SAVE_CHANGES);
			}
		} catch (Exception exc) {
			_logger.LogError("Error: { } Message: { }", exc.StackTrace, exc.Message);

			throw;
		}
	}
	
	public async Task<WorkOrderDeleteDto> GetForDeleteAsync(int id) {
		return await _dbContext.WorkOrders
			.Where(wo => !wo.IsDeleted && !wo.IsCompleted)
			.Select(wo => new WorkOrderDeleteDto {
				Id = wo.Id,
				Name = wo.Name,
				StartDate = wo.StartDate,
				FinishDate = wo.FinishDate,
				ClientName = _dbContext.Clients
					.Single(cl => cl.Id == wo.ClientId)
					.Name,})
			.SingleAsync(wo => wo.Id == id);
	}

	public async Task<WorkOrderViewDto> GetForViewAsync(int id) {
		return await _dbContext.WorkOrders
			.Select(wo => new WorkOrderViewDto {
				Id = wo.Id,
				Name = wo.Name,
				StartDate = wo.StartDate,
				FinishDate = wo.FinishDate,
				IsCompleted = wo.IsCompleted,
				IsDeleted = wo.IsDeleted,
				//Client = _dbContext.Clients
				//	.Select(cl => 
				//		new ClientViewDto() {
				//			Id = cl.Id,
				//			Name = cl.Name,
				//			VatNumber = cl.VatNumber,
				//			WorkOrderId = cl.WorkOrderId
				//		}
				//	)
				//	.Single(cl => cl.WorkOrderId == wo.Id)
			})
			.SingleOrDefaultAsync(wo => wo.Id == id);
	}
	
	public async Task<WorkOrderFormDto> GetForEditAsync(int id) {
		return await _dbContext.WorkOrders
			.Where(wo => !wo.IsDeleted && !wo.IsCompleted)
			.Select(wo => 
				new WorkOrderFormDto {
					Id = wo.Id,
					Name = wo.Name,
					StartDate = wo.StartDate,
					FinishDate = wo.FinishDate,
					ClientId = wo.ClientId,
					ClientName = _dbContext.Clients
						.Single(cl => cl.Id == wo.ClientId)
						.Name,
				}
			)
			.SingleAsync(wo => wo.Id == id);
	}

    public async Task<List<WorkOrderSelectDto>> SearchWorkOrderAsync(string workOrder) {
        return await _dbContext.WorkOrders
			.Where(wo => !wo.IsDeleted && !wo.IsCompleted)
			.Select(wo => 
				new WorkOrderSelectDto{
					Id = wo.Id,
					Name = wo.Name,
					ClientName = _dbContext.Clients
						.Single(cl => cl.WorkOrderId == wo.Id)
						.Name
				}
			)
			.Where(e => EF.Functions.ILike(e.Name , $"%{workOrder}%"))
			.ToListAsync();
    }

	public async Task<List<WorkOrderViewDto>> GetPaginated(int limit = 0, int offset = 5) {
		return await _dbContext.WorkOrders
			.OrderBy(wo => wo.Name)
			.Skip(limit)
			.Take(offset)
            .AsSplitQuery()
			.Select(wo => new WorkOrderViewDto {
				Id = wo.Id,
				Name = wo.Name,
				StartDate = wo.StartDate,
				FinishDate = wo.FinishDate,
				IsCompleted = wo.IsCompleted,
				IsDeleted = wo.IsDeleted,
				//Client = _dbContext.Clients
				//	.Select(cl => new ClientViewDto() {
				//		Id = cl.Id,
				//		Name = cl.Name,
				//		VatNumber = cl.VatNumber,
				//		WorkOrderId = wo.Id
				//	})
				//	.Single(cl => cl.Id == wo.ClientId)
			})
			.ToListAsync();
	}

	public async Task<int> GetSize() => await _dbContext.WorkOrders.CountAsync();
}