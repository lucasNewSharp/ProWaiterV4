using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using ProWaiter.Web.Models;
using ProWaiter.Web.Util;
using Microsoft.AspNet.Identity.EntityFramework;
using ProWaiter.Web.Models.GestoresBD;

namespace ProWaiter.Web.APIs
{
    public class UsuariosController : ApiController
    {
        public class Usuario
        {
            public string Login { get; set; }

        }

        private ApplicationDbContext db = new ApplicationDbContext();

        // GET: api/Usuarios
        public IEnumerable<Usuario> GetUsuarios()
        {
            return db.Users.Select(u => new Usuario { Login = u.UserName });
        }

        public IEnumerable<Usuario> GetUsuarios(string nomeGrupo)
        {
            var role = db.Roles.Where(r => r.Id == nomeGrupo).Include(r => r.Users).SingleOrDefault();
            if (role == null) return new Usuario[] { };
            return role.Users.Select(u => new Usuario { Login = u.UserId });
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}