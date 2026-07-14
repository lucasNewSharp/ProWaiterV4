using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ProWaiter.Validador.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ProWaiter.Validador
{
    public class ProWaiterValidadorContext : DbContext
    {        
        public ProWaiterValidadorContext(DbContextOptions<ProWaiterValidadorContext> options): base(options)
        {            
            
        }

        public DbSet<Validacao> Licencas { get; set; }
    }
}
