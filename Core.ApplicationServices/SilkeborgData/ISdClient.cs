using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.ApplicationServices
{
    public interface ISdClient
    {
        SdKoersel.AnsaettelseKoerselOpret20170501Type SendRequest(SdKoersel.AnsaettelseKoerselOpretInputType requestData);
    }
}
