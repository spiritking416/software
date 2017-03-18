using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using LineDevice;
using HNCAPI;
using Collector;

namespace SCADA
{
    public partial class UserControlParaman : UserControl
    {
        const string STR_NO_CONNECTION = "未连接";
        string[] STR_PARM = { "参数号", "名称", "参数值", "生效方式", "默认值", "最小值", "最大值" };
        int[] columnHeadWidth = { 80, 200, 100, 120, 100, 100, 100 };
        string[] STR_ACT = { "保存生效", "立即生效", "复位生效", "重启生效" };
        private short ClientNo = -1;
        int rowNum = 0;

        int m_fileNo = 0;
        int m_recNo = 0;

        public UserControlParaman()
        {
            InitializeComponent();

            dataGridViewParam.AllowUserToAddRows = false;
            dataGridViewParam.ReadOnly = true;
            dataGridViewParam.ColumnCount = 7;
            dataGridViewParam.RowHeadersVisible = false;
            for (int i = 0; i < STR_PARM.Length; i++)
            {
                dataGridViewParam.Columns[i].Name = STR_PARM[i];
                dataGridViewParam.Columns[i].Width = columnHeadWidth[i];
            }
            timerUpdataShow.Enabled = true;
        }

        public void InitParamanTreeView(short clientNo)
        {
            this.ClientNo = clientNo;
            if (!CollectShare.Instance().HNC_NetIsConnect(clientNo))
            {
                return;
            }
            treeViewParaman.Nodes.Clear();

            //short clientNo = 0;
            if (CollectShare.Instance().HNC_NetIsConnect(clientNo))
            {
                treeViewParaman.Nodes.Add(new TreeNode(STR_NO_CONNECTION));
            }
            else
            {
                int ret = AddParamanItemFrmNet(clientNo);
                if (ret < 0)
                {
                    MessageBox.Show("网络读取参数结构错误！");
                }
            }
           UpdatalistViewParm();
        }

        private int AddParamanItemFrmNet(short clientNo)
        {
            int fileNum = HNCDATADEF.PARAMAN_MAX_FILE_LIB;
            string fileName = "";
            int ret = 0;
            for (int i = 0; i < fileNum; i++)
            {
                ret = CollectShare.Instance().HNC_ParamanGetFileName(i, ref fileName, clientNo);
                if (ret < 0)
                {
                    break;
                }

                TreeNode rootNode = new TreeNode(fileName);
                treeViewParaman.Nodes.Add(rootNode);

                int recNum = -1;
                ret = CollectShare.Instance().HNC_ParamanGetSubClassProp(i, (Byte)ParaSubClassProp.SUBCLASS_NUM, ref recNum, clientNo);

                if (recNum < 0)
                {
                    ret = recNum;
                    break;
                }
                else if (recNum > 1)
                {
                    string recName = "";
                    ret = CollectShare.Instance().HNC_ParamanGetSubClassProp(i, (Byte)ParaSubClassProp.SUBCLASS_NAME, ref recName, clientNo);
                    if (ret < 0)
                    {
                        break;
                    }

                    for (int j = 0; j < recNum; j++)
                    {
                        rootNode.Nodes.Add(new TreeNode(recName + j.ToString()));
                    }
                }
            }
            return ret;
        }


        private void InitdataGridViewParam(int fileNo, int recNo, short clientNo)
        {
            int ret = -1;           
            ret = CollectShare.Instance().HNC_ParamanGetSubClassProp(fileNo, (Byte)ParaSubClassProp.SUBCLASS_ROWNUM, ref rowNum, clientNo);
            if (rowNum < 0)
            {
                return;
            }
          
            ret = CollectShare.Instance().HNC_ParamanRewriteSubClass(fileNo, recNo, clientNo);
            if (ret < 0)
            {
                return;
            }
            if (rowNum < dataGridViewParam.RowCount)//删除多余的行
            {
                for (int index = dataGridViewParam.RowCount - 1; index >= rowNum; index--)
                {
                    dataGridViewParam.Rows.RemoveAt(index);
                }
            }
            for (int index = 0; index < dataGridViewParam.RowCount; index++)
            {
                dataGridViewParam.Rows[index].Cells[0].Value = null;
            }
            timerUpdataShow.Enabled = true;
        }

        private int GetParItemText(int fileNo, int recNo, int row, short clientNo, string[] strPar)
        {
            Int16 dupNum = 0;
            Int16 dupNo = 0;
            Int32 index = -1;
            Int32 parmID = -1;
            int ret = -1;
            ret = CollectShare.Instance().HNC_ParamanTransRow2Index(fileNo, recNo, row, ref index, ref dupNum, ref dupNo, clientNo);
            if (index < 0)
            {
                return -1;
            }

           //获取生效方式
            Int32 actType = -1;
            ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_ACT, ref actType, clientNo);
            if (actType < 0)
            {
                return -1;
            }

            switch (actType)
            {
                //case (sbyte)CDataDef.PAR_ACT_TYPE.PARA_ACT_SAVE:
                case (sbyte)ParaActType.PARA_ACT_SAVE:
                    strPar[3] = STR_ACT[0];
                    break;
                case (sbyte)ParaActType.PARA_ACT_NOW:
                    strPar[3] = STR_ACT[1];
                    break;
                case (sbyte)ParaActType.PARA_ACT_RST:
                    strPar[3] = STR_ACT[2];
                    break;
                case (sbyte)ParaActType.PARA_ACT_PWR:
                    strPar[3] = STR_ACT[3];
                    break;
            }

            //获取参数号
            ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_ID, ref parmID, clientNo);
            if (ret < 0)
            {
                return -1;
            }
            strPar[0] = parmID.ToString("D6");

            //获取参数名称
            ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_NAME, ref strPar[1], clientNo);
            if (ret < 0)
            {
                return -1;
            }

            //获取参数储存类型
            Int32 storeType = -1;
            ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_STORE, ref storeType, clientNo);
            if (storeType < 0)
            {
                return -1;
            }

            //获取参数值、默认值、最小值和最大值
            int iVal = 0;
            double dVal = 0;
            //const int DFT = 1;
            //const int MIN = 2;
            //const int MAX = 3;
            switch (storeType)
            {
                case (sbyte)HNCDATATYPE.DTYPE_BOOL:
                case (sbyte)HNCDATATYPE.DTYPE_UINT:
                case (sbyte)HNCDATATYPE.DTYPE_INT:
                    ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                    if (ret < 0)
                    {
                        break;
                    }
                    strPar[2] = iVal.ToString();
                    ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_DFVALUE, ref iVal, clientNo);
                    if (ret < 0)
                    {
                        break;
                    }
                    strPar[4] = iVal.ToString();
                    ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MINVALUE, ref iVal, clientNo);
                    if (ret < 0)
                    {
                        break;
                    }
                    strPar[5] = iVal.ToString();
                    ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MAXVALUE, ref iVal, clientNo);
                    if (ret < 0)
                    {
                        break;
                    }
                    strPar[6] = iVal.ToString();
                    break;
                case (sbyte)HNCDATATYPE.DTYPE_FLOAT:
                    ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref dVal, clientNo);
                    if (ret < 0)
                    {
                        break;
                    }
                    strPar[2] = dVal.ToString("F6");
                    ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_DFVALUE, ref dVal, clientNo);
                    if (ret < 0)
                    {
                        break;
                    }
                    strPar[4] = dVal.ToString("F6");
                    ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MINVALUE, ref dVal, clientNo);
                    if (ret < 0)
                    {
                        break;
                    }
                    strPar[5] = dVal.ToString("F6");
                    ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MAXVALUE, ref dVal, clientNo);
                    if (ret < 0)
                    {
                        break;
                    }
                    strPar[6] = dVal.ToString("F6");
                    break;
                case (sbyte)HNCDATATYPE.DTYPE_STRING:
                    ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref strPar[2], clientNo);
                    if (ret < 0)
                    {
                        break;
                    }
                    strPar[4] = "N/A";
                    strPar[5] = "N/A";
                    strPar[6] = "N/A";
                    break;
                case (sbyte)HNCDATATYPE.DTYPE_HEX4:
                    ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                    strPar[2] = "0x" + iVal.ToString("X2");
                    strPar[4] = "N/A";
                    strPar[5] = "N/A";
                    strPar[6] = "N/A";
                    break;
                case (sbyte)HNCDATATYPE.DTYPE_BYTE:
                    sbyte[] araayBt = new sbyte[HNCDATATYPE.PARAM_STR_LEN];
                    ret = CollectShare.Instance().HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref actType, clientNo);
                    strPar[2] = GetStringFrmByte(araayBt);
                    strPar[4] = "N/A";
                    strPar[5] = "N/A";
                    strPar[6] = "N/A";
                    break;
                default:
                    strPar[2] = "0";
                    break;
            }
            if (ret < 0)
            {
                return -1;
            }

            return 0;
        }

        private string GetStringFrmByte(sbyte[] array)
        {
            string strByte = "";

            int len = 0;
            for (len = 0; len < array.Length; len++)
            {
                if (array[len] < 0)
                {
                    break;
                }
            }
            if (len == 0)
            {
                return strByte;
            }
            strByte += array[0];
            for (int i = 1; i < len; i++)
            {
                strByte += ",";
                strByte += array[i];
            }

            return strByte;
        }

        private void treeViewParaman_AfterSelect_1(object sender, TreeViewEventArgs e)
        {
            if (e.Node.Nodes.Count > 0)
            {
                return;
            }
            if (e.Node.Parent == null)
            {
                m_fileNo = e.Node.Index;
                m_recNo = 0;
            }
            else
            {
                m_fileNo = e.Node.Parent.Index;
                m_recNo = e.Node.Index;
            }
            rowNum = 0;
            InitdataGridViewParam(m_fileNo, m_recNo, ClientNo);
        }

        private void UpdatalistViewParm()
        {
            m_fileNo = 0;
            m_recNo = 0;
            rowNum = 0;
            InitdataGridViewParam(m_fileNo, m_recNo, ClientNo);
        }

        private void timerUpdataShow_Tick(object sender, EventArgs e)
        {
            if (!dataGridViewParam.Visible)
            {
                timerUpdataShow.Enabled = false;
                return;
            }
            int index;
            if (dataGridViewParam.Rows.Count < rowNum)
            {
                if (dataGridViewParam.Rows.Count == 0)
                {
                    dataGridViewParam.Rows.Add();
                }
                if (dataGridViewParam.Rows[dataGridViewParam.Rows.Count - 1].Displayed)//动态增加行
                {
                    int rowNum_chang = 15;
                    int rowNum_nowsum = dataGridViewParam.Rows.Count;
                    rowNum_nowsum += rowNum_chang;
                    if (rowNum < rowNum_nowsum)
                    {
                        rowNum_nowsum = rowNum;
                    }
                    for (index = dataGridViewParam.RowCount; index < rowNum_nowsum; index++)
                    {
                        dataGridViewParam.Rows.Add();
                    }
                }
            }
            for (index = 0; index < dataGridViewParam.Rows.Count; index++)
            {
                    if (dataGridViewParam.Rows[index].Displayed && dataGridViewParam.Rows[index].Cells[0].Value == null)
                    {
                        string[] strPar = new string[7];
                        if (GetParItemText(m_fileNo, m_recNo, index, ClientNo, strPar) == -1)
                        {
                            break;
                        }
                        for (int jj = 0; jj < 7; jj++)
                        {
                            dataGridViewParam.Rows[index].Cells[jj].Value = strPar[jj];
                        }
                }
            }
        }

        public void ClearAlldata()
        {
            timerUpdataShow.Enabled = false;
            treeViewParaman.Nodes.Clear();
            dataGridViewParam.Rows.Clear();
        }
    }
}
