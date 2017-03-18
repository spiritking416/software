using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using PLC;
using Collector;
using ScadaHncData;

namespace SCADA
{
    public class PLCDataShare
    {
        private String LindeStr;
        public static List<PLC_MITSUBISHI_HNC8> m_plclist = new List<PLC_MITSUBISHI_HNC8>();
        //上报设备动作事件信息，获取失败时返回值为null
        private ShareData m_ShareData;
        public PLCDataShare(ShareData SData)
        {
            m_ShareData = SData;
            LindeStr = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.productionline]);
        }

        public void StarCollectThread()
        {
            foreach (PLC_MITSUBISHI_HNC8 m_slplc in m_plclist)
            {
                m_slplc.StarCollectThread();
                if (m_slplc.system == SCADA.m_xmlDociment.PLC_System[0])
                {
                    PLC_MITSUBISHI_HNC8.AutoSendMotionHandler += new PLC.PLC_MITSUBISHI_HNC8.EventHandler<PLC.PlcEvent>(this.AutoSendDataHandler);
                }
                else if (m_slplc.system == SCADA.m_xmlDociment.PLC_System[1])
                {
                    CollectHNCPLC.SendMonitorMsgHandler += new Collector.CollectHNCPLC.EventHandler<PLC.PlcEvent>(this.AutoSendDataHandler);
                }
            }
        }
        /// <summary>
        /// 添加PLC设备对象
        /// </summary>
        /// <param name="plc"></param>
        public void AddPLC(PLC_MITSUBISHI_HNC8 plc)
        {
            m_plclist.Add(plc);
        }

        /// <summary>
        /// 关闭
        /// </summary>
        public void ClosePLC_MITSUBISHI()
        {
             foreach (PLC_MITSUBISHI_HNC8 m_slplc in m_plclist)
            {
                m_slplc.Close();
            }
        }

        /// <summary>
        /// 事件自动触发处理，生成PLC上报数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="Args"></param>
        private void AutoSendDataHandler(object sender, PlcEvent Args)
        {
            if (Args.ACTION_ID != null && Args.EQUIP_CODE != null
                && Args.ACTION_ID.Length > 0 && Args.EQUIP_CODE.Length > 0)
            {
                if (Args.Type == "RGV")
                {
                    RGV_LOCATION node = new RGV_LOCATION();
                    node.EQUIP_CODE = Args.EQUIP_CODE;
                    node.WC_CODE = LindeStr;
                    node.RGV_LOCATION_Column = Args.ACTION_ID;
                    node.STATE = sbyte.Parse(Args.Value.ToString());
                    node.RGV_SPEED = decimal.Parse(Args.ArrLabel);
                    node.HAPPEN_TIME = DateTime.Now;
                    RGVData SendData = new RGVData(node);
                    lock (m_ShareData.m_lockrgvData)
                    {
                        m_ShareData.rgvData.Add(SendData);
                        while (m_ShareData.rgvData.Count > m_ShareData.agvData_CountMax)
                        {
                            m_ShareData.rgvData.RemoveAt(0);
                        }
                    }
                }
                else
                {
                    MONITOR_INFO node = new MONITOR_INFO();
                    node.ACTION_ID = Args.ACTION_ID;
                    node.EQUIP_CODE = Args.EQUIP_CODE;
                    node.HAPPEN_TIME = DateTime.Now;
                    PLCData SendData = new PLCData(node);
                    lock (m_ShareData.m_lockplcData)
                    {
                        m_ShareData.plcData.Add(SendData);
                        while (m_ShareData.plcData.Count > m_ShareData.plcData_CountMax)
                        {
                            m_ShareData.plcData.RemoveAt(0);
                        }
                    }
                }
            }
        }

    }


}
