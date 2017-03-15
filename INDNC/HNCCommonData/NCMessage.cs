using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNCAPI.Data
{
    [Serializable]
    public class NCMessage
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

    /// <summary>
    /// NCMessage Index参数的定义
    /// </summary>
    public class NCMessageFunction
    {
        public static int REG_SET = 1;
        public static int REG_CLR = 0;

        public static int PARAMAN_SET = 0;
        public static int PARAMAN_SAVE = 1;
    }
}
