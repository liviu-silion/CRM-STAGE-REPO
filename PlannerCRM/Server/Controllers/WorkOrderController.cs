using PlannerCRM.Server.Models.Entities;

namespace PlannerCRM.Server.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class WorkOrderController(IWorkOrderRepository workOrderRepository) : CrudController<WorkOrder>
{
    private readonly IWorkOrderRepository _workOrderRepository = workOrderRepository;

    [Authorize(Roles = nameof(Roles.OPERATION_MANAGER))]
    [HttpGet("get/for/edit/{workOrderId}")]
    public async Task<WorkOrderFormDto> GetWorkOrderForEditByIdAsync(int workOrderId) =>
        await _workOrderRepository.GetForEditByIdAsync(workOrderId);

    [HttpGet("get/for/view/{workOrderId}")]
    public async Task<WorkOrderViewDto> GetWorkOrderForViewByIdAsync(int workOrderId) =>
        await _workOrderRepository.GetForViewByIdAsync(workOrderId);

    [Authorize(Roles = nameof(Roles.OPERATION_MANAGER))]
    [HttpGet("get/for/delete/{workOrderId}")]
    public async Task<WorkOrderDeleteDto> GetWorkOrderForDeleteByIdAsync(int workOrderId) =>
        await _workOrderRepository.GetForDeleteByIdAsync(workOrderId);
    
    [HttpGet("search/{workOrder}")]
    public async Task<List<WorkOrderSelectDto>> SearchWorkOrderAsync(string workOrderName) =>
        await _workOrderRepository.SearchWorkOrderAsync(workOrderName);

    [HttpGet("get/size")] 
    public async Task<int> GetSizeAsync() =>
        await _workOrderRepository.GetWorkOrdersSizeAsync();

    [HttpGet("get/paginated/{offset}/{limit}")]
    public async Task<List<WorkOrderViewDto>> GetPaginatedWorkOrdersAsync(int offset = 0, int limit = 5) =>
        await _workOrderRepository.GetPaginatedWorkOrdersAsync(offset, limit);
}