using System;
using System.Collections.Generic;
using System.Text;

namespace PortalRevendedorProWaiter.Shared
{
    public class DefaultRequestResult
    {
        public bool Successful { get; set; }
        public IEnumerable<string> Errors { get; set; }
    }
}
