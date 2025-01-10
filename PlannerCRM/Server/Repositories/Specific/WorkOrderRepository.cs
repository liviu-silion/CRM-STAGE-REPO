namespace PlannerCRM.Server.Repositories.Specific;

public class WorkOrderRepository(AppDbContext context, IMapper mapper)
    : Repository<WorkOrder, WorkOrderDto>(context, mapper)
{
    private readonly AppDbContext _context = context;
    private readonly IMapper _mapper = mapper;

    public override async Task AddAsync(WorkOrder model)
    {
        await _context.ClientWorkOrders.AddAsync(
            new ClientWorkOrder { 
                FirmClientId = model.FirmClientId,
                WorkOrderId = model.Id
            }
        );
        await _context.SaveChangesAsync();
        
        await base.AddAsync(model);
    }

    public override async Task EditAsync(WorkOrder model, int id)
    {
        var clientWorkOrder = await _context.ClientWorkOrders
            .SingleAsync(x => x.WorkOrderId == id);

        clientWorkOrder.FirmClientId = model.FirmClientId;
        clientWorkOrder.WorkOrderId = model.Id;

        _context.Update(clientWorkOrder);

        await _context.SaveChangesAsync();

        await  base.EditAsync(model, id);
    }

    public override async Task DeleteAsync(int id)
    {
        var workOrder = await _context.WorkOrders
            .Include(w => w.Activities)
            .Include(w => w.WorkOrderCost)
            .SingleAsync(w => w.Id == id);

        _context.Remove(workOrder);
        
        await _context.SaveChangesAsync();
    }

    public override async Task<WorkOrderDto> GetByIdAsync(int id)
    {
        var workOrder = await _context.WorkOrders
            .Include(w => w.Activities)
            .Include(w => w.WorkOrderCost)
            .Include(w => w.FirmClient)
            .SingleAsync(w => w.Id == id);

        return _mapper.Map<WorkOrderDto>(workOrder);
    }

    public override async Task<ICollection<WorkOrderDto>> GetWithPagination(int limit, int offset)
    {
        var workOrder = await _context.WorkOrders
            .OrderBy(w => w.Id)
            .Skip(offset)
            .Take(limit)
            .Include(w => w.Activities)
            .Include(w => w.WorkOrderCost)
            .Include(w => w.FirmClient)
            .ToListAsync();

        return _mapper.Map<ICollection<WorkOrderDto>>(workOrder);
    }

    public async Task<WorkOrderDto> SearchWorOrderByTitle(string worOrderTitle)
    {
        var foundWorkOrder = await _context.WorkOrders
            .Include(wo => wo.WorkOrderCost)
            .Include(wo => wo.FirmClient)
            .Include(wo => wo.Activities)
            .SingleOrDefaultAsync(wo => EF.Functions.ILike(wo.Name, $"%{worOrderTitle}%"));

        return _mapper.Map<WorkOrderDto>(foundWorkOrder);
    }

    public async Task<ICollection<ActivityDto>> FindAssociatedActivitiesByWorkOrderId(int workOrderId)
    {
        var foundActivities = await _context.Activities
            .Include(ac => ac.WorkOrder)
            .Include(ac => ac.ActivityWorkTimes)
            .Include(ac => ac.EmployeeActivities)
            .Include(ac => ac.Employees)
            .Where(ac => ac.WorkOrderId == workOrderId)
            .ToListAsync();

        return _mapper.Map<ICollection<ActivityDto>>(foundActivities);
    }

    public async Task<ICollection<WorkOrderDto>> FindAssociatedWorkOrdersByClientId(int clientId)
    {
        var foundWorkOrder = await _context.WorkOrders
            .Include(wo => wo.WorkOrderCost)
            .Include(wo => wo.FirmClient)
            .Include(wo => wo.Activities)
            .Where(wo => wo.FirmClientId == clientId)
            .ToListAsync();

        return _mapper.Map<ICollection<WorkOrderDto>>(foundWorkOrder);
    }
}