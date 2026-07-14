using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PortalRevendedorProWaiter.Server.Util.Email
{
    public interface IGestorEmails
    {
        Task EnviarEmailEsqueciMinhaSenha(string email, IUrlHelper urlHelper, HttpRequest httpRequest);
        Task EnviarEmailConfirmarEmail(string emailParaReenviar, IUrlHelper urlHelper, HttpRequest httpRequest);
    }
}
