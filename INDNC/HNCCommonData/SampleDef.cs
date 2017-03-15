using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HNCAPI.Data
{
   public enum SampleType
    {
        PROGID = 0, RUNLINE = 1, CURRENT = 2, CMDPOS = 3, ACTPOS = 4, CH_STATUS = 5, TOOLNO = 6, TOOL_SWITCH_TIME = 7, ACTSPD = 8       
    };
   public class CHANNEL_STATUS
   {
       public const Int32 CH_STATE_CYCLING = 0x0020; //自动运行中
   };
   [Serializable]
   public class SampleConfigure
   {
       public SampleConfigure(Int32 channel, Int32 component, SampleType type
                            , Int32 multipleCof, Int32 divideCof, Int32 offsetCof)
       {
           this._Channel = channel;
           this._Component = component;
           this._SampleType = type;
           this._MultipleCof = multipleCof;
           this._DivideCof = divideCof;
           this._OffsetCof = offsetCof;
       }
       public const Int32 REDISCHANNELCOUNT = 1;
        private Int32 _Channel;
        public System.Int32 Channel
        {
            get { return _Channel; }
            set { _Channel = value; }
        }
        private Int32 _Component;
        public System.Int32 Component
        {
            get { return _Component; }
            set { _Component = value; }
        }
        private SampleType _SampleType;
        public SampleType SampleType
        {
            get { return _SampleType; }
            set { _SampleType = value; }
        }
        private Int32 _MultipleCof;
        public System.Int32 MultipleCof
        {
            get { return _MultipleCof; }
            set { _MultipleCof = value; }
        }
        private Int32 _DivideCof;
        public System.Int32 DivideCof
        {
            get { return _DivideCof; }
            set { _DivideCof = value; }
        }
        private Int32 _OffsetCof;
        public System.Int32 OffsetCof
        {
            get { return _OffsetCof; }
            set { _OffsetCof = value; }
        }
    }

}
