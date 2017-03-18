using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace SCADA
{
    public class RFIDSTRMassgeStr
    {
        public int MassgeType;
        public int cncindex;
        public String[] ChanPingXuLie;
        public String RfidEQUIP_CODE;
        public String GongXu;
        public RFIDSTRMassgeStr(int MassgeType, int cncindex, String[] ChanPingXuLie, String GongXu, String RfidEQUIP_CODE)
        {
            this.MassgeType = MassgeType;
            this.cncindex = cncindex;
            this.ChanPingXuLie = new String[ChanPingXuLie.Length];
            for (int ii = 0; ii < ChanPingXuLie.Length; ii++)
            {
                this.ChanPingXuLie[ii] = ChanPingXuLie[ii];
            }
            this.GongXu = GongXu;
            this.RfidEQUIP_CODE = RfidEQUIP_CODE;
        }
    }

    public class RFIDDATAT
    {
        public int RFIDDataDataTableDataMax = 500;
        int ActionDataCountMax = 100;
        private object RFIDReadDataDataTable_Look = new object();
        public System.Data.DataTable RFIDDataDataTable = new System.Data.DataTable("1");
        public String[] RFIDReadDataStructInit = {"序号", "产品系列号", "待加工工序", "已加工工序", "工序01加工信息", "工序02加工信息", "工序03加工信息",
                                                 "工序04加工信息","工序05加工信息","工序06加工信息","工序07加工信息","工序08加工信息","工序09加工信息"};
        public String[] ActionStr = { "读取", "写入", "加工", "工序" };
        public String SpileCh = ";";
        public event EventHandler<String> MassgeHandler;

        public EventHandler<RFIDSTRMassgeStr> GetMassgeHandler;

        public enum RFIDReadDataStructInit_Index
        {
            SN = 0,
            PSN,
            PENDING_PROCEDURE,
            FINISHED_PROCEDURE,
            PROCEDURE01_INFOMATION,
            PROCEDURE02_INFOMATION,
            PROCEDURE03_INFOMATION,
            PROCEDURE04_INFOMATION,
            PROCEDURE05_INFOMATION,
            PROCEDURE06_INFOMATION,
            PROCEDURE07_INFOMATION,
            PROCEDURE08_INFOMATION,
            PROCEDURE09_INFOMATION
        }

        private String RFIDDataDataTable_FileName;
        public RFIDDATAT()
        {
            String[] Pathstr = SCADA.MainForm.RFIDDataFilePath.Split('\\');
            RFIDDataDataTable_FileName = SCADA.MainForm.RFIDDataFilePath.Replace(Pathstr[Pathstr.Length - 1], "");
            RFIDDataDataTable_FileName = RFIDDataDataTable_FileName.Substring(0, RFIDDataDataTable_FileName.Length - 1);
            if (!System.IO.Directory.Exists(RFIDDataDataTable_FileName))
            {
                System.IO.Directory.CreateDirectory(RFIDDataDataTable_FileName);
            }
            RFIDDataDataTable_FileName += "\\" + Pathstr[Pathstr.Length - 1];

            RFIDDataDataTable_XMLFile_load(RFIDDataDataTable_FileName);

            GetMassgeHandler = new EventHandler<RFIDSTRMassgeStr>(this.RFIDSTRMassgeStrFuc);
        }

        public void AppExit()
        {
            DBWriteToXml(RFIDDataDataTable, RFIDDataDataTable_FileName);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="FilePath"></param>
        /// <returns></returns>
        public void RFIDDataDataTable_XMLFile_load(String FilePath)
        {
            lock (RFIDReadDataDataTable_Look)
            {
                String m_FilePath = FilePath;
                if (System.IO.File.Exists(m_FilePath))
                {
                    System.IO.FileInfo fileInfo = new System.IO.FileInfo(m_FilePath);
                    if (fileInfo.Length > 0)
                    {
                        RFIDDataDataTable = DBReadFromXml(m_FilePath);
                    }
                }
                if (RFIDDataDataTable.Columns.Count == 0 || (RFIDDataDataTable.Rows.Count == 0 && RFIDDataDataTable.Columns.Count > RFIDReadDataStructInit.Length))
                {
                    RFIDDataDataTable.Columns.Clear();
                    for (int ii = 0; ii < RFIDReadDataStructInit.Length; ii++)
                    {
                        RFIDDataDataTable.Columns.Add(RFIDReadDataStructInit[ii], typeof(string));
                    }
                }
            }
        }


        private void RFIDSTRMassgeStrFuc(object ob, RFIDSTRMassgeStr MassgeStr)
        {
            lock (RFIDReadDataDataTable_Look)
            {
                RFIDSTRMassgeStr m_MassgeStr = new RFIDSTRMassgeStr(MassgeStr.MassgeType, MassgeStr.cncindex,MassgeStr.ChanPingXuLie,MassgeStr.GongXu,MassgeStr.RfidEQUIP_CODE);
                switch (m_MassgeStr.MassgeType)
                {
                    case 0://ReadRfid
                        ReadRfid(ref m_MassgeStr.ChanPingXuLie, ref m_MassgeStr.RfidEQUIP_CODE);
                        break;
                    case 1://JiTaiJiaGong
                        JiTaiJiaGong(m_MassgeStr.cncindex, m_MassgeStr.ChanPingXuLie);
                        break;
                    case 2://WriteRfid
                        WriteRfid(ref m_MassgeStr.ChanPingXuLie, ref m_MassgeStr.RfidEQUIP_CODE, m_MassgeStr.GongXu);
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 工件加工机台号的添加
        /// </summary>
        /// <param name="cncIndex"></param>
        /// <param name="GongXu"></param>
        /// <param name="ChanPingXuLie"></param>
        private void JiTaiJiaGong(int cncIndex, String [] ChanPingXuLie)
        {
//             lock (RFIDReadDataDataTable_Look)
            {
//                 int m_cncindex = cncIndex;
//                 String[] m_ChanPingXuLie = new String[ChanPingXuLie.Length];
//                 for (int ii = 0; ii < ChanPingXuLie.Length; ii++)
//                 {
//                     m_ChanPingXuLie[ii] = ChanPingXuLie[ii];
//                 }
                for (int ii = 0; ii < RFIDDataDataTable.Rows.Count; ii++)
                {
                    bool FindFlg = false;
                    if (ChanPingXuLie[0] != null && RFIDDataDataTable.Rows[ii][(int)RFIDReadDataStructInit_Index.PSN].ToString() == ChanPingXuLie[0])
                    {
                        MainForm.cnclist[cncIndex].reportChanPingXuLieHao[0] = ChanPingXuLie[0];//产品序列号01
                        FindFlg = true;
                    }
                    else if (ChanPingXuLie[1] != null && RFIDDataDataTable.Rows[ii][(int)RFIDReadDataStructInit_Index.PSN].ToString() == ChanPingXuLie[1])
                    {
                        MainForm.cnclist[cncIndex].reportChanPingXuLieHao[1] = ChanPingXuLie[1];//产品序列号02
                        FindFlg = true;
                    }
                    if (FindFlg)
                    {
                        System.Data.DataRow r = RFIDDataDataTable.Rows[ii];
                        InserActionData2DataTable(ActionStr[2], ref MainForm.cnclist[cncIndex].JiTaiHao, ref r);
                    }
                }
            }
        }

        private void ReadRfid(ref String[] RfidData, ref String RfidEQUIP_CODE)
        {
//             lock (RFIDReadDataDataTable_Look)
            {
//                 String[] m_RfidData = new String[RfidData.Length];
//                 for (int ii = 0; ii < RfidData.Length; ii++)
//                 {
//                     m_RfidData[ii] = RfidData[ii];
//                 }
//                 String m_RfidEQUIP_CODE = RfidEQUIP_CODE;
                String[] Dataarr;
                for (int jj = 0; jj < 2; jj++)
                {
                    String chanpingxulihao = RfidData[jj];
                    if (chanpingxulihao.Length > 0)//产品序列号
                    {
                        bool FindFlg = false;
                        for (int ii = 0; ii < RFIDDataDataTable.Rows.Count; ii++)
                        {
                            if (RFIDDataDataTable.Rows[ii][(int)RFIDReadDataStructInit_Index.PSN].ToString() == chanpingxulihao)
                            {
                                System.Data.DataRow r = RFIDDataDataTable.Rows[ii];
                                InserActionData2DataTable(ActionStr[0], ref RfidEQUIP_CODE, ref r);
                                Dataarr = new string[RFIDReadDataStructInit.Length - 1];
                                Dataarr[0] = chanpingxulihao;
                                for (int kk = 3; kk < RfidData.Length; kk++)
                                {
                                    Dataarr[kk - 2] = RfidData[kk];
                                }

                                int aass = 1;
                                for (; aass < Dataarr.Length + 1; aass++)//将读到的数据插入表格
                                {
                                    r[aass] = Dataarr[aass - 1];
                                }
                                FindFlg = true;
                                break;
                            }
                        }
                        if (!FindFlg)
                        {
                            Dataarr = new string[RFIDReadDataStructInit.Length - 1];
                            Dataarr[0] = chanpingxulihao;
                            for (int ii = 3; ii < RfidData.Length; ii++)
                            {
                                Dataarr[ii - 2] = RfidData[ii];
                            }
                            InserNewRfidData2DataTable(ref Dataarr, ref RfidEQUIP_CODE);
                        }
                    }
                }
            }
        }

        private void WriteRfid(ref String[] RfidData, ref String RfidEQUIP_CODE, String GongXu)
        {
//             lock (RFIDReadDataDataTable_Look)
            {
//                 String[] m_RfidData = new String[RfidData.Length];
//                 for (int ii = 0; ii < RfidData.Length; ii++)
//                 {
//                     m_RfidData[ii] = RfidData[ii];
//                 }
//                 String m_RfidEQUIP_CODE = RfidEQUIP_CODE;
//                 String m_GongXu = GongXu.ToString();
                for (int jj = 0; jj < 2; jj++)
                {
                    String chanpingxulihao = RfidData[jj];
                    if (chanpingxulihao.Length > 0)//产品序列号
                    {
                        for (int ii = 0; ii < RFIDDataDataTable.Rows.Count; ii++)
                        {
                            if (RFIDDataDataTable.Rows[ii][(int)RFIDReadDataStructInit_Index.PSN].ToString() == chanpingxulihao)
                            {
                                System.Data.DataRow r = RFIDDataDataTable.Rows[ii];
                                InserActionData2DataTable(ActionStr[1] + SpileCh + ActionStr[3] + GongXu + SpileCh, ref RfidEQUIP_CODE, ref r);
                            }
                        }
                    }
                }
            }
        }
        /// <summary>
        /// 添加RFID数据到列表
        /// </summary>
        /// <param name="Dataarr"></param>
        /// <param name="RfidEQUIP_CODE"></param>
        private void InserNewRfidData2DataTable(ref String[] Dataarr,ref String RfidEQUIP_CODE)
        {
            if (RFIDDataDataTable.Rows.Count == RFIDDataDataTableDataMax)
            {
                RFIDDataDataTable.Rows.RemoveAt(RFIDDataDataTableDataMax - 1);
            }
            System.Data.DataRow r;
            r = RFIDDataDataTable.NewRow();
            r[0] = "0";
            if (RFIDDataDataTable.Rows.Count == 0)
            {
                RFIDDataDataTable.Rows.Add(r);
            }
            else
            {
                RFIDDataDataTable.Rows.InsertAt(r, 0);
            }
            int ii = 1;
            for (; ii < Dataarr.Length + 1; ii++)//将读到的数据插入表格
            {
                RFIDDataDataTable.Rows[0][ii] = Dataarr[ii - 1];
            }
            for (ii = 0; ii < RFIDDataDataTable.Rows.Count; ii++)
            {
                RFIDDataDataTable.Rows[ii][(int)RFIDReadDataStructInit_Index.SN] = ii.ToString();
            }
            InserActionData2DataTable(ActionStr[0], ref RfidEQUIP_CODE, ref r);
        }

        /// <summary>
        /// 插入动作数据
        /// </summary>
        /// <param name="RCNCEQUIP_CODE"></param>
        /// <param name="r"></param>
        public void InserActionData2DataTable(String ActionStr, ref String RCNCEQUIP_CODE, ref System.Data.DataRow r)
        {
            int ActionDataCount = RFIDDataDataTable.Columns.Count - RFIDReadDataStructInit.Length;
            bool youkong = false;
            for (int ii = RFIDReadDataStructInit.Length; ii < RFIDDataDataTable.Columns.Count; ii++)
            {
                if (r[ii].ToString().Length == 0)
                {
                    r[ii] = ActionStr + SpileCh + RCNCEQUIP_CODE + SpileCh + DateTime.Now.ToString();
                    youkong = true;
                    break;
                }
            }
            if (!youkong)
            {
                if (ActionDataCount + 1 < ActionDataCountMax)
                {
                    String addstr = "第" + (ActionDataCount + 1).ToString() + "次操作";
                    RFIDDataDataTable.Columns.Add(addstr, typeof(string));
                }
                r[RFIDDataDataTable.Columns.Count - 1] = ActionStr + SpileCh + RCNCEQUIP_CODE + SpileCh + DateTime.Now.ToString();
            }
            String str = "0";
            if (MassgeHandler != null)
            {
                MassgeHandler.BeginInvoke(null, str, null, null);
            }
        }

        /// <summary>
        /// 将DataTable的内容写入到XML文件中
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="address">XML文件地址</param>
        public static bool DBWriteToXml(System.Data.DataTable dt, string address)
        {
            try
            {
                //如果文件DataTable.xml存在则直接删除
                if (System.IO.File.Exists(address))
                {
                    System.IO.File.Delete(address);
                }

                System.Xml.XmlTextWriter writer =
                    new System.Xml.XmlTextWriter(address, Encoding.GetEncoding("GBK"));
                writer.Formatting = System.Xml.Formatting.Indented;

                //XML文档创建开始
                writer.WriteStartDocument();

                writer.WriteComment("DataTable: " + dt.TableName);

                writer.WriteStartElement("DataTable"); //DataTable开始

                writer.WriteAttributeString("TableName", dt.TableName);
                writer.WriteAttributeString("CountOfRows", dt.Rows.Count.ToString());
                writer.WriteAttributeString("CountOfColumns", dt.Columns.Count.ToString());

                writer.WriteStartElement("ClomunName", ""); //ColumnName开始

                for (int i = 0; i < dt.Columns.Count; i++)
                {
                    writer.WriteAttributeString(
                        "Column" + i.ToString(), dt.Columns[i].ColumnName);
                }

                writer.WriteEndElement(); //ColumnName结束

                //按行各行
                for (int j = 0; j < dt.Rows.Count; j++)
                {
                    writer.WriteStartElement("Row" + j.ToString(), "");

                    //打印各列
                    for (int k = 0; k < dt.Columns.Count; k++)
                    {
                        writer.WriteAttributeString(
                            "Column" + k.ToString(), dt.Rows[j][k].ToString());
                    }

                    writer.WriteEndElement();
                }

                writer.WriteEndElement(); //DataTable结束

                writer.WriteEndDocument();
                writer.Close();

                //XML文档创建结束
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        /// <summary>
        /// 从XML文件中读取一个DataTable
        /// </summary>
        /// <param name="dt">数据源</param>
        /// <param name="address">XML文件地址</param>
        /// <returns></returns>
        public static System.Data.DataTable DBReadFromXml(string address)
        {
            System.Data.DataTable dt = new System.Data.DataTable();

            try
            {
                if (!System.IO.File.Exists(address))
                {
                    throw new Exception("文件不存在!");
                }

                System.Xml.XmlDocument xmlDoc = new System.Xml.XmlDocument();
                xmlDoc.Load(address);

                System.Xml.XmlNode root = xmlDoc.SelectSingleNode("DataTable");

                //读取表名
                dt.TableName = ((System.Xml.XmlElement)root).GetAttribute("TableName");
                //Console.WriteLine("读取表名： {0}", dt.TableName);

                //读取行数
                int CountOfRows = 0;
                if (!int.TryParse(((System.Xml.XmlElement)root).
                    GetAttribute("CountOfRows").ToString(), out CountOfRows))
                {
                    throw new Exception("行数转换失败");
                }

                //读取列数
                int CountOfColumns = 0;
                if (!int.TryParse(((System.Xml.XmlElement)root).
                    GetAttribute("CountOfColumns").ToString(), out CountOfColumns))
                {
                    throw new Exception("列数转换失败");
                }

                //从第一行中读取记录的列名
                foreach (System.Xml.XmlAttribute xa in root.ChildNodes[0].Attributes)
                {
                    dt.Columns.Add(xa.Value);
                    //Console.WriteLine("建立列： {0}", xa.Value);
                }

                //从后面的行中读取行信息
                for (int i = 1; i < root.ChildNodes.Count; i++)
                {
                    string[] array = new string[root.ChildNodes[0].Attributes.Count];
                    for (int j = 0; j < array.Length; j++)
                    {
                        array[j] = root.ChildNodes[i].Attributes[j].Value.ToString();
                    }
                    dt.Rows.Add(array);
                    //Console.WriteLine("行插入成功");
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
