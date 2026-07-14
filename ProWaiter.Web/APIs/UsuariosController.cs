using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.EntityFrameworkCore;

using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using ProWaiter.Web.Models;
using ProWaiter.Web.Util;

using ProWaiter.Web.Models.GestoresBD;

namespace ProWaiter.Web.APIs
{
    public class UsuariosController : ControllerBase
    {
        public class Usuario
        {
            public string Login { get; set; }

        }

        private ProWaiterContext db = new ProWaiterContext();

        // GET: api/Usuarios
        public IEnumerable<Usuario> GetUsuarios()
        {
            return db.Users.Select(u => new Usuario { Login = u.UserName });
        }

        public IEnumerable<Usuario> GetUsuarios(string nomeGrupo)
        {
            var role = db.Roles.Where(r => r.Id == nomeGrupo).SingleOrDefault();
            if (role == null) return new Usuario[] { };
            return new Usuario[] { };
        }
        
//         // // protected void Dispose(bool disposing)
//         {
//             if (disposing)
//             {
//                 // // db.Dispose();
//             }
//             // base.Dispose(disposing);
//         }
    }
}
