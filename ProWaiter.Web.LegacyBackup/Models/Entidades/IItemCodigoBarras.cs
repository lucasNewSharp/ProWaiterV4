using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ProWaiter.Web.Models.Entidades
{
    public interface IItemCodigoBarras
    {
        string Nome { get; }
        string CodBarras { get; }
    }
}