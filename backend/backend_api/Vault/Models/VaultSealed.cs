using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace backend_api.Vault.Models
{
    public class VaultSealed
    {
        public string type { get; set; }
        public bool initialized { get; set; }
        public bool @sealed {get;set;}

        public int t { get; set; }

        public int n { get; set; }

        public int progress { get; set; }

        public string nonce { get; set; }

        public string version { get; set; }

        public bool migration { get; set; }

        public string cluster_name { get; set; }

        public string cluster_id { get; set; }

        public bool recovery_seal { get; set; }

        public string storage_type { get; set; }

 

    }
    
}
