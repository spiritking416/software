using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HNCAPI.Data
{
    [Serializable]
    public class ToolBrokenInfo
    {
        public String Channel { get; set; }
        public String GCodeFile { get; set; }
        public Int32 ToolNo { get; set; }
        public bool IsBroken { get; set; }
    }
}
