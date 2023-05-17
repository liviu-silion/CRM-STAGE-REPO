using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PlannerCRM.Server.CustomExceptions;
using PlannerCRM.Server.Services;
using PlannerCRM.Shared.DTOs.EmployeeDto.Forms;
using PlannerCRM.Shared.Models;
using static Microsoft.AspNetCore.Http.StatusCodes;

namespace PlannerCRM.Server.Controllers;

[Authorize(Roles = nameof(Roles.ACCOUNT_MANAGER))]
[ApiController]
[Route("[controller]")]
public class ApplicationUserController : ControllerBase
{
    private readonly ApplicationUserRepository _repo;
    private readonly Logger<ApplicationUserRepository> _logger;

    public ApplicationUserController(
        ApplicationUserRepository repo,
        Logger<ApplicationUserRepository> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    [Authorize(Roles = nameof(Roles.ACCOUNT_MANAGER))]
    [HttpPost("add/user")]
    public async Task<ActionResult> AddUser(EmployeeAddFormDto dto)
    {
        try
        {
            await _repo.AddAsync(dto);

            return Ok("Utente aggiunto con successo!");
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
        catch (InvalidOperationException invalidOpExc)
        {
            _logger.LogError(invalidOpExc.Message, invalidOpExc.StackTrace);
            return BadRequest(invalidOpExc.Message);
        }
        catch (DuplicateElementException duplicateElemExc)
        {
            _logger.LogError(duplicateElemExc.Message, duplicateElemExc.StackTrace);
            return BadRequest(duplicateElemExc.Message);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc.StackTrace, exc.Message);
            return StatusCode(Status503ServiceUnavailable);
        }
    }

    [Authorize(Roles = nameof(Roles.ACCOUNT_MANAGER))]
    [HttpPut("edit/user/{oldEmail}")]
    public async Task<ActionResult> EditUser(
        EmployeeEditFormDto dto,
        string oldEmail)
    {
        try
        {
            await _repo.EditAsync(dto, oldEmail);

            return Ok("Utente modificato con successo!");
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
        catch (KeyNotFoundException keyNotFoundExc)
        {
            _logger.LogError(keyNotFoundExc.Message, keyNotFoundExc.StackTrace);
            return NotFound(keyNotFoundExc.Message);
        }
        catch (InvalidOperationException invalidOpExc)
        {
            _logger.LogError(invalidOpExc.Message, invalidOpExc.StackTrace);
            return BadRequest(invalidOpExc.Message);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc.StackTrace, exc.Message);
            return StatusCode(Status503ServiceUnavailable);
        }
    }

    [Authorize(Roles = nameof(Roles.ACCOUNT_MANAGER))]
    [HttpDelete("delete/user/{email}")]
    public async Task<ActionResult> DeleteUser(string email)
    {
        try
        {
            await _repo.DeleteAsync(email);

            return Ok("Utente eliminato con successo!");
        }
        catch (NullReferenceException nullRefExc)
        {
            _logger.LogError(nullRefExc.Message, nullRefExc.StackTrace);
            return BadRequest(nullRefExc.Message);
        }
        catch (KeyNotFoundException keyNotFoundExc)
        {
            _logger.LogError(keyNotFoundExc.Message, keyNotFoundExc.StackTrace);
            return BadRequest(keyNotFoundExc.Message);
        }
        catch (Exception exc)
        {
            _logger.LogError(exc.StackTrace, exc.Message);
            return StatusCode(Status503ServiceUnavailable);
        }
    }
}