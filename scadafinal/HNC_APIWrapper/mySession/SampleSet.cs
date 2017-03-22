using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using HNCAPI.Data;

namespace HNCAPI
{
    public class SampleSet
    {
        private List<SamplChSet> _listSamplChSet;

        internal class SamplChSet
        {
            public Int32 type;
            public Int32 axis;
            public Int32 offset;
            public Int32 datalen;

            public SamplChSet(Int32 type, Int32 axis, Int32 offset, Int32 datalen)
            {
                this.type = type;
                this.axis = axis;
                this.offset = offset;
                this.datalen = datalen;
            }
        }

        public SampleSet()
        {
            _listSamplChSet = new List<SamplChSet>();

            _listSamplChSet.Add(new SamplChSet(1, 0, 0, 0));
            _listSamplChSet.Add(new SamplChSet(2, 0, 0, 0));
            _listSamplChSet.Add(new SamplChSet(6, 0, 0, 0));
            _listSamplChSet.Add(new SamplChSet(1, 1, 0, 0));
            _listSamplChSet.Add(new SamplChSet(2, 1, 0, 0));
            _listSamplChSet.Add(new SamplChSet(6, 1, 0, 0));
            _listSamplChSet.Add(new SamplChSet(1, 2, 0, 0));
            _listSamplChSet.Add(new SamplChSet(2, 2, 0, 0));
            _listSamplChSet.Add(new SamplChSet(6, 2, 0, 0));
            _listSamplChSet.Add(new SamplChSet(5, 5, 0, 0));
            _listSamplChSet.Add(new SamplChSet(6, 5, 0, 0));
            _listSamplChSet.Add(new SamplChSet(102, -1, 4, 4));
            _listSamplChSet.Add(new SamplChSet(102, -1, 7, 4));
            _listSamplChSet.Add(new SamplChSet(108, -1, 0, 4));
            _listSamplChSet.Add(new SamplChSet(112, -1, 116, 1));
            _listSamplChSet.Add(new SamplChSet(113, -1, 189, 4));
        }

        public void ReadSamplConfigFile()
        {
            String configFile = "SamplConfig.xml";
            XmlDocument xmldoc = new XmlDocument();
            Int32 type = 0;
            Int32 axis = 0;
            Int32 offset = 0;
            Int32 datalen = 0;

            try
            {
                if(File.Exists(configFile))
                {
                    _listSamplChSet.Clear();
                }
                else
                {
                    return;
                }
                xmldoc.Load(configFile);

                XmlNode SampleConfig = xmldoc.SelectSingleNode("/SampleConfig");
                XmlNodeList childlist = SampleConfig.SelectNodes("ChSet");
                foreach (XmlNode node in childlist)
                {
                    type = Convert.ToInt32(node.Attributes["type"].Value);
                    axis = Convert.ToInt32(node.Attributes["axis"].Value);
                    offset = Convert.ToInt32(node.Attributes["offset"].Value);
                    datalen = Convert.ToInt32(node.Attributes["datalen"].Value);
                    _listSamplChSet.Add(new SamplChSet(type, axis, offset, datalen));
                }
            }
            catch (Exception e)
            {
                Console.Out.WriteLine("SimpleSet"+e.Message);
            }
        }

        public List<SampleConfigure> GetSampleConfig()
        {
            List<SampleConfigure> listConfig = new List<SampleConfigure>();

            
            //_listSamplChSet.Add(new SamplChSet(1, 0, 0, 0));
            //_listSamplChSet.Add(new SamplChSet(2, 0, 0, 0));
            //_listSamplChSet.Add(new SamplChSet(6, 0, 0, 0));
            //_listSamplChSet.Add(new SamplChSet(1, 1, 0, 0));
            //_listSamplChSet.Add(new SamplChSet(2, 1, 0, 0));
            //_listSamplChSet.Add(new SamplChSet(6, 1, 0, 0));
            //_listSamplChSet.Add(new SamplChSet(1, 2, 0, 0));
            //_listSamplChSet.Add(new SamplChSet(2, 2, 0, 0));
            //_listSamplChSet.Add(new SamplChSet(6, 2, 0, 0));
            //_listSamplChSet.Add(new SamplChSet(2, 5, 0, 0));
            //_listSamplChSet.Add(new SamplChSet(6, 5, 0, 0));
            //_listSamplChSet.Add(new SamplChSet(102, -1, 4, 4));
            //_listSamplChSet.Add(new SamplChSet(102, -1, 7, 4));
            //_listSamplChSet.Add(new SamplChSet(108, -1, 0, 4));

            listConfig.Add(new SampleConfigure(0, 0, SampleType.CMDPOS, 1, 100000, 0));
            listConfig.Add(new SampleConfigure(1, 0, SampleType.ACTPOS, 1, 100000, 0));
            listConfig.Add(new SampleConfigure(2, 0, SampleType.CURRENT, 1, 1, 0));
            listConfig.Add(new SampleConfigure(3, 1, SampleType.CMDPOS, 1, 100000, 0));
            listConfig.Add(new SampleConfigure(4, 1, SampleType.ACTPOS, 1, 100000, 0));
            listConfig.Add(new SampleConfigure(5, 1, SampleType.CURRENT, 1, 1, 0));
            listConfig.Add(new SampleConfigure(6, 2, SampleType.CMDPOS, 1, 100000, 0));
            listConfig.Add(new SampleConfigure(7, 2, SampleType.ACTPOS, 1, 100000, 0));
            listConfig.Add(new SampleConfigure(8, 2, SampleType.CURRENT, 1, 1, 0));
            listConfig.Add(new SampleConfigure(9, 5, SampleType.ACTSPD, 1, 1, 0));
            listConfig.Add(new SampleConfigure(10, 5, SampleType.CURRENT, 1, 1, 0));
            listConfig.Add(new SampleConfigure(11, -1, SampleType.RUNLINE, 1, 1, 0));
            listConfig.Add(new SampleConfigure(12, -1, SampleType.PROGID, 1, 1, 0));
            listConfig.Add(new SampleConfigure(13, -1, SampleType.CH_STATUS, 1, 1, 0));
            listConfig.Add(new SampleConfigure(14, -1, SampleType.TOOL_SWITCH_TIME, 1, 1, 0));
            listConfig.Add(new SampleConfigure(15, -1, SampleType.TOOLNO, 1, 1, 0));

            return listConfig;
        }

        public void SetSamplConfig(Int16 clientNo)
        {
            HncApi.HNC_SamplSetChannel(_listSamplChSet.Count, clientNo);
            for (Int32 i = 0; i < _listSamplChSet.Count; i++)
            {
                if (_listSamplChSet[i].type >= 100)
                {
                    HncApi.HNC_SamplSetRegType(i, _listSamplChSet[i].type, _listSamplChSet[i].offset, _listSamplChSet[i].datalen, clientNo);
                }
                else
                {
                    HncApi.HNC_SamplSetPropertyType(i, _listSamplChSet[i].type, _listSamplChSet[i].axis, clientNo);
                }
            }
        }
    }
}
