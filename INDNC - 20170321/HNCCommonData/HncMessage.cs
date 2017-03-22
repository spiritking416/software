using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNCAPI.Data
{
    [Serializable]
    public class HncMessage<T>
    {
        public String Header { get; set; }
        public T Body { get; set; }
    }
}
