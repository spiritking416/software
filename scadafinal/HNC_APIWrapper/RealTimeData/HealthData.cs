using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace HNCAPI
{

    [Serializable]
    public class HealthData
    {
        public double XaxisHealthIndex { get; set; }   //X轴健康
        public double YaxisHealthIndex { get; set; }   //Y轴健康
        public double ZaxisHealthIndex { get; set; }   //Y轴健康指数
        public double SpdlHealthIndex { get; set; }    //主轴健康
        public double ToolsHealthIndex { get; set; } //刀库健康指数
        public double MacHealthIndex { get; set; }    //机床健康指 


        public void SetValue(Array arry)
        {
            XaxisHealthIndex = (double)(arry.GetValue(0,0));
            YaxisHealthIndex = (double)(arry.GetValue(0, 1));
            ZaxisHealthIndex = (double)(arry.GetValue(0, 2));
            SpdlHealthIndex = (double)(arry.GetValue(0, 3));
            ToolsHealthIndex = (double)(arry.GetValue(0, 4));
            MacHealthIndex = (double)(arry.GetValue(0, 5));
        }
    }
}
