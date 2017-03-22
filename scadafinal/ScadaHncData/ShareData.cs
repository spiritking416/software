using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace ScadaHncData
{
    [Serializable]
    public class ShareData : MarshalByRefObject
    {
        Dictionary<UInt64, string> myDictionary = new Dictionary<UInt64, string>();
       
        public int ncDatas_CountMax = 200;
        public int plcData_CountMax = 1000;
        public int agvData_CountMax = 300;
        public int rgvData_CountMax = 100;


        public List<HNCData> ncDatas;
        public List<PLCData> plcData;
        public List<AGVData> agvData;
        public List<RGVData> rgvData;

        public List<MES_DISPATCH> m_workorderlist;
        public List<STRAY_M> m_straylist;
        public List<QUALTY_PARA> m_qtestlist;
        public List<EQUIP_STATE> m_equipstate;
       // public List<MES_EQUIP_CMD> MES_EQUIP_CMDList;

        public List<SYGOLE_TOOL_INFO> m_ToolInfolist;//刀库信息
        public List<MES_EQUIP_CMD> m_equipcmdlist;  //设备指令信息
        public String m_getcnctextfilestr;//获取到的cnc文本文件内容
        public String m_getcnctextfilename;//获取cnc文本文件文件名
        public String m_getcncEQUIP_CODE;//获取cnc部件ID

        public IntPtr m_Ptr;//保存了MainForm的句柄
        public bool Conneted;

        public object m_lockplcData = new object();
        public object m_lockagvData = new object();
        public object m_lockrgvData = new object();
        public object m_lockProdReport = new object();
        public object m_lockequipstate = new object();
        public object m_lockToolInfo = new object();
        public object m_lockEquipcmd = new object();
        public object m_lockgetcnctextfilestr = new object();

        public bool Isconeted = false;

        public ShareData(IntPtr m_Ptr)
        {
            ncDatas = new List<HNCData>();
            plcData = new List<PLCData>();
            agvData = new List<AGVData>();
            rgvData = new List<RGVData>();
            m_workorderlist = new List<MES_DISPATCH>();
            m_straylist = new List<STRAY_M>();
            m_qtestlist = new List<QUALTY_PARA>();
            m_equipstate = new List<EQUIP_STATE>();
            m_equipcmdlist = new List<MES_EQUIP_CMD>();
            m_workorderlist.Clear();
            m_straylist.Clear();
            m_qtestlist.Clear();
            m_equipcmdlist.Clear();
            this.m_Ptr = m_Ptr;
        }
        public static ShareData _gShareData;


        public List<EQUIP_STATE> GetEquipmentState()
        {
            var list = new List<EQUIP_STATE>();
            if (m_equipstate.Count > 0)
            {
                lock (m_lockequipstate)
                {
                    foreach (var item in m_equipstate)
                    {
                        list.Add(new EQUIP_STATE()
                        {
                            EQUIP_CODE = item.EQUIP_CODE,// VARCHAR2(50),设备ID
                            EQUIP_CODE_CNC = item.EQUIP_CODE_CNC, // VARCHAR2(50),cnc:SN号
                            EQUIP_STATE_Column = item.EQUIP_STATE_Column, // VARCHAR2(10),设备状态，字符串表述
                            STATE_VALUE = item.STATE_VALUE, // FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
                            SWTICH_TIME = item.SWTICH_TIME, // DATE,时间戳
                            FLAG = item.FLAG, // NUMBER (1,0)
                            SN = item.SN, // NUMBER
                            MARK_TIME = item.MARK_TIME,// TIMESTAMP(6)
                            MARK_DATE = item.MARK_DATE, // VARCHAR2(10)
                            EQUIP_TYPE = item.EQUIP_TYPE, //设备类型，0:CNC,1:ROBOT,2:PLC,3:RFID,4:RGV
                        }
                        );
                    }
                    m_equipstate.Clear();
                }
            }
            return list;

        }

        public List<MONITOR_INFO> GetMonitorEventInfo()
        {
            var list = new List<MONITOR_INFO>();
            lock (m_lockplcData)
            {
                foreach (var item in plcData)
                {
                    list.Add(new MONITOR_INFO()
                    {
                        ACTION_ID = item.MONITOR_INFO_node.ACTION_ID,
                        EQUIP_CODE = item.MONITOR_INFO_node.EQUIP_CODE,
                        HAPPEN_TIME = item.MONITOR_INFO_node.HAPPEN_TIME
                    }
                    );
                }
                plcData.Clear();
            }
            return list;
        }

        public List<AGV_MONITOR_INFO> GetAGVMonitorEventInfo()
        {
            var list = new List<AGV_MONITOR_INFO>();
            lock (m_lockagvData)
            {
                foreach (var item in agvData)
                {
                    list.Add(new AGV_MONITOR_INFO()
                    {
                        ACTION_ID = item.AGV_MONITOR_INFO_node.ACTION_ID,
                        EQUIP_CODE = item.AGV_MONITOR_INFO_node.EQUIP_CODE,
                    }
                    );
                }
                agvData.Clear();
            }
            return list;
        }

        public List<RGV_LOCATION> GetRGVMonitorEventInfo()
        {
            var list = new List<RGV_LOCATION>();
            lock (m_lockrgvData)
            {
                foreach (var item in rgvData)
                {
                    list.Add(new RGV_LOCATION()
                    {
                        EQUIP_CODE = item.RGV_LOCATION_node.EQUIP_CODE,
                        WC_CODE = item.RGV_LOCATION_node.WC_CODE,
                        HAPPEN_TIME = item.RGV_LOCATION_node.HAPPEN_TIME,
                        RGV_LOCATION_Column = item.RGV_LOCATION_node.RGV_LOCATION_Column,
                        RGV_SPEED = item.RGV_LOCATION_node.RGV_SPEED,
                        STATE = item.RGV_LOCATION_node.STATE
                    }
                    );
                }
                rgvData.Clear();
            }
            return list;
        }

        public List<Report_Infos> GetProdReport()
        {
            var vProdReptInfoLst = new List<Report_Infos>();
            lock (m_lockProdReport)
            {
                foreach(var vItem in ncDatas)
                {
                    if (null == vItem.sysData)
                    {
                        continue;
                    }
                    vProdReptInfoLst.Add(new Report_Infos() { sysData = vItem.sysData, reportList = new List<PROD_REPORT>(vItem.reportList) });
                    vItem.reportList.Clear();
                }
                
            }
            return vProdReptInfoLst;
        }

        //添加刀具信息
        public List<SYGOLE_TOOL_INFO> GetToolInfo()
        {

            var vToolInfoLst = new List<SYGOLE_TOOL_INFO>();
            lock (m_lockToolInfo)
            {
                //填充自己的代码
                //  m_ToolInfolist.Add
            }
            return vToolInfoLst;
        }

        //获取CNC文本文件
        public String GetCNCTextFileStr(String EQUIP_CODE,String FileNamge)
        {
            lock (m_lockgetcnctextfilestr)
            {
                m_getcnctextfilestr = "";
                m_getcnctextfilename = FileNamge;//获取cnc文本文件文件名
                m_getcncEQUIP_CODE = EQUIP_CODE;//获取cnc部件ID
                SendMessage(m_Ptr, 0x0400 + 200, 0, 0);
            }
            return m_getcnctextfilestr;
        }

        [DllImport("user32.dll")]
        public static extern void PostMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        [DllImport("user32.dll")]
        public static extern void SendMessage(IntPtr hWnd, int msg, int wParam, int lParam);
        public bool AddWorkOrder2List(List<MES_DISPATCH> TaskDatanode)// 生产派工单表
        {
            try
            {
                m_workorderlist.Clear();
                foreach (MES_DISPATCH tmp in TaskDatanode)
                {
                    m_workorderlist.Add(tmp);
                }
                if (TaskDatanode.Count > 0)
                {
                    PostMessage(m_Ptr, 0x0400 + 100, 0, 0);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString() + "\t\t Err：" + ex.Message);
                return false;
            }

            return true;
        }

        public bool AddMES_EQUIP_CMD2List(List<MES_EQUIP_CMD> CMDList)// 设备指令
        {
            try
            {
                m_equipcmdlist.Clear();
                lock (m_lockEquipcmd)
                {
                    foreach (MES_EQUIP_CMD tmp in CMDList)
                    {
                        m_equipcmdlist.Add(tmp);
                    }
                }
                if (CMDList.Count > 0)
                {
                    PostMessage(m_Ptr, 0x0400 + 100, 1, 0);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString() + "\t\t Err：" + ex.Message);
                return false;
            }

            return true;
        }



        public bool AddStray2List(List<STRAY_M> TaskDatanode) //小料盘
        {
            try
            {
                //TaskDatanode.createDate = System.DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss");  //2015.10.27
                foreach (STRAY_M tmp in TaskDatanode)
                {
                    m_straylist.Add(tmp);
                }
                if (TaskDatanode.Count > 0)
                {
                    PostMessage(m_Ptr, 0x0400 + 100, 10, 0);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString() + "\t\t Err：" + ex.Message);
                return false;
            }
            return true;
        }

        public bool AddQtest2List(List<QUALTY_PARA> TaskDatanode)//质量检测参考参数，下发
        {
            try
            {
                //TaskDatanode.createDate = System.DateTime.Now.ToString("yyyy/MM/dd-HH:mm:ss");  //2015.10.27
                foreach (QUALTY_PARA tmp in TaskDatanode)
                {
                    m_qtestlist.Add(tmp);
                }
                if (TaskDatanode.Count > 0)
                {
                    PostMessage(m_Ptr, 0x0400 + 100, 20, 0);
                }
            }
            catch (System.Exception ex)
            {
                Console.WriteLine(DateTime.Now.ToString()+"\t\t Err："+ ex.Message);
                return false;
            }

            return true;
        }



    }
}