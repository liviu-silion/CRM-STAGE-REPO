﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using PlannerCRM.Client.Services;
using PlannerCRM.Client.Services.Crud;
using PlannerCRM.Shared.CustomExceptions;
using PlannerCRM.Shared.DTOs.EmployeeDto.Forms;
using PlannerCRM.Shared.Models;

namespace PlannerCRM.Client.Pages.AccountManager.Add;

[Authorize(Roles = nameof(Roles.ACCOUNT_MANAGER))]
public partial class AccountManagerAddUserForm
{   
    private readonly Logger<EmployeeAddFormDto> _logger;
    
    public AccountManagerAddUserForm(
            Logger<EmployeeAddFormDto> logger) 
    {
        _logger = logger;
    }

    [Inject] private AccountManagerCrudService AccountManagerService { get; set; }
    [Inject] private NavigationManager NavigationManager { get; set; }
    [Inject] private NavigationLockService NavLockService { get; set; }

    public EmployeeAddFormDto _Model = new();
    public EditContext _EditContext { get; set; }
    public string _TypeField { get; set; } = InputType.PASSWORD.ToString().ToLower();
    public bool _IsError { get; set; }
    public string _Message { get; set; }
    public bool _IsCheckboxClicked { get; set; }

    protected override void OnInitialized() {
        _EditContext = new(_Model);
    }

    public void SwitchShowPassword() {
        if (_IsCheckboxClicked) {
            _IsCheckboxClicked = false;
            _TypeField = InputType.TEXT.ToString().ToLower();
        } else {
            _IsCheckboxClicked = true;
            _TypeField = InputType.PASSWORD.ToString().ToLower();
        }
    }

    public void RedirectToPage() {
        NavigationManager.NavigateTo("/account-manager");
    }

    public void OnClickCancel() {
        RedirectToPage();
    }

    public async Task OnClickConfirm() {
        try {
            _Model.EmployeeSalaries = new();
            _Model.EmployeeSalaries
                .Add(new EmployeeSalaryDto {
                    Id = _Model.Id,
                    EmployeeId = _Model.Id,
                    Salary = _Model.HourlyRate 
                        ?? throw new NullReferenceException($"""Proprietà {nameof(_Model.HourlyRate)} non può essere null."""),
                    StartDate = _Model.StartDateHourlyRate 
                        ?? throw new NullReferenceException($"""Proprietà {nameof(_Model.StartDateHourlyRate)} non può essere null."""),
                    FinishDate = _Model.FinishDateHourlyRate 
                        ?? throw new NullReferenceException($"""Proprietà {nameof(_Model.FinishDateHourlyRate)} non può essere null."""),
                });

            var responseEmployee = await AccountManagerService.AddUserAsync(_Model);
            var responseUser = await AccountManagerService.AddEmployeeAsync(_Model);

            if (!responseEmployee.IsSuccessStatusCode) {
                _Message = await responseEmployee.Content.ReadAsStringAsync();
            } else if (!responseUser.IsSuccessStatusCode) {
                _Message = await responseUser.Content.ReadAsStringAsync();
            } else {
                RedirectToPage();
            }
            _IsError = true;
        } catch (NullReferenceException nullRefExc) {
            _logger.Log(LogLevel.Error, nullRefExc.Message);
            _Message = nullRefExc.Message;
            _IsError = true;
        }
        catch (Exception exc)
        {
            _logger.Log(LogLevel.Error, exc.Message);
            _Message = exc.Message;
            _IsError = true;
        }
    }
}
