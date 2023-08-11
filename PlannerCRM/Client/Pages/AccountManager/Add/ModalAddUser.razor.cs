namespace PlannerCRM.Client.Pages.AccountManager.Add;

[Authorize(Roles = nameof(Roles.ACCOUNT_MANAGER))]
public partial class ModalAddUser : ComponentBase
{
    [Parameter] public string Title { get; set; }

    [Inject] public AccountManagerCrudService AccountManagerService { get; set; }
    [Inject] public NavigationManager NavManager { get; set; }

    private EmployeeFormDto _model;
    private string _currentPage;
    
    protected override void OnInitialized() {
        _model = new();
        _currentPage = NavManager.Uri.Replace(NavManager.BaseUri, "/");
    }

    private async Task OnClickModalConfirm(EmployeeFormDto returnedModel) {
        await AccountManagerService.AddUserAsync(returnedModel);
        await AccountManagerService.AddEmployeeAsync(returnedModel);

        NavManager.NavigateTo(_currentPage, true);
    }
}