// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable


using System.ComponentModel.DataAnnotations;
using System.Text;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;

using SoundpaysAdd.Identity.Models;
using SoundpaysAdd.Services.Repositories;

namespace SoundpaysAdd.UI.Areas.Identity.Pages.Account
{
    public class ForgotPasswordModel : PageModel
    {
        #region Prop

        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        private readonly IEmailSenderService _emailSender;
        private readonly ApiUserRepositoryAsync _user;


        #endregion
        public ForgotPasswordModel(UserManager<ApplicationUser> userManager,
            IEmailSenderService emailSender,
            IUserRepositoryAsync user)
        {
            _userManager = userManager;
            _emailSender = emailSender;
            //_user = user;

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
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _userManager.FindByEmailAsync(Input.Email);

                    if (user == null || !(await _userManager.IsEmailConfirmedAsync(user)))
                    {
                        return RedirectToPage("./ForgotPasswordConfirmation");
                    }
                    var code = await _userManager.GeneratePasswordResetTokenAsync(user);
                    code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                    var callbackUrl = Url.Page(
                        "/Account/ResetPassword",
                        pageHandler: null,
                        values: new { code },
                        protocol: Request.Scheme);
                    var subject = "Reset Password Request!";
                    JObject emailParams = new JObject();
                    emailParams.Add("callbackUrl", callbackUrl);
                    //7 template number
                    await _emailSender.SendEmailAsync(user.Email, subject, emailParams, 7);

                    return RedirectToPage("./ForgotPasswordConfirmation");
                }
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Sequence contains more than one element."))
                {
                    ModelState.AddModelError(string.Empty, "Please try with different email.");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Please try again.");
                }
            }

            return Page();
        }
    }
}
