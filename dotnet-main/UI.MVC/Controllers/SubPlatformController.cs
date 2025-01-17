using System.Net;
using System.Net.Mail;
using System.Security.Claims;
using IP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PIP.BL;
using PIP.BL.IManagers;
using PIP.Domain.Deelplatform;

using UI.MVC.Models.Dto;
using Companion = PIP.Domain.User;

namespace UI.MVC.Controllers;

public class SubPlatformController : Controller
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserManager _userBlManager;
    private readonly IProjectManager _projectManager;
    private readonly IFlowManager _flowManager;

    public SubPlatformController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
        IUserManager userBlManager, IProjectManager projectManager, IFlowManager flowManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _userBlManager = userBlManager;
        _projectManager = projectManager;
        _flowManager = flowManager;
    }

    [Authorize(Roles = "subplatformadministrator")]
    public Task<IActionResult> AddCompanion()
    {
        return Task.FromResult<IActionResult>(View());
    }


    [HttpPost]
    [Authorize(Roles = "subplatformadministrator")]
    public async Task<IActionResult> AddCompanion(CompanionDto companionDto)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var currentUser =
            _userBlManager.GetSubPlatformAdminAndSubplatform(User.FindFirstValue((ClaimTypes.NameIdentifier)));

        if (!ModelState.IsValid)
        {
            return View(companionDto);
        }


        var user = new Companion.Companion()
        {
            UserName = companionDto.Name, Email = companionDto.Name, EmailConfirmed = true,
            Subplatform = currentUser.Subplatform
        };


        var result = await _userManager.CreateAsync(user, companionDto.Password);


        if (result.Succeeded)
        {
            if (!await _roleManager.RoleExistsAsync("companion"))
            {
                await _roleManager.CreateAsync(new IdentityRole("companion"));
            }


            await _userManager.AddToRoleAsync(user, "companion");
            _userBlManager.AddUserToSubplatform(currentUser.Subplatform.SubplatformId, user);


            TempData["SuccessMessage"] = "De begeleider is succesvol aangemaakt.";
            return RedirectToAction("AddCompanion");
        }
        else
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }

            return RedirectToAction("AddCompanion");
        }
    }

    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult GetCompanionsOfProject(int id)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.GetProject(id);
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        _userBlManager.GetCompanionsOfProject(id);
        return null;
    }


    [Authorize(Roles = "subplatformadministrator,companion")]
    public Task<IActionResult> Projects()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Task.FromResult<IActionResult>(Redirect("/Identity/Account/Login"));
        }
        string currentUser = User.FindFirstValue(ClaimTypes.NameIdentifier);
        long subplatformid = 0;
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (User.IsInRole("subplatformadministrator"))
        {
                 subplatformid = _userBlManager.GetSubPlatformAdminAndSubplatform(userId).Subplatform.SubplatformId; 
         
        }
        else
        {
            subplatformid = _userBlManager.GetCompanionAndSubplatform(userId).Subplatform.SubplatformId;
        }


        if (currentUser != null)
        {
            var projects = _projectManager.ReadProjectsFromSubplatform(subplatformid);


            var viewModel = new CreateSubPlatformAdminViewModel
            {
                Projects = projects
            };

            return Task.FromResult<IActionResult>(View(viewModel));
        }

        return Task.FromResult<IActionResult>(null);
    }

    [HttpPost]
    [Authorize(Roles = "subplatformadministrator")]
    public Task<IActionResult> DeleteProject(long projectId)
    {
        var project = _projectManager.GetProject(projectId);

        if (project != null)
        {
            _projectManager.DeleteProject(projectId);

            TempData["SuccessMessage"] = "Het project is succesvol verwijderd.";
        }
        else
        {
            TempData["ErrorMessage"] = "Het project kon niet worden gevonden.";
        }


        return Task.FromResult<IActionResult>(RedirectToAction("Projects"));
    }

    [Authorize(Roles = "subplatformadministrator")]
    public async Task<IActionResult> GetThemes()
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }


        var currentUser = await _userManager.GetUserAsync(User);


        if (currentUser != null)
        {
            var projects = _projectManager.GetProjectsOfUser(currentUser);


            var viewModel = new CreateSubPlatformAdminViewModel
            {
                Projects = projects
            };

            return View(viewModel);
        }

        return View();
    }

    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult SendEmailToFlowUsers(long flowId)
    {
        var flow = _flowManager.GetFlow(flowId);
        if (!ModelState.IsValid)
        {
            return View(flow);
        }
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.GetProject(flow.Project.ProjectId);
        if (project == null)
        {
            return NotFound();
            
        }
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Redirect("/Identity/Account/AccessDenied");
        }

        return View(flow);
    }

    [HttpPost]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult SendEmailToFlowUsers(SendEmailToFlowUsersDto sendEmailToFlowUsersDto)
    {
        var users = _userBlManager.GetAllUsersFromFlow(sendEmailToFlowUsersDto.FlowId);
        var flow = _flowManager.GetFlow(sendEmailToFlowUsersDto.FlowId);
        if (!ModelState.IsValid)
        {
            return View(flow);
        }
        
        
        var project = _projectManager.GetProject(flow.Project.ProjectId);
        if (project == null)
        { 
            return NotFound();
            
        }
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }
        foreach (var user in users)
        {
            var email = user.Email;
            using (SmtpClient client = new SmtpClient("smtp-relay.brevo.com", 587))
            {
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential("programmersinparis@gmail.com", "2fUXd3JPj6nWmIrc");

                MailMessage message = new MailMessage();
                message.From = new MailAddress("programmersinparis@gmail.com");
                message.To.Add(email);
                message.Subject = sendEmailToFlowUsersDto.Subject;

                string htmlBody = $@"
<!DOCTYPE html>
<html>
<head>
    <link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css'>
</head>
<body>
    <div class='container'>
        <div class='jumbotron'>
            <h1 class='display-4'>{sendEmailToFlowUsersDto.Subject}</h1>
            <p class='lead'>{sendEmailToFlowUsersDto.MessageToUsers}</p>
            <hr class='my-4'>
            <p class='lead'>
                <a href='phygital.programmersinparis.net/user/unsubscribe' class='btn btn-primary'>Afmelden van updates</a>
            </p>
        </div>
    </div>
</body>
</html>";

                message.Body = htmlBody;
                message.IsBodyHtml = true;

                try
                {
                    client.Send(message);

                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] =
                        "Er is een fout opgetreden bij het verzenden van de e-mail: " + ex.Message;
                }
            }
        }
        TempData["SuccessMessage"] = "E-mail succesvol verzonden naar alle deelnemers.";
        return RedirectToAction("SendEmailToFlowUsers", new { flowId = sendEmailToFlowUsersDto.FlowId });
    }
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult AddProject()
    {
        return View();
    }

    [HttpPost]
    [Authorize(Roles = "subplatformadministrator")]
    public async Task<IActionResult> AddProject(Project project)
    {
        if (!ModelState.IsValid) return View(project);

        var user = await _userManager.GetUserAsync(User);
        var admin =  _userBlManager.GetSubPlatformAdminAndSubplatform(user.Id);
        Project projectToAdd = new Project()
        {
            SubPlatformAdmin = user, Name = project.Name, Description = project.Description, IsActive = project.IsActive, Subplatform = admin.Subplatform
        };
        _projectManager.AddProject(projectToAdd);
        TempData["Message"] = "Het project is succesvol toegevoegd.";
        return RedirectToAction("Projects");
    }


    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult UpdateProject(long id)
    {
        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        var project = _projectManager.GetProject(id);
        if (project == null)
        {
            TempData["RedirectMessage"] = "Het project kon niet worden gevonden.";
            return RedirectToAction("Index", "Home");
        }
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        return View(project);
    }

    [HttpPost]
    [Authorize(Roles = "subplatformadministrator")]
    public IActionResult UpdateProject([FromBody] ProjectDto projectDto)
    {
    

        if (!User.Identity!.IsAuthenticated)
        {
            return Redirect("/Identity/Account/Login");
        }

        if (!ModelState.IsValid)
        {
            return View();
        }

        var project = _projectManager.GetProject(projectDto.ProjectId);
        if (project == null)
        {
            TempData["RedirectMessage"] = "Het project kon niet worden gevonden.";
            return RedirectToAction("Index","Home");
        }
        bool userManager = User.Identity.Name == project.SubPlatformAdmin.UserName;
        if (!userManager)
        {
            return Unauthorized();
        }

        project.IsActive = projectDto.isActive;
        project.Description = projectDto.Description;
        project.Name = projectDto.Name;
        project.CirculaireFlow = projectDto.Circulair;
        project.BackgroundColor = projectDto.BackgroundColor;
        project.Font = projectDto.Font;
        _projectManager.UpdateProject(project);
        return RedirectToAction("Index", "Home");
    }
}