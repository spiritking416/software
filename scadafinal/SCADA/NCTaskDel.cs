using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ScadaHncData;

namespace SCADA
{
    public class NCTaskDel
    {
        public String[] TaskNodeName = {"编号","工单状态", "机台编号", "派工单号", "物料编码", "生产数量","报工数量",
                                        "计划开始时间", "计划完成时间", "NC编号", "NC版本号", "作业指导号",
                                        "作业指导版本号", "线体", "工序编码", "生产订单", "操作标示", 
                                        "SN", "时间戳", "日期", "NC文件路径", "NC作业指导书路径","物料标识" };//工单状态: -1忽略，0待派工，1派工失败，2派工成功，3正在加工，4加工完成
        public enum NodeName
        {
            编号 = 0,
            工单状态,
            机台编号,
            派工单号,
            物料编码,
            生产数量,
            报工数量,
            计划开始时间,
            计划完成时间,
            NC编号,
            NC版本号,
            作业指导号,
            作业指导版本号,
            线体,
            工序编码,
            生产订单,
            操作标示,
            SN,
            时间戳,
            日期,
            NC文件路径,
            NC作业指导书路径,
            short_name
        }
        int TaskManCount = 30;
        private System.Data.DataTable TaskDb;
        String NCTaskXMLFileName;
        public String CNCBuJianID;
        private System.Data.DataRow nowTaskNode;//正在处理的工单

        public NCTaskDel(String LoadFileName, String CNCBuJianID)
        {
            this.CNCBuJianID = CNCBuJianID;
            NCTaskLoadXMLFile(LoadFileName);
            System.Threading.Thread t = new System.Threading.Thread(this.AutoSaveData2XmlFuc);//开启保存后台线程
            t.Start();
        }
        private void NCTaskLoadXMLFile(String FileName)//将XML数据加载到TaskDb
        {
            String[] Pathstr = FileName.Split('\\');
            Pathstr[0] = FileName.Replace(Pathstr[Pathstr.Length - 1], "");
            NCTaskXMLFileName = Pathstr[0] + CNCBuJianID + Pathstr[Pathstr.Length - 1];
            Pathstr[0] = Pathstr[0].Substring(0, Pathstr[0].Length - 1);
            if (!System.IO.Directory.Exists(Pathstr[0]))
            {
                System.IO.Directory.CreateDirectory(Pathstr[0]);
            }
            if (System.IO.File.Exists(NCTaskXMLFileName))
            {
                try
                {
                    TaskDb = SCADA.RFIDDATAT.DBReadFromXml(NCTaskXMLFileName);
                    if (TaskDb.TableName.Length == 0)
                    {
                        TaskDb.TableName = ((int)LineDevice.CNC.CNCCMDRegValueType.IN).ToString();
                    }
                    for (int ii = 0; ii < TaskDb.Rows.Count;ii++ )
                    {
                        if (TaskDb.Rows[ii][(int)NodeName.工单状态].ToString() == "3")
                        {
                            nowTaskNode = TaskDb.Rows[ii];
                        }
                    }
                }
                catch

                {
                    System.IO.File.Delete(NCTaskXMLFileName + "errer");
                    System.IO.File.Move(NCTaskXMLFileName, NCTaskXMLFileName + "errer");
                    MakeDefaulDB(ref TaskDb);
                    System.Windows.Forms.MessageBox.Show(NCTaskXMLFileName + "格式已经被破坏，文件被备份为：" + NCTaskXMLFileName + "errer");
                }
            }
            else
            {
                MakeDefaulDB(ref TaskDb);
            }
        }
        private void MakeDefaulDB(ref System.Data.DataTable m_TaskDb)//初始化工单结构
        {
            if (m_TaskDb == null)
            {
                m_TaskDb = new System.Data.DataTable();
            }
            else
            {
                m_TaskDb.Rows.Clear();
                m_TaskDb.Columns.Clear();
            }
            m_TaskDb.TableName = ((int)LineDevice.CNC.CNCCMDRegValueType.IN).ToString(); //开机
            for (int index = 0; index < TaskNodeName.Length; index++)
            {
                m_TaskDb.Columns.Add(TaskNodeName[index]);
            }
        }
        public System.Data.DataTable GetTaskDb()
        {
            return TaskDb.Copy();
        }

        public void AddMCTask(MES_DISPATCH node)
        {
            switch (node.FLAG)
            {
                case 1://新增
                case 4://尾单
                    if (nowTaskNode == null && TaskDb.Rows.Count > 0
                        && TaskDb.Rows[TaskDb.Rows.Count - 1][(int)NodeName.工单状态].ToString() == "4")
                    {
                        NCTaskInser2DB(node, "3");
                    }
                    else
                    {
                        NCTaskInser2DB(node, "0");
                    }
                    AutoSaveData2Xml_Flag = true;
                    SaveData2Xml_threaFucEvent.Set();
                    break;
                case 2://修改
                case 3://删除
                    ModifNCtask(node);
                    NCTaskInser2DB(node, "-1");
                    AutoSaveData2Xml_Flag = true;
                    SaveData2Xml_threaFucEvent.Set();
                    break;
                default:
                    break;
            }
            
        }
        private void ModifNCtask(MES_DISPATCH node)//修改已有工单
        {
            for (int ii = 0; ii < TaskDb.Rows.Count; ii++)
            {
                if (TaskDb.Rows[ii][(int)NodeName.派工单号].ToString() == node.DISPATCH_CODE)
                {
                    switch (node.FLAG)
                    {
                        case 2://修改
                            TaskDb.Rows[ii][(int)NodeName.生产数量] = node.QTY;
                            break;
                        case 3://删除
                            if (TaskDb.Rows[ii][(int)NodeName.工单状态].ToString() == "3")//正在加工的工单
                            {
                                if (TaskDb.Rows[ii][(int)NodeName.操作标示].ToString() != "4")//非尾单  强制切单
                                {
                                    if (nowTaskNode != null && TaskDb.Rows.Count > (int.Parse(nowTaskNode[(int)NodeName.编号].ToString()) + 1))
                                    {
                                        nowTaskNode = TaskDb.Rows[ii + 1];
                                        nowTaskNode[(int)NodeName.操作标示] = "3";
                                    }
                                }
                            }
                            TaskDb.Rows[ii][(int)NodeName.工单状态] = "-1";
                            break;
                        default:
                            break;
                    }
                }
            }
        }
        private void NCTaskInser2DB(MES_DISPATCH node, String Stated)//将派工单添加到TaskDb
        {
            if (TaskDb.Rows.Count >= TaskManCount)
            {
                if (TaskDb.Rows[0][(int)NodeName.工单状态].ToString() == "3")
                {
                    for (int ii = 1; ii < TaskDb.Rows.Count; ii++)
                    {
                        if (TaskDb.Rows[ii][(int)NodeName.工单状态].ToString() != "-1")
                        {
                            TaskDb.Rows[ii][(int)NodeName.工单状态] = "3";
                            break;
                        }
                    }
                }
                TaskDb.Rows.RemoveAt(0);
            }
            for (int ii = 0; ii < TaskDb.Rows.Count;ii++ )
            {
                TaskDb.Rows[ii][(int)NodeName.编号] = ii;
            }
            string[] array = new string[TaskDb.Columns.Count];
            array[(int)NodeName.编号] = TaskDb.Rows.Count.ToString();
            array[(int)NodeName.机台编号] = node.EQUIP_CODE;
            array[(int)NodeName.派工单号] = node.DISPATCH_CODE;
            array[(int)NodeName.物料编码] = node.MATERIAL_CODE;
            array[(int)NodeName.生产数量] = node.QTY.ToString();
            array[(int)NodeName.计划开始时间] = node.PLAN_START_DATE.ToString();
            array[(int)NodeName.计划完成时间] = node.PLAN_END_DATE.ToString();
            array[(int)NodeName.NC编号] = node.NC_ID;
            array[(int)NodeName.NC版本号] = node.NC_VER;
            array[(int)NodeName.作业指导号] = node.OP_DOC;
            array[(int)NodeName.作业指导版本号] = node.OP_DOC_VER;
            array[(int)NodeName.线体] = node.LINE;
            array[(int)NodeName.工序编码] = node.OP_CODE;
            array[(int)NodeName.生产订单] = node.ORDER_CODE;
            array[(int)NodeName.操作标示] = node.FLAG.ToString();
            array[(int)NodeName.SN] = node.SN.ToString();
            array[(int)NodeName.时间戳] = node.MARK_TIME.ToString();
            array[(int)NodeName.日期] = node.MARK_DATE;
            array[(int)NodeName.NC文件路径] = node.NC_PATH;
            array[(int)NodeName.NC作业指导书路径] = node.OP_DOC_PATH;
            array[(int)NodeName.报工数量] = "0";
            array[(int)NodeName.工单状态] = Stated;
            array[(int)NodeName.short_name] = node.Short_name;
            TaskDb.Rows.Add(array);
        }


        public void ClearNCTask()//任务清洗线
        {
            int maxfilenum = 50;
            String[] Pathstr = NCTaskXMLFileName.Split('\\');
            Pathstr[0] = NCTaskXMLFileName.Replace(Pathstr[Pathstr.Length - 1], "");
            String str,str1;
            int ii = 1;
            for(; ii <= maxfilenum;ii++)
            {
                str = Pathstr[0] + ii.ToString() + "_n" + Pathstr[Pathstr.Length - 1];
                if(System.IO.File.Exists(str))
                {
                    if (ii == maxfilenum)
                    {
                        str1 = Pathstr[0] + "1_n" + Pathstr[Pathstr.Length - 1];
                        System.IO.File.Delete(str1);
                        SCADA.RFIDDATAT.DBWriteToXml(TaskDb, str1);
                    }
                    else
                    {
                        str1 = Pathstr[0] + (ii + 1).ToString() + "_n" + Pathstr[Pathstr.Length - 1];
                        System.IO.File.Delete(str1);
                        SCADA.RFIDDATAT.DBWriteToXml(TaskDb, str1);
                    }
                    str1 = Pathstr[0] + ii.ToString() + "_" + ii.ToString() + Pathstr[Pathstr.Length - 1];
                    System.IO.File.Delete(str1);
                    System.IO.File.Move(str, str1);
                    break;
                }
            }
            if (ii == (maxfilenum + 1))//找不到
            {
                str = Pathstr[0] + "1_n" + Pathstr[Pathstr.Length - 1];
                SCADA.RFIDDATAT.DBWriteToXml(TaskDb, str);
            }
//             TaskDb = new System.Data.DataTable();
            MakeDefaulDB(ref TaskDb);
            AutoSaveData2Xml_Flag = true;
            SaveData2Xml_threaFucEvent.Set();
        }

        public void GetDISPATCHSendOrRepor(bool Flag, out System.Data.DataRow node)//为报工或者派工获取数据
        {
            node = null;
//             for (int ii = 0; ii < TaskDb.Rows.Count; ii++)
            {
                if (Flag)//派工  只能派送第一个
                {
                    if (TaskDb.Rows.Count > 0)
                    {
                        if(TaskDb.Rows[0][(int)NodeName.工单状态].ToString() == "0"
                        || TaskDb.Rows[0][(int)NodeName.工单状态].ToString() == "1")
                        {
                            node = TaskDb.Rows[0];
                            nowTaskNode = node;
                        }
                    }
                }
                else//报工
                {
                    int ii = 0;
                    for (; ii < TaskDb.Rows.Count; ii++)
                    {
                        if (TaskDb.Rows[ii][(int)NodeName.工单状态].ToString() == "3")
                        {
                            node = TaskDb.Rows[ii];
                            nowTaskNode = node;
                            break;
                        }
                    }
                    if (ii == TaskDb.Rows.Count)
                    {
                        ii = 1;
                        for (; ii < TaskDb.Rows.Count; ii++)
                        {
                            if (TaskDb.Rows[ii][(int)NodeName.工单状态].ToString() == "0")
                            {
                                TaskDb.Rows[ii][(int)NodeName.工单状态] = "3";
                                node = TaskDb.Rows[ii];
                                nowTaskNode = node;
                                break;
                            }
                        }
                    }
                }
            }
           
        }


        //获取已经派工的派工单
        public String Get_DISPATCHShortname()
        {
            String str = "";
            if (nowTaskNode != null && nowTaskNode.ItemArray.Length > (int)NodeName.short_name)
            {
                str = nowTaskNode[(int)NodeName.short_name].ToString();
            }
            str = "0105";
            return str;
        }
        public String GetTaskDbName()
        {
            return TaskDb.TableName;
        }

        public void SetTaskDbName(String name)
        {
            TaskDb.TableName = name;
            AutoSaveData2Xml_Flag = true;
            SaveData2Xml_threaFucEvent.Set();
        }

        public void SendCNCTaskOKOrNot(bool Flag)//首次手动派工是否成功
        {
            if (nowTaskNode != null)
            {
                if (Flag)//成功
                {
                    nowTaskNode[(int)NodeName.工单状态] = "3";//状态转为正在加工
                }
                else
                {
                    nowTaskNode[(int)NodeName.工单状态] = "1";//状态转为派送失败
                }
                AutoSaveData2Xml_Flag = true;
                SaveData2Xml_threaFucEvent.Set();
            }
        }

        public void SetCNCReportNum()//报工成功，报工数加1
        {
            if (nowTaskNode == null)
            {
            }
            else
            {
                int repersum = 0, repersum1 = 0;
                int.TryParse(nowTaskNode[(int)NodeName.报工数量].ToString(), out repersum);
                repersum++;
                nowTaskNode[(int)NodeName.报工数量] = repersum.ToString();
                if (int.TryParse(nowTaskNode[(int)NodeName.生产数量].ToString(), out repersum1))
                {
                    if (repersum >= repersum1 && nowTaskNode[(int)NodeName.操作标示].ToString() != "4")//非尾单自动切单
                    {
                        AutoSendTask();
                    }
                }
                else
                {
                    if (nowTaskNode[(int)NodeName.操作标示].ToString() != "4")
                    {
                        AutoSendTask();
                    }
                }
                AutoSaveData2Xml_Flag = true;
                SaveData2Xml_threaFucEvent.Set();
            }
        }

        private void AutoSendTask()//自动派单
        {
            if (nowTaskNode != null)
            {
                int next = int.Parse(nowTaskNode[(int)NodeName.编号].ToString()) + 1;
                TaskDb.Rows[next - 1][(int)NodeName.工单状态] = "4";//标示已经加工完成
                if (TaskDb.Rows.Count > next)
                {
                    TaskDb.Rows[next][(int)NodeName.工单状态] = "3";//标示下一个工单为正在加工的工单
                }
            }
        }

        private bool AutoSaveData2XmlFuc_Running = true;
        private bool AutoSaveData2Xml_Flag = false;
        private System.Threading.AutoResetEvent SaveData2Xml_threaFucEvent = new System.Threading.AutoResetEvent(true);
        private void AutoSaveData2XmlFuc()
        {
            while (AutoSaveData2XmlFuc_Running)
            {
                if (AutoSaveData2Xml_Flag)
                {
                    try
                    {
                        SCADA.RFIDDATAT.DBWriteToXml(TaskDb, NCTaskXMLFileName);
                        AutoSaveData2Xml_Flag = false;
                        if (TaskDataForm.ctrlTaskData != null &&
                            TaskDataForm.ctrlTaskData.NCTaskMessageChangeHand != null)
                        {
                            TaskDataForm.ctrlTaskData.NCTaskMessageChangeHand.BeginInvoke(this, null, null, null);
                        }
                        System.Threading.Thread.Sleep(1000);
                    }
                    catch 
                    {

                    }
                }
                else
                {
                    SaveData2Xml_threaFucEvent.WaitOne();
                }
            }
        }

        public void KillSaveData2Xml_threaFuc()
        {
            AutoSaveData2XmlFuc_Running = false;
            AutoSaveData2Xml_Flag = true;
            SaveData2Xml_threaFucEvent.Set();
        }
    }
}
