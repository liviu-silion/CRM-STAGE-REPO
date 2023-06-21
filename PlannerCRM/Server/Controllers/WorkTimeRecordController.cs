using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlannerCRM.Shared.CustomExceptions;
using PlannerCRM.Server.Services;
using PlannerCRM.Shared.DTOs.WorkTimeDto.Form;
using PlannerCRM.Shared.DTOs.WorkTimeDto.Views;
using static PlannerCRM.Shared.Constants.ConstantValues;
using static PlannerCRM.Shared.Constants.SuccessfulFeedBack;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace PlannerCRM.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class WorkTimeRecordController : ControllerBase
{
    private readonly WorkTimeRecordRepository _repo;
    private readonly Logger<WorkTimeRecordRepository> _logger;

    public WorkTimeRecordController(
        WorkTimeRecordRepository repo,
        Logger<WorkTimeRecordRepository> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [HttpPost("add")]
    public async Task<ActionResult> AddWorkTimeRecord(WorkTimeRecordFormDto dto)
    {
        try
        {
            await _repo.AddAsync(dto);

            return Ok(WORKTIMERECORD_ADD);
        }
        catch (NullReferenceException nullRefExc)
        {
            _logger.LogError(nullRefExc.Message, nullRefExc.StackTrace);
            return NotFound(nullRefExc.Message);
        }
        catch (ArgumentNullException argNullExc)
        {
            _logger.LogError(argNullExc.Message, argNullExc.StackTrace);
            return BadRequest(argNullExc.Message);
        }
        catch (DuplicateElementException duplicateElemExc)
        {
            _logger.LogError(duplicateElemExc.Message, duplicateElemExc.StackTrace);
            return BadRequest(duplicateElemExc.Message);
        }
        catch (DbUpdateException dbUpdateExc)
        {
            _logger.LogError(dbUpdateExc.Message, dbUpdateExc.StackTrace);
            return BadRequest(dbUpdateExc.Message);
        }
        catch (Exception exc)
        {
            return StatusCode(Status503ServiceUnavailable, exc.Message);
        }
    }

    [HttpPut("edit")]
    public async Task<ActionResult> EditWorkTimeRecord(WorkTimeRecordFormDto dto)
    {
        try
        {
            await _repo.EditAsync(dto);

            return Ok(WORKTIMERECORD_EDIT);
        }
        catch (NullReferenceException nullRefExc)
        {
            _logger.LogError(nullRefExc.Message, nullRefExc.StackTrace);
            return BadRequest(nullRefExc.Message);
        }
        catch (ArgumentNullException argNullExc)
        {
            _logger.LogError(argNullExc.Message, argNullExc.StackTrace);
            return BadRequest(argNullExc.Message);
        }
        catch (KeyNotFoundException keyNotFoundExc)
        {
            _logger.LogError(keyNotFoundExc.Message, keyNotFoundExc.StackTrace);
            return NotFound(keyNotFoundExc.Message);
        }
        catch (DuplicateElementException duplicateElemExc)
        {
            _logger.LogError(duplicateElemExc.Message, duplicateElemExc.StackTrace);
            return BadRequest(duplicateElemExc.Message);
        }
        catch (DbUpdateException dbUpdateExc)
        {
            _logger.LogError(dbUpdateExc.Message, dbUpdateExc.StackTrace);
            return BadRequest(dbUpdateExc.Message);
        }
        catch (Exception exc)
        {
            return StatusCode(Status503ServiceUnavailable, exc.Message);
        }
    }

    [HttpGet("get/{activityId}/{employeeId}")]
    public async Task<WorkTimeRecordViewDto> GetWorkTimeRecord(int activityId, int employeeId)
    {
        try
        {
            return await _repo.GetAsync(activityId, employeeId);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc.Message, exc.StackTrace);
            return new();
        }
    }

    [HttpGet("get/all")]
    public async Task<List<WorkTimeRecordViewDto>> GetAllWorkTimeRecords()
    {
        try
        {
            return await _repo.GetAllAsync();
        }
        catch (Exception exc)
        {
            _logger.LogError(exc.Message, exc.StackTrace);
            return new List<WorkTimeRecordViewDto>();
        }
    }

    [HttpGet("get/by/employee/{employeeId}")]
    public async Task<WorkTimeRecordViewDto> GetAllWorkTimeRecordsByWorkOrder(int employeeId)
    {
        try
        {
            return await _repo.GetByEmployeeIdAsync(employeeId);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc.Message, exc.StackTrace);
            return new WorkTimeRecordViewDto();
        }
    }
}
