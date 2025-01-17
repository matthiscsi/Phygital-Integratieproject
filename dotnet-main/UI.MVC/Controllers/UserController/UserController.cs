using Microsoft.AspNetCore.Mvc;
using PIP.BL;
using PIP.BL.IManagers;

namespace UI.MVC.Controllers.UserController;

public class UserController : Controller
{
    private readonly IUserManager _userManager;


    public UserController(IUserManager userManager)
    {
        _userManager = userManager;
    }

    // GET
    public IActionResult Index()
    {
        return View();
    }

    public IActionResult Unsubscribe()
    {
        return View();
    }

    [HttpPost]
    public IActionResult Unsubscribe(string email)
    {
        _userManager.DeleteUserByEmail(email);

        TempData["UnsubscribeMessage"] = $"Je bent uitgeschreven voor updates op het e-mailadres: {email}";

        return RedirectToAction("Index", "Home");
    }
}