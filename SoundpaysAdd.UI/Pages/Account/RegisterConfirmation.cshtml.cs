// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Identity.Models;


namespace SoundpaysAdd.UI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterConfirmationModel : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public RegisterConfirmationModel(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Email { get; set; }

        public string EmailConfirmationMessage { get; set; }
        public bool RegistrationSuccess { get; set; }

        public async Task<IActionResult> OnGetAsync(string email)
        {
            try
            {
                if (email == null)
                {
                    return RedirectToPage("/Index");
                }
                var user = await _userManager.FindByEmailAsync(email);
                RegistrationSuccess = user != null;
                if (user == null)
                {
                    return NotFound($"Unable to load user with email '{email}'.");
                }
                EmailConfirmationMessage = "Please check your email to confirm your account.";
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Sequence contains more than one element."))
                {
                    EmailConfirmationMessage = "Please try again with different email.";
                }
                else
                {
                    EmailConfirmationMessage = "Something went wrong!";
                }
                RegistrationSuccess = false;
            }
            return Page();
        }
    }
}
