using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNCAPI
{
    [Serializable]
    public class Variables<T>
    {
        public T Value { get; set; }

        public override String ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
