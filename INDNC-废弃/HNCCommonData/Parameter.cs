using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNCAPI.Data
{
    [Serializable]
    public class Parameter<T>
    {
        public String Name { get; set; }
        public Int32 Id { get; set; }
        public T PropValue { get; set; }
        public T DefaultValue { get; set; }
        public Int32 EffectWay { get; set; }
        public T MaxValue { get; set; }
        public T MinValue { get; set; }
        public Int32 StoreType { get; set; }

        public override String ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
}
