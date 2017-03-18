using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HNCAPI.Data;

namespace HNCAPI
{
    
    public class ComponentSample
    {
        private SampleConfigure _Configure;
        public SampleConfigure Configure
        {
            get { return _Configure; }
            set { _Configure = value; }
        }
        private System.Collections.Generic.List<Int32> _SampleData;
        public System.Collections.Generic.List<Int32> SampleData
        {
            get { return _SampleData; }
            set { _SampleData = value; }
        }
        public ComponentSample(SampleConfigure conf) 
        {
            _Configure = conf;
            _SampleData = new List<int>();
        }
        public ComponentSample CloneEmpty()
        {
            ComponentSample cp = new ComponentSample(_Configure);
            return cp;
        }
    }
}
