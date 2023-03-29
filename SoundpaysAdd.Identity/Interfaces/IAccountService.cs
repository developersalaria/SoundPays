using SoundpaysAdd.Core.DTO;
using SoundpaysAdd.Core.DTO.Account;

using SoundpaysAdd.Data.DTO.Account;


namespace SoundpaysAdd.Identity.Interfaces
{
    public interface IAccountService
    {
        Task<SoundpaysAdd.Core.Wrappers.Response<AuthenticationResponse>> AuthenticateAsync(AuthenticationRequest request, string ipAddress);
        Task<SoundpaysAdd.Core.Wrappers.Response<string>> RegisterAsync(RegisterRequest request, string origin);
        Task<SoundpaysAdd.Core.Wrappers.Response<string>> ConfirmEmailAsync(string userId, string code);
        Task ForgotPassword(ForgotPasswordRequest model, string origin);
        Task<SoundpaysAdd.Core.Wrappers.Response<string>> ResetPassword(ResetPasswordRequest model);
        Task<LoggedInUserViewModel> GetCurrentUserDetailsByNameAsync(string name);
        Task<bool> LockoutUserAsync(string userId);
        Task<Core.Wrappers.Response<string>> LoginTwoStep(string phoneNumber);
        Task<Core.Wrappers.Response<AuthenticationResponse>> LoginTwoStep(bool rememberMe, string twoFactorCode, bool rememberMachine, string ipAddress);

    }
}
