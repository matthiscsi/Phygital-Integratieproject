using System.Net;
using System.Net.Mail;
using IP.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PIP.BL.IManagers;
using PIP.Domain.Deelplatform;
using PIP.Domain.User;
using UI.MVC.Models.Dto;

namespace UI.MVC.Controllers.Administrator
{
    public class AdministratorController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly IUserManager _userBlManager;
        private readonly ISubPlatformManager _subPlatformManager;

        public AdministratorController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager,
            IUserManager userBlManager, ISubPlatformManager subPlatformManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _userBlManager = userBlManager;
            _subPlatformManager = subPlatformManager;
        }
        
        [Authorize(Roles = "admin")]
        public IActionResult CreateSubPlatformAdmin()
        {
            return View();
        }

        [Authorize(Roles = "admin")]
        public IActionResult GetAllSubPlatform()
        {
            return View(_userBlManager.GetAllSubPlatformAdminsWithSubplatform());
        }

        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> CreateSubPlatformAdmin(
            CreateSubPlatformAdminViewModel createSubPlatformAdminViewModel)
        {
            var user = new SubPlatformAdministrator
            {
                UserName = createSubPlatformAdminViewModel.Email, Email = createSubPlatformAdminViewModel.Email,
                OrganizationName = createSubPlatformAdminViewModel.Name
            };

            if (!ModelState.IsValid)
            {
                return View();
            }

            var subplatform = _subPlatformManager.AddSubPlatform(new Subplatform
            {
                CustomerName = createSubPlatformAdminViewModel.Name,
                MainText = createSubPlatformAdminViewModel.MainText, Email = createSubPlatformAdminViewModel.Email,
                Link = createSubPlatformAdminViewModel.Link
            });


            user.Subplatform = subplatform;
            user.EmailConfirmed = true;

            var result = await _userManager.CreateAsync(user, createSubPlatformAdminViewModel.Password);
            if (result.Succeeded)
            {
                if (!await _roleManager.RoleExistsAsync("subplatformadministrator"))
                {
                    await _roleManager.CreateAsync(new IdentityRole("subplatformadministrator"));
                }


                await _userManager.AddToRoleAsync(user, "subplatformadministrator");

                using (SmtpClient client = new SmtpClient("smtp-relay.brevo.com", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("programmersinparis@gmail.com", "2fUXd3JPj6nWmIrc");

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("programmersinparis@gmail.com");
                    message.To.Add(createSubPlatformAdminViewModel.Email);
                    message.Subject = "Uw account is aangemaakt";

                    string htmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css'>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='jumbotron'>
                                <h1 class='display-4'>Uw account is aangemaakt</h1>
                                <p class='lead'>Beste {createSubPlatformAdminViewModel.Name},</p>
                                <hr class='my-4'>
                                <p>Uw account is succesvol aangemaakt. Hieronder vindt u uw inloggegevens:</p>
                                <ul class='list-group'>
                                    <li class='list-group-item'><strong>E-mailadres:</strong> {createSubPlatformAdminViewModel.Email}</li>
                                    <li class='list-group-item'><strong>Wachtwoord:</strong> {createSubPlatformAdminViewModel.Password}</li>
                                </ul>
                                <p class='lead'>
                                    <a class='btn btn-primary btn-lg' href='https://phygital.programmersinparis.net/Identity/Account/Login' role='button'>Inloggen</a>
                                </p>
                            </div>
                        </div>
                    </body>
                    </html>";

                    message.Body = htmlBody;
                    message.IsBodyHtml = true;

                    client.Send(message);
                }

                TempData["SuccessMessage"] = "De e-mail is succesvol verzonden.";
                return RedirectToAction("CreateSubPlatformAdmin");
            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return View();
            }
        }


        [HttpGet]
        [Authorize(Roles = "admin")]
        public IActionResult EditSubPlatform(string id)
        {
            var subPlatformAdmin = _userBlManager.GetSubPlatformAdminAndSubplatform(id);
            if (subPlatformAdmin == null)
            {
                return NotFound();
            }


            var subPlatformDto = new SubPlatformAdministratorDto()
            {
                Id = subPlatformAdmin.Id,
                Name = subPlatformAdmin.OrganizationName,
                Email = subPlatformAdmin.Email,
                SubPlatformId = subPlatformAdmin.Subplatform.SubplatformId,
                MainText = subPlatformAdmin.Subplatform.MainText,
                Link = subPlatformAdmin.Subplatform.Link
            };

            return View(subPlatformDto);
        }

        [HttpPost]
        public async Task<IActionResult> EditSubPlatform(SubPlatformAdministratorDto subPlatformDto)
        {
            if (!ModelState.IsValid)
            {
                return View(subPlatformDto);
            }

            var subPlatform = _userBlManager.GetSubPlatformAdminAndSubplatform(subPlatformDto.Id);
            if (subPlatform == null)
            {
                return NotFound();
            }


            subPlatform.OrganizationName = subPlatformDto.Name;
            subPlatform.UserName = subPlatformDto.Email;

            subPlatform.Email = subPlatformDto.Email;
            subPlatform.Subplatform.CustomerName = subPlatformDto.Name;
            subPlatform.Subplatform.MainText = subPlatformDto.MainText;

            subPlatform.Subplatform.Link = subPlatformDto.Link;

            var result = await _userManager.UpdateAsync(subPlatform);
            if (!result.Succeeded)
            {
                return View(subPlatformDto);
            }

            TempData["SuccessMessage"] = "De subplatform is succesvol aangepast.";
            return RedirectToAction("GetAllSubPlatform");
        }
    }
}