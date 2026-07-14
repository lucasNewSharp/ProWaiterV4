using Microsoft.AspNetCore.Components.Authorization;
using PortalRevendedorProWaiter.Shared;
using PortalRevendedorProWaiter.Shared.IdentityModel;
using PortalRevendedorProWaiter.Shared.ViewModels;
using System.Threading.Tasks;

namespace PortalRevendedorProWaiter.Client.Services
{
    public interface IAuthService
    {
        Task<LoginResult> Login(LoginModel loginModel);
        Task Logout();
        Task<DefaultRequestResult> Register(RegisterModel registerModel);
        Task<DefaultRequestResult> ConfirmEmail(ConfirmEmailModel confirmEmailModel);
        Task<DefaultRequestResult> ResendEmailConfirmation(LoginModel loginModel);
        Task<DefaultRequestResult> ResendForgotPasswordEmail(ForgotPasswordModel forgotPasswordModel);
        Task<DefaultRequestResult> ResetPassword(ResetPasswordModel resetPasswordModel);        
    }
}
