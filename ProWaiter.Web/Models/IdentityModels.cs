using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace ProWaiter.Web.Models
{
    public class ApplicationUser : IdentityUser
    {
        [Display(Name = "Usuário")]
        public override string UserName
        {
            get { return base.UserName; }
            set { base.UserName = value; }
        }
    }
}