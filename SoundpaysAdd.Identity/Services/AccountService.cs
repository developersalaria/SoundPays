using SoundpaysAdd.Data.DTO.Account;
using SoundpaysAdd.Identity.Interfaces;
using SoundpaysAdd.Core.Wrappers;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using SoundpaysAdd.Core.DTO.Account;
using SoundpaysAdd.Core.Enums;
using System.Data;
using SoundpaysAdd.Core.Helpers;
using SoundpaysAdd.Identity.Models;
using SoundpaysAdd.Data.DTO.Email;
using Microsoft.EntityFrameworkCore;
using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.Interfaces;

namespace Infrastructure.Identity.Services
{
    public class AccountService : IAccountService
    {
        #region Prop
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly JWTSettings _jwtSettings;
        private readonly ISmsSender _smsSender;

        #endregion

        #region Ctor
        public AccountService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, SignInManager<ApplicationUser> signInManager,
            IOptions<JWTSettings> jWTSettings, ISmsSender smsSender)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _jwtSettings = jWTSettings.Value;
            _smsSender = smsSender;
        }

        #endregion

        //public async Task<SoundpaysAdd.Core.Wrappers.Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        //{
        //    AuthenticationResponse response = new AuthenticationResponse();
        //    try
        //    {
        //        var user = await _userManager.FindByEmailAsync(request.Email);
        //        if (user == null)
        //        {
        //            return new SoundpaysAdd.Core.Wrappers.Response<AuthenticationResponse>(response, Constants.InvalidNamePassword, false);
        //        }
        //        var result = await _signInManager.PasswordSignInAsync(user.UserName, request.Password, false, lockoutOnFailure: false);
        //        if (!result.Succeeded)
        //        {
        //            if (user.TwoFactorEnabled)
        //            {
        //                return new SoundpaysAdd.Core.Wrappers.Response<AuthenticationResponse>(response, Constants.TwoFactorEnabled, false);
        //            }
        //            return new SoundpaysAdd.Core.Wrappers.Response<AuthenticationResponse>(response, Constants.InvalidNamePassword, false);
        //        }
        //        if (!user.EmailConfirmed)
        //        {
        //            return new SoundpaysAdd.Core.Wrappers.Response<AuthenticationResponse>(response, Constants.EmailNotConfirm, false);
        //        }

        //        JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
        //        response.Id = user.Id;
        //        response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
        //        response.Email = user.Email;
        //        response.UserName = user.UserName;
        //        var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
        //        response.Roles = rolesList.ToList();
        //        response.IsVerified = user.EmailConfirmed;
        //        var refreshToken = GenerateRefreshToken(ipAddress);
        //        response.RefreshToken = refreshToken.Token;
        //        return new SoundpaysAdd.Core.Wrappers.Response<AuthenticationResponse>(response, Constants.LoggedInSuccess, true);
        //    }
        //    catch(Exception ex) {

        //        return new SoundpaysAdd.Core.Wrappers.Response<AuthenticationResponse>(response, ex.Message, false);
        //    }
        //}

        public async Task<SoundpaysAdd.Core.Wrappers.Response<string>> RegisterAsync(RegisterRequest request, string origin)
        {
            var userWithSameUserName = await _userManager.FindByNameAsync(request.UserName);
            if (userWithSameUserName != null)
            {
                return new SoundpaysAdd.Core.Wrappers.Response<string>(message: $"Username '{request.UserName}' is already taken.", succeeded: false);
            }
            var user = new ApplicationUser
            {
                Email = request.Email,
                UserName = request.UserName
            };
            var userWithSameEmail = await _userManager.FindByEmailAsync(request.Email);
            if (userWithSameEmail == null)
            {
                var result = await _userManager.CreateAsync(user, request.Password);
                if (result.Succeeded)
                {
                    await _userManager.AddToRoleAsync(user, Roles.Administrator.ToString());
                    var verificationUri = await SendVerificationEmail(user, origin);
                    //TODO: Attach Email Service here and configure it via appsettings
                    //await _emailService.SendAsync(new EmailRequest() { From = "mail@codewithmukesh.com", To = user.Email, Body = $"Please confirm your account by visiting this URL {verificationUri}", Subject = "Confirm Registration" });
                    return new SoundpaysAdd.Core.Wrappers.Response<string>(message: $"User Registered. Please confirm your account by visiting this URL {verificationUri}", succeeded: true);
                }
                else
                {
                    return new SoundpaysAdd.Core.Wrappers.Response<string>(message: "", succeeded: false, errors: result.Errors.Select(x => x.Description).ToList());
                }
            }
            else
            {
                return new SoundpaysAdd.Core.Wrappers.Response<string>(message: $"Email {request.Email} is already registered.", succeeded: false);
            }
        }

        //private async Task<JwtSecurityToken> GenerateJWToken(ApplicationUser user)
        //{
        //    var userClaims = await _userManager.GetClaimsAsync(user);
        //    var roles = await _userManager.GetRolesAsync(user);

        //    var roleClaims = new List<Claim>();

        //    for (int i = 0; i < roles.Count; i++)
        //    {
        //        roleClaims.Add(new Claim("roles", roles[i]));
        //    }

        //    string ipAddress = IpHelper.GetIpAddress();

        //    var claims = new[]
        //    {
        //        new Claim(JwtRegisteredClaimNames.Sub, user.UserName),
        //        new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
        //        new Claim(JwtRegisteredClaimNames.Email, user.Email),
        //        new Claim("uid", user.Id),
        //        new Claim("ip", ipAddress)
        //    }
        //    .Union(userClaims)
        //    .Union(roleClaims);

        //    var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
        //    var signingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        //    var jwtSecurityToken = new JwtSecurityToken(
        //        issuer: _jwtSettings.Issuer,
        //        audience: _jwtSettings.Audience,
        //        claims: claims,
        //        expires: DateTime.UtcNow.AddMinutes(_jwtSettings.DurationInMinutes),
        //        signingCredentials: signingCredentials);
        //    return jwtSecurityToken;
        //}

        private string RandomTokenString()
        {
            using var rngCryptoServiceProvider = new RNGCryptoServiceProvider();
            var randomBytes = new byte[40];
            rngCryptoServiceProvider.GetBytes(randomBytes);
            // convert random bytes to hex string
            return BitConverter.ToString(randomBytes).Replace("-", "");
        }

        private async Task<string> SendVerificationEmail(ApplicationUser user, string origin)
        {
            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            var route = "api/account/confirm-email/";
            var _enpointUri = new Uri(string.Concat($"{origin}/", route));
            var verificationUri = QueryHelpers.AddQueryString(_enpointUri.ToString(), "userId", user.Id);
            verificationUri = QueryHelpers.AddQueryString(verificationUri, "code", code);
            //Email Service Call Here
            return verificationUri;
        }

        public async Task<SoundpaysAdd.Core.Wrappers.Response<string>> ConfirmEmailAsync(string userId, string code)
        {
            var user = await _userManager.FindByIdAsync(userId);
            code = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
            {
                return new SoundpaysAdd.Core.Wrappers.Response<string>(user.Id, message: $"Account confirmed for {user.Email}. You can now use the /api/Account/authenticate endpoint.", succeeded: true);
            }
            else
            {
                return new SoundpaysAdd.Core.Wrappers.Response<string>(message: $"An error occured while confirming {user.Email}.", succeeded: false);
            }
        }

        private RefreshToken GenerateRefreshToken(string ipAddress)
        {
            return new RefreshToken
            {
                Token = RandomTokenString(),
                Expires = DateTime.UtcNow.AddDays(7),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };
        }

        public async Task ForgotPassword(ForgotPasswordRequest model, string origin)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);

            // always return ok response to prevent email enumeration
            if (account == null) return;

            var code = await _userManager.GeneratePasswordResetTokenAsync(account);
            var route = "api/account/reset-password/";
            var _enpointUri = new Uri(string.Concat($"{origin}/", route));
            var emailRequest = new EmailRequest()
            {
                Body = $"You reset token is - {code}",
                To = model.Email,
                Subject = "Reset Password",
            };
            //await _emailService.SendAsync(emailRequest);
        }

        public async Task<SoundpaysAdd.Core.Wrappers.Response<string>> ResetPassword(ResetPasswordRequest model)
        {
            var account = await _userManager.FindByEmailAsync(model.Email);
            if (account == null) return new SoundpaysAdd.Core.Wrappers.Response<string>(message: $"No accounts registered with {model.Email}.", succeeded: false);
            var result = await _userManager.ResetPasswordAsync(account, model.Token, model.Password);
            if (result.Succeeded)
            {
                return new SoundpaysAdd.Core.Wrappers.Response<string>(message: Constants.PasswordResetSuccess, succeeded: true);

            }
            else
            {
                return new SoundpaysAdd.Core.Wrappers.Response<string>(message: $"Error occured while reseting the password!", succeeded: false);
            }
        }

        public async Task<SoundpaysAdd.Core.Wrappers.Response<string>> LoginTwoStep(string phoneNumber)
        {
            var twoFactorCode = "";
            //get user by phone number
            var user = await _userManager.Users.Where(x => x.PhoneNumber == phoneNumber).FirstOrDefaultAsync();
            if (user == null)
            {
                return new SoundpaysAdd.Core.Wrappers.Response<string>(message: $"No accounts registered with enterd phone number!", succeeded: false);
            }
            else
            {
                //login user
                var result = await _signInManager.PasswordSignInAsync(user, "SoundpaysAdd#321", false, true);
                if (result.RequiresTwoFactor)
                {
                    //generate two factor code
                    twoFactorCode = await _userManager.GenerateTwoFactorTokenAsync(user, "Email");
                    //send sms
                    await _smsSender.SendSmsAsync(phoneNumber, twoFactorCode);
                    return new SoundpaysAdd.Core.Wrappers.Response<string>(message: $"OTP sent successfully!", succeeded: true);
                }
                else
                {
                    return new SoundpaysAdd.Core.Wrappers.Response<string>(message: $"No accounts registered with enterd phone number!", succeeded: false);
                }
            }
        }

        public async Task<SoundpaysAdd.Core.Wrappers.Response<AuthenticationResponse>> LoginTwoStep(bool rememberMe, string twoFactorCode, bool rememberMachine, string ipAddress)
        {
            AuthenticationResponse response = new AuthenticationResponse();
            var user = await _signInManager.GetTwoFactorAuthenticationUserAsync();
            if (user == null)
            {
                return new SoundpaysAdd.Core.Wrappers.Response<AuthenticationResponse>(response, Constants.InvalidPhoneNumber, false);
            }

            var authenticatorCode = twoFactorCode.Replace(" ", string.Empty).Replace("-", string.Empty);

            var result = await _signInManager.TwoFactorSignInAsync("Email", authenticatorCode, false, false);

            //JwtSecurityToken jwtSecurityToken = await GenerateJWToken(user);
            //response.Id = user.Id;
            //response.JWToken = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
            response.Email = user.Email;
            response.UserName = user.UserName;
            var rolesList = await _userManager.GetRolesAsync(user).ConfigureAwait(false);
            response.Roles = rolesList.ToList();
            response.IsVerified = user.EmailConfirmed;
            var refreshToken = GenerateRefreshToken(ipAddress);
            response.RefreshToken = refreshToken.Token;
            return new SoundpaysAdd.Core.Wrappers.Response<AuthenticationResponse>(response, Constants.LoggedInSuccess, true);
        }

        #region Identity User
        /// <summary>
        /// Get current user roles
        /// </summary>
        /// <param name="identity"></param>
        /// <returns></returns>
        public async Task<LoggedInUserViewModel> GetCurrentUserDetailsByNameAsync(string name)
        {
            LoggedInUserViewModel model = new LoggedInUserViewModel();
            try
            {
                var user = await _userManager.FindByNameAsync(name);
                if (user != null)
                {
                    model.UserName = user.UserName;
                    model.UserId = user.Id;
                    var roles = await _userManager.GetRolesAsync(user);
                    foreach (var role in roles)
                    {
                        var roleDetail = await _roleManager.FindByNameAsync(role);
                        if (roleDetail != null)
                        {
                            model.Roles.Add(new UserRoleModel
                            {
                                UserIdentityRoleId = roleDetail.Id,
                                UserRoleName = roleDetail.Name
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return model;
        }


        /// <summary>
        /// Enable Lockout User
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> LockoutUserAsync(string userId)
        {
            bool status = true;
            try
            {
                var user = await _userManager.FindByIdAsync(userId);
                if (user != null)
                {
                    user.LockoutEnabled = true;
                    user.LockoutEnd = DateTime.Now.AddYears(100);
                    await _userManager.UpdateAsync(user);
                }
            }
            catch (Exception ex)
            {
                status = false;
            }
            return status;
        }

        public Task<Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress)
        {
            throw new NotImplementedException();
        }

        Task<LoggedInUserViewModel> IAccountService.GetCurrentUserDetailsByNameAsync(string name)
        {
            throw new NotImplementedException();
        }
        #endregion
    }

}
