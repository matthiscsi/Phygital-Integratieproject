// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;

namespace UI.MVC.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IEmailSender _emailSender;

        public ForgotPasswordModel(UserManager<IdentityUser> userManager, IEmailSender emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.FindByEmailAsync(Input.Email);
                if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                {
                    // Don't reveal that the user does not exist or is not confirmed
                    return RedirectToPage("./ForgotPasswordConfirmation");
                }

                // For more information on how to enable account confirmation and password reset please
                // visit https://go.microsoft.com/fwlink/?LinkID=532713
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var callbackUrl = Url.Page(
                    "/Account/ResetPassword",
                    pageHandler: null,
                    values: new { area = "Identity", code },
                    protocol: Request.Scheme);

                /*        await _emailSender.SendEmailAsync(
                            Input.Email,
                            "Reset Password",
                            $"Please reset your password by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.");*/

                using (SmtpClient client = new SmtpClient("smtp-relay.brevo.com", 587))
                {
                    client.EnableSsl = true;
                    client.UseDefaultCredentials = false;
                    client.Credentials = new NetworkCredential("programmersinparis@gmail.com", "2fUXd3JPj6nWmIrc");

                    MailMessage message = new MailMessage();
                    message.From = new MailAddress("programmersinparis@gmail.com");
                    message.To.Add(user.Email);
                    message.Subject = "Wachtwoord vergeten kan gebeuren";

                    string htmlBody = $@"
            <!DOCTYPE html>
            <html>
            <head>
                <link rel='stylesheet' href='https://maxcdn.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css'>
            </head>
            <body>
                <div class='container'>
                    <div class='jumbotron'>
                        <h1 class='display-4'>Reset Uw Wachtwoord</h1>
                        <p class='lead'>Beste gebruiker,</p>
                        <hr class='my-4'>
                        <p>U heeft aangevraagd om uw wachtwoord te resetten. Klik op de onderstaande link om uw wachtwoord te resetten:</p>
                        <a href='{HtmlEncoder.Default.Encode(callbackUrl)}' class='btn btn-primary'>Wachtwoord Resetten</a>
                    </div>
                </div>
            </body>
            </html>";

                    message.Body = htmlBody;
                    message.IsBodyHtml = true;

                    client.Send(message);
                }

                return RedirectToPage("./ForgotPasswordConfirmation");
            }

            return Page();
        }
    }
}