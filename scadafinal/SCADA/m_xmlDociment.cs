using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.Windows.Forms;

namespace SCADA
{
    public class m_xmlDociment
    {
        private XmlDocument XML_doc ;

        public static String[] Default_Path_str = { "Root", "CNC", "ROBOT", "PLC", "RFID", "Item", "X", "Y", "serial", "LocalIpAddr", "linetype", "User" ,"PLCAlarmTb"};
        public static String[] Default_Attributes_str1 = { "SUM", "id", "workshop", "productionline", "type", "system", "ip", "port", "SN", "EQUIP_CODE", "remark" };
        public static String[] Default_Attributesstr1_value = { "1", "1", "#1", "#1", "A", "hnc8", "192.168.1.1", "10001", "0", "0", "" };
        public static String[] Default_Attributes_str2 = { "SUM", "address", "name", "appear", "EQUIP_CODE"/*设备ID*/, "ACTION_ID"/*动作ID*/, "remark" };
        public static String[] Default_Attributesstr2_value = { "1", "0", "急停", "否", "null", "null", "null" };
        public static String[] Default_Attributes_RFID = { "SUM", "id", "workshop", "productionline", "PLCserial", "jitaihao", "ReadDevice", "ReadAddressStar", "ReadAddressSet", "WriteDevice", "WriteAddressStar", "WriteAddressSet", "MonitorBit", "ip", "port", "remark" };
        public static String[] Default_RFID_STR = { "序号", "ID", "所属车间", "所属产线", "所属PLC的序号", "机台号", "读地址类型", "读起始地址", "读地址格式", "写地址类型", "写起始地址", "写地址格式", "监控位","IP", "端口", "备注" };
        public static String[] Default_Attributes_RFID_value = { "1", "1", "#1", "#1", "0", "0", "D", "0", "6,6,2,1,1,2,2,2,2,2,2,2,2,2", "D", "0", "6,6,2,1,1,2,2,2,2,2,2,2,2,2", "M,0,1", "192.168.1.1", "10001", "" };
        

        public static String[] PLC_System = { "三菱", "hnc8" };
        public static String[] Default_Attributes_CNCLocalIp = { "CNCLocalIp" };
        public static String[] Default_CNCLocalIVvalue = { "192.168.1.1" };
        public static String[] Default_Attributes_linetype = { "linetype" };
        public static String[] Default_linetype_value = { "一拖二", "RGV小车" };
        public static String[] Default_Attributes_User = { "name", "password" };
        public static String[] Default_Username_value = { "操作者", "管理员" };
        public static String[] Default_Attributes_PLCAlarmTb = { "AlarmNo", "Tex" };
        public static String[] Default_Attributes_PLCAlarmTb_value = { "0", "" };
        public static String[] Default_PLCAlarmTb_STR = { "序号", "报警号", "报警内容" };

        public static String[] Default_MITSUBISHI_Device1 = { "X", "D", "Buffer", "M", "L", "F", "V" };
        public static String[] Default_MITSUBISHI_Device2 = { "Y", "S", "B", "C", "W", "SM", "FD", "SD" };
        public static String[] Default_HNC8_Device1 = { "X", "R", "F"};
        public static String[] Default_HNC8_Device2 = { "Y", "G", "B"};
        public static String[] Default_MITSUBISHI_DeviceAddress1 = { "16-0-1FFF", "10-0-32287", "16-0-3FFF", "10-0-8191", "10-0-8191", "10-0-2047", "10-0-2047" };//地址格式：进制-起始地址-终止地址
        public static String[] Default_MITSUBISHI_DeviceAddress2 = { "16-0-1FFF", "10-0-12287", "16-0-1FFF", "10-0-1023", "10-0-1000", "10-0-700", "10-0-4", "10-0-2047" };
        public static String[] Default_HNC8_DeviceAddress1 = { "8-0-512", "8-0-2288", "16-0-3120" };
        public static String[] Default_HNC8_DeviceAddress2 = { "8-0-512", "16-0-3120", "32-0-1722" };


        public static String Path_linetype = Default_Path_str[(int)Path_str.Root] + "/" + Default_Path_str[(int)Path_str.linetype];
        public static String PathRoot_CNC = Default_Path_str[(int)Path_str.Root] + "/" + Default_Path_str[(int)Path_str.CNC];
        public static String PathRoot_CNCLocalIp = Default_Path_str[(int)Path_str.Root] + "/" + Default_Path_str[(int)Path_str.LocalIpAddr];
        public static String PathRoot_CNC_Item = PathRoot_CNC + "/" + Default_Path_str[(int)Path_str.Item];
        public static String PathRoot_ROBOT = Default_Path_str[(int)Path_str.Root] + "/" + Default_Path_str[(int)Path_str.ROBOT];
        public static String PathRoot_ROBOT_Item = PathRoot_ROBOT + "/" + Default_Path_str[(int)Path_str.Item];
        public static String PathRoot_PLC = Default_Path_str[(int)Path_str.Root] + "/" + Default_Path_str[(int)Path_str.PLC];
        public static String PathRoot_PLC_Item = PathRoot_PLC + "/" + Default_Path_str[(int)Path_str.Item];
        public static String PathRoot_RFID = Default_Path_str[(int)Path_str.Root] + "/" + Default_Path_str[(int)Path_str.RFID];
        public static String PathRoot_RFID_Item = PathRoot_RFID + "/" + Default_Path_str[(int)Path_str.Item];
        public static String PathRoot_User = Default_Path_str[(int)Path_str.Root] + "/" + Default_Path_str[(int)Path_str.User];
        public static String PathRoot_User_Item = PathRoot_User + "/" + Default_Path_str[(int)Path_str.Item];
        public static String PathRoot_PLCAlarmTb = Default_Path_str[(int)Path_str.Root] + "/" + Default_Path_str[(int)Path_str.PLCAlarmTb];
        public static String PathRoot_PLCAlarmTb_Item = PathRoot_PLCAlarmTb + "/" + Default_Path_str[(int)Path_str.Item];

        public enum Path_str
        {
            Root = 0,
            CNC,
            ROBOT,
            PLC,
            RFID,
            Item,
            X,
            Y,
            serial,
            LocalIpAddr,
            linetype,
            User,
            PLCAlarmTb
        }

        public enum Attributes_str1
        {
            serial = 0,
            id,
            workshop,
            productionline,
            type,
            system,
            ip,
            port,
            SN,
            EQUIP_CODE,
            remark
        }

        public enum Attributes_str2
        {
            serial = 0,
            address,
            name,
            appear,
            EQUIP_CODE,
            ACTION_ID,
            remark
        }

        public enum Attributes_RFID
        {
            serial = 0,
            id,
            workshop,
            productionline,
            PLCserial,
            jitaihao,
            ReadDevice,
            ReadAddressStar,
            ReadAddressSet,
            WriteDevice,
            WriteAddressStar,
            WriteAddressSet,
            MonitorBit,
            ip,
            port,
            remark
        }

        /// <summary>
        /// 创建XML全局变量并load文件进行初始化
        /// 文件不存在则创建一个新的默认文件并初始全局对象
        /// </summary>
        /// <param name="FilePath">文件路径</param>
        /// <returns></returns>
        public char m_load(String FilePath)
        {
            XML_doc = new XmlDocument();//配置文件XML文件对象实例
            String[] Pathstr = FilePath.Split('\\');
            String FileDirectory = FilePath.Replace(Pathstr[Pathstr.Length - 1], "");
            FileDirectory = FileDirectory.Substring(0, FileDirectory.Length - 1);
            if (!System.IO.Directory.Exists(FileDirectory))
            {
                System.IO.Directory.CreateDirectory(FileDirectory);
            }
            if (System.IO.File.Exists(FilePath))
            {
                try 
                {
                    XML_doc.Load(FilePath);
//                     string path = Default_Path_str[(int)Path_str.Root] + "/" + Default_Path_str[1];
                    for (int ii = 0; ii < Default_Attributesstr1_value.Length; ii++)
                    {
                        Default_Attributesstr1_value[ii] = ((XmlElement)XML_doc.SelectSingleNode(PathRoot_CNC)).Attributes[ii].Value;
                    }
                    return (char)1;
                }
                catch 
                {
                    MakeDefaultSetXML(FilePath);
                    SaveXml2File(FilePath);
                    System.IO.File.Delete(FilePath + "errer");
                    System.IO.File.Move(FilePath, FilePath + "errer");
                    System.Windows.Forms.MessageBox.Show(FilePath + "格式已经被破坏，文件被备份为：" + FilePath + "errer");
                    return (char)0;
                }
            }
            else
            {
                MakeDefaultSetXML(FilePath);
                SaveXml2File(FilePath);
                return (char)0;
            }
        }

        /// <summary>
        /// 生成默认设置XML对象
        /// </summary>
        /// <param name="FilePath"></param>
        private void MakeDefaultSetXML(String FilePath)
        {
            try
            {
                XmlDeclaration dec = XML_doc.CreateXmlDeclaration("1.0", "utf-8", null);
                XML_doc.AppendChild(dec);
                XmlNode root = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Root]);//一级

                ///产线类型
                XmlNode node = XML_doc.CreateElement(Default_Path_str[(int)Path_str.linetype]);//二级
                XmlAttribute xmlat;
                int jj = 0;
                for (jj = 0; jj < Default_Attributes_linetype.Length; jj++)
                {
                    xmlat = XML_doc.CreateAttribute(Default_Attributes_linetype[jj]);
                    xmlat.Value = Default_linetype_value[0];
                    node.Attributes.Append(xmlat);
                }
                root.AppendChild(node);

                ///CNC本地ip默认
                node = XML_doc.CreateElement(Default_Path_str[(int)Path_str.LocalIpAddr]);//二级
                jj = 0;
                for (jj = 0; jj < Default_Attributes_CNCLocalIp.Length; jj++)
                {
                    xmlat = XML_doc.CreateAttribute(Default_Attributes_CNCLocalIp[jj]);
                    xmlat.Value = Default_CNCLocalIVvalue[jj];
                    node.Attributes.Append(xmlat);
                }
                root.AppendChild(node);


                ///用户管理
                node = XML_doc.CreateElement(Default_Path_str[(int)Path_str.User]);//二级
                xmlat = XML_doc.CreateAttribute(Default_Attributes_str1[0]);//SUM
                xmlat.Value = "2";
                node.Attributes.Append(xmlat);
                xmlat = XML_doc.CreateAttribute(Default_Attributes_User[0]);//name
                xmlat.Value = "User";
                node.Attributes.Append(xmlat);
                xmlat = XML_doc.CreateAttribute(Default_Attributes_User[1]);//password
                xmlat.Value = "";
                node.Attributes.Append(xmlat);
                XmlNode node_user = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Item] + "0");//三级
                xmlat = XML_doc.CreateAttribute(Default_Path_str[(int)Path_str.serial]);//serial
                xmlat.Value = "0";
                node_user.Attributes.Append(xmlat);
                xmlat = XML_doc.CreateAttribute(Default_Attributes_User[0]);//name
                xmlat.Value = Default_Username_value[0];//操作者
                node_user.Attributes.Append(xmlat);
                xmlat = XML_doc.CreateAttribute(Default_Attributes_User[1]);//password
                xmlat.Value = "";
                node_user.Attributes.Append(xmlat);
                node.AppendChild(node_user);
                node_user = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Item] + "1");//三级
                xmlat = XML_doc.CreateAttribute(Default_Path_str[(int)Path_str.serial]);//serial
                xmlat.Value = "1";
                node_user.Attributes.Append(xmlat);
                xmlat = XML_doc.CreateAttribute(Default_Attributes_User[0]);//name
                xmlat.Value = Default_Username_value[1];
                node_user.Attributes.Append(xmlat);
                xmlat = XML_doc.CreateAttribute(Default_Attributes_User[1]);//password
                xmlat.Value = "";
                node_user.Attributes.Append(xmlat);
                node.AppendChild(node_user);
                root.AppendChild(node);



                ///CNC默认
                node = XML_doc.CreateElement(Default_Path_str[(int)Path_str.CNC]);//二级
                jj = 0;
                for (jj = 0; jj < Default_Attributes_str1.Length; jj++)
                {
                    xmlat = XML_doc.CreateAttribute(Default_Attributes_str1[jj]);
                    xmlat.Value = Default_Attributesstr1_value[jj];
                    node.Attributes.Append(xmlat);
                }
                XmlNode Element = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Item] + "0");//三级
                node.AppendChild(Element);
                root.AppendChild(node);

                ///Robot默认
                node = XML_doc.CreateElement(Default_Path_str[(int)Path_str.ROBOT]);//二级
                for (jj = 0; jj < Default_Attributes_str1.Length; jj++)
                {
                    xmlat = XML_doc.CreateAttribute(Default_Attributes_str1[jj]);
                    xmlat.Value = Default_Attributesstr1_value[jj];
                    node.Attributes.Append(xmlat);
                }
                XmlNode node1 = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Item] + "0");//三级
                XmlNode node2 = XML_doc.CreateElement(Default_Path_str[(int)Path_str.X]);//四级
                for (jj = 0; jj < Default_Attributes_str2.Length; jj++)
                {
                    xmlat = XML_doc.CreateAttribute(Default_Attributes_str2[jj]);
                    xmlat.Value = Default_Attributesstr2_value[jj];
                    node2.Attributes.Append(xmlat);
                }
                Element = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Item] + "0");
                node2.AppendChild(Element);
                XmlNode node3 = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Y]);//四级
                for (jj = 0; jj < Default_Attributes_str2.Length; jj++)
                {
                    xmlat = XML_doc.CreateAttribute(Default_Attributes_str2[jj]);
                    xmlat.Value = Default_Attributesstr2_value[jj];
                    node3.Attributes.Append(xmlat);
                }
                Element = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Item] + "0");
                node3.AppendChild(Element);
                node1.AppendChild(node2);
                node1.AppendChild(node3);
                node.AppendChild(node1);
                root.AppendChild(node);


                ///PLC默认
                node = XML_doc.CreateElement(Default_Path_str[(int)Path_str.PLC]);//二级
                for (jj = 0; jj < Default_Attributes_str1.Length; jj++)
                {
                    xmlat = XML_doc.CreateAttribute(Default_Attributes_str1[jj]);
                    xmlat.Value = Default_Attributesstr1_value[jj];
                    node.Attributes.Append(xmlat);
                }
                node1 = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Item] + "0");//三级
                node.AppendChild(node1);
                root.AppendChild(node);

                ///RFID默认
                node = XML_doc.CreateElement(Default_Path_str[(int)Path_str.RFID]);//二级
                for (jj = 0; jj < Default_Attributes_RFID.Length; jj++)
                {
                    xmlat = XML_doc.CreateAttribute(Default_Attributes_RFID[jj]);
                    xmlat.Value = Default_Attributes_RFID_value[jj];
                    node.Attributes.Append(xmlat);
                }
                Element = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Item] + "0");//三级
                node.AppendChild(Element);
                root.AppendChild(node);

                //PLC报警定义列表
                node = XML_doc.CreateElement(Default_Path_str[(int)Path_str.PLCAlarmTb]);//二级
                xmlat = XML_doc.CreateAttribute(Default_Attributes_str1[0]);//SUM
                xmlat.Value = Default_Attributesstr1_value[0];
                node.Attributes.Append(xmlat);
                for (jj = 0; jj < Default_Attributes_PLCAlarmTb.Length; jj++)
                {
                    xmlat = XML_doc.CreateAttribute(Default_Attributes_PLCAlarmTb[jj]);
                    xmlat.Value = Default_Attributes_PLCAlarmTb_value[jj];
                    node.Attributes.Append(xmlat);
                }
                Element = XML_doc.CreateElement(Default_Path_str[(int)Path_str.Item] + "0");//三级
                node.AppendChild(Element);
                root.AppendChild(node);


                XML_doc.AppendChild(root);

                InserDefaulAttributes(PathRoot_CNC, 0);
                InserDefaulAttributes(PathRoot_ROBOT, 0);
                InserDefaulAttributes(PathRoot_ROBOT_Item + "0" + "/" + Default_Path_str[(int)Path_str.X], 0);
                InserDefaulAttributes(PathRoot_ROBOT_Item + "0" + "/" + Default_Path_str[(int)Path_str.Y], 0);
                InserDefaulAttributes(PathRoot_PLC, 0);
                InserDefaulAttributes(PathRoot_RFID, 0);
                InserDefaulAttributes(PathRoot_PLCAlarmTb, 0);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show("生成默认设置文档失败！\r\n" +
                    ex.ToString());
            }
        }

        public void MakeXmlPLCDevicePath(String path)
        {
            String[] tmp = path.Split('/');
            String path1 = path.Substring(0, path.Length - tmp[tmp.Length - 1].Length - 1);
            XmlNode node = XML_doc.SelectSingleNode(path1);
            XmlNode node1 = XML_doc.CreateElement(tmp[tmp.Length - 1]);
            XmlAttribute xmlat;
            xmlat = XML_doc.CreateAttribute(Default_Attributes_str2[0]);
            xmlat.Value = "0";
            node1.Attributes.Append(xmlat);
            xmlat = XML_doc.CreateAttribute(Default_Attributes_str2[1]);
            String PLCSystem = m_Read(path1, -1, Default_Attributes_str1[5]);
            xmlat.Value = FindDeviceAddress(ref tmp[tmp.Length - 1], ref PLCSystem);
            node1.Attributes.Append(xmlat);
            for (int jj = 2; jj < Default_Attributes_str2.Length; jj++)
            {
                xmlat = XML_doc.CreateAttribute(Default_Attributes_str2[jj]);
                xmlat.Value = Default_Attributesstr2_value[jj];
                node1.Attributes.Append(xmlat);
            }
            node.AppendChild(node1);
        }

        public String FindDeviceAddress(ref String Device,ref String SystemStr)
        {
            if (SystemStr == PLC_System[0])
            {
                for (int ii = 0; ii < Default_MITSUBISHI_Device1.Length; ii++)
                {
                    if (Device == Default_MITSUBISHI_Device1[ii])
                    {
                        return Default_MITSUBISHI_DeviceAddress1[ii];
                    }
                }
                for (int ii = 0; ii < Default_MITSUBISHI_Device2.Length; ii++)
                {
                    if (Device == Default_MITSUBISHI_Device2[ii])
                    {
                        return Default_MITSUBISHI_DeviceAddress2[ii];
                    }
                }
            }
            else if (SystemStr == PLC_System[1])
            {
                for (int ii = 0; ii < Default_HNC8_Device1.Length; ii++)
                {
                    if (Device == Default_HNC8_Device1[ii])
                    {
                        return Default_HNC8_DeviceAddress1[ii];
                    }
                }
                for (int ii = 0; ii < Default_HNC8_Device2.Length; ii++)
                {
                    if (Device == Default_HNC8_Device2[ii])
                    {
                        return Default_HNC8_DeviceAddress2[ii];
                    }
                }
            }
            return "0";
        }
        /// <summary>
        /// element节点列表最后面插入一个节点
        /// </summary>
        /// <param name="path">插入路径</param>
        /// <param name="elementname">插入的节点名称</param>
        public void InserNode(String path)
        {
            try
            {
                string itemname = Default_Path_str[5] + XML_doc.SelectSingleNode(path).ChildNodes.Count.ToString();
                XmlNode m_node = XML_doc.CreateElement(itemname);
                string path3 = "";
                if (path == PathRoot_ROBOT/* || path == PathRoot_PLC*/)
                {
//                     if (path == PathRoot_ROBOT)
                    {
                        path3 = PathRoot_ROBOT;
                    }
//                     else 
//                     {
//                         path3 = PathRoot_PLC;
//                     }
                    
                    XmlNode node2 = XML_doc.CreateElement(Default_Path_str[6]);//四级
                    for (int jj = 0; jj < Default_Attributes_str2.Length; jj++)
                    {
                        XmlAttribute xmlat = XML_doc.CreateAttribute(Default_Attributes_str2[jj]);
                        xmlat.Value = Default_Attributesstr2_value[jj];
                        node2.Attributes.Append(xmlat);
                    }
                    XmlElement Element = XML_doc.CreateElement(Default_Path_str[5] + "0");
                    node2.AppendChild(Element);
                    XmlNode node3 = XML_doc.CreateElement(Default_Path_str[7]);//四级
                    for (int jj = 0; jj < Default_Attributes_str2.Length; jj++)
                    {
                        XmlAttribute xmlat = XML_doc.CreateAttribute(Default_Attributes_str2[jj]);
                        xmlat.Value = Default_Attributesstr2_value[jj];
                        node3.Attributes.Append(xmlat);
                    }
                    Element = XML_doc.CreateElement(Default_Path_str[5] + "0");
                    node3.AppendChild(Element);
                    m_node.AppendChild(node2);
                    m_node.AppendChild(node3);
                    XML_doc.SelectSingleNode(path).AppendChild(m_node);

                }
                XML_doc.SelectSingleNode(path).AppendChild(m_node);
                InserDefaulAttributes(path, XML_doc.SelectSingleNode(path).ChildNodes.Count - 1);
                if (path3 != "")
                {
                    InserDefaulAttributes(path3 + "/" + itemname + "/" + Default_Path_str[6], 0);
                    InserDefaulAttributes(path3 + "/" + itemname + "/" + Default_Path_str[7], 0);
                }

                m_UpdateAttribute(path, -1, Default_Attributes_str1[0], XML_doc.SelectSingleNode(path).ChildNodes.Count.ToString());
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(path + "插入失败！\r\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 插入默认属性
        /// </summary>
        /// <param name="path">节点路径</param>
        /// <param name="serial">节点序号</param>
        public void InserDefaulAttributes(String path, int serial)
        {
            XmlAttribute xmlat;
            try
            {
                XmlNodeList m_xmllist = XML_doc.SelectSingleNode(path).ChildNodes;
                if (m_xmllist.Count > serial)
                {
                    XmlElement xe = (XmlElement)m_xmllist.Item(serial);
                    xmlat = XML_doc.CreateAttribute(Default_Path_str[(int)Path_str.serial]);
                    xmlat.Value = serial.ToString();
                    xe.Attributes.Append(xmlat);

                    foreach (XmlAttribute xa in XML_doc.SelectSingleNode(path).Attributes)
                    {
                        if (xa.Name != Default_Attributes_str1[0] && xa.Name != Default_Path_str[8])
                        {
                            xmlat = XML_doc.CreateAttribute(xa.Name);
                            xmlat.Value = xa.Value;
                            xe.Attributes.Append(xmlat);
                        }
                    }
                }

            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(path + "添加默认属性失败！\r\n" +
                    ex.ToString());
            }

        }

        /// <summary>
        /// element节点列表最后面删除一个节点
        /// </summary>
        /// <param name="path"></param>
        /// <param name="Element"></param>
        public void DeleNode(String path, int index)
        {
            try
            {
                if (XML_doc.SelectSingleNode(path).ChildNodes.Count > index)
                {
                    XML_doc.SelectSingleNode(path).RemoveChild(XML_doc.SelectSingleNode(path).ChildNodes.Item(index));
                    m_UpdateAttribute(path, -1, Default_Attributes_str1[0], XML_doc.SelectSingleNode(path).ChildNodes.Count.ToString());
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(path + "插入失败！\r\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 保存xml对象到文件
        /// </summary>
        /// <param name="FilePath"></param>
        public void SaveXml2File(String FilePath)
        {
            try
            {
                XML_doc.Save(FilePath);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(FilePath + "保存失败！\r\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 读取XML_doc中的节点的属性
        /// </summary>
        /// <param name="path">XML_doc中节点路径</param>
        /// <param name="serial">节点序号</param>
        /// <param name="attribute">节点的属性</param>
        /// <returns>读到的值</returns>
        public String m_Read(String path, int serial, String attribute)
        {
            string str = "";
            try
            {
                XmlNodeList m_xmllist = XML_doc.SelectSingleNode(path).ChildNodes;
                if (serial == -1)
                {
                    XmlElement m_element = (XmlElement)XML_doc.SelectSingleNode(path);
                    str = m_element.GetAttribute(attribute);
                }
                else if (m_xmllist.Count > serial)
                {
                    XmlElement xe = (XmlElement)m_xmllist.Item(serial);
                    str = xe.GetAttribute(attribute);//读取对应节点的对应属性
                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(path + "序号为：" + serial.ToString() + "属性为：" + attribute  
                    + "读取失败！\r\n" +
                    ex.ToString());
            }
            return str;
        }


        /// <summary>
        /// 在serial中跟新（属性存在）或者插入一个属性（属性不存在），按照默认属性插入
        /// </summary>
        /// <param name="serial">节点序号</param>
        /// <param name="attribute">节点的属性</param>
        /// <param name="value">属性值</param>
        /// <returns></returns>
        public void m_UpdateAttribute(String path, int serial, String attribute, String value)//serial == -1是节点本身,serial == -2是子节点最后一个节点
        {
            try
            {
                XmlNodeList m_xmllist = XML_doc.SelectSingleNode(path).ChildNodes;
                if (serial == -1)//路径本身节点
                {
                    XmlElement m_element = (XmlElement)XML_doc.SelectSingleNode(path);
                    m_element.SetAttribute(attribute, value);
                }
                else if (m_xmllist.Count > serial)
                {
                    XmlElement xe;

                    if (serial == -2)//子节点最后一个节点
                    {
                        xe = (XmlElement)m_xmllist.Item(m_xmllist.Count - 1);
                    }
                    else
                    {
                        xe = (XmlElement)m_xmllist.Item(serial);
                    }
                    if (xe.GetAttribute(attribute).Equals(null))//属性不存在
                    {
                    }
                    else
                    {
                        xe.SetAttribute(attribute, value);
                    }

                }
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show( "属性修改失败！\r\n" +
                    ex.ToString());
            }
        }

        /// <summary>
        /// 车间号
        /// </summary>
        public void ChangeDefault_AllAttributes(String Default_Attributes_str,String value)
        {
            for (int ii = -1; ii < XML_doc.SelectSingleNode(PathRoot_CNC).ChildNodes.Count; ii++)
            {
                m_UpdateAttribute(PathRoot_CNC, ii, Default_Attributes_str, value);
            }
            for (int ii = -1; ii < XML_doc.SelectSingleNode(PathRoot_ROBOT).ChildNodes.Count; ii++)//robot
            {
                m_UpdateAttribute(PathRoot_ROBOT, ii, Default_Attributes_str, value);
            }
            for (int ii = -1; ii < XML_doc.SelectSingleNode(PathRoot_PLC).ChildNodes.Count; ii++)//plc
            {
                m_UpdateAttribute(PathRoot_PLC, ii, Default_Attributes_str, value);
            }
            for (int ii = -1; ii < XML_doc.SelectSingleNode(PathRoot_RFID).ChildNodes.Count; ii++)//rfid
            {
                m_UpdateAttribute(PathRoot_RFID, ii, Default_Attributes_str, value);
            }
            if (Default_Attributes_str1[2] == Default_Attributes_str)
            {
                Default_Attributesstr1_value[2] = value;
            }
            else if (Default_Attributes_str1[3] == Default_Attributes_str)
            {
                Default_Attributesstr1_value[3] = value;
            }
        }

        /// <summary>
        /// PLC系统修改
        /// </summary>
        public void ChangeDefault_PlcSystemAttributes(Int32 Index,String value)
        {
            if (Index >= 0 && Index < XML_doc.SelectSingleNode(PathRoot_PLC).ChildNodes.Count)
            {
                String Path = PathRoot_PLC_Item + Index.ToString();
                m_UpdateAttribute(PathRoot_PLC, Index, Default_Attributes_str1[5], value);
                int ChildNodes_Count = XML_doc.SelectSingleNode(Path).ChildNodes.Count;
                for (int jj = 0; jj < ChildNodes_Count; jj++)
                {
                    XML_doc.SelectSingleNode(Path).RemoveChild(XML_doc.SelectSingleNode(Path).ChildNodes.Item(0));
                }
//                 m_UpdateAttribute(PathRoot_PLC, Index, Default_Path_str[8], "0");
            }
//             for (int ii = -1; ii < XML_doc.SelectSingleNode(PathRoot_PLC).ChildNodes.Count; ii++)//plc
//             {
//                 m_UpdateAttribute(PathRoot_PLC, ii, Default_Attributes_str1[5], value);
//                 if (ii == 0)
//                 {
//                     int ChildNodes_Count = XML_doc.SelectSingleNode(PathRoot_PLC_Item + "0").ChildNodes.Count;
//                     for (int jj = 0; jj < ChildNodes_Count; jj++)
//                     {
//                         XML_doc.SelectSingleNode(PathRoot_PLC_Item + "0").RemoveChild(XML_doc.SelectSingleNode(PathRoot_PLC_Item + "0").ChildNodes.Item(0));
//                     }
//                     m_UpdateAttribute(PathRoot_PLC, -1, Default_Attributes_str1[0], XML_doc.SelectSingleNode(PathRoot_PLC).ChildNodes.Count.ToString());
//                 }
//                 else if (ii > 0)
//                 {
//                     DeleNode(PathRoot_PLC, ii);
//                 }
//             }
//             Default_Attributesstr1_value[5] = value;


        }

        /// <summary>
        /// 将默认属性填入列表
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Default_Attributes_Begin"></param>
        /// <param name="Default_Attributes_End"></param>
        /// <param name="dgvr"></param>
        public void GetDefault_Attributes2GridViewRow(String Path, ref DataGridViewRow dgvr)
        {
            XmlElement m_element = (XmlElement)XML_doc.SelectSingleNode(Path);
            for (int ii = 0; ii < m_element.Attributes.Count; ii++)
            {
                dgvr.Cells[ii].Value = m_element.Attributes[ii].Value;
            }
        }

        /// <summary>
        /// 将属性填入列表
        /// </summary>
        /// <param name="Path"></param>
        /// <param name="Default_Attributes_Begin"></param>
        /// <param name="Default_Attributes_End"></param>
        /// <param name="dgvr"></param>
        public void Attributes2GridViewRow(String Path, ref DataGridViewRow dgvr)
        {
            XmlElement m_element = (XmlElement)XML_doc.SelectSingleNode(Path);
            int AttributesCount = GetDefault_AttributesCount(Path);
            if (dgvr.Cells.Count < AttributesCount)
            {
                MessageBox.Show("dgvr.Cells.Count < AttributesCount");
                return;
            }
            for (int ii = 0; ii < AttributesCount; ii++)
            {
                dgvr.Cells[ii].Value = m_element.Attributes[ii].Value;
            }
        }

        public int GetDefault_AttributesCount(String Path)
        {
            return ((XmlElement)XML_doc.SelectSingleNode(Path)).Attributes.Count;
        }

        public void GridViewRow2XmlAttributes(String Path ,ref DataGridViewRow dgvr)
        {
            XmlElement m_element = (XmlElement)XML_doc.SelectSingleNode(Path);
            Int32 AttributesCount = GetDefault_AttributesCount(Path);
            for (int ii = 0; ii < AttributesCount; ii++)
            {
                if (dgvr.Cells[ii].Value != null)
                {
                    m_element.Attributes[ii].Value = dgvr.Cells[ii].Value.ToString();
                }
            }
        }

        public bool CheckNodeExist(String Pathstr)
        {
            try
            {
                if (XML_doc.SelectSingleNode(Pathstr) != null)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch
            {
                return false;
            }
        }

        public void SetUserPassword(String UserName, String Password)
        {
            XmlNodeList m_xmllist = XML_doc.SelectSingleNode(PathRoot_User).ChildNodes;
            for (int ii = 0; ii < m_xmllist.Count; ii++)
            {
                if (m_Read(PathRoot_User, ii, Default_Attributes_User[0]) == UserName)
                {
                    XmlElement xe = (XmlElement)m_xmllist.Item(ii);
                    xe.SetAttribute(Default_Attributes_User[1], MainForm.MakeSingSn(Password));
                    break;
                }
            }
        }

        public bool CheckUserPassword(String UserName,String Password)
        {
            bool retf = false;
            XmlNodeList m_xmllist = XML_doc.SelectSingleNode(PathRoot_User).ChildNodes;
            for (int ii = 0; ii < m_xmllist.Count;ii++ )
            {
                if(m_Read(PathRoot_User, ii, Default_Attributes_User[0]) == UserName)
                {
                    if (m_Read(PathRoot_User, ii, Default_Attributes_User[1]) == MainForm.MakeSingSn(Password))
                    {
                        retf = true;
                        break;
                    }
                }
            }
            return retf;
        }

        public String[] GetUserNameStrArr()
        {
            XmlNodeList m_xmllist = null;
            String[] retf = null;
            if(CheckNodeExist(PathRoot_User))
            {
                m_xmllist = XML_doc.SelectSingleNode(PathRoot_User).ChildNodes;
                retf = new String[m_xmllist.Count];
                for (int ii = 0; ii < m_xmllist.Count;ii++ )
                {
                    retf[ii] = m_Read(PathRoot_User, ii, Default_Attributes_User[0]);
                }
            }
            return retf;
        }

    }
}
