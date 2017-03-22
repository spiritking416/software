using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HNCAPI.Data
{
    [Serializable]
    public class SampleCode
    {
        private Dictionary<Int32, Dictionary<SampleType, List<Int32>>> _data;
        public String JobId { get; set; }
        public Int32 SecId { get; set; }
        public String SampleDate { get; set; }
        public String GCodeFile { get; set; }
        public String ProId { get; set; }
        public Int32 LineId { get; set; }
        public Dictionary<int, Dictionary<SampleType, List<int>>> Data
        {
            get
            {
                if (_data == null)
                    _data = new Dictionary<int, Dictionary<SampleType, List<int>>>();
                return _data;
            }
        }
        public SampleCode Clone()
        {
            SampleCode ret = new SampleCode();
            ret.JobId = this.JobId;
            ret.SecId = this.SecId;
            ret.SampleDate = this.SampleDate;
            ret.GCodeFile = this.GCodeFile;
            ret.ProId = this.ProId;
            ret.LineId = this.LineId;
            foreach(Int32 key in this.Data.Keys)
            {
                Dictionary<SampleType, List<Int32>> ComData = new Dictionary<SampleType, List<int>>();
                foreach(SampleType key1 in Data[key].Keys)
                {
                    List<Int32> sData = new List<int>();
                    sData.AddRange(Data[key][key1]);
                    ComData.Add(key1, sData);
                }
                ret.Data.Add(key, ComData);
            }
            return ret;

        }
    }
}
