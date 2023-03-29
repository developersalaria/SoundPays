using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Net.Mail;
using SoundpaysAdd.Identity.Models;
using SoundpaysAdd.Core.DTO.Account;

namespace SoundpaysAdd.UI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class LoginModel : PageModel
    {
        #region Prop
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly ILogger<LoginModel> _logger;
        #endregion

        #region Ctor
        public LoginModel(SignInManager<ApplicationUser> signInManager,
            ILogger<LoginModel> logger,
            UserManager<ApplicationUser> userManager
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;

        }

        #endregion

        #region Bind Prop
        [BindProperty]
        public LoginViewModel loginViewModel { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        public string ReturnUrl { get; set; }
        [TempData]
        public string ErrorMessage { get; set; }
        #endregion

        #region Methods
        public async Task OnGetAsync(string returnUrl = null)
        {
            if (!string.IsNullOrEmpty(ErrorMessage))
            {
                ModelState.AddModelError(string.Empty, ErrorMessage);
            }

            returnUrl = returnUrl ?? Url.Content("~/");

            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();

            ReturnUrl = returnUrl;
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            returnUrl = returnUrl ?? Url.Content("~/Home/Index");
            try
            {
                var user = new ApplicationUser();
                //if (ModelState.IsValid)
                //{
                var userName = loginViewModel.Email;
                if (IsValidEmail(loginViewModel.Email))
                {
                    user = await _userManager.FindByEmailAsync(loginViewModel.Email);
                    if (user != null)
                    {
                        userName = user.UserName;
                    }
                }
                var result = await _signInManager.PasswordSignInAsync(userName, loginViewModel.Password, loginViewModel.RememberMe, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    _logger.LogInformation("User logged in.");
                    return LocalRedirect("~/Home/Index");
                }
                //if (result.RequiresTwoFactor)
                //{

                //    var code = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                //    var subject = "2FA Code";
                //    JObject emailParams = new JObject();
                //    emailParams.Add("code", code);
                //    //5 template number
                //    await _emailSender.SendEmailAsync(user.Email, subject, emailParams, 5);

                //    return RedirectToPage("LoginWith2fa", new { ReturnUrl = returnUrl, RememberMe = loginViewModel.RememberMe });
                //}
                if (result.IsLockedOut)
                {
                    _logger.LogWarning("User account locked out.");
                    return RedirectToPage("Lockout");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                    return Page();
                }
                //}
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return Page();
            }
        }
        public bool IsValidEmail(string emailaddress)
        {
            try
            {
                MailAddress m = new MailAddress(emailaddress);
                return true;
            }
            catch (FormatException)
            {
                return false;
            }
        }
        #endregion
    }
}
