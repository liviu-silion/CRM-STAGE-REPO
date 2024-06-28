namespace PlannerCRM.Server.Repositories;

public class WorkTimeRecordRepository
{
    private readonly AppDbContext _dbContext;
    private readonly DtoValidatorUtillity _validator;
	private readonly ILogger<DtoValidatorUtillity> _logger;

    public WorkTimeRecordRepository(
        AppDbContext db, 
        DtoValidatorUtillity validator, 
        Logger<DtoValidatorUtillity> logger) 
    {
	    _dbContext = db;
	    _validator = validator;
		_logger = logger;
	}

    public async Task AddAsync(WorkTimeRecordFormDto dto) {
        try {
            var isValid = _validator.ValidateWorkTime(dto);

            if (isValid) {
                await _dbContext.WorkTimeRecords.AddAsync(await dto.MapToWorkTimeRecord(_dbContext));
        
				if (await _dbContext.SaveChangesAsync() == 0) {
					throw new DbUpdateException(ExceptionsMessages.IMPOSSIBLE_SAVE_CHANGES);
				}
            } else {
                throw new DbUpdateException(ExceptionsMessages.IMPOSSIBLE_SAVE_CHANGES);
            }
        } catch (Exception exc) {
            _logger.LogError("\nError: {0} \n\nMessage: {1}", exc.StackTrace, exc.Message);
            
            throw;
        }
    }

    public async Task DeleteAsync(int id) {
        var workTimeRecordDelete = await _dbContext.WorkTimeRecords
            .SingleAsync(wtr => wtr.Id == id)
                ?? throw new KeyNotFoundException(ExceptionsMessages.OBJECT_NOT_FOUND);
        
        _dbContext.WorkTimeRecords.Remove(workTimeRecordDelete);
        
        if (await _dbContext.SaveChangesAsync() == 0) {
            throw new DbUpdateException(ExceptionsMessages.IMPOSSIBLE_SAVE_CHANGES);
        }
    }
    
    public async Task EditAsync(WorkTimeRecordFormDto dto) {
        try {
            var isValid = _validator.ValidateWorkTime(dto);
            
            if (isValid) {
                var model = await _dbContext.WorkTimeRecords
                    .SingleAsync(wtr => wtr.Id == dto.Id);
        
                model = await dto.MapToWorkTimeRecord(_dbContext);
                _dbContext.Update(model);
        
				if (await _dbContext.SaveChangesAsync() == 0) {
					throw new DbUpdateException(ExceptionsMessages.IMPOSSIBLE_SAVE_CHANGES);
				}
            } else {
                throw new DbUpdateException(ExceptionsMessages.IMPOSSIBLE_SAVE_CHANGES);
            }
        } catch (Exception exc) {
            _logger.LogError("\nError: {0} \n\nMessage: {1}", exc.StackTrace, exc.Message);
            
            throw;
        }
    }

    public async Task<WorkTimeRecordViewDto> GetAsync(int workOrderId, int activityId, string employeeId) {
        var hasElements = await _dbContext.WorkTimeRecords
            .AnyAsync(wtr => 
                wtr.ActivityId == activityId && 
                wtr.EmployeeId == employeeId);
                
        return hasElements 
            ? await _dbContext.WorkTimeRecords
                .Select(wtr => wtr.MapToWorkTimeRecordViewDto(_dbContext, workOrderId, activityId, employeeId))
                .OrderByDescending(wtr => wtr.Hours)
                .FirstAsync(wtr => 
                    wtr.WorkOrderId == workOrderId && 
                    wtr.ActivityId == activityId && 
                    wtr.EmployeeId == employeeId)
            : new();
    }

    public async Task<List<WorkTimeRecordViewDto>> GetPaginatedWorkTimeRecordsAsync(int limit, int offset) {
        return await _dbContext.WorkTimeRecords
            .OrderBy(wtr => wtr.Id)
            .Skip(limit)
            .Take(offset)
            .Select(wtr => wtr.MapToWorkTimeRecordViewDto())
            .ToListAsync();
    }

    public async Task<WorkTimeRecordViewDto> GetAllWorkTimeRecordsByEmployeeIdAsync(string employeeId) {
        return await _dbContext.WorkTimeRecords
            .Select(wtr => wtr.MapToWorkTimeRecordViewDto())
            .SingleAsync(wtr => wtr.EmployeeId == employeeId);
    }
}