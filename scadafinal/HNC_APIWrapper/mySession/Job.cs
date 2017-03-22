using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using HNCAPI.Data;

namespace HNCAPI
{

    class Job
    {
        private ComponentSample _PGROPChannel;
        private ComponentSample _LINEChannel;
        private ComponentSample _ChannelStatus;
        private Machine _Machine;
        private String _JobID;
        private Int32 _SecID;
        private System.Collections.Generic.Dictionary<Int32, ComponentSample> _SampleDic;
        public System.Collections.Generic.Dictionary<Int32, ComponentSample> SampleDic
        {
            get { return _SampleDic; }
            set { _SampleDic = value; }
        }
        public Job(Machine mac)
        {
            _SampleDic = new Dictionary<int, ComponentSample>();
            _Machine = mac;
            _SecID = -1;
        }
        private JobSection MakeEmptySection()
        {
            JobSection CurrentSec = new JobSection();

            for (int i = 0; i < _SampleDic.Count; i++)
            {
                CurrentSec.SampleDic[i] = _SampleDic[i].CloneEmpty();
            }
            CurrentSec.JobID = _JobID;
            return CurrentSec;
        }
        private String GetGCodeFileName(Int32 Proid)
        {
            return _Machine.GetProgramNameById(Proid);
        }
        private void RemoveSampleData(JobSection sec)
        {
            for (int i = 0; i < _SampleDic.Count; i++)
            {
                _SampleDic[i].SampleData.RemoveRange(0, sec.SampleDic[0].SampleData.Count);
            }
        }
        private void SetSpecialChannel()
        {
            if (_PGROPChannel == null)
            {
                for (int i = 0; i < _SampleDic.Count; i++)
                {
                    ComponentSample sample = _SampleDic[i];
                    if (sample.Configure.SampleType == SampleType.PROGID)
                    {
                        _PGROPChannel = sample;
                    }
                    else if (sample.Configure.SampleType == SampleType.RUNLINE)
                    {
                        _LINEChannel = sample;
                    }
                    else if (sample.Configure.SampleType == SampleType.CH_STATUS)
                    {
                        _ChannelStatus = sample;
                    }
                }
            }
        }
        private List<JobSection> CreateSection()
        {
            lock (_SampleDic)
            {
                String macSN = _Machine.MachineSN;
                if (String.IsNullOrEmpty(macSN)) return null;
                SetSpecialChannel();
                if (_LINEChannel.SampleData.Count == 0) return null;
                List<JobSection> listret = new List<JobSection>();
                Int32 CurrentRow = _LINEChannel.SampleData[0];
                Int32 PreStatus = _ChannelStatus.SampleData[0] & CHANNEL_STATUS.CH_STATE_CYCLING;
                JobSection CurrentSec = MakeEmptySection();
                #region CreatSection
                for (Int32 i = 0; i < _LINEChannel.SampleData.Count; i++)
                {
                    if ((_ChannelStatus.SampleData[i] & CHANNEL_STATUS.CH_STATE_CYCLING) != PreStatus)//状态变化了
                    {
                        if (PreStatus == 0)//从不加工到加工
                        {
                            GenerateJobId(_PGROPChannel.SampleData[i]);
                        }
                        else//从加工到不加工
                        {
                            _JobID = "";
                        }
                        listret.Add(CurrentSec);
                        CurrentSec = MakeEmptySection();
                        CurrentRow = _LINEChannel.SampleData[i];
                        PreStatus = _ChannelStatus.SampleData[i] & CHANNEL_STATUS.CH_STATE_CYCLING;

                    }
                    else if (PreStatus != 0)//加工中
                    {
                        if (String.IsNullOrEmpty(_JobID))//SCADA启动时机床已经在加工中
                        {
                            GenerateJobId(_PGROPChannel.SampleData[i]);
                            CurrentSec.JobID = _JobID;
                        }
                        if (CurrentRow != _LINEChannel.SampleData[i])
                        {
                            listret.Add(CurrentSec);
                            CurrentSec = MakeEmptySection();
                            CurrentRow = _LINEChannel.SampleData[i];
                        }
                        else if (CurrentSec.SampleDic[0].SampleData.Count > 999)//在程序运行过程中,会出现停留在一行上等待的情况.
                        {
                            listret.Add(CurrentSec);
                            CurrentSec = MakeEmptySection();
                            CurrentRow = _LINEChannel.SampleData[i];
                        }
                    }
                    else if (CurrentSec.SampleDic[0].SampleData.Count > 99)//没有加工
                    {
                        listret.Add(CurrentSec);
                        CurrentSec = MakeEmptySection();
                        CurrentRow = _LINEChannel.SampleData[i];
                    }
                    if (listret.Count>=10) break;
                    for (int j = 0; j < _SampleDic.Count; j++)
                    {
                        CurrentSec.SampleDic[j].SampleData.Add(_SampleDic[j].SampleData[i]);
                    }
                   

                }
                #endregion
                for (int i = 0; i < listret.Count; i++)
                {
                    RemoveSampleData(listret[i]);
                }
                if (listret.Count > 0) return listret;
                return null;
                

            }
        }
        private void GenerateJobId(Int32 Pid)
        {
            String GCodeName = this.GetGCodeFileName(56);
            if (String.IsNullOrEmpty(GCodeName)) 
            {
                _JobID = "";
                return;
            }
            //String dataTime = System.DateTime.Now.ToBinary().ToString();
            long curTime = CurrentTime();
            _JobID = String.Format("{0}^{1}^{2}", GCodeName, curTime, _Machine.MachineSN);
        }
        private String GetGCodeFileName()
        {
            if (String.IsNullOrEmpty(_JobID)) return "";
            String[] names=_JobID.Split('^');
            return names[0];
        }
        private SampleCode CreateCode(JobSection sec)
        {
            SampleCode code = new SampleCode();
            code.JobId = sec.JobID;
            //code.JobId = "";
            if(String.IsNullOrEmpty(sec.JobID))
            {
                _SecID = -1;
                code.SecId = _SecID;
            }
            else
            {
                code.SecId = ++_SecID;
            }
            code.GCodeFile = GetGCodeFileName();
            //code.GCodeFile = "";
            long tickes = CurrentTime();
            code.SampleDate = tickes.ToString();
            //code.SampleDate = "";
            foreach (Int32 key in sec.SampleDic.Keys)
            {
                ComponentSample com = sec.SampleDic[key];
                if (com.Configure.Channel == _PGROPChannel.Configure.Channel)
                {
                    code.ProId =this.GetGCodeFileName(com.SampleData[0]);
                    continue;
                }
                else if (com.Configure.Channel == _LINEChannel.Configure.Channel)
                {
                    
                    code.LineId = com.SampleData[0];
                    continue;
                }
                if (code.Data.Keys.Contains(com.Configure.Component) == false)
                {
                    Dictionary<SampleType, List<Int32>> list = new Dictionary<SampleType, List<int>>();
                    code.Data.Add(com.Configure.Component, list);
                }
                code.Data[com.Configure.Component].Add(com.Configure.SampleType, com.SampleData);

            }
            return code;


        }

        private long CurrentTime()
        {
            return (System.DateTime.UtcNow.Ticks - new DateTime(1970, 1, 1, 0, 0, 0).Ticks) / 10000;
        }
        public List<SampleCode> CreateLines()
        {
            try
            {
                List<JobSection> secs = CreateSection();
                if (secs == null) return null;
                String MacSN = _Machine.MachineSN;
                if (String.IsNullOrEmpty(MacSN)) return null;
                List<SampleCode> ret = new List<SampleCode>();
                for (int i = 0; i < secs.Count; i++)
                {
                    JobSection sec = secs[i];
                    SampleCode code = CreateCode(sec);
                    System.Threading.Thread.Sleep(1);
                    ret.Add(code);
                }
                return ret;
            }
            catch (Exception e)
            {
                Console.Write("CreateLines" + e.StackTrace);
            }
            return null;

        }
        public void AddData(List<List<Int32>> data)
        {
            lock (_SampleDic)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    ComponentSample sample = _SampleDic[i];
                    sample.SampleData.AddRange(data[i]);
                }
            }
        }
        
    }
}
