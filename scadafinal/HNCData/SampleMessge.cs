using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HNCAPI.Data
{
    [Serializable]
    public class SampleMessge
    {
        public string Channel { get; set; }
        public List<int> ComponentId { get; set; }
        public List<SampleType> DateType { get; set; }
    }
}
