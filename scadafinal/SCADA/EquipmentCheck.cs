using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    public class EquipmentCheck:IDisposable
    {
        public String[] CheckdataGridView_title = { MessageString.EquipmentCheck_CheckdataGridView_title_SN, MessageString.EquipmentCheck_CheckdataGridView_title_DeviceCode, MessageString.EquipmentCheck_CheckdataGridView_title_Name, MessageString.EquipmentCheck_CheckdataGridView_title_Status,MessageString.EquipmentCheck_CheckdataGridView_title_Content};
        public enum CheckdataGridView_titleArr_Index
        {
            SN= 0,
            DeviceCode,
            Name,
            Status,
            Content
        }
        public bool CheckdataGridView_DB_ChangeFlg = false;
        private System.Data.DataTable CheckdataGridView_DB = new System.Data.DataTable();
        public class AlarmSendData
        {
            public AlarmSendData()
            {
                this.NeedFindTeX = false;
                this.BujianID = "";
                this.alardat = new ScadaHncData.AlarmData();
            }
            public bool NeedFindTeX;
            public String BujianID;
            public ScadaHncData.AlarmData alardat;
        }
        private Dictionary<int, string> PLCAlarmTab = new Dictionary<int, string>();//PLC报警内容和报警号的对照字典

        public System.EventHandler<AlarmSendData> AlarmSendDataEvenHandle;
        public System.EventHandler<ScadaHncData.EQUIP_STATE> StateChageEvenHandle;
        public EquipmentCheck()
        {
            InitPLCAlarmNoTb();
            lock (CheckdataGridView_DBCLock)
            {
                for (int ii = 0; ii < CheckdataGridView_title.Length; ii++)
                {
                    CheckdataGridView_DB.Columns.Add(CheckdataGridView_title[ii]);
                }
                String[] rowstr = new String[CheckdataGridView_title.Length];


                string get_str = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, -1, m_xmlDociment.Default_Attributes_str1[0]);//SUM
                int CNCSUM = int.Parse(get_str);
                for (int ii = 0; ii < CNCSUM; ii++)//CNC
                {
                    rowstr[(int)CheckdataGridView_titleArr_Index.SN] = CheckdataGridView_DB.Rows.Count.ToString();
                    rowstr[(int)CheckdataGridView_titleArr_Index.DeviceCode] = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_CNC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.EQUIP_CODE]);
                    rowstr[(int)CheckdataGridView_titleArr_Index.Name] = MessageString.EquipmentCheck_Machine + (rowstr[(int)CheckdataGridView_titleArr_Index.DeviceCode].Length>3?rowstr[(int)CheckdataGridView_titleArr_Index.DeviceCode].Substring(rowstr[(int)CheckdataGridView_titleArr_Index.DeviceCode].Length - 3, 3):"");
                    rowstr[(int)CheckdataGridView_titleArr_Index.Status] = MessageString.EquipmentCheck_Status_Offline;
                    CheckdataGridView_DB.Rows.Add(rowstr);
                }

                get_str = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, -1, m_xmlDociment.Default_Attributes_str1[0]);
                for (int ii = 0; ii < int.Parse(get_str); ii++)
                {
                    rowstr[(int)CheckdataGridView_titleArr_Index.SN] = CheckdataGridView_DB.Rows.Count.ToString();
                    rowstr[(int)CheckdataGridView_titleArr_Index.Name] = "PLC";
                    rowstr[(int)CheckdataGridView_titleArr_Index.DeviceCode] = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.EQUIP_CODE]);
                    CheckdataGridView_DB.Rows.Add(rowstr);

                    String PLCSystem = MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, ii, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.system]);
                    String[] Device;
                    if (PLCSystem == m_xmlDociment.PLC_System[0])
                    {
                        Device = new String[m_xmlDociment.Default_MITSUBISHI_Device1.Length + m_xmlDociment.Default_MITSUBISHI_Device2.Length];
                        for (int qq = 0; qq < m_xmlDociment.Default_MITSUBISHI_Device1.Length; qq++)
                        {
                            Device[qq] = m_xmlDociment.Default_MITSUBISHI_Device1[qq];
                        }
                        for (int qq = 0; qq < m_xmlDociment.Default_MITSUBISHI_Device2.Length; qq++)
                        {
                            Device[m_xmlDociment.Default_MITSUBISHI_Device1.Length + qq] = m_xmlDociment.Default_MITSUBISHI_Device2[qq];
                        }
                    }
                    else if (PLCSystem == m_xmlDociment.PLC_System[1])
                    {
                        Device = new String[m_xmlDociment.Default_HNC8_Device1.Length + m_xmlDociment.Default_HNC8_Device2.Length];
                        for (int qq = 0; qq < m_xmlDociment.Default_HNC8_Device1.Length; qq++)
                        {
                            Device[qq] = m_xmlDociment.Default_HNC8_Device1[qq];
                        }
                        for (int qq = 0; qq < m_xmlDociment.Default_HNC8_Device2.Length; qq++)
                        {
                            Device[m_xmlDociment.Default_HNC8_Device1.Length + qq] = m_xmlDociment.Default_HNC8_Device2[qq];
                        }
                    }
                    else
                    {
                        Device = new String[0];
                    }
                    for (int jj = 0; jj < Device.Length; jj++)
                    {
                        string pathstr1 = m_xmlDociment.PathRoot_PLC_Item + ii.ToString();//Root/PLC/Itemii
                        string pathstr2 = pathstr1 + "/" + Device[jj];//"";
                        if (!MainForm.m_xml.CheckNodeExist(pathstr2))
                        {
                            continue;
                        }

                        Int32 Count = Int32.Parse(MainForm.m_xml.m_Read(pathstr2, -1, m_xmlDociment.Default_Attributes_str1[0]));//SUM
                        for (int kk = 0; kk < Count; kk++)
                        {
                            if (MainForm.m_xml.m_Read(pathstr2, kk, m_xmlDociment.Default_Attributes_str2[(int)m_xmlDociment.Attributes_str2.ACTION_ID]) == "PLCALARM")//PLC自定义的报警
                            {
                                rowstr[(int)CheckdataGridView_titleArr_Index.SN] = CheckdataGridView_DB.Rows.Count.ToString();
                                rowstr[(int)CheckdataGridView_titleArr_Index.Name] = MainForm.m_xml.m_Read(pathstr2, kk, m_xmlDociment.Default_Attributes_str2[(int)m_xmlDociment.Attributes_str2.name]);
                                rowstr[(int)CheckdataGridView_titleArr_Index.DeviceCode] = MainForm.m_xml.m_Read(pathstr2, kk, m_xmlDociment.Default_Attributes_str2[(int)m_xmlDociment.Attributes_str2.EQUIP_CODE]);
                                CheckdataGridView_DB.Rows.Add(rowstr);
                            }
                        }
                    }
                }
                for (int ii = CNCSUM; ii < CheckdataGridView_DB.Rows.Count; ii++)
                {
                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status] = MessageString.EquipmentCheck_Status_Offline;
                }
                AlarmSendDataEvenHandle = new EventHandler<AlarmSendData>(this.EquipMentAlarTexChangeFuc);
                StateChageEvenHandle = new EventHandler<ScadaHncData.EQUIP_STATE>(this.EquipMentSateShangeFuc);
                CheckdataGridView_DB_ChangeFlg = true;
            }
        }


        private void InitPLCAlarmNoTb()
        {
//             if (MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLC, 0, m_xmlDociment.Default_Attributes_str1[(int)m_xmlDociment.Attributes_str1.system]) == m_xmlDociment.PLC_System[0])//三菱
            {
                PLCAlarmTab.Clear();
                int Sum = 0;
                if (int.TryParse(MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLCAlarmTb, -1, m_xmlDociment.Default_Attributes_str1[0]), out Sum))
                {
                    for (int ii = 0; ii < Sum; ii++)
                    {
                        int no = 0;
                        if (int.TryParse(MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLCAlarmTb, ii, m_xmlDociment.Default_Attributes_PLCAlarmTb[0]), out no)
                            && !PLCAlarmTab.ContainsKey(no))
                        {
                            PLCAlarmTab.Add(no, MainForm.m_xml.m_Read(m_xmlDociment.PathRoot_PLCAlarmTb, ii, m_xmlDociment.Default_Attributes_PLCAlarmTb[1]));
                        }
                    }
                }
            }
        }

        object CheckdataGridView_DBCLock = new object();
        private void EquipMentSateShangeFuc(object obj, ScadaHncData.EQUIP_STATE m_State)
        {
            if (SCADA.MainForm.SendEQUIP_STATEHandler != null)//更改PLC设备状态数据
            {
                SCADA.MainForm.SendEQUIP_STATEHandler.BeginInvoke(this, m_State, null, null);
            }
            lock(CheckdataGridView_DBCLock)
            {
                int ii = 0;
                for (; ii < CheckdataGridView_DB.Rows.Count; ii++)
                {
                    if ((CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.DeviceCode]).ToString() == m_State.EQUIP_CODE)
                    {
                        switch ((int)m_State.STATE_VALUE)// FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
                        {
                            case -1:
                                CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status] =MessageString.EquipmentCheck_Status_Normal;
                                break;
                            case 0:
                                CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status] = MessageString.EquipmentCheck_Status_Idle;
                                break;
                            case 1:
                                CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status] = MessageString.EquipmentCheck_Status_running;
                                break;
                            case 2:
                                CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status] = MessageString.EquipmentCheck_Status_Cycling;
                                break;
                            case 3:
                                CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status] = MessageString.EquipmentCheck_Status_EStop;
                                break;
                            default:
                                break;
                        }
                        break;
                    }
                }
                if (ii == MainForm.cnclist.Count)//后续的PLC相关的状态跟踪着PLC状态变化
                {
                    for (; ii < CheckdataGridView_DB.Rows.Count; ii++)
                    {
                        CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status] =
                            CheckdataGridView_DB.Rows[MainForm.cnclist.Count][(int)CheckdataGridView_titleArr_Index.Status];
                    }
                }
            }
            CheckdataGridView_DB_ChangeFlg = true;
        }

        private void EquipMentAlarTexChangeFuc(object obj, AlarmSendData Meg)
        {
            if (Meg.NeedFindTeX)//在对照表中查找报警内容
            {
                if (!PLCAlarmTab.ContainsKey(Meg.alardat.alarmNo))
                {
                    return;
                }
                Meg.alardat.alarmTxt = PLCAlarmTab[Meg.alardat.alarmNo];
            }
            lock (CheckdataGridView_DBCLock)
            {
                int ii = 0;
                for (; ii < CheckdataGridView_DB.Rows.Count; ii++)
                {
                    if (Meg.BujianID == CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.DeviceCode].ToString())
                    {
                        if (!Meg.NeedFindTeX)
                        {
                            String str = /*Meg.alardat.alarmNo.ToString() + ":" +*/ Meg.alardat.alarmTxt + "；  ";
                            if (CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status].ToString().Contains(str))
                            {
                                if (Meg.alardat.isOnOff != 1)
                                {
                                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status] =
                                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status].ToString().Replace(str, "");
                                    CheckdataGridView_DB_ChangeFlg = true;
                                }
                            }
                            else
                            {
                                if (Meg.alardat.isOnOff == 1)
                                {
                                    String str1 = CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status].ToString();
                                    str1 += str;
                                    CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status] = str1;
                                    CheckdataGridView_DB_ChangeFlg = true;
                                }
                            }
                        }
                        else
                        {
                            CheckdataGridView_DB.Rows[ii][(int)CheckdataGridView_titleArr_Index.Status] = Meg.alardat.alarmTxt;
                            CheckdataGridView_DB_ChangeFlg = true;
                        }
                        break;
                    }
                }
            }
        }



        public System.Data.DataTable GetCheckdataGridView_DB()
        {
            CheckdataGridView_DB_ChangeFlg = false;
            return this.CheckdataGridView_DB.Copy();
        }

        public void Dispose()
        {
            CheckdataGridView_DB.Dispose();
        }
    }
}
