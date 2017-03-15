using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNCAPI.Data
{
    [Serializable]
    public class NC_SetValueMessage
    {
        public String Type { get; set; }
        public String SubType { get; set; }
        public Int32 Index { get; set; }
        public String Value { get; set; }
        public override string ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
