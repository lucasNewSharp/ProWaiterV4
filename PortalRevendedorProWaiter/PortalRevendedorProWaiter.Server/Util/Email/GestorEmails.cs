using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Identity;
using System.Web;

namespace PortalRevendedorProWaiter.Server.Util.Email
{
    public class GestorEmails : IGestorEmails
    {
        private readonly IEmailSender _emailSender;
        private readonly ILogger _logger;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IHttpContextAccessor _httpContextAcessor;

        public GestorEmails(IEmailSender emailSender,
            ILogger<GestorEmails> logger,
            UserManager<IdentityUser> userManager,
            IHttpContextAccessor httpContextAccessor)
        {
            _emailSender = emailSender;
            _logger = logger;
            _userManager = userManager;
            _httpContextAcessor = httpContextAccessor;
        }


        public async Task EnviarEmailConfirmarEmail(string emailParaReenviar, IUrlHelper urlHelper, HttpRequest httpRequest)
        {
            try
            {
                var user = await _userManager.FindByEmailAsync(emailParaReenviar);

                var userId = await _userManager.GetUserIdAsync(user);
                var email = await _userManager.GetEmailAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = HttpUtility.UrlEncode(code);

                string callback = httpRequest.Scheme + "://" + httpRequest.Host + "/ConfirmEmail?userId=" + userId + "&code=" + code;

                string link = string.Format("Confime a sua conta <a href='{0}'>clicando aqui</a>.", HtmlEncoder.Default.Encode(callback));

                StringBuilder corpo = new StringBuilder("<h1>Bem vindo ao Portal ProWaiter</h1>")
                .Append("<p>")
                .Append(link)
                .Append("</p>");

                await _emailSender.SendEmailAsync(email, "Confiramação de e-mail", corpo.ToString());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task EnviarEmailEsqueciMinhaSenha(string email, IUrlHelper urlHelper, HttpRequest httpRequest)
        {
            var user = await _userManager.FindByEmailAsync(email);

            // For more information on how to enable account confirmation and password reset please 
            // visit https://go.microsoft.com/fwlink/?LinkID=532713
            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = HttpUtility.UrlEncode(code);

            var callbackUrl = httpRequest.Scheme + "://" + httpRequest.Host + "/ResetPassword?code=" + code;

            string link = string.Format("Para criar uma nova senha <a href='{0}'>clique aqui</a>.", HtmlEncoder.Default.Encode(callbackUrl));

            StringBuilder corpo = new StringBuilder("<h1>Cuide da sua senha!!!</h1>")
            .Append("<p>")
            .Append(link)
            .Append("</p>");


            await _emailSender.SendEmailAsync(email, "Resetar senha", corpo.ToString());
        }
    }
}