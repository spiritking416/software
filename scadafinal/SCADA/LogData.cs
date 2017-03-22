using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SCADA
{
    public class LogData
    {
        public static String[] LogDataNode0Name = { "Root", "System", "Equipment", "Network" };
        public static String[] LogDataNode1Name = { "System_security", "System_runing", "Equipment_CNC", "Equipment_ROBOT", "Equipment_PLC", "Equipment_RFID" };
        public static String[] LogDataNode2Attributes = { "Level", "TimeCreated", "Provider", "EventID", "Keywords", "EventData" };//来源，事件ID，等级，解释，创建时间,事件数据
        public static String[] LogDataNode2Level = { "消息", "警告", "错误", "严重", "审核"};
        public static String[] DataDataTableShowColumnStr = { "级别", "日期和时间", "来源", "事件ID", "事件描述", "事件数据" };
        int MAXLogDataNode = 10000;//大约5000条1M
        public enum Node0Name
        {
            Root = 0,
            System ,
            Equipment,
            Network
        }
        public enum Node1Name
        {
            System_security = 0,
            System_runing,
            Equipment_CNC,
            Equipment_ROBOT,
            Equipment_PLC,
            Equipment_RFID
        }
        public enum Node2Attributes
        {
            Level = 0,
            TimeCreated,
            Provider,
            EventID,
            Keywords,
            EventData
        }
        public enum Node2Level
        {
            MESSAGE = 0,
            WARNING,
            ERROR,
            FAULT,
            AUDIT
        }

        private XDocument LogXmlDoc;
        private String FileNmae;
        public System.Data.DataTable ShowEventDataDataTable = new System.Data.DataTable();
        private System.Threading.Thread SaveThread;
        public System.EventHandler<EventHandlerSendParm> AddLogMsgHandler;

        /// <summary>
        /// 创建XML全局变量并load文件进行初始化
        /// 文件不存在则创建一个新的默认文件并初始全局对象
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public char m_load(String FilePath,bool Flg)
        {
            char ret = (char)0;
            FileNmae = FilePath;

            String[] Pathstr = FilePath.Split('\\');
            Pathstr[0] = FilePath.Replace(Pathstr[Pathstr.Length - 1], "");
            Pathstr[0] = Pathstr[0].Substring(0, Pathstr[0].Length - 1);
            if (!System.IO.Directory.Exists(Pathstr[0]))
            {
                System.IO.Directory.CreateDirectory(Pathstr[0]);
            }
            if (System.IO.File.Exists(FilePath))
            {
                try
                {
                    LogXmlDoc = XDocument.Load(FilePath);
                }
                catch
                {
                    System.IO.File.Delete(FilePath + "errer");
                    System.IO.File.Move(FilePath, FilePath + "errer");
                    MakeXMLDefaultstructure();
                    LogXmlDoc.Save(FilePath);
                    System.Windows.Forms.MessageBox.Show(FilePath + "格式已经被破坏，文件被备份为：" + FilePath + "errer");
                }
                ret = (char)1;
            }
            else
            {
                MakeXMLDefaultstructure();
                LogXmlDoc.Save(FilePath);
            }
            if (Flg)
            {
                SaveThread = new System.Threading.Thread(SaveFuc);
                SaveThread.Start();
            }
            AddLogMsgHandler = new EventHandler<EventHandlerSendParm>(this.EventHandlerFuc);
            return ret;
        }


        private bool SaveLogXmlDoc_Flag = false;
        private object LogXmlDoc_Lock = new object();
        private System.Threading.AutoResetEvent SaveData2Xml_threaFucEvent = new System.Threading.AutoResetEvent(true);
        bool LogSaveRunning = true;
        private void SaveFuc()
        {
            while (LogSaveRunning)
            {
                if (SaveLogXmlDoc_Flag)
                {
                    if (LogXmlDoc.Element(LogDataNode0Name[(int)Node0Name.Root]).Descendants().Count() > MAXLogDataNode)//记录超过10000条时保存为历史记录
                    {
                        string s = DateTime.Now.ToString();
                        s = s.Replace(" ", "");
                        s = s.Replace(":", "");
                        s = s.Replace("/", "");
                        string[] arr = FileNmae.Split('\\');
                        s = s + arr[arr.Length - 1];
                        s = FileNmae.Replace(arr[arr.Length - 1], s);
                        LogXmlDoc.Save(s);
                        MakeXMLDefaultstructure();
                        LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
                        SendParm.Node1NameIndex = (int)LogData.Node1Name.System_security;
                        SendParm.LevelIndex = (int)LogData.Node2Level.MESSAGE;
                        SendParm.EventID = ((int)LogData.Node2Level.MESSAGE).ToString();
                        SendParm.Keywords = "保存历史日志";
                        SendParm.EventData = "历史日志文件：" + s;
                        SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);

                    }

                    LogXmlDoc.Save(FileNmae);
                    SaveLogXmlDoc_Flag = false;
                }
                else
                {
                    SaveData2Xml_threaFucEvent.WaitOne();
                }
//                System.Threading.Thread.Sleep(2000);
            }
            LogXmlDoc.Save(FileNmae);//退出最后保存
        }

        public void ExitApp()
        {
            LogData.EventHandlerSendParm SendParm = new LogData.EventHandlerSendParm();
            SendParm.Node1NameIndex = (int)LogData.Node1Name.System_security;
            SendParm.LevelIndex = (int)LogData.Node2Level.MESSAGE;
            SendParm.EventID = ((int)LogData.Node2Level.MESSAGE).ToString();
            SendParm.Keywords = "软件退出";
            SendParm.EventData = "用户:" + SetForm.LogInUserName;
            SCADA.MainForm.m_Log.AddLogMsgHandler.Invoke(this, SendParm);

            LogSaveRunning = false;
            SaveData2Xml_threaFucEvent.Set();
        }
        /// <summary>
        /// 生成默认设置XML对象
        /// </summary>
        /// <param name="FilePath"></param>
        private void MakeXMLDefaultstructure()
        {
            try
            {
                LogXmlDoc = new XDocument(
                    new XElement(LogDataNode0Name[(int)Node0Name.Root],
                        new XElement(LogDataNode0Name[(int)Node0Name.System]),
                        new XElement(LogDataNode0Name[(int)Node0Name.Equipment]),
                        new XElement(LogDataNode0Name[(int)Node0Name.Network])
                        ));
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("生成默认设置文档失败！\r\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 添加日志入口
        /// </summary>
        /// <param name="Node0Str"></param>
        /// <param name="Node1Str"></param>
        /// <param name="Attributes_Provider"></param>
        /// <param name="Attributes_EventID"></param>
        /// <param name="Attributes_Level"></param>
        /// <param name="Attributes_Keywords"></param>
        /// <param name="Attributes_TimeCreated"></param>
        /// <param name="Attributes_EventData"></param>
        private void AddEvent2Xml(EventHandlerSendParm Param)
        {
            lock(LogXmlDoc_Lock)
            {
                int Node0index;

                if (Param.Node1NameIndex == (int)LogData.Node1Name.System_runing
                    || Param.Node1NameIndex == (int)LogData.Node1Name.System_security)
                {
                    Node0index = (int)LogData.Node0Name.System;
                }
                else if (Param.Node1NameIndex == (int)LogData.Node1Name.Equipment_CNC
                    || Param.Node1NameIndex == (int)LogData.Node1Name.Equipment_PLC
                    || Param.Node1NameIndex == (int)LogData.Node1Name.Equipment_RFID
                    || Param.Node1NameIndex == (int)LogData.Node1Name.Equipment_ROBOT)
                {
                    Node0index = (int)LogData.Node0Name.Equipment;
                }
                else
                {
                    Node0index = (int)LogData.Node0Name.Network;
                }
                LogXmlDoc.Element(LogDataNode0Name[(int)Node0Name.Root]).Element(
                    LogData.LogDataNode0Name[Node0index]).AddFirst(
                    new XElement(LogData.LogDataNode1Name[Param.Node1NameIndex],
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.Level], LogData.LogDataNode2Level[Param.LevelIndex]),
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.TimeCreated], DateTime.Now.ToString()),
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.Provider], Param.Provider),
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.EventID], Param.EventID),
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.Keywords], Param.Keywords),
                    new XAttribute(LogDataNode2Attributes[(int)Node2Attributes.EventData], Param.EventData)
                    ));
            }
            SaveLogXmlDoc_Flag = true;
            SaveData2Xml_threaFucEvent.Set();
        }


        public struct EventHandlerSendParm
        {
            public int Node1NameIndex;
            public int LevelIndex;
            public String Provider;
            public String EventID;
            public String Keywords;
            public String EventData;
        }
        private void EventHandlerFuc(object ob, EventHandlerSendParm Param)
        {
            try
            {
                if (Param.Provider == null && ob != null)
                {
                    Param.Provider = ob.ToString().Split(',')[0];
                }
                AddEvent2Xml(Param);
            }
            catch
            {

            }
        }

        public void EventHandlershanshu(object ob, EventHandlerSendParm Param)
        {
            try
            {
                if (Param.Provider == null && ob != null)
                {
                    Param.Provider = ob.ToString().Split(',')[0];
                }
                AddEvent2Xml(Param);
            }
            catch
            {

            }
        }
        /// <summary>
        /// 从XML文件中读取一个DataTable
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="address">XML文件地址</param>
        /// <returns></returns>
        public System.Data.DataTable ReadFromXml(String ParThStr)
        {
            System.Data.DataTable dt = new System.Data.DataTable();
            try
            {
                for (int ii = 0; ii < DataDataTableShowColumnStr.Length;ii++ )
                {
                    dt.Columns.Add(DataDataTableShowColumnStr[ii]);
                }

                String[] StrArr = ParThStr.Split('\\');

                if (StrArr.Length == 1)//
                {
                    var n = from c in LogXmlDoc.Element(LogDataNode0Name[(int)Node0Name.Root]).Elements(StrArr[0])
                            select c;
                    foreach (var item in n)
                    {
                        foreach(var itemsub in item.Elements())
                        {
                            string[] array = new string[DataDataTableShowColumnStr.Length];
                            for (int ii = 0; ii < DataDataTableShowColumnStr.Length; ii++)
                            {
                                array[ii] = itemsub.Attribute(LogDataNode2Attributes[ii]).Value;
                            }
                            dt.Rows.Add(array);
                        }
                    }

                }
                else if (StrArr.Length == 2)
                {
                    var n = from c in LogXmlDoc.Element(LogDataNode0Name[(int)Node0Name.Root]).Elements(StrArr[0]).Elements(StrArr[1])
                            select c;
                    foreach (var item in n)
                    {
                        string[] array = new string[DataDataTableShowColumnStr.Length];
                        for (int ii = 0; ii < DataDataTableShowColumnStr.Length; ii++)
                        {
                            array[ii] = item.Attribute(LogDataNode2Attributes[ii]).Value;
                        }
                        dt.Rows.Add(array);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return new System.Data.DataTable();
            }

            return dt;
        }

    }
}
