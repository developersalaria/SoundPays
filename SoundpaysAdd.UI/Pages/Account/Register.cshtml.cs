using System.Net.Mail;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json.Linq;
using SoundpaysAdd.Core.DTO.Account;
using SoundpaysAdd.Core.Interfaces;
using SoundpaysAdd.Core.Models;
using SoundpaysAdd.Identity.Models;

namespace SoundpaysAdd.UI.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class RegisterModel : PageModel
    {
        #region Prop
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly ILogger<RegisterModel> _logger;
        //private readonly IEmailSenderService _emailSender;
        //private readonly IOrganizationRepositoryAsync _organizationRepository;
        //private readonly IUserOrganizationRepositoryAsync _userOrganizationRepository;
        //private readonly ICoordinatorRepositoryAsync _coordinatorRepository;
        //private readonly IUserRepositoryAsync _userRepository;
        #endregion

        #region Ctor
        public RegisterModel(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            ILogger<RegisterModel> logger

         )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;

        }
        #endregion

        #region Bind Prop
        [BindProperty]
        public RegisterViewModel registerViewModel { get; set; }
        public string? ReturnUrl { get; set; }
        public IList<AuthenticationScheme> ExternalLogins { get; set; }
        #endregion

        #region Methods
        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                if (await _userManager.FindByEmailAsync(registerViewModel.Email) != null)
                {
                    ModelState.AddModelError(string.Empty, $"Email {registerViewModel.Email} is already registered.");
                    return Page();
                }

                returnUrl = returnUrl ?? Url.Content("~/");
                //ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
                var user = new ApplicationUser
                {
                    UserName = registerViewModel.UserName.Replace(" ", string.Empty),
                    Email = registerViewModel.Email,
                    PhoneNumber = registerViewModel.PhoneNumber,
                    EmailConfirmed = true,
                    PhoneNumberConfirmed = true,
                };
                //add identity user
                var result = await _userManager.CreateAsync(user, registerViewModel.Password);

                if (!result.Succeeded)
                {
                    foreach (var error in result.Errors)
                    {
                        if (error.Code == "DuplicateUserName")
                        {
                            error.Description = $"Username '{registerViewModel.UserName}' is already taken. ";
                        }

                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                    return Page();
                }

                //add identity role
                await _userManager.AddToRoleAsync(user, Core.Enums.Roles.Advertiser.ToString());

            }

            catch (Exception ex)
            {
                string errorMessage = ex.Message.Contains("Sequence contains more than one element.") ? $"Email {registerViewModel.Email} is already registered." : "Please try again.";
                ModelState.AddModelError(string.Empty, errorMessage);
            }
            return Page();
        }
        #endregion
    }
}


