using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using HNCAPI.Data;

namespace HNCAPI
{
    class RealTimeDataSubscriber
    {
        private String _KeyName;
        private List<Int32> _ComponentId;
        private List<SampleType> _DateType;
        public RealTimeDataSubscriber(String ListKey,List<Int32> Component,List<SampleType> DataType)
        {
            _KeyName = ListKey;
            _ComponentId = Component;
            _DateType = DataType;
        }
        public string KeyName
        {
            get
            {
                return _KeyName;
            }

            set
            {
                _KeyName = value;
            }
        }
        public List<SampleCode>  ETL(List<SampleCode> Alldata)
        {
            List<SampleCode> ret = new List<SampleCode>();
            for(int i=0;i<Alldata.Count;i++)
            {
                SampleCode code = Alldata[i].Clone();
                foreach(Int32 key in Alldata[i].Data.Keys)
                {
                    if(_ComponentId.Contains(key)==false)
                    {
                        code.Data.Remove(key);
                    }
                    else
                    {
                        foreach(SampleType key2 in Alldata[i].Data[key].Keys)
                        {
                            if(_DateType.Contains(key2)==false)
                            {
                                code.Data[key].Remove(key2);
                            }
                        }
                    }
                }
                ret.Add(code);

            }
            return ret;

        }
    }
}
