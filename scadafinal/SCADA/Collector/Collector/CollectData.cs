using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ScadaHncData;

namespace Collector
{
     public class CollectDeviceData
    {
         public virtual bool GatherData(SystemData sysdata) { return true; }
    }

     public class CollectDeviceCHData
     {
         public virtual bool GatherData(ChannelData chdata,short clientNo) { return true; }
     }

     public class CollectDeviceAXData
     {
         public virtual bool GatherData(AxisData chdata, short clientNo) { return true; }
     }
}