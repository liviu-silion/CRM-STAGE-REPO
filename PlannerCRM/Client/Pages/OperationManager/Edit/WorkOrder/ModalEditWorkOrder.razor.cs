namespace PlannerCRM.Client.Pages.OperationManager.Edit.WorkOrder;

[Authorize(Roles = nameof(Roles.OPERATION_MANAGER))]
public partial class ModalEditWorkOrder : ComponentBase
{
    [Parameter] public int Id { get; set; }
    [Parameter] public string Title { get; set; }
    
    [Inject] public OperationManagerCrudService OperationManagerService { get; set; }
    [Inject] public NavigationManager NavManager { get; set; }

    private WorkOrderFormDto _model;
    private string _currentPage;

    protected override async Task OnInitializedAsync() {
        _model = await OperationManagerService.GetWorkOrderForEditAsync(Id);
    }

    protected override void OnInitialized() {
        _model = new();
        _currentPage = NavManager.Uri.Replace(NavManager.BaseUri, "/");
    }

    private async Task OnClickModalConfirm(WorkOrderFormDto returnedModel) {
        await OperationManagerService.EditWorkOrderAsync(returnedModel);

        NavManager.NavigateTo(_currentPage, true);
    }

}