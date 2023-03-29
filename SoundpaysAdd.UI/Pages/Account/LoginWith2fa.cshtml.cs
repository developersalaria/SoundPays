// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using SoundpaysAdd.Identity.Models;

namespace SoundpaysAdd.UI.Areas.Identity.Pages.Account
{
    public class LoginWith2faModel : PageModel
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<LoginWith2faModel> _logger;

        public LoginWith2faModel(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager,
            ILogger<LoginWith2faModel> logger)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _logger = logger;
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
        public bool RememberMe { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string ReturnUrl { get; set; }

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
            [StringLength(7, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Text)]
            [Display(Name = "Code")]
            public string TwoFactorCode { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Display(Name = "Remember this machine")]
            public bool RememberMachine { get; set; }
        }

        public async Task<IActionResult> OnGetAsync(bool rememberMe, string returnUrl = null)
        {
            try
            {
                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

                if (user == null)
                {
                    return RedirectToAction("Login", new { returnUrl = returnUrl });
                }

                ReturnUrl = returnUrl;
                RememberMe = rememberMe;

            }
            catch (Exception ex)
            {
                return RedirectToAction("Login", new { returnUrl = returnUrl });
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(bool rememberMe, string returnUrl = null)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return RedirectToAction("Login", new { returnUrl = returnUrl });
                }

                returnUrl = returnUrl ?? Url.Content("~/User");

                var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
                if (user == null)
                {
                    return RedirectToAction("Login", new { returnUrl = returnUrl });
                }

                var authenticatorCode = Input.TwoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);
                var result = await _signInManager.TwoFactorSignInAsync("Email", authenticatorCode, rememberMe, Input.RememberMachine);

                var userId = await _userManager.GetUserIdAsync(user);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User with ID '{UserId}' logged in with 2fa.", user.Id);
                    return LocalRedirect(returnUrl);
                }
                else if (result.IsLockedOut)
                {
                    _logger.LogWarning("User with ID '{UserId}' account locked out.", user.Id);
                    return RedirectToPage("./Lockout");
                }
                else
                {
                    _logger.LogWarning("Invalid authenticator code entered for user with ID '{UserId}'.", user.Id);
                    ModelState.AddModelError(string.Empty, "Invalid authenticator code.");
                    return Page();
                }
            }
            catch(Exception ex)
            {
                return RedirectToAction("Login", new { returnUrl = returnUrl });
            }
        }
    }
}
