using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlannerCRM.Shared.DTOs.EmployeeDto.Forms;
using PlannerCRM.Shared.Models;

namespace PlannerCRM.Server.Controllers;

[ApiController]
[Route("[controller]")]
public class AccountController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly SignInManager<IdentityUser> _signInManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public AccountController(
        UserManager<IdentityUser> userManager,
        SignInManager<IdentityUser> signInManager,
        RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _roleManager = roleManager;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(EmployeeLoginDTO employee) {
        if (!ModelState.IsValid) {
            return BadRequest("Input non valido!");
        }

        var user = await _userManager.FindByEmailAsync(employee.Email);

        if (user == null) {
            return BadRequest("Utente non trovato!");
        } else {
            var userPasswordIsCorrect = await _userManager.CheckPasswordAsync(user, employee.Password);

            if (!userPasswordIsCorrect) {
                return BadRequest("Password sbagliata!");
            } else {
                await _signInManager.SignInAsync(user, true);
                return Ok("Connesso!");
            }
        }
    }

    [Authorize(Roles = nameof(Roles.ACCOUNT_MANAGER))]
    [HttpPost("add/user")]
    public async Task<ActionResult> AddUser(EmployeeAddForm employeeAdd) {
        if (!ModelState.IsValid) {
            return BadRequest("Input non valido.");
        }

        var person = await _userManager.FindByEmailAsync(employeeAdd.Email);
        
        if (person != null) {
            return BadRequest("Utente già esistente.");
        } else {
            var user = new IdentityUser {
                Email = employeeAdd.Email,
                EmailConfirmed = true,
                UserName = employeeAdd.Email
            };
            
            await _userManager.CreateAsync(user, employeeAdd.Password);
            var userRole = await _roleManager.Roles
                .SingleAsync(aspRole => Enum.GetNames(typeof(Roles)).Any(role => role == aspRole.Name));
            
            await _userManager.AddToRoleAsync(user, userRole.Name);
        }

        return Ok("Utente aggiunto con successo!");
    }

    [Authorize(Roles = nameof(Roles.ACCOUNT_MANAGER))]
    [HttpPut("edit/user/{oldEmail}")]
    public async Task<ActionResult> EditUser(EmployeeEditForm employeeEdit, string oldEmail) {
        if (!ModelState.IsValid) {
            return BadRequest("Input non valido!");
        }

        var person = await _userManager.FindByEmailAsync(oldEmail);
        
        if (person == null) {
            return BadRequest("Utente non trovato!");
        } else if (person != null) {
            var user = new IdentityUser {
                Email = employeeEdit.Email,
                EmailConfirmed = true,
                UserName = employeeEdit.Email
            };

            await _userManager.CreateAsync(user, employeeEdit.Password);  //Da corregere il CreateAsync
            await _userManager.AddToRoleAsync(user, employeeEdit.Role.ToString());
        }
        
        return Ok("Utente modificato con successo!");
    }
    
    [Authorize(Roles = nameof(Roles.ACCOUNT_MANAGER))]
    [HttpDelete("delete/user/{email}")]
    public async Task<ActionResult> DeleteUser(string email) {
        var user = await _userManager.FindByEmailAsync(email);
        
        if (user == null) {
            return BadRequest("Utente non trovato.");
        }
        
        await _userManager.DeleteAsync(user);
        return Ok("Utente eliminato con successo!");
    }

    [HttpGet]
    [Route("user/role")]
    public async Task<string> GetUserRole() {
        var user = await _userManager.FindByNameAsync(User.Identity.Name);
        var roles = await _userManager.GetRolesAsync(user);
        
        return roles.ToList().Count() != 0 //riscrivere la query in modo più pulito
            ? roles.ToList()[0] 
            : string.Empty;
    }

    [Authorize]
    [HttpGet("logout")]
    public async Task Logout() {
        await _signInManager.SignOutAsync();
    }

    [HttpGet("current/user/info")]
    public CurrentUser CurrentUserInfo() {
        return new CurrentUser {
            IsAuthenticated = User.Identity.IsAuthenticated,
            UserName = User.Identity.Name,
            Claims = User.Claims
                .ToDictionary(c => c.Type, c => c.Value)
        };
    }
}