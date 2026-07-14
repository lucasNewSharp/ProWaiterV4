using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProWaiter.ICBox.Model
{
    public class GestorConfiguracoes
    {
        private const string CodConfiguracaoPortaCOM = "PortaCOMIntegracaoICBox";
        private const string CodUltimoTelefoneDetectado = "UltimoTelefoneDetectado";

        private ProWaiterDbContext _context;
        private ProWaiterDbContext Context
        {
            get
            {
                if (_context == null)
                    _context = new ProWaiterDbContext();
                return _context;
            }
        }

        public void SalvarConfiguracao(string portaCOM)
        {
            Configuracao config = ObterConfiguracaoPortaCOM();
            if (config != null)
            {
                config.Valor = portaCOM;
                Context.Entry(config).State = EntityState.Modified;
                Context.SaveChanges();
            }
        }

        public Configuracao ObterConfiguracaoPortaCOM()
        {            
            Configuracao config = Context.Configuracoes.Where(c => c.Codigo == CodConfiguracaoPortaCOM).SingleOrDefault();
            return config;
        }

        public void SalvarTelefoneDetectado(string telefone)
        {            
            Configuracao config = Context.Configuracoes.Where(c => c.Codigo == CodUltimoTelefoneDetectado).SingleOrDefault();
            if (config != null)
            {
                config.Valor = string.IsNullOrEmpty(telefone) ? string.Empty : telefone;
                _context.Entry(config).State = EntityState.Modified;
                _context.SaveChanges();
            }
        }
    }
}
