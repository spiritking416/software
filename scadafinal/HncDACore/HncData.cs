using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.Remoting;
using System.Runtime.Remoting.Channels;
using System.Runtime.Remoting.Channels.Tcp;
using HncDataInterfaces;
using System.Runtime.Serialization;
using System.IO;
using System.Threading;
using hncData;
using System.Xml.Serialization;
using System.Xml;
using HNCAPI;

namespace HncDACore
{
    public class HncDataService : MarshalByRefObject, IHncDataService
    {
        private pgData db = new pgData();
        public HncDataService()
        {
        }

        public IHncData Getresume()
        {
            HncData resume = new HncData();
            resume.FirstName = "David";
            resume.LastName = "Talbot";
            resume.Title = "Senior Software Architect";
            resume.Body = "Did some cool stuff";
            return (IHncData)resume;
        }

        public String GetformattedResume()
        {
            return Getresume().GetformattedResume();
        }

        public List<AxisInfo> GetAxisInfo(string MacID)
        {
            List<AxisInfo> Axislist = new List<AxisInfo>();

            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            if (Mac == null)
            {
                return Axislist;
            }

            /*
            string[] saAxisName = Mac.m_sAxisName.Split(',');
            string[] saAxisNoTemp = Mac.m_sAxisNo.Split(',');

            int[] ax = new int[saAxisNoTemp.Length];
            for (int i = 0; i < saAxisNoTemp.Length; i++)
            {
                ax[i] = Convert.ToInt32(saAxisNoTemp[i]);
            }
            */

            string[] saAxisName = new string[4] {"x","y","z","s" };
            int[] ax = new int[4] {0,1,2,5 };
            for (int i = 0; i < ax.Length; i++)
            {
                int actPos = 0;
                int cmdPos = 0;
                int actPosWcs = 0;
                int cmdPosWcs = 0;
                int actPosRcs = 0;
                int cmdPosRcs = 0;

                int leftFeed = 0;
                double loadCur = 0;

                //  double ratedCurrent = 0;

                var axisinfo = new AxisInfo();

                int ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS, ax[i], ref actPos, Mac.ClientNO);//取机床实际位置
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_POS, ax[i], ref cmdPos, Mac.ClientNO);//取机床指令位置

                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS_WCS, ax[i], ref actPosWcs, Mac.ClientNO);//取工件实际位置
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_POS_WCS, ax[i], ref cmdPosWcs, Mac.ClientNO);//取工件指令位置

                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS_RCS, ax[i], ref actPosRcs, Mac.ClientNO);//取相对实际位置
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_POS_RCS, ax[i], ref cmdPosRcs, Mac.ClientNO);//取相对指令位置
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_LOAD_CUR, ax[i], ref loadCur, Mac.ClientNO);           //负载电流
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_LEFT_TOGO, ax[i], ref leftFeed, Mac.ClientNO);// 剩余进给

                axisinfo.axName = saAxisName[i];              //轴名
                axisinfo.actPosition = actPos / 100000.0;  //机床实际位置                     

                axisinfo.actPositionWcs = actPosWcs / 100000.0;  //工件实际位置
                axisinfo.leftFeed = leftFeed / 100000.0;
                axisinfo.loadCur = loadCur;

                // axisinfo.spdlLoad = loadCur / ratedCurrent;
                Axislist.Add(axisinfo);
                //AxisMovelist.Add(axismovement);
            }
            return Axislist;
        }

        //public List<ToolParaIndexinfo> GetToolParaIndexinfo(string MacID)//获取参数的值
        //{
        //    List<ToolParaIndexinfo> ParaIndexlist = new List<ToolParaIndexinfo>();
        //    //List<AxisInfo> Axislist = new List<AxisInfo>();

        //    var Mac = (from a in Monitor.ConnectionList
        //               where a.MacID == MacID
        //               select a).FirstOrDefault();
        //    try
        //    {

        //        int ret = 0;
        //        //short clientNo = NetDll.HncNetConnect("115.156.243.173", 10001);
        //        //short clientNo = NetDll.HncNetConnect(Mac.IP,Mac.Port);
        //        ret = GlobalSession.HNC_NetIsConnect(Mac.ClientNO);
        //        if (ret == 0)
        //        {

        //            DateTime pDate = DateTime.Now;
        //            //myLog.Info("Seccost:" + (DateTime.Now - pDate).TotalMilliseconds.ToString());
        //            int chanNum = 0;
        //            ret = GlobalSession.HNC_SystemGetValue((int)HncSystem.HNC_SYS_CHAN_NUM, ref chanNum, Mac.ClientNO);//获取系统数据的值,获取通道号
        //            int temp = 0;
        //            ret = GlobalSession.HNC_ToolGetToolPara(1, (int)ToolParaIndex.INFTOOL_CH, ref temp, Mac.ClientNO);//刀具参数
        //            int toolnum = GlobalSession.HNC_ToolGetMaxToolNum(Mac.ClientNO);
        //            double tetool_para0 = 0;
        //            double tetool_para1 = 0;
        //            double tetool_para2 = 0;
        //            double tetool_para3 = 0;
        //            double tetool_para4 = 0;
        //            double tetool_para5 = 0;
        //            double tetool_para6 = 0;
        //            double tetool_para7 = 0;
        //            double tetool_para8 = 0;
        //            double tetool_para9 = 0;
        //            double tetool_para = 0;
        //            double inftool_type = 0;
        //            double inftool_magz = 0;
        //            int toolNO = 0;
        //            double gtool_dir = 0;
        //            double gtool_len1 = 0;
        //            double gtool_len2 = 0;
        //            double gtool_len3 = 0;
        //            double gtool_rad1 = 0;
        //            double gtool_ang1 = 0;
        //            double wtool_len1 = 0;
        //            double wtool_rad1 = 0;
        //            double wtool_ang1 = 0;
        //            double extool_s_limit = 0;
        //            double extool_f_limit = 0;
        //            double extool_large_left = 0;
        //            double extool_large_right = 0;
        //            double motool_sequ = 0;
        //            double motool_multi = 0;
        //            double motool_max_life = 0;
        //            double motool_alm_life = 0;
        //            double motool_act_life = 0;
        //            double motool_max_count = 0;
        //            double motool_alm_count = 0;
        //            double motool_act_count = 0;
        //            double motool_max_wear = 0;
        //            double motool_alm_wear = 0;
        //            double motool_act_wear = 0;
        //            ret = GlobalSession.HNC_ToolMagGetToolNo((int)MagzHeadIndex.MAGZTAB_CUR_TOOL, 0, ref toolNO, Mac.ClientNO);//取当前刀具号
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.TETOOL_PARA0, ref tetool_para0, Mac.ClientNO);//工艺参数1
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.TETOOL_PARA1, ref tetool_para1, Mac.ClientNO);//工艺参数2
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.TETOOL_PARA2, ref tetool_para2, Mac.ClientNO);//工艺参数3
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.TETOOL_PARA3, ref tetool_para3, Mac.ClientNO);//工艺参数4
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.TETOOL_PARA4, ref tetool_para4, Mac.ClientNO);//工艺参数5
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.TETOOL_PARA5, ref tetool_para5, Mac.ClientNO);//工艺参数6
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.TETOOL_PARA6, ref tetool_para6, Mac.ClientNO);//工艺参数7
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.TETOOL_PARA7, ref tetool_para7, Mac.ClientNO);//工艺参数8
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.TETOOL_PARA8, ref tetool_para8, Mac.ClientNO);//工艺参数9
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.TETOOL_PARA9, ref tetool_para9, Mac.ClientNO);//工艺参数10
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.TETOOL_TOTAL, ref tetool_para, Mac.ClientNO);//
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.INFTOOL_TYPE, ref inftool_type, Mac.ClientNO);//刀具类型
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.INFTOOL_MAGZ, ref inftool_magz, Mac.ClientNO);//刀具所属刀库号
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.GTOOL_DIR, ref gtool_dir, Mac.ClientNO);//方向
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.GTOOL_LEN1, ref gtool_len1, Mac.ClientNO);//铣刀具长度，车X偏置
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.GTOOL_LEN2, ref gtool_len2, Mac.ClientNO);//车Y偏置
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.GTOOL_LEN3, ref gtool_len3, Mac.ClientNO);//车Z偏置
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.GTOOL_RAD1, ref gtool_rad1, Mac.ClientNO);//刀尖半径
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.GTOOL_ANG1, ref gtool_ang1, Mac.ClientNO);//刀具几何参数：角度
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.WTOOL_LEN1, ref wtool_len1, Mac.ClientNO);//铣长度磨损，车Z磨损
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.WTOOL_RAD1, ref wtool_rad1, Mac.ClientNO);//铣半径磨损，车X磨损
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.WTOOL_ANG1, ref wtool_ang1, Mac.ClientNO);//刀具磨损参数，角度
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.EXTOOL_S_LIMIT, ref extool_s_limit, Mac.ClientNO);//S转速限制
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.EXTOOL_F_LIMIT, ref extool_f_limit, Mac.ClientNO);//F转速限制
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.EXTOOL_LARGE_LEFT, ref extool_large_left, Mac.ClientNO);//大刀具干涉左刀位
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.EXTOOL_LARGE_RIGHT, ref extool_large_right, Mac.ClientNO);//大刀具干涉右刀位
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.MOTOOL_SEQU, ref motool_sequ, Mac.ClientNO);//优先级
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.MOTOOL_MULTI, ref motool_multi, Mac.ClientNO);//倍率
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.MOTOOL_MAX_LIFE, ref motool_max_life, Mac.ClientNO);//最大寿命
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.MOTOOL_ALM_LIFE, ref motool_alm_life, Mac.ClientNO);//预警寿命
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.MOTOOL_ACT_LIFE, ref motool_act_life, Mac.ClientNO);//实际寿命
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.MOTOOL_MAX_COUNT, ref motool_max_count, Mac.ClientNO);//最大计件数
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.MOTOOL_ALM_COUNT, ref motool_alm_count, Mac.ClientNO);//预警计件数
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.MOTOOL_ACT_COUNT, ref motool_act_count, Mac.ClientNO);//实际计件数
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.MOTOOL_MAX_WEAR, ref motool_max_wear, Mac.ClientNO);//最大磨损
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.MOTOOL_ALM_WEAR, ref motool_alm_wear, Mac.ClientNO);//预警磨损
        //            ret = GlobalSession.HNC_ToolGetToolPara(toolNO, (int)ToolParaIndex.MOTOOL_ACT_WEAR, ref motool_act_wear, Mac.ClientNO);//实际磨损
        //            var toolparaindexinfo = new ToolParaIndexinfo();
        //            toolparaindexinfo.tetool_para = tetool_para;
        //            toolparaindexinfo.tetool_para0 = tetool_para0;
        //            toolparaindexinfo.tetool_para1 = tetool_para1;
        //            toolparaindexinfo.tetool_para2 = tetool_para2;
        //            toolparaindexinfo.tetool_para3 = tetool_para3;
        //            toolparaindexinfo.tetool_para4 = tetool_para4;
        //            toolparaindexinfo.tetool_para5 = tetool_para5;
        //            toolparaindexinfo.tetool_para6 = tetool_para6;
        //            toolparaindexinfo.tetool_para7 = tetool_para7;
        //            toolparaindexinfo.tetool_para8 = tetool_para8;
        //            toolparaindexinfo.tetool_para9 = tetool_para9;
        //            toolparaindexinfo.inftool_type = inftool_type;
        //            toolparaindexinfo.inftool_magz = inftool_magz;
        //            toolparaindexinfo.gtool_dir = gtool_dir;
        //            toolparaindexinfo.gtool_len1 = gtool_len1;
        //            toolparaindexinfo.gtool_len2 =gtool_len2;
        //            toolparaindexinfo.gtool_len3 = gtool_len3;
        //            toolparaindexinfo.gtool_rad1 = gtool_rad1;
        //            toolparaindexinfo.gtool_ang1 = gtool_ang1;
        //            toolparaindexinfo.extool_s_limit = extool_s_limit;
        //            toolparaindexinfo.extool_f_limit = extool_f_limit;
        //            toolparaindexinfo.extool_large_left = extool_large_left;
        //            toolparaindexinfo.extool_large_righ = extool_large_right;
        //            toolparaindexinfo.motool_sequ = motool_sequ;
        //            toolparaindexinfo.motool_multi = motool_multi;
        //            toolparaindexinfo.motool_max_life = motool_max_life;
        //            toolparaindexinfo.motool_alm_life = motool_alm_life;
        //            toolparaindexinfo.motool_act_life = motool_act_life;
        //            toolparaindexinfo.motool_max_count = motool_max_count;
        //            toolparaindexinfo.motool_alm_count  = motool_alm_count;
        //            toolparaindexinfo.motool_act_count = motool_act_count;
        //            toolparaindexinfo.motool_max_wear = motool_max_wear;
        //            toolparaindexinfo.motool_alm_wear = motool_alm_wear;
        //            toolparaindexinfo.motool_act_wear = motool_act_wear;

        //            ParaIndexlist.Add(toolparaindexinfo);
        //        }
        //        //       myLog.Info("cost:" + (DateTime.Now - pDate).TotalMilliseconds.ToString());
        //        // NetDll.HncNetExit();

        //        return ParaIndexlist;
        //    }
        //    catch (Exception er)
        //    {
        //        return ParaIndexlist;
        //    }


        //}

        public List<AddAxisInfo> GetAddAxisInfo(string MacID)
        {
            List<AddAxisInfo> AddAxislist = new List<AddAxisInfo>();

            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            if (Mac == null)
            {
                return AddAxislist;
            }
            string[] saAxisNoTemp = Mac.m_sAxisNo.Split(','); //<----------此处有bug-------标注者：zb 20151013
            int[] ax = new int[saAxisNoTemp.Length];
            for (int i = 0; i < saAxisNoTemp.Length; i++)
            {
                ax[i] = Convert.ToInt32(saAxisNoTemp[i]);
            }

            foreach (var axis in ax)
            {
                string axName = "";

                //   int cuttingToolUse = 0;
                //   int GcodeRunRow = 0;
                int followError = 0;
                int wcsZero = 0;
                int compensation = 0;
                //     int actPosition2 = 0;
                int synError = 0;
                int zPluseOffset = 0;
                int electricMacPosition = 0;
                int cmdPluse = 0;
                int actPluse = 0;
                double electricMacRollSpeed = 0;
                double waveFrequence = 0;
                double driveCurrent = 0;      //驱动单元电流
                double loadCurrent = 0;          //负载电流
                double ratedCurrent = 0;         //额定电流
                int progPos = 0;
                int actPos = 0;
                int cmdPos = 0;
                int actPosWcs = 0;
                int cmdPosWcs = 0;
                int actPosRcs = 0;
                int cmdPosRcs = 0;
                int leftFeed = 0;
                var AddAxisInfo = new AddAxisInfo();

                //     int breakPointPosition = 0;

                //     int programPositon = 0;
                //   ret = NetDll.HncChanGetValue((int)HncChannel.HNC_CHAN_BP_POS, 0, 0, ref breakPointPosition, 1, 0);//断点位置
                int ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_NAME, axis, ref axName, Mac.ClientNO);//取轴名
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS, axis, ref actPos, Mac.ClientNO);//取机床实际位置
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_POS, axis, ref cmdPos, Mac.ClientNO);//取机床指令位置

                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS_WCS, axis, ref actPosWcs, Mac.ClientNO);//取工件实际位置
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_POS_WCS, axis, ref cmdPosWcs, Mac.ClientNO);//取工件指令位置

                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS_RCS, axis, ref actPosRcs, Mac.ClientNO);//取相对实际位置
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_POS_RCS, axis, ref cmdPosRcs, Mac.ClientNO);//取相对指令位置

                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_LEFT_TOGO, axis, ref leftFeed, Mac.ClientNO);// 剩余进给
                //  ret = NetDll.HncChanGetValue((int)HncChannel.HNC_CHAN_TOOL_USE, 0, 0, ref cuttingToolUse, 1, Mac.ClientNO);  //当前刀具
                //   ret = NetDll.HncChanGetValue((int)HncChannel.HNC_CHAN_RUN_ROW, 0, 0, ref GcodeRunRow, 1, Mac.ClientNO);      //当前代码行号
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_FOLLOW_ERR, axis, ref followError, Mac.ClientNO);         //跟踪误差
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_WCS_ZERO, axis, ref wcsZero, Mac.ClientNO);               //工件零点

                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_PROG_POS, axis, ref progPos, Mac.ClientNO);               //工件零点
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_COMP, axis, ref compensation, Mac.ClientNO);              //补偿值
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_DRIVE_CUR, axis, ref driveCurrent, Mac.ClientNO);           //驱动单元电流
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_LOAD_CUR, axis, ref loadCurrent, Mac.ClientNO);           //负载电流
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_RATED_CUR, axis, ref ratedCurrent, Mac.ClientNO);           //负载电流
                //   ret = NetDll.HncAxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS2, 0, ref actPosition2, 1, 0);                 //实坐标2
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_SYN_ERR, axis, ref synError, Mac.ClientNO);               //同步误差
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ZSW_DIST, axis, ref zPluseOffset, Mac.ClientNO);          //Z脉偏移
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ENC_CNTR, axis, ref electricMacPosition, Mac.ClientNO);   //电机位置
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_PULSE, axis, ref cmdPluse, Mac.ClientNO);             //指令脉冲
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_PULSE, axis, ref actPluse, Mac.ClientNO);             //实际脉冲
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_MOTOR_REV, axis, ref electricMacRollSpeed, Mac.ClientNO); //电机转速
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_WAVE_FREQ, axis, ref waveFrequence, Mac.ClientNO);         //波形频率

                //    ret = NetDll.HncAxisGetValue((int)HncAxis.HNC_AXIS_PROG_POS, 0, ref programPositon, 1, 0);         //编程位置

                AddAxisInfo.axName = axName;

                AddAxisInfo.actPosition = actPos / 100000.0;  //机床实际位置
                AddAxisInfo.cmdPosition = cmdPos / 100000.0;  //机床指令位置

                AddAxisInfo.actPositionWcs = actPosWcs / 100000.0;  //工件实际位置
                AddAxisInfo.cmdPositionWcs = cmdPosWcs / 100000.0;  //工件指令位置

                AddAxisInfo.actPositionRcs = actPosRcs / 100000.0;  //相对实际位置
                AddAxisInfo.cmdPositionRcs = cmdPosRcs / 100000.0;  //相对指令位置

                AddAxisInfo.loadCur = loadCurrent;
                AddAxisInfo.leftFeed = leftFeed / 100000.0;
                AddAxisInfo.followError = followError;
                AddAxisInfo.wcsZero = wcsZero / 100000;
                AddAxisInfo.electricMacPosition = electricMacPosition;
                AddAxisInfo.actPluse = actPluse;
                AddAxisInfo.electricMacRollSpeed = electricMacRollSpeed;
                AddAxisInfo.waveFrequence = waveFrequence;
                AddAxisInfo.synError = synError;
                AddAxisInfo.zPluseOffset = zPluseOffset;
                AddAxisInfo.driveCurrent = driveCurrent;
                AddAxisInfo.ratedCurrent = ratedCurrent;
                AddAxisInfo.progPos = progPos / 100000.0;
                AddAxisInfo.compensation = compensation;
                AddAxislist.Add(AddAxisInfo);
            }
            return AddAxislist;
        }

        public HncChannelValue GetHncChannelValue(string MacID)
        {
            //  List<HncChannelValue> HncChannlList = new List<HncChannelValue>();
            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            try
            {
                int ret = 0;
                 if (Monitor.GlobalSession.HNC_NetIsConnect(Mac.ClientNO))
                {

                    int breakPointPosition = 0;
                    int cuttingToolUse = 0;
                    int GcodeRunRow = 0;
                    int toolNO = 0;
                    string progName = "";
                    //int ToolUse = 0;
                    int ToolReady = -1;
                    int ToolType = 0;
                    int Cycle = 0;
                    int Hold = 0;
                    var ChannelValue = new HncChannelValue();
                    ret = Monitor.GlobalSession.HNC_ToolMagGetToolNo((int)MagzHeadIndex.MAGZTAB_CUR_TOOL, 0, ref toolNO, Mac.ClientNO);//取当前刀具号
                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_TOOL_USE, 0, 0, ref cuttingToolUse, Mac.ClientNO);  //


                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_TOOL_RDY, 0, 0, ref ToolReady, Mac.ClientNO);//取准备刀具号
                    ret = Monitor.GlobalSession.HNC_ToolGetToolPara(cuttingToolUse, (int)ToolParaIndex.INFTOOL_TYPE, ref ToolType, Mac.ClientNO); //取刀具类型


                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_BP_POS, 0, 0, ref breakPointPosition, Mac.ClientNO);//
                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_CYCLE, 0, 0, ref Cycle, Mac.ClientNO);//  循环启动
                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_HOLD, 0, 0, ref Hold, Mac.ClientNO);//     进给保持
                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_RUN_ROW, 0, 0, ref GcodeRunRow, Mac.ClientNO);      //
                    ret = Monitor.GlobalSession.HNC_FprogGetFullName(0, ref progName, Mac.ClientNO);//取当前运行G代码名字
                    int Cntr = 0;
                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_PART_CNTR, 0, 0, ref Cntr, Mac.ClientNO);//加工计数
                    int Stati = 0;
                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_PART_STATI, 0, 0, ref Stati, Mac.ClientNO);//工件总数

                    ChannelValue.toolNO = toolNO;
                    ChannelValue.GcodeRunRow = GcodeRunRow;
                    ChannelValue.GcodeName = progName;
                    ChannelValue.breakPointPosition = breakPointPosition;
                    ChannelValue.cuttingToolUse = cuttingToolUse;
                    ChannelValue.Cycle = Cycle;
                    ChannelValue.Hold = Hold;
                    ChannelValue.ToolReady = ToolReady;
                    ChannelValue.ToolType = ToolType;
                    ChannelValue.Cntr = Cntr;
                    ChannelValue.Stati = Stati;
                    return ChannelValue;
                }

            }
            catch (Exception er)
            {
                Console.Write(er.Message);

            }
            return null;
        }

        public AxisMotion GetAxisMotion(string MacID)
        {
            //  List<AxisMotion> list = new List<AxisMotion>();
            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            try
            {
                int ret = 0;
                if (Monitor.GlobalSession.HNC_NetIsConnect(Mac.ClientNO))
                {
                    int cmdVel = 0;
                    int actVel = 0;
                    //   double loadCur = 0;
                    //   double driveCur = 0;

                    double actSpdlSpeed = 0;
                    double cmdSpdlSpeed = 0;
                    double actFeedRate = 0;
                    double cmdFeedRate = 0;
                    double loadCur = 0;
                    double ratedCurrent = 0;
                    int feedOverRide = 0;
                    int rapidOverRide = 0;
                    int spdlOverRide = 0;
                    var axismovement = new AxisMotion();


                    //  ret = NetDll.HncAxisGetValue((int)HncAxis.HNC_AXIS_LOAD_CUR, 5, ref loadCur, 1, Mac.ClientNO);//主轴负载电流
                    //   ret = NetDll.HncAxisGetValue((int)HncAxis.HNC_AXIS_RATED_CUR, 5, ref driveCur, 1, Mac.ClientNO);//主轴额定电流

                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_ACT_SPDL_SPEED, 0, 0, ref actSpdlSpeed, Mac.ClientNO);//主轴实际速度
                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_CMD_SPDL_SPEED, 0, 0, ref cmdSpdlSpeed, Mac.ClientNO);//主轴指令速度

                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_ACT_FEEDRATE, 0, 0, ref actFeedRate, Mac.ClientNO);//实际进给速度
                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_CMD_FEEDRATE, 0, 0, ref cmdFeedRate, Mac.ClientNO);//指令进给速度

                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_FEED_OVERRIDE, 0, 0, ref feedOverRide, Mac.ClientNO);//进给修调
                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_RAPID_OVERRIDE, 0, 0, ref rapidOverRide, Mac.ClientNO);//快移修调
                    ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_SPDL_OVERRIDE, 0, 0, ref spdlOverRide, Mac.ClientNO);//主轴倍率

                    ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_VEL, 0, ref cmdVel, Mac.ClientNO);//指令速度
                    ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_VEL, 0, ref actVel, Mac.ClientNO);//实际速度
                    ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_LOAD_CUR, 5, ref loadCur, Mac.ClientNO);//负载电流
                    ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_RATED_CUR, 5, ref ratedCurrent, Mac.ClientNO);   //额定电流
                    if ((int)ratedCurrent == 0)
                    { axismovement.spdlLoad = 0; }
                    else
                    {
                        axismovement.spdlLoad = loadCur / ratedCurrent;
                    }

                    //  double CurrentP = 100 * (loadCur / driveCur);
                    //   axismovement.Currentload = CurrentP;


                    axismovement.actSpdlSpeed = actSpdlSpeed; //主轴实际速度
                    axismovement.cmdSpdlSpeed = cmdSpdlSpeed; //主轴指令速度

                    axismovement.actFeedSpeed = actFeedRate; //进给实际速度
                    axismovement.cmdFeedSpeed = cmdFeedRate; //进给指令速度

                    axismovement.spdlOvrd = spdlOverRide; //主轴修调
                    axismovement.feedOvrd = feedOverRide; //进给修调
                    axismovement.rapidOvrd = rapidOverRide;//快移修调

                    return axismovement;

                }

            }
            catch (Exception er)
            {
                Console.Write(er.Message);
            }
            return null;
        }

        public List<MacGcodeFile> GetMacGcodeFile(string MacID)
        {
            List<MacGcodeFile> list = new List<MacGcodeFile>();
            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            try
            {
                string dirName = "../prog"; //待获取信息的文件夹路径
                //short clientNo = NetDll.HncNetConnect("115.156.243.173", 10001);
                // short clientNo = NetDll.HncNetConnect("115.156.243.173", 10001);
                ncfind_t[] fileInfo = new ncfind_t[128];
                //  Mac.ClientNO = GlobalSession.HNC_NetConnect(Mac.IP, (ushort)Mac.Port);
                ushort num = 0;
                int ret = Monitor.GlobalSession.HNC_NetFileGetDirInfo(dirName, fileInfo, ref num, Mac.ClientNO);
                if (ret == 0)
                {

                    for (int i = 0; i < num; i++)
                    {
                        if (fileInfo[i].info.name == "." || fileInfo[i].info.name == "..")
                            continue;
                        var macGcodeFile = new MacGcodeFile();
                        macGcodeFile.filename = fileInfo[i].info.name;

                        macGcodeFile.filesize = Math.Ceiling((decimal)(fileInfo[i].info.size) / 1024).ToString() + "K";
                        macGcodeFile.filetime = fileInfo[i].time;
                        list.Add(macGcodeFile);
                    }

                }
                return list;
            }
            catch (Exception er)
            {
                Console.Write(er.Message);
                return list;
            }



        }

        public List<ToolPara> GetToolPara(string MacID)
        {
            List<ToolPara> Toollist = new List<ToolPara>();

            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            try
            {

                if (Monitor.GlobalSession.HNC_NetIsConnect(Mac.ClientNO))
                {

                    var list = GetToolBase(MacID);
                    double gtool_len1 = 0;
                    double gtool_rad1 = 0;
                    double wtool_len1 = 0;
                    double wtool_rad1 = 0;
                    string toolNoStart = "";
                    string toolNum = "";
                    int ToolType = 0;
                    int get_toolNoStart = paraGetVal(2, 0, "040127", Mac.ClientNO, ref toolNoStart);
                    int get_toolNum = paraGetVal(2, 0, "040128", Mac.ClientNO, ref toolNum);
                    int ToolNoStart = Convert.ToInt32(toolNoStart);
                    int ToolNum = Convert.ToInt32(toolNum);
                    for (; ToolNoStart <= ToolNum; ToolNoStart++)
                    {
                        var Toolinfo = new ToolPara();
                        int ret = Monitor.GlobalSession.HNC_ToolGetToolPara(ToolNoStart, (int)ToolParaIndex.INFTOOL_TYPE, ref ToolType, Mac.ClientNO); //取刀具类型
                        ret = Monitor.GlobalSession.HNC_ToolGetToolPara(ToolNoStart, (int)ToolParaIndex.GTOOL_LEN1, ref gtool_len1, Mac.ClientNO);//铣刀具长度，车X偏置23
                        ret = Monitor.GlobalSession.HNC_ToolGetToolPara(ToolNoStart, (int)ToolParaIndex.GTOOL_RAD1, ref gtool_rad1, Mac.ClientNO);//刀尖半径 5
                        ret = Monitor.GlobalSession.HNC_ToolGetToolPara(ToolNoStart, (int)ToolParaIndex.WTOOL_LEN1, ref wtool_len1, Mac.ClientNO);//铣长度磨损，车Z磨损 3
                        ret = Monitor.GlobalSession.HNC_ToolGetToolPara(ToolNoStart, (int)ToolParaIndex.WTOOL_RAD1, ref wtool_rad1, Mac.ClientNO);//铣半径磨损，车X磨损 1

                        //第0行显示当前刀



                        Toolinfo.ToolNO = ToolNoStart;     //刀号
                        Toolinfo.ToolType = ToolType;
                        Toolinfo.ToolPos = (from a in list
                                            where a.ToolNO == ToolNoStart
                                            select a.ToolPos).FirstOrDefault();
                        Toolinfo.ToolLength = gtool_len1;  //刀具长度               
                        Toolinfo.ToolRadius = gtool_rad1;
                        Toolinfo.wToolLength = wtool_len1;
                        Toolinfo.wToolRad = wtool_rad1;
                        Toollist.Add(Toolinfo);

                    }
                }

                return Toollist;
            }
            catch (Exception er)
            {
                Console.Write(er.Message);
                return Toollist;
            }


        }


        public string TransGModeToStr(int mid)
        {
            string midTostring = mid.ToString();
            string modeSTR;
            if (midTostring.Length == 1)
            {
                modeSTR = "G0" + midTostring;
            }
            else
            {
                modeSTR = "G" + midTostring;
            }

            return modeSTR;
        }

        private int GetValue(short clientNo, string[] value)
        {
            int rowNum = 0;
            Int16 dupNum = 0;//参数重复个数
            Int16 dupNo = 0;//参数重复编号
            Int32 index = -1;//参数的索引值
            int ret = Monitor.GlobalSession.HNC_ParamanGetSubClassProp(1, (Byte)ParaSubClassProp.SUBCLASS_ROWNUM, ref rowNum, clientNo);//获取每个小类有多少行参数
            if (rowNum < 0)
            {
                return -2;//子类行数小于0的异常
            }
            ret = Monitor.GlobalSession.HNC_ParamanRewriteSubClass(1, 0, clientNo);//加载子类，参数分别是大类分类号0-6，子类分类号0-...不同的大类和子类相应的分类参数不同
            if (ret < 0)
            {
                return -3;//加载子类时出错的异常
            }

            for (int i = 0; i < rowNum; i++)
            {
                ret = Monitor.GlobalSession.HNC_ParamanTransRow2Index(1, 0, i, ref index, ref dupNum, ref dupNo, clientNo);//通过参数类别(大类号)、子类号(小类号)、行号(列号)获取指定参数的索引值
                if (index < 0)
                {
                    return -1;
                }
                Int32 parmID = -1;
                ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(1, 0, index, (Byte)ParaPropType.PARA_PROP_ID, ref parmID, clientNo);
                if (ret < 0)
                {
                    return -2;
                }
                if (parmID.ToString("D6") == "010220")
                {
                    sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                    ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(1, 0, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                    value[0] = GetStringFrmByte(araayBt);
                    continue;
                }
                else if (parmID.ToString("D6") == "010221")
                {
                    sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                    ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(1, 0, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                    value[1] = GetStringFrmByte(araayBt);
                    continue;
                }
                else if (parmID.ToString("D6") == "010223")
                {
                    sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                    ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(1, 0, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                    value[2] = GetStringFrmByte(araayBt);
                    continue;
                }
                else if (parmID.ToString("D6") == "010224")
                {
                    sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                    ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(1, 0, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                    value[3] = GetStringFrmByte(araayBt);
                    break;
                }
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

        public List<string> GetGmode(string MacID)
        {
            List<string> modestr = new List<string>();
            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            try
            {
                int chanNum = 0;
                int ret = Monitor.GlobalSession.HNC_SystemGetValue((int)HncSystem.HNC_SYS_CHAN_NUM, ref chanNum, Mac.ClientNO);//获取系统数据的值,获取通道号
                int mid = 0;
                string[] GmodeParaStr = new string[4];
                int get_GmodeParaStr = GetValue(Mac.ClientNO, GmodeParaStr);

                if (get_GmodeParaStr == 0)
                {

                    string str = GmodeParaStr[0] + "," + GmodeParaStr[1];
                    string[] GmodeStr = str.Split(',');
                    for (int i = 0; i < GmodeStr.Length; i++)
                    {
                        if (GmodeStr[i] == "-1")
                        {
                            break;//参数必须按顺序填写，遇到-1后面的就不处理了
                        }
                        else
                        {
                            ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_MODAL, 0, Convert.ToInt32(GmodeStr[i]), ref mid, Mac.ClientNO); //获取通道数据的值  

                            modestr.Add(TransGModeToStr(mid));
                        }
                    }
                }
                return modestr;
            }
            catch (Exception er)
            {
                Console.Write(er.Message);
                return modestr;
            }
        }

        public bool SendGcodeFile(string MacID, string GcodeName, string GcodeContent)
        {
            string gcodeFilename = System.AppDomain.CurrentDomain.BaseDirectory + "/gcode/" + GcodeName;
            File.WriteAllText(gcodeFilename, GcodeContent, Encoding.GetEncoding("GB2312"));
            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            string dir = "../prog";
            string dirName = dir + "/" + GcodeName; //目标机器接收文件后，储存的完整文件路径名
            int ret = Monitor.GlobalSession.HNC_NetFileSend(gcodeFilename, dirName, Mac.ClientNO);
            if (ret == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public string GetGcodeContent(string MacID, string GcodeName)
        {
            string localName;
            string localDir;
            //string localName;
            localDir = System.AppDomain.CurrentDomain.BaseDirectory + "gcode\\";
            if (!Directory.Exists(localDir))//判断文件夹是否存在 
            {
                Directory.CreateDirectory(localDir);//不存在则创建文件夹 
            }
            // string localName = str + "Logs/" + MacID + "--" + DateTime.Now.ToString("yyyy-MM-dd-HH-mm") + "--" + Logfile + ".LOG";//+DateTime.Now.ToShortTimeString(); //本地接收文件后，文件的保存路径

            string dstName = "../prog/" + GcodeName;  //待获取的文件，在目标机器的路径名
            try
            {
                localName = localDir + GcodeName;
                var Mac = (from a in Monitor.ConnectionList
                           where a.MacID == MacID
                           select a).FirstOrDefault();
                //-----------zb-------20151223-start
                int ret = Monitor.GlobalSession.HNC_NetFileGet(localName, dstName, Mac.ClientNO);
                // int ret = 0;
                //-----------zb-------20151223-end
                //var fileText = File.ReadAllText(localName);
                // Encoding.Default.GetString(
                //StreamReader sw = new StreamReader(File.Open(localName, FileMode.Open));
                //string GcodeContent = sw.ReadToEnd();
                //sw.Close();

                if (ret == 0)   //文件获取成功
                {
                    string GcodeContent = System.IO.File.ReadAllText(localName, Encoding.Default);
                    return GcodeContent;
                }
                else   //文件获取成功
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        //刀具接口参数介绍：toolNO待查刀具号，QerToolPara待查刀具参数索引，ClientNO连接号
        public int? GetToolData(int toolNO, int QerToolPara, string QermacID)
        {
            int? toolValue = null;
            if (QermacID == null)
            {
                var LiskData = (from a in Monitor.ConnectionList
                                where a.MacID == QermacID
                                select a).FirstOrDefault();
                int tempToolValue = -1;
                int ret = Monitor.GlobalSession.HNC_ToolGetToolPara(toolNO, QerToolPara, ref tempToolValue, LiskData.ClientNO);//获取参数值
                if (0 == ret)
                {
                    toolValue = tempToolValue;
                }
            }
            return toolValue;
        }

        public int paraGetVal(int fileNo, int recNo, string ParaCode, short clientNo, ref string sParaVal)
        {

            int nRet = -1;
            int rowNum = 0;

            nRet = Monitor.GlobalSession.HNC_ParamanGetSubClassProp(fileNo, (Byte)ParaSubClassProp.SUBCLASS_ROWNUM, ref rowNum, clientNo);//获取每个小类有多少行参数
            if (rowNum < 0)
            {
                return -2;//子类行数小于0的异常
            }

            nRet = Monitor.GlobalSession.HNC_ParamanRewriteSubClass(fileNo, recNo, clientNo);//加载子类，参数分别是大类分类号0-6，子类分类号0-...不同的大类和子类相应的分类参数不同
            if (nRet < 0)
            {
                return -3;//加载子类时出错的异常
            }

            for (int i = 0; i < rowNum; i++)//把每行参数读取出来
            {
                Int16 dupNum = 0;//参数重复个数
                Int16 dupNo = 0;//参数重复编号
                Int32 index = -1;//参数的索引值

                nRet = Monitor.GlobalSession.HNC_ParamanTransRow2Index(fileNo, recNo, i, ref index, ref dupNum, ref dupNo, clientNo);//通过参数类别(大类号)、子类号(小类号)、行号(列号)获取指定参数的索引值
                if (index < 0)
                {
                    return -1;
                }

                Int32 parmID = -1;
                nRet = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_ID, ref parmID, clientNo);
                if (nRet < 0)
                {
                    return -1;
                }

                if (parmID.ToString("D6") == ParaCode)
                {
                    Int32 storeType = -1;
                    nRet = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_STORE, ref storeType, clientNo);
                    if (storeType < 0)
                    {
                        return -1;
                    }

                    int iVal = 0;
                    double dVal = 0;

                    switch (storeType)
                    {
                        case (sbyte)PAR_STORE_TYPE.TYPE_BOOL:
                        case (sbyte)PAR_STORE_TYPE.TYPE_UINT:
                        case (sbyte)PAR_STORE_TYPE.TYPE_INT:
                            {
                                nRet = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                                if (nRet < 0)
                                {
                                    break;
                                }
                                sParaVal = iVal.ToString();
                                break; //本条case块结束
                            }
                        case (sbyte)PAR_STORE_TYPE.TYPE_FLOAT:
                            {
                                nRet = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref dVal, clientNo);
                                if (nRet < 0)
                                {
                                    break;
                                }
                                sParaVal = dVal.ToString("F6");

                                break;
                            }
                        case (sbyte)PAR_STORE_TYPE.TYPE_STRING:
                            {
                                nRet = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref sParaVal, clientNo);
                                if (nRet < 0)
                                {
                                    return -1;
                                }
                                break;
                            }
                        case (sbyte)PAR_STORE_TYPE.TYPE_HEX4:
                            {
                                nRet = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                                if (nRet < 0)
                                {
                                    return -1;
                                }

                                sParaVal = iVal.ToString();
                                break;
                            }
                        case (sbyte)PAR_STORE_TYPE.TYPE_BYTE:
                            {
                                sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                                nRet = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                                if (nRet < 0)
                                {
                                    return -1;
                                }
                                sParaVal = GetStringFrmByte(araayBt);
                                break;
                            }
                        default:
                            {
                                sParaVal = null;
                                return -1;

                            }
                    }
                    break;
                }
            }
            return 0;
        }

        public int GetParaFormMac(int fileNo, int recNo, string ParaCode, string sMacID, ref string sParaVal)
        {
            var a = (from ct in Monitor.ConnectionList
                     where ct.MacID == sMacID
                     select ct).FirstOrDefault();
            short nClientNo;
            if (null != a)
                nClientNo = a.ClientNO;
            else
                return -1;
            if (0 != paraGetVal(fileNo, recNo, ParaCode, nClientNo, ref sParaVal))
                return -2;
            return 0;
        }

        //页面参数存储接口
        public int Save2DBForPara_FromIF(string macID, string sVerTitle)
        {
            bool bIsFirst = false;
            var a = (from ct in Monitor.ConnectionList
                     where ct.MacID == macID
                     select ct).FirstOrDefault();
            if (a.Connected)
            {
                hncData.NCParaVer tableQerVerID = new NCParaVer();
                try
                {
                    tableQerVerID = db.NCParaVer.Where(t => t.MacID == a.MacID).OrderByDescending(t => t.VerID).FirstOrDefault();//查询该macID的最新一条记录，即VerID值最大
                }
                catch
                {
                    //Monitor.RemoteDBIsReady2 = false;
                }
                if (tableQerVerID == null)
                {
                    bIsFirst = true;
                }

                var tableNCParaVer = new NCParaVer();
                tableNCParaVer.MacID = a.MacID;
                tableNCParaVer.VerTitle = sVerTitle;
                tableNCParaVer.VerCreateTime = DateTime.Now;
                Monitor.SaveToDBIsOK(ref db, true, tableNCParaVer);
                try
                {
                    tableQerVerID = db.NCParaVer.Where(t => t.MacID == a.MacID).OrderByDescending(t => t.VerID).FirstOrDefault();//查询该macID的最新一条记录，即VerID值最大
                }
                catch
                {
                    //Monitor.RemoteDBIsReady2 = false;
                }

                if (bIsFirst)
                {
                    //第一次存库，所有参数都存入数据库。
                    int nRet = 0;
                    /* Tc
                    var tableNCParaVer = new NCParaVer();
                    tableNCParaVer.MacID = a.MacID;
                    tableNCParaVer.VerTitle = sVerTitle;
                    tableNCParaVer.VerCreateTime = DateTime.Now;
                    db.AddToNCParaVer(tableNCParaVer);
                    db.SaveChanges();
                    tableQerVerID = db.NCParaVer.Where(t => t.MacID == a.MacID).OrderByDescending(t => t.VerID).FirstOrDefault();//查询该macID的最新一条记录，即VerID值最大
                    Tc*/

                    int fileNum = 6;//系统最大分类数，大类
                    for (int i = 0; i < fileNum; i++)
                    {
                        int recNum = -1;//对应大类的子类分类数，小类
                        nRet = Monitor.GlobalSession.HNC_ParamanGetSubClassProp(i, (Byte)ParaSubClassProp.SUBCLASS_NUM, ref recNum, 0);//根据大的分类类别0-6共7个索引，获取参数的子类的属性。属性包括：子类名，子类行数，子类数。
                        //本函数根据(Byte)ParaSubClassProp.SUBCLASS_NUM的不同，返回不同的值到ref中

                        if (recNum < 0)
                        {
                            nRet = recNum;
                            break;
                        }
                        else if (recNum > 0)
                        {
                            for (int j = 0; j < recNum; j++)
                            {
                                nRet = UpdateParaItem(i, j, a.ClientNO, a.MacID, tableQerVerID.VerID);
                                if (nRet < 0)
                                {
                                    break;//当某个大类的某个小类读取出错时，跳出循环，不再继续读，返回错误代码ret。
                                }
                            }
                            Monitor.SaveToDBIsOK(ref db, false, null);
                        }
                        if (nRet < 0)
                        {
                            break;
                        }
                    }
                    return nRet;
                }
                else
                {
                    //非第一次，只存变化的值。
                    long lLastVerID = tableQerVerID.VerID;
                    //bool bIsSaved = false;
                    int nRet = 0;

                    int fileNum = 6;//系统最大分类数，大类
                    for (int i = 0; i < fileNum; i++)//开始对每一个比较，观察是否有变化
                    {
                        int recNum = -1;//对应大类的子类分类数，小类
                        nRet = Monitor.GlobalSession.HNC_ParamanGetSubClassProp(i, (Byte)ParaSubClassProp.SUBCLASS_NUM, ref recNum, 0);//根据大的分类类别0-6共7个索引，获取参数的子类的属性。属性包括：子类名，子类行数，子类数。
                        //本函数根据(Byte)ParaSubClassProp.SUBCLASS_NUM的不同，返回不同的值到ref中
                        if (recNum < 0)
                        {
                            nRet = recNum;
                            break;
                        }
                        else if (recNum > 0)
                        {
                            for (int j = 0; j < recNum; j++)
                            {
                                nRet = Save2DBPerSubCls(i, j, a.ClientNO, macID, lLastVerID, sVerTitle);//,ref bIsSaved);
                                if (nRet != 0)
                                {
                                    return -1;//----存储时出错！！
                                }
                            }//小类for循环

                            Monitor.SaveToDBIsOK(ref db, false, null);

                        }//else if (recNum > 0)
                    }//大类for循环
                    return nRet;
                }
            }// if (a.Connected)
            return -1;//函数执行的错误类型为所选的机床未连接
        }

        public int Save2DBPerSubCls(int fileNo, int recNo, short clientNo, string MacID, long lLastVerID, string sVerTitle)//,ref bool bIsSaved)
        {
            int ret = 0;
            int rowNum = 0;
            ret = Monitor.GlobalSession.HNC_ParamanGetSubClassProp(fileNo, (Byte)ParaSubClassProp.SUBCLASS_ROWNUM, ref rowNum, clientNo);//获取每个小类有多少行参数
            if (rowNum < 0)
            {
                return -2;//子类行数小于0的异常
            }

            ret = Monitor.GlobalSession.HNC_ParamanRewriteSubClass(fileNo, recNo, clientNo);//加载子类，参数分别是大类分类号0-6，子类分类号0-...不同的大类和子类相应的分类参数不同
            if (ret < 0)
            {
                return -3;//加载子类时出错的异常
            }

            for (int i = 0; i < rowNum; i++)//把每行参数读取出来，放在strPtr数组中，每一个数组元素对应当前行参数的某列数据
            {
                bool bIsChanged = false;

                Int16 dupNum = 0;//参数重复个数
                Int16 dupNo = 0;//参数重复编号
                Int32 index = -1;//参数的索引值


                ret = Monitor.GlobalSession.HNC_ParamanTransRow2Index(fileNo, recNo, i, ref index, ref dupNum, ref dupNo, clientNo);//通过参数类别(大类号)、子类号(小类号)、行号(列号)获取指定参数的索引值
                if (index < 0)
                {
                    return -1;
                }


                Int32 parmID = -1;
                ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_ID, ref parmID, clientNo);
                if (ret < 0)
                {
                    return -1;
                }
                //获取参数储存类型
                Int32 storeType = -1;
                ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_STORE, ref storeType, clientNo);
                if (storeType < 0)
                {
                    return -1;
                }

                int iVal = 0;
                double dVal = 0;
                string NowParaVal = null;

                switch (storeType)
                {
                    case (sbyte)PAR_STORE_TYPE.TYPE_BOOL:
                    case (sbyte)PAR_STORE_TYPE.TYPE_UINT:
                    case (sbyte)PAR_STORE_TYPE.TYPE_INT:
                        {
                            ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                            if (ret < 0)
                            {
                                break;
                            }
                            NowParaVal = iVal.ToString();//参数当前值

                            break; //本条case块结束
                        }
                    case (sbyte)PAR_STORE_TYPE.TYPE_FLOAT:
                        {
                            ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref dVal, clientNo);
                            if (ret < 0)
                            {
                                break;
                            }
                            NowParaVal = dVal.ToString("F6");

                            break;
                        }
                    case (sbyte)PAR_STORE_TYPE.TYPE_STRING:
                        {
                            ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref NowParaVal, clientNo);
                            if (ret < 0)
                            {
                                break;
                            }
                            break;
                        }
                    case (sbyte)PAR_STORE_TYPE.TYPE_HEX4:
                        {
                            ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                            NowParaVal = "0x" + iVal.ToString("X2");
                            break;
                        }
                    case (sbyte)PAR_STORE_TYPE.TYPE_BYTE:
                        {
                            sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                            ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                            NowParaVal = GetStringFrmByte(araayBt);
                            break;
                        }
                    default:
                        {
                            NowParaVal = null;
                            break;
                        }
                }//switch

                if (null != NowParaVal)
                {
                    if ("" == NowParaVal)
                    {
                        NowParaVal = " ";
                    }
                    bIsChanged = QerIsChangedPerRrd(MacID, parmID.ToString("D6"), NowParaVal, lLastVerID);
                    /*Tc
                     if (bIsChanged && !bIsSaved)
                    {
                        bIsSaved = true;
                        var tableNCParaVer = new NCParaVer();
                        tableNCParaVer.MacID = MacID;
                        tableNCParaVer.VerTitle = sVerTitle;
                        tableNCParaVer.VerCreateTime = DateTime.Now;
                        db.AddToNCParaVer(tableNCParaVer);
                        db.SaveChanges();
                    }
                    Tc*/
                    if (bIsChanged)
                    {
                        //var tableQerVerID = db.NCParaVer.Where(t => t.MacID == MacID).OrderByDescending(t => t.VerID).FirstOrDefault();//查询该macID的最新一条记录，即VerID值最大

                        //写当前值表
                        var writeNcNow = new NCParameter();
                        writeNcNow.ParaCode = parmID.ToString("D6");
                        writeNcNow.MacID = MacID;
                        if (NowParaVal.Length > 32)
                        {
                            NowParaVal = NowParaVal.Substring(0, 32);
                        }
                        if (NowParaVal == "")
                        {
                            NowParaVal = " ";
                        }
                        writeNcNow.NowVal = NowParaVal;
                        writeNcNow.ChangeTime = DateTime.Now;
                        writeNcNow.ParaVerID = lLastVerID;//tableQerVerID.VerID;

                        Monitor.SaveToDBIsOK(ref db, true, writeNcNow);
                    }
                }
            }//for
            return 0;
        }

        public bool QerIsChangedPerRrd(string MacID, string sParaCode, string NowParaVal, long lLastVerID) //从最新的VerID开始往前迭代查询，观察paraID->NowParaVal是否已经保存。如果有变化，返回true，否则false.
        {
            long lLstParaVerID = lLastVerID;
            while (lLstParaVerID != 0)
            {
                hncData.NCParameter QerNCPara = new NCParameter();
                try
                {
                    QerNCPara = db.NCParameter.Where(t => (t.MacID == MacID) && (t.ParaCode == sParaCode) && (t.ParaVerID == lLstParaVerID)).FirstOrDefault();//查询该macID的最新一条记录，即VerID值最大
                }
                catch
                {
                    //Monitor.RemoteDBIsReady2 = false;
                }
                if (null != QerNCPara)
                {
                    if (NowParaVal != QerNCPara.NowVal)
                        return true;
                    else
                        return false;
                }
                --lLstParaVerID;
            }
            return false;
        }

        //返回当前机床状态接口，函数执行出错时，返回错误代码999
        private int UpdateParaItem(int fileNo, int recNo, short clientNo, string MacID, long lVerID)
        {
            int ret = 0;
            int rowNum = 0;
            ret = Monitor.GlobalSession.HNC_ParamanGetSubClassProp(fileNo, (Byte)ParaSubClassProp.SUBCLASS_ROWNUM, ref rowNum, clientNo);//获取每个小类有多少行参数
            if (rowNum < 0)
            {
                return -2;//子类行数小于0的异常
            }

            ret = Monitor.GlobalSession.HNC_ParamanRewriteSubClass(fileNo, recNo, clientNo);//加载子类，参数分别是大类分类号0-6，子类分类号0-...不同的大类和子类相应的分类参数不同
            if (ret < 0)
            {
                return -3;//加载子类时出错的异常
            }

            string[] strPar = new string[8];//读出每条参数的暂存数组
            int myEffectWay = 999;
            int myDataType = 999;
            int index1 = -1;



            for (int i = 0; i < rowNum; i++)//把每行参数读取出来，放在strPtr数组中，每一个数组元素对应当前行参数的某列数据
            {
                ret = GetParContent(fileNo, recNo, i, clientNo, strPar, ref myEffectWay, ref myDataType, ref index1);//（大类号，子类号，当前列号，clientNo，字符串数组）
                if (ret == -1)
                {
                    break; //在读某一条参数的内容时出现异常，跳出，不再继续读该小分类的参数
                }

                //写当前值表
                var writeNcNow = new NCParameter();
                writeNcNow.ParaCode = strPar[0];
                writeNcNow.MacID = MacID;
                if (strPar[2].Length > 32)
                {
                    strPar[2] = strPar[2].Substring(0, 32);
                }
                writeNcNow.NowVal = strPar[2];
                writeNcNow.ChangeTime = DateTime.Now;
                writeNcNow.ParaVerID = lVerID;
                Monitor.SaveToDBIsOK(ref db, true, writeNcNow);
            }
            return ret;
        }

        public int GetNowMachineState(string sMacID)
        {
            int nNowState = 999;
            var a = (from ct in Monitor.ConnectionList
                     where ct.MacID == sMacID
                     select ct).FirstOrDefault();
            if (null != a)
            {
                return a.m_nMachineState;
            }
            return nNowState;
        }

        //写文件批量采样接口 返回值0->成功，-1->MacID空错误，-2->采样频率，块大小配置出差，-3->调用StartSampl出错
        public int StartSmplRecord(string MacID, string[] SamplAxis, HncSampleType SamplType)
        {
            int nSampleFreq = int.Parse(System.Configuration.ConfigurationManager.AppSettings["nSampleFreqAppConfig"]);
            int nFileBlockSize = int.Parse(System.Configuration.ConfigurationManager.AppSettings["nFileBlockSizeAppConfig"]);

            if (null == MacID) return -1;
            if (nSampleFreq <= 0 || nFileBlockSize <= 0) return -2;
            int nRetVal = 0;// Monitor.StarSampl(MacID, SamplAxis, SamplType, nSampleFreq, nFileBlockSize);
            if (0 == nRetVal)
                return 0;
            else
                return -3;
        }

        //返回值-2->开启采样失败，-1->参数非法，0->默认，未开启采样功能,1->正在执行采样，2->与实时显示同步
        public int GetSamplState(string MacID)
        {
            var a = (from ct in Monitor.ConnectionList
                     where ct.MacID == MacID
                     select ct).FirstOrDefault();
            if (null != a)
            {
                return a.m_nSamplStatFlagForFile;
            }
            else
            {
                return -1;
            }
        }


        //实时批量采样接口
        public HncDataInterfaces.CSampDataType[] GetSamplData(string MacID, string[] SamplAxis, HncSampleType SamplType)
        {
            //var a = (from ct in Monitor.ConnectionList
            //         where ct.MacID == MacID
            //         select ct).FirstOrDefault();
            //if (null == a)
            //{
            //    return null;
            //}
            //lock(Monitor.oLockForSamp)
            //    Monitor.m_nCounterForRTSamplTmr = int.Parse(System.Configuration.ConfigurationManager.AppSettings["CounterForRTSamplTmr"]);
            ////锁定运行的代码段

            //var RedTmrForSamplData = (from ct in Monitor.m_lstRealtimeTmrForSampl
            //                          where ct.m_sMacID == MacID
            //                          select ct).FirstOrDefault();

            //if (null == RedTmrForSamplData)
            //{
            //    Monitor.RTStarSampl(MacID, SamplAxis, SamplType);
            //    return null;
            //}

            //if (SamplAxis.Length != RedTmrForSamplData.m_saSamplAxis.Length || 0 == GlobalSession.HNC_SamplGetStat(a.ClientNO))
            //{
            //    Monitor.RTStopSampl(MacID);
            //    Monitor.RTStarSampl(MacID, SamplAxis, SamplType);
            //    return null;
            //}
            ////-----------------------------------出队start  
            //int nDataBlockNum = 0;
            //int[] naAxisIndex = new int[SamplAxis.Length];
            //int nIndexFornaAxisIndex = 0;
            //for (int i = 0; i < SamplAxis.Length; i++)
            //{
            //    int nAxisNameIndex = -1;
            //    for (int nAxisIndexTemp = 0; nAxisIndexTemp < RedTmrForSamplData.m_saSamplAxis.Length; ++nAxisIndexTemp)
            //    {
            //        if (0 == RedTmrForSamplData.m_queArrRTData[nAxisIndexTemp].Count)
            //        {
            //            return null;
            //        }
            //        if (SamplAxis[i] == RedTmrForSamplData.m_queArrRTData[nAxisIndexTemp].Peek().m_sAxisName)
            //        {
            //            naAxisIndex[nIndexFornaAxisIndex++] = nAxisNameIndex = nAxisIndexTemp;
            //            break;
            //        }
            //    }
            //    if (nAxisNameIndex < 0)
            //        return null;
            //    if (0 == i)
            //        nDataBlockNum = RedTmrForSamplData.m_queArrRTData[nAxisNameIndex].Count;
            //    else
            //        nDataBlockNum = RedTmrForSamplData.m_queArrRTData[nAxisNameIndex].Count < nDataBlockNum ? RedTmrForSamplData.m_queArrRTData[nAxisNameIndex].Count : nDataBlockNum;
            //}



            //HncDataInterfaces.CSampDataType[] caSamplData = new HncDataInterfaces.CSampDataType[nDataBlockNum * SamplAxis.Length];
            //for (int i = 0; i < nDataBlockNum * SamplAxis.Length; i++)
            //{
            //    caSamplData[i] = new HncDataInterfaces.CSampDataType();
            //    caSamplData[i].m_faSamplData = new float[10];
            //}

            //int nSave2ArrIndex = 0;
            //for (int j = 0; j < SamplAxis.Length; j++)
            //{
            //    for (int i = 0; i < nDataBlockNum; i++)
            //    {
            //        CSampDataType cSamplDataTemp = new CSampDataType();
            //        lock (Monitor.thisLock)
            //            cSamplDataTemp = RedTmrForSamplData.m_queArrRTData[naAxisIndex[j]].Dequeue().m_cSampDataType;
            //        caSamplData[nSave2ArrIndex].m_dtSample = cSamplDataTemp.m_dtSample;
            //        caSamplData[nSave2ArrIndex].m_faSamplData = cSamplDataTemp.m_faSamplData;
            //        caSamplData[nSave2ArrIndex].m_nGCodeRunRow = cSamplDataTemp.m_nGCodeRunRow;
            //        caSamplData[nSave2ArrIndex].m_nRealSize = cSamplDataTemp.m_nRealSize;
            //        caSamplData[nSave2ArrIndex++].m_nTimeInter = cSamplDataTemp.m_nTimeInter;
            //    }
            //}
            ////-----------------------------------出队end
            //return caSamplData;
            return null;
        }

        //参数fileNo->大类号 recNo->小类号 nPageNum->访问第几页的数据 nRecordPerPage->每页多少条数据 
        public ListForParaRT GetParaValRealtime(int fileNo, int recNo, int nPageNum, int nRecordPerPage, int nTotalRecordsIndex, string macID)
        {
            ListForParaRT listParaVal = new ListForParaRT();
            List<DataItem_ParaRT> temp = new List<DataItem_ParaRT>();

            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == macID
                       select a).FirstOrDefault();

            int nRet = -1;
            int rowNum = 0;//对应每个小类的列数，大类相同的每个小类的列数都是相等的，只需取一遍
            nRet = -1;
            nRet = Monitor.GlobalSession.HNC_ParamanGetSubClassProp(fileNo, (Byte)ParaSubClassProp.SUBCLASS_ROWNUM, ref rowNum, Mac.ClientNO);//获取每个小类有多少行参数
            if (nRet != 0)
            {
                return null;//子类行数小于0的异常
            }

            int nSubNum = 1;//对应大类的子类分类数，小类
            if (-1 == nTotalRecordsIndex)
            {
                nRet = Monitor.GlobalSession.HNC_ParamanGetSubClassProp(fileNo, (Byte)ParaSubClassProp.SUBCLASS_NUM, ref nSubNum, 0);//根据大的分类类别0-6共7个索引，获取参数的子类的属性。属性包括：子类名，子类行数，子类数。
                if (0 != nRet)
                {
                    return null;
                }
                listParaVal.nTotalRecordsNum = rowNum * nSubNum;//获取相应的总记录数，用于分页使用
            }
            else listParaVal.nTotalRecordsNum = rowNum;

            if (rowNum > nRecordPerPage)
            {
                rowNum = (rowNum - ((nPageNum - 1) * nRecordPerPage)) > nRecordPerPage ? nRecordPerPage : (rowNum - ((nPageNum - 1) * nRecordPerPage));
            }

            nRet = Monitor.GlobalSession.HNC_ParamanRewriteSubClass(fileNo, recNo, Mac.ClientNO);//加载子类，参数分别是大类分类号0-6，子类分类号0-...不同的大类和子类相应的分类参数不同
            if (nRet < 0)
            {
                return null;//加载子类时出错的异常
            }

            string[] strPar = new string[8];//读出每条参数的暂存数组
            int myEffectWay = 999;
            int myDataType = 999;
            int index1 = -1;

            for (int i = (nPageNum - 1) * nRecordPerPage; i < ((nPageNum - 1) * nRecordPerPage) + rowNum; i++)//把每行参数读取出来
            {
                int nFlag = GetParContent(fileNo, recNo, i, Mac.ClientNO, strPar, ref myEffectWay, ref myDataType, ref index1);
                if (nFlag == 0)
                    temp.Add(new DataItem_ParaRT
                    {
                        m_sParaCode = strPar[0],
                        m_sParaName = strPar[1],
                        m_sDefaultVal = strPar[4],
                        m_sMaxVal = strPar[6],
                        m_sMinVal = strPar[5],
                        m_sEffectWay = myEffectWay,
                        m_sNowVal = strPar[2],
                        m_nDataType = myDataType
                    });
            }
            listParaVal.ListForPaData = temp;
            return listParaVal;
        }

        private int GetParContent(int fileNo, int recNo, int row, short clientNo, string[] strPar, ref int EffectWay, ref int DataType, ref int index1)//参数为（大类号，子类号，当前列号，clientNo，字符串数组），根据参数读出一条记录的内容。
        {
            Int16 dupNum = 0;//参数重复个数
            Int16 dupNo = 0;//参数重复编号
            Int32 index = -1;//参数的索引值


            int ret = -1;
            ret = Monitor.GlobalSession.HNC_ParamanTransRow2Index(fileNo, recNo, row, ref index, ref dupNum, ref dupNo, clientNo);//通过参数类别(大类号)、子类号(小类号)、行号(列号)获取指定参数的索引值
            if (index < 0)
            {
                return -1;
            }
            index1 = index;

            //获取生效方式
            Int32 actType = -1;
            ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_ACT, ref actType, clientNo);
            if (actType < 0)
            {
                return -1;
            }
            /*
             * 0->保存生效
             * 1->立即生效
             * 2->复位生效
             * 3->重启生效
             * 4->隐藏未启用
             */
            EffectWay = actType;

            //获取参数号
            Int32 parmID = -1;
            ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_ID, ref parmID, clientNo);
            if (ret < 0)
            {
                return -1;
            }
            strPar[0] = parmID.ToString("D6");
            //if (strPar[0] == "010220")
            //{
            //    index1 = index;
            //    break;
            //}
            //continue;


            //获取参数名称
            ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_NAME, ref strPar[1], clientNo);
            if (ret < 0)
            {
                return -1;
            }


            //获取参数储存类型
            Int32 storeType = -1;
            ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_STORE, ref storeType, clientNo);
            if (storeType < 0)
            {
                return -1;
            }
            DataType = storeType;

            //获取参数值、默认值、最小值和最大值
            int iVal = 0;
            double dVal = 0;
            switch (storeType)
            {
                case (sbyte)PAR_STORE_TYPE.TYPE_BOOL:
                case (sbyte)PAR_STORE_TYPE.TYPE_UINT:
                case (sbyte)PAR_STORE_TYPE.TYPE_INT:
                    {
                        ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[2] = iVal.ToString();//参数当前值

                        strPar[3] = iVal.ToString();//参数当前值
                        strPar[7] = iVal.ToString();//参数当前值

                        ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_DFVALUE, ref iVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[4] = iVal.ToString();//参数默认值

                        ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MINVALUE, ref iVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[5] = iVal.ToString();//最小值

                        ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MAXVALUE, ref iVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[6] = iVal.ToString();//最大值

                        break; //本条case块结束
                    }
                case (sbyte)PAR_STORE_TYPE.TYPE_FLOAT:
                    {
                        ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref dVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[2] = dVal.ToString("F6");
                        ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_DFVALUE, ref dVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[4] = dVal.ToString("F6");
                        ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MINVALUE, ref dVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[5] = dVal.ToString("F6");
                        ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_MAXVALUE, ref dVal, clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[6] = dVal.ToString("F6");
                        break;
                    }
                case (sbyte)PAR_STORE_TYPE.TYPE_STRING:
                    {
                        ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref strPar[2], clientNo);
                        if (ret < 0)
                        {
                            break;
                        }
                        strPar[4] = "N/A";
                        strPar[5] = "N/A";
                        strPar[6] = "N/A";
                        break;
                    }
                case (sbyte)PAR_STORE_TYPE.TYPE_HEX4:
                    {
                        ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, ref iVal, clientNo);
                        strPar[2] = "0x" + iVal.ToString("X2");
                        strPar[4] = "N/A";
                        strPar[5] = "N/A";
                        strPar[6] = "N/A";
                        break;
                    }
                case (sbyte)PAR_STORE_TYPE.TYPE_BYTE:
                    {
                        sbyte[] araayBt = new sbyte[HncDataDef.DATA_STR_LEN];
                        ret = Monitor.GlobalSession.HNC_ParamanGetParaProp(fileNo, recNo, index, (Byte)ParaPropType.PARA_PROP_VALUE, araayBt, clientNo);
                        strPar[2] = GetStringFrmByte(araayBt);
                        strPar[4] = "N/A";
                        strPar[5] = "N/A";
                        strPar[6] = "N/A";
                        break;
                    }
                default:
                    {
                        strPar[2] = null;
                        strPar[4] = null;
                        strPar[5] = null;
                        strPar[6] = null;
                        break;
                    }
            }

            if (ret < 0)
            {
                return -1;
            }
            return 0;
        }

        public bool DownLoadPLCData(string MacID, string PlcDataName)
        {
            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
             if (Monitor.GlobalSession.HNC_NetIsConnect(Mac.ClientNO))
            {

                string localFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "PLCs/";
                string localFileFullName = localFilePath + MacID + "--" + PlcDataName + ".DIT";//+DateTime.Now.ToShortTimeString(); //本地接收文件后，文件的保存路径
                string dstPath = "../plc";
                string dstFileName = PlcDataName + ".DIT";
                string dstFileFullName = dstPath + dstFileName; //待获取的文件，在目标机器的路径名

                /////////////////////////////////////////////////////////////////

                UInt16 paraUI = 0;
                ncfind_t[] fileInfo = new ncfind_t[HncDataDef.MAX_FILE_NUM_PER_DIR];
                Monitor.GlobalSession.HNC_NetFileGetDirInfo(dstPath, fileInfo, ref paraUI, Mac.ClientNO);//此函数的返回值永远都是0--成功，即使没有获取到信息,而应该通过paraUI的条数确定是否取到了文件信息。


                if (1 > paraUI)
                {
                    return false;
                }


                ncfind_t ncfInfo = fileInfo.Where(t => t.info.name == dstFileName).LastOrDefault();
                if ((0 == ncfInfo.handle) || ("0" == ncfInfo.time) || (null == ncfInfo.info.name) || (0 == ncfInfo.info.size))
                {
                    return false;
                }

                DateTime dstFileCreatTime = DateTime.ParseExact(ncfInfo.time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.CurrentCulture);

                var LocalFile = new FileInfo(localFileFullName);
                if (LocalFile.Exists)
                {
                    if (dstFileCreatTime.Equals(LocalFile.CreationTime) && (ncfInfo.info.size == LocalFile.Length))
                    {
                        return true;
                    }
                }
                else
                {
                    if (!Directory.Exists(localFilePath))//判断文件夹是否存在 
                    {
                        Directory.CreateDirectory(localFilePath);//不存在则创建文件夹 
                    }
                }

                if (0 == Monitor.GlobalSession.HNC_NetFileGet(localFileFullName, dstFileFullName, Mac.ClientNO))
                {
                    new FileInfo(localFileFullName).CreationTime = dstFileCreatTime;
                    return true;
                }

            }
            return false;
        }

        ////ˇˇˇˇˇˇˇˇˇ20150907
        private bool ReadPLCNameFromRomteCFG(string MacID, short nClientNo, ref string PlcName)
        {
            string localFileFullName = System.AppDomain.CurrentDomain.BaseDirectory + MacID + "_LNC32.CFG";


            #region LookupDir

            ushort num = 0;
            ncfind_t[] fileInfo = new ncfind_t[HncDataDef.MAX_FILE_NUM_PER_DIR];
            int rst = Monitor.GlobalSession.HNC_NetFileGetDirInfo("../plc", fileInfo, ref num, nClientNo);//此函数的返回值永远都是0--成功，即使没有获取到信息,而应该通过paraUI的条数确定是否取到了文件信息。

            if (1 < num)
            {
                //List<string> strfileList = new List<string>();
                string strFileInfo = "";
                foreach (var vItem in fileInfo)
                {
                    //strfileList.Add(vItem.info.name + "   " + vItem.info.size); 
                    strFileInfo += "\n\t\t" + vItem.info.name + "   " + vItem.info.size;
                }
                //File.WriteAllLines(System.AppDomain.CurrentDomain.BaseDirectory + "filelist.txt", strfileList, Encoding.Default);
                Console.Write(strFileInfo + "\n");
            }

            #endregion

            if (0 == Monitor.GlobalSession.HNC_NetFileGet(localFileFullName, "./LNC32.CFG", nClientNo))
            {
                string cfgText = File.ReadAllText(localFileFullName, Encoding.ASCII);
                if ((null != cfgText) && (cfgText.Length > 5))
                {
                    string[] strPlcNode = cfgText.Substring(cfgText.IndexOf("PLCNAME"), 13).Split('=');
                    if ((null != strPlcNode) && (strPlcNode.Length > 1))
                    {
                        PlcName = strPlcNode[1];
                        //    File.Delete(localFileFullName);
                        Console.Write("\n\t\tPLC--" + PlcName + "\n");
                        return true;
                    }
                }
            }
            return false;
        }
        ////^^^^^^^^^^^^^^^^^20150907

        public PLCFileRemoting DownLoadPLCData2WevServer(string MacID, PLCFileRemoting svrPLCInfo)
        {
            PLCFileRemoting plcInfoOnMac = new PLCFileRemoting();
            plcInfoOnMac.bIsAutoDownLoad = false;
            plcInfoOnMac.FileByteSize = 0;
            plcInfoOnMac.nFileStatus = 0;
            plcInfoOnMac.PlcDataName = "";
            plcInfoOnMac.FileByteDatas = null;

            short nClientNo = -1;
            //         int ret = GetHncNetState_OBJ(MacID, out nClientNo);
            string PlcDataName = "";

            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            if ((null == Mac) || (Mac.ClientNO < 0) || (Mac.ClientNO > 255))
            {
                return plcInfoOnMac;
            }
            nClientNo = Mac.ClientNO;
            if (Monitor.GlobalSession.HNC_NetIsConnect(Mac.ClientNO))
            {
                if ((null != svrPLCInfo) && ((null != svrPLCInfo.PlcDataName) && ("" != svrPLCInfo.PlcDataName)))
                {
                    PlcDataName = svrPLCInfo.PlcDataName;
                }
                ////ˇˇˇˇˇˇˇˇˇ20150907 判断从web服务端过来的数据是否为"",
                ///此段代码在原基础上实现直接从数控系统的接口中读出PLC文件的名称，自动搜索下载当前数控系统对应的PLC数据文件，之前的功能依旧保留
                else if ((null != svrPLCInfo) && ("" == svrPLCInfo.PlcDataName))
                {

                    //if (0 != GlobalSession.HNC_ParamanGetStr(HncDataDef.PARAMAN_FILE_NCU, 0, 302, ref PlcDataName, nClientNo))
                    //{
                    //    PlcDataName = "";
                    //}
                    //int nIp4 = 0;
                    //if (0 != GlobalSession.HNC_ParamanGetI32(HncDataDef.PARAMAN_FILE_NCU, 0, 48, ref nIp4, nClientNo))
                    //{
                    //    PlcDataName = "";
                    //}

                    //if (0 != GlobalSession.HNC_ParamanGetItem(HncDataDef.PARAMAN_FILE_NCU, 0, 302, ref PlcDataName, nClientNo))
                    //{
                    //    PlcDataName = "";
                    //}
                    //if (0 != paraGetVal(HncDataDef.PARAMAN_FILE_NCU, 0,"000048",nClientNo,ref PlcDataName))
                    //{
                    //     PlcDataName = "";
                    //}

                    if (!ReadPLCNameFromRomteCFG(MacID, nClientNo, ref PlcDataName))
                    {
                        PlcDataName = "";
                    }
                }
                ////^^^^^^^^^^^^^^^^^20150907 判断从web服务端过来的数据是否为"",
                ///此段代码在原基础上实现直接从数控系统的接口中读出PLC文件的名称，自动搜索下载当前数控系统对应的PLC数据文件，之前的功能依旧保留

                string localFilePath = System.AppDomain.CurrentDomain.BaseDirectory + "PLCs/";
                string localFileFullName = localFilePath + MacID + "__" + PlcDataName + ".DIT";//+DateTime.Now.ToShortTimeString(); //本地接收文件后，文件的保存路径
                string dstPath = "../plc";
                string dstFileName = PlcDataName + ".DIT";
                /////////////////////////////////////////////////////////////////

                ushort num = 0;
                ncfind_t[] fileInfo = new ncfind_t[HncDataDef.MAX_FILE_NUM_PER_DIR];
                int rst = Monitor.GlobalSession.HNC_NetFileGetDirInfo(dstPath, fileInfo, ref num, nClientNo);//此函数的返回值永远都是0--成功，即使没有获取到信息,而应该通过paraUI的条数确定是否取到了文件信息。
                                                                                                     /*
                                                                                                                     if (1 > num)
                                                                                                                     {
                                                                                                                         return plcInfoOnMac;
                                                                                                                     }
                                                                                                     */
                                                                                                     //修改为下面的if else 【Code1】
                #region 【Code1】
                DateTime dstFileCreatTime = new DateTime(0);
                if (1 < num)
                {
                    ncfind_t ncfInfo = fileInfo.Where(t => t.info.name == dstFileName).LastOrDefault();
                    // DateTime dstFileCreatTime = new DateTime(0);
                    if ((0 == ncfInfo.handle) || ("0" == ncfInfo.time) || (null == ncfInfo.info.name) || (0 == ncfInfo.info.size))
                    {
                        if (svrPLCInfo.bIsAutoDownLoad)//文件不存在的时候，如果为真，则查找plc目录下创建日期最新的文件DIT文件
                        {
                            //查找创建日期最新的文件
                            for (int nIndexFile = 0; nIndexFile < num; nIndexFile++)
                            {
                                if ((null == fileInfo[nIndexFile].time) || (0 == fileInfo[nIndexFile].handle) || (null == fileInfo[nIndexFile].info.name) || (0 == fileInfo[nIndexFile].info.size))
                                {
                                    continue;
                                }

                                DateTime datetm = DateTime.ParseExact(fileInfo[nIndexFile].time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                                if ((datetm > dstFileCreatTime) && (fileInfo[nIndexFile].info.name.ToUpper().EndsWith(".DIT")))
                                {
                                    dstFileCreatTime = datetm;
                                    ncfInfo = fileInfo[nIndexFile];
                                }

                            }
                        }
                        else
                        {
                            return plcInfoOnMac;
                        }
                    }
                    else
                    {
                        dstFileCreatTime = DateTime.ParseExact(ncfInfo.time, "yyyy-MM-dd HH:mm", System.Globalization.CultureInfo.CurrentCulture);
                    }

                    PlcDataName = ncfInfo.info.name.ToUpper();
                    localFileFullName = localFilePath + MacID + "__" + PlcDataName;// +".DIT";

                    //判断Web服务器端与数控系统端的PLC文件一致性
                    if ((dstFileCreatTime.Equals(svrPLCInfo.PlcFileCreateTimeInSys)) && (ncfInfo.info.size == svrPLCInfo.FileByteSize))
                    {
                        plcInfoOnMac.nFileStatus = 1;
                        return plcInfoOnMac;
                    }
                    else
                    {
                        //比较数控系统和采集器本地的PLC文件
                        var LocalFile = new FileInfo(localFileFullName);
                        if (LocalFile.Exists)
                        {
                            //如果数控系统上的PLC文件和采集器本地的一致，但是和Web服务器上的不一致，则直接从采集器端下载
                            if (dstFileCreatTime.Equals(LocalFile.CreationTime) && (ncfInfo.info.size == LocalFile.Length))
                            {

                                try
                                {
                                    plcInfoOnMac.FileByteDatas = new byte[LocalFile.Length];

                                    //读取PLC文件
                                    int nReadLen = LocalFile.OpenRead().Read(plcInfoOnMac.FileByteDatas, 0, (int)LocalFile.Length);
                                    if (LocalFile.Length == nReadLen)
                                    {
                                        plcInfoOnMac.nFileStatus = 2;
                                        plcInfoOnMac.PlcDataName = LocalFile.Name;
                                        plcInfoOnMac.PlcFileCreateTimeInSys = LocalFile.CreationTime;
                                        plcInfoOnMac.FileByteSize = LocalFile.Length;
                                    }
                                    else
                                    {
                                        plcInfoOnMac.nFileStatus = 3;
                                        plcInfoOnMac.FileByteDatas = null;
                                    }


                                }
                                catch
                                {
                                    plcInfoOnMac.nFileStatus = 3;
                                    plcInfoOnMac.FileByteDatas = null;
                                }

                                return plcInfoOnMac;

                            }
                        }
                        else
                        {
                            if (!Directory.Exists(localFilePath))//判断文件夹是否存在 
                            {
                                Directory.CreateDirectory(localFilePath);//不存在则创建文件夹 
                            }
                        }
                    }



                    dstFileName = PlcDataName;// +".DIT";
                }
                #endregion

                string dstFileFullName = dstPath + "/" + dstFileName; //待获取的文件，在目标机器的路径名
                if (0 == Monitor.GlobalSession.HNC_NetFileGet(localFileFullName, dstFileFullName, nClientNo))
                {
                    var LocalFile = new FileInfo(localFileFullName);
                    if (num > 1)
                    {
                        LocalFile.CreationTime = dstFileCreatTime;
                    }
                    try
                    {
                        plcInfoOnMac.FileByteDatas = new byte[LocalFile.Length];

                        //读取PLC文件
                        int nReadLen = LocalFile.OpenRead().Read(plcInfoOnMac.FileByteDatas, 0, (int)LocalFile.Length);
                        if (LocalFile.Length == nReadLen)
                        {
                            plcInfoOnMac.nFileStatus = 2;
                            plcInfoOnMac.PlcDataName = LocalFile.Name;
                            plcInfoOnMac.PlcFileCreateTimeInSys = LocalFile.CreationTime;
                            plcInfoOnMac.FileByteSize = LocalFile.Length;
                        }
                        else
                        {
                            plcInfoOnMac.nFileStatus = 3;
                            plcInfoOnMac.FileByteDatas = null;
                        }
                        Console.Write(DateTime.Now.ToLocalTime().ToString() + "\tLocalFile.Name" + LocalFile.Name + "\tLocalFile.Length:" + LocalFile.Length.ToString());

                    }
                    catch
                    {
                        plcInfoOnMac.nFileStatus = 3;
                        plcInfoOnMac.FileByteDatas = null;
                    }

                    return plcInfoOnMac;
                }

            }
            plcInfoOnMac.nFileStatus = -1;//网络联通失败，无法从数控系统读取
            return plcInfoOnMac;
        }

        private readonly string RegTypeString = "XYFGRWDBP";

        private readonly UInt32[] ValType = new UInt32[] { 8, 8, 16, 16, 8, 16, 32, 32, 32 };

        private readonly UInt32[] RegCountMax = new UInt32[] { 511, 511, 1721, 3119, 3119, 399, 199, 99, 199 };

        private int GetTypeID(string strRegType)
        {
            strRegType = strRegType.Trim().ToUpper();
            if (strRegType.Length < 1)
            {
                return -1;
            }

            return RegTypeString.IndexOf(strRegType.ToCharArray()[0]);

        }

        private int CheckRegIndex(int RegTypeID, int nIndex)
        {
            return (RegCountMax[RegTypeID] < (UInt32)nIndex) ? (int)(RegCountMax[RegTypeID]) : nIndex;
        }

        public int? GetRegValueOne(string MacID, string strRegType, int nIndex)
        {
            try
            {
                var Mac = (from a in Monitor.ConnectionList
                           where a.MacID == MacID
                           select a).FirstOrDefault();
                if (Monitor.GlobalSession.HNC_NetIsConnect(Mac.ClientNO))
                {
                    int nRegType = GetTypeID(strRegType);
                    if ((0 > nRegType) || (RegTypeString.Length <= nRegType))
                    {
                        return null;
                    }
                    Byte bVal8 = 0x00;
                    Int16 nVal16 = 0x00;
                    Int32 nVal32 = 0x00;

                    if (8 == ValType[nRegType])
                    {
                        if (0 == Monitor.GlobalSession.HNC_RegGetValue(nRegType, CheckRegIndex(nRegType, nIndex), ref bVal8, Mac.ClientNO))
                        {
                            return bVal8;
                        }

                    }
                    else if (16 == ValType[nRegType])
                    {
                        if (0 == Monitor.GlobalSession.HNC_RegGetValue(nRegType, CheckRegIndex(nRegType, nIndex), ref nVal16, Mac.ClientNO))
                        {
                            return nVal16;
                        }
                    }
                    else if (32 == ValType[nRegType])
                    {
                        if (0 == Monitor.GlobalSession.HNC_RegGetValue(nRegType, CheckRegIndex(nRegType, nIndex), ref nVal32, Mac.ClientNO))
                        {
                            return nVal32;
                        }
                    }
                }
            }
            catch (Exception er)
            {
                Console.Write(er.Message);
                return null;
            }

            return null;
        }

        public List<ToolBase> GetToolBase(string MacID)
        {
            List<ToolBase> ToolBase = new List<ToolBase>();

            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            try
            {

                int ret = 0;
                if (Monitor.GlobalSession.HNC_NetIsConnect(Mac.ClientNO))
                {


                    int ToolPosNum = 0;
                    int ToolPosStart = 0;
                    int MAGZTAB_CUR_TOOL = 0;
                    int ToolBaseN0 = 0;

                    ret = Monitor.GlobalSession.HNC_ToolGetMagBase(1, (int)MagzHeadIndex.MAGZTAB_TOOL_NUM, ref ToolPosNum, Mac.ClientNO); // 单刀库，比如此处为6，则左侧为0~5列



                    for (; ToolPosStart <= ToolPosNum; ToolPosStart++)
                    {
                        var Toolbase = new ToolBase();
                        ret = Monitor.GlobalSession.HNC_ToolGetMagBase(1, (int)MagzHeadIndex.MAGZTAB_CUR_TOOL, ref MAGZTAB_CUR_TOOL, Mac.ClientNO);//第0行显示当前刀
                        ret = Monitor.GlobalSession.HNC_ToolMagGetToolNo(1, ToolPosStart, ref ToolBaseN0, Mac.ClientNO);
                        if (ToolPosStart == 0)
                        {
                            ToolBaseN0 = MAGZTAB_CUR_TOOL;
                        }
                        Toolbase.ToolNO = ToolBaseN0;              //刀号
                        Toolbase.ToolPos = ToolPosStart;  //刀位                    

                        ToolBase.Add(Toolbase);
                    }
                }

                return ToolBase;
            }
            catch (Exception er)
            {
                Console.Write(er.Message);
                return ToolBase;
            }


        }

        public List<int?> GetRegsValuesMore(string MacID, string strRegType, List<int> LstIndex)
        {
            List<int?> lstRegValues = new List<int?>();

            foreach (var vItem in LstIndex)
            {
                lstRegValues.Add(GetRegValueOne(MacID, strRegType, vItem));
            }

            return lstRegValues;

        }

        public string GetSysValue(string sMacID, HncSystem nType)
        {
            var a = (from ct in Monitor.ConnectionList
                     where ct.MacID == sMacID
                     select ct).FirstOrDefault();
            if (null == a)
            {
                return null;
            }
            if (((int)nType >= 11 && (int)nType <= 13) || ((int)nType >= 24 && (int)nType <= 31))
            {
                string sDataTemp = "";
                int nRet = Monitor.GlobalSession.HNC_SystemGetValue((int)nType, ref sDataTemp, a.ClientNO);
                if (0 == nRet)
                    return sDataTemp;
            }
            else
            {
                int sDataTemp = 0;
                int nRet = Monitor.GlobalSession.HNC_SystemGetValue((int)nType, ref sDataTemp, a.ClientNO);
                if (0 == nRet)
                    return sDataTemp.ToString();
            }
            return null;
        }

        public string[] GetSysValue(string sMacID, HncSystem[] naType)
        {
            var a = (from ct in Monitor.ConnectionList
                     where ct.MacID == sMacID
                     select ct).FirstOrDefault();
            if (null == a)
            {
                return null;
            }
            string[] saDataTemp = new string[naType.Length];
            int nArrayIndex = 0;
            foreach (var varType in naType)
            {
                if (((int)varType >= 11 && (int)varType <= 13) || ((int)varType >= 24 && (int)varType <= 31))
                {
                    string sData = "";
                    int nRet = Monitor.GlobalSession.HNC_SystemGetValue((int)varType, ref sData, a.ClientNO);
                    if (0 == nRet)
                        saDataTemp[nArrayIndex++] = sData;
                    else
                        saDataTemp[nArrayIndex++] = "";
                }
                else
                {
                    int nData = 0;
                    int nRet = Monitor.GlobalSession.HNC_SystemGetValue((int)varType, ref nData, a.ClientNO);
                    if (0 == nRet)
                        saDataTemp[nArrayIndex++] = nData.ToString();
                    else
                        saDataTemp[nArrayIndex++] = "";
                }
            }
            return saDataTemp;
        }

        public List<paraDataForDiag> getParaForDiag(string strMacID, List<paraQerTypeForDiag> lstQerPara)
        {
            List<paraDataForDiag> paraTemp = new List<paraDataForDiag>();
            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == strMacID
                       select a).FirstOrDefault();
            if (null == Mac)
            {
                return null;
            }
            foreach (var a in lstQerPara)
            {
                int nRet = -1;
                int rowNum = 0;
                nRet = Monitor.GlobalSession.HNC_ParamanGetSubClassProp(a.m_nBigCls, (Byte)ParaSubClassProp.SUBCLASS_ROWNUM, ref rowNum, Mac.ClientNO);//获取每个小类有多少行参数
                if (rowNum < 0)
                {
                    return null;//子类行数小于0的异常
                }
                nRet = Monitor.GlobalSession.HNC_ParamanRewriteSubClass(a.m_nBigCls, a.m_nSmallCls, Mac.ClientNO);//加载子类，参数分别是大类分类号0-6，子类分类号0-...不同的大类和子类相应的分类参数不同
                if (nRet < 0)
                {
                    return null;//加载子类时出错的异常
                }
                for (int i = 0; i < rowNum; i++)//把每行参数读取出来
                {
                    Int16 dupNum = 0;//参数重复个数
                    Int16 dupNo = 0;//参数重复编号
                    Int32 index = -1;//参数的索引值

                    nRet = Monitor.GlobalSession.HNC_ParamanTransRow2Index(a.m_nBigCls, a.m_nSmallCls, i, ref index, ref dupNum, ref dupNo, Mac.ClientNO);//通过参数类别(大类号)、子类号(小类号)、行号(列号)获取指定参数的索引值
                    if (index < 0)
                    {
                        return null;
                    }

                    Int32 parmID = -1;
                    nRet = Monitor.GlobalSession.HNC_ParamanGetParaProp(a.m_nBigCls, a.m_nSmallCls, index, (Byte)ParaPropType.PARA_PROP_ID, ref parmID, Mac.ClientNO);
                    if (nRet < 0)
                    {
                        return null;
                    }

                    if (parmID.ToString("D6") == a.m_sParaCode.ToString("D6"))
                    {
                        string[] strPar = new string[8];//读出每条参数的暂存数组
                        int myEffectWay = 999;
                        int myDataType = 999;
                        int index1 = -1;

                        int nFlag = GetParContent(a.m_nBigCls, a.m_nSmallCls, i, Mac.ClientNO, strPar, ref myEffectWay, ref myDataType, ref index1);

                        if (nFlag == 0)
                            paraTemp.Add(new paraDataForDiag
                            {
                                m_nBigCls = a.m_nBigCls,
                                m_sParaCode = strPar[0],
                                m_sParaName = strPar[1],
                                m_sDefaultVal = strPar[4],
                                m_sMaxVal = strPar[6],
                                m_sMinVal = strPar[5],
                                m_sNowVal = strPar[2],
                            });
                    }
                }//for
            }//foreach
            return paraTemp;
        }

        //-------------------------------给手机小组接口----------------------start
        public List<string> GetGcodeInfo_ForArd(string MacID) //by zb
        {
            if (MacID == null)
            {
                return null;
            }
            List<string> lstGcode = new List<string>();
            string sGcodeName = null, sGcodeContent = null;
            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            if (Mac != null)
            {
                int ret = -1;
                ret = Monitor.GlobalSession.HNC_FprogGetFullName(0, ref sGcodeName, Mac.ClientNO);//取当前运行G代码名字
                if (sGcodeName.Contains("../prog")) sGcodeName = sGcodeName.Remove(0, 8);
                if (0 == ret)
                {
                    sGcodeContent = GetGcodeContent(MacID, sGcodeName);
                    if (sGcodeName != null && sGcodeContent != null)
                    {
                        lstGcode.Add(sGcodeName);
                        lstGcode.Add(sGcodeContent);
                    }
                }
            }
            if (0 == lstGcode.Count)
            {
                return null;
            }
            return lstGcode;
        }
        public UInt32 GetGcodeRunRow(string MacID)
        {

            if (MacID == null)
            {
                return 0;
            }
            int GcodeRunRow = 0;

            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == MacID
                       select a).FirstOrDefault();
            if (Mac != null)
            {
                if (Monitor.GlobalSession.HNC_NetIsConnect(Mac.ClientNO))
                {
                    Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_RUN_ROW, 0, 0, ref GcodeRunRow, Mac.ClientNO);
                }
            }
            return (UInt32)((GcodeRunRow < 0) ? 0 : GcodeRunRow);
        }
        //-------------------------------给手机小组接口----------------------end

        //------------------------------给创景数据接口----------------------------start
        public SimuData[] GetSimuDataSet(string sMacID)
        {
            var a = (from ct in Monitor.ConnectionList
                     where ct.MacID == sMacID
                     select ct).FirstOrDefault();
            int nSamplChFlagTemp = a.m_bSamplChFlag;
            if (null == a || !(a.Connected))
            {
                return null;//机床不存在或处于未连接状态
            }
            SimuData[] aSimuData = new SimuData[100];
            string[] saAxisName = a.m_sAxisName.Split(',');
            string[] saAxisNoTemp = a.m_sAxisNo.Split(',');
            int[] naAxisNo = new int[saAxisNoTemp.Length];
            for (int i = 0; i < saAxisNoTemp.Length; ++i)
            {
                naAxisNo[i] = Convert.ToInt32(saAxisNoTemp[i]);
            }
            for (int i = 0; i < 100; ++i)
            {
                int ret = -1;
                int nActPosi = 0;
                for (int j = 0; j < naAxisNo.Length; ++j)
                {
                    switch (saAxisName[j])
                    {
                        case "X":
                        case "x":
                            ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS, naAxisNo[j], ref nActPosi, a.ClientNO);//取机床实际位置
                            aSimuData[i].m_sPosi_X = (nActPosi / 100000.0).ToString();
                            break;
                        case "Y":
                        case "y":
                            ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS, naAxisNo[j], ref nActPosi, a.ClientNO);//取机床实际位置
                            aSimuData[i].m_sPosi_Y = (nActPosi / 100000.0).ToString();
                            break;
                        case "Z":
                        case "z":
                            ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS, naAxisNo[j], ref nActPosi, a.ClientNO);//取机床实际位置
                            aSimuData[i].m_sPosi_Z = (nActPosi / 100000.0).ToString();
                            break;
                    }
                }
                int feedOverRide = 0;
                ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_FEED_OVERRIDE, 0, 0, ref feedOverRide, a.ClientNO);//进给修调
                aSimuData[i].m_sFeed = feedOverRide.ToString();
                int spdlOverRide = 0;
                ret = Monitor.GlobalSession.HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_SPDL_OVERRIDE, 0, 0, ref spdlOverRide, a.ClientNO);//主轴倍率
                aSimuData[i].m_sSped = spdlOverRide.ToString();
                double loadCur = 0.0;
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_LOAD_CUR, 5, ref loadCur, a.ClientNO);//负载电流
                double ratedCurrent = 0.0;
                ret = Monitor.GlobalSession.HNC_AxisGetValue((int)HncAxis.HNC_AXIS_RATED_CUR, 5, ref ratedCurrent, a.ClientNO);   //额定电流
                if ((int)ratedCurrent == 0)
                    aSimuData[i].m_sCurtMain = (0).ToString();
                else
                    aSimuData[i].m_sCurtMain = (loadCur / ratedCurrent).ToString();
            }
            return aSimuData;
        }
        //------------------------------给创景数据接口----------------------------end

        public string GetSummaryJson(string MacID)
        {
            var axisList = GetAxisInfo(MacID);
            var axismovement = GetAxisMotion(MacID);
            var channelValue = GetHncChannelValue(MacID);
            //        var state = resService.GetSysValue("", HncSystem.HNC_SYS_ACCESS_LEVEL);
            //                var state =resService.GetSysValue("",HncSystem.HNC_SYS_ACCESS_LEVEL); // .GetNowMachineState(macID);
            //var stateItem =dbnew.MacStateLog.OrderByDescending(t => t.ID).FirstOrDefault(t => t.MacID == macID);
            //var workMode = dbnew.WorkModeDic.Where(t => t.WorkMode == stateItem.WorkMode).FirstOrDefault();

            List<string> Gmodelist = GetGmode(MacID);
            double actSpdlSpeed = Math.Floor(axismovement.actSpdlSpeed);  //主轴实际速度
            string spdlState;
            string KeepTimeStr = "";
            if (actSpdlSpeed == 0)
            {
                spdlState = "停止";
            }
            else
            {
                spdlState = "转动";
            }
            //if (channelValue.GcodeName != gCodeName2)
            //{
            //    var lastRunEnd = dbnew.MacStateLog.Where(t => t.MacID == macID && t.State == 5 && t.GcodeName == channelValue.GcodeName).OrderByDescending(t => t.ID).FirstOrDefault();
            //    if (axismovement.actFeedSpeed == 0 && axismovement.actSpdlSpeed == 0) //判断暂停了
            //    {
            //        KeepTimeStr = StopTime;

            //    }
            //    else
            //    {
            //        var lastRunStart = dbnew.MacStateLog.Where(t => t.MacID == macID && t.State == 1 && t.GcodeName == channelValue.GcodeName).OrderByDescending(t => t.ID).FirstOrDefault();
            //        progKeepTime = DateTime.Now - lastRunStart.CreateTime;
            //        KeepTimeStr = progKeepTime.Hours + "时" + progKeepTime.Minutes + "分" + progKeepTime.Seconds + "秒";

            //    }


            //}
            //  StopTime = KeepTimeStr;
            //  gCodeName2 = channelValue.GcodeName;
            Dictionary<int, string> toolBase = new Dictionary<int, string>();
            toolBase.Add(0, "平刀");
            toolBase.Add(1, "粗加工刀");
            toolBase.Add(2, "精加工刀");
            toolBase.Add(3, "切槽刀");
            toolBase.Add(4, "丝锥");
            toolBase.Add(5, "铣刀");
            toolBase.Add(6, "圆弧刀");
            toolBase.Add(7, "钻头");
            toolBase.Add(8, "钻头");
            var toolType = from a in toolBase where a.Key == channelValue.ToolType select a.Value;
            var toolImg = from a in toolBase where a.Key == channelValue.ToolType select a.Value.ToString() + ".png";
            int[] regs = new int[] { 480, 482, 483 };
            var regValues = GetRegsValuesMore(MacID, "Y", regs.ToList());
            //int? Regvalue480 =  resService.GetRegValueOne(macID, "Y", 480);
            //int? Regvalue483 =resService.GetRegValueOne(macID, "Y", 483);
            // int? Regvalue482 =0; //resService.GetRegValueOne(macID, "Y", 482);
            Dictionary<string, int?> RegvalueArray = new Dictionary<string, int?>();
            for (int i = 0; i < regs.Length; i++)
            {
                RegvalueArray.Add(regs[i].ToString(), regValues[i]);

            }
            var Regvaluelist = RegvalueArray.ToArray();

            var obj = new
            {
                axisList,
                actSpdlSpeed = Math.Floor(axismovement.actSpdlSpeed).ToString(),  //主轴实际速度
                cmdSpdlSpeed = Math.Floor(axismovement.cmdSpdlSpeed).ToString(),  //主轴指令速度
                actFeedSpeed = axismovement.actFeedSpeed.ToString("0.00"),      //进给实际速度
                cmdFeedSpeed = axismovement.cmdFeedSpeed.ToString("0.00"),     //进给指令速度
                spdlOvrd = axismovement.spdlOvrd,                            //主轴倍率
                feedOvrd = axismovement.feedOvrd,                             //进给倍率
                rapidOvrd = axismovement.rapidOvrd,                             //快移倍率
                //  toolNO = channelValue.toolNO,
                toolUse = channelValue.cuttingToolUse.ToString(),
                toolReady = channelValue.ToolReady.ToString(),
                state = 1, //stateItem.State, 
                DescText = "运行",//stateItem.DescText,
                WorkModeDes = "自动",//workMode.WorkModeDes,
                toolType,
                toolImg,
                GcodeName = channelValue.GcodeName,
                GcodeRunRow = channelValue.GcodeRunRow,
                workpieceNumNow = channelValue.Cntr,  //加工计数
                workpieceNumTotal = channelValue.Stati, //工件总数
                spdlloar = 100 * axismovement.spdlLoad,
                cycle = channelValue.Cycle,   //循环启动
                hold = channelValue.Hold,     //进给保持
                // state,                      //当前状态
                Gmodelist,
                spdlState,
                KeepTimeStr,
                Regvaluelist
            };

            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);

        }

        public bool SendSampXmlData2NC(string sMacID, string sGCodeName, DateTime dtXmlFileName, HncDataInterfaces.HealthIndex cHealthDat)
        {
            if (!CreateXmlFile(sGCodeName, dtXmlFileName, cHealthDat.SpdlHealthIndex.ToString(), cHealthDat.XaxisHealthIndex.ToString(), cHealthDat.YaxisHealthIndex.ToString(), cHealthDat.ZaxisHealthIndex.ToString(), cHealthDat.MacHealthIndex.ToString()))
                return false;
            string sXmlFileName = sGCodeName + "_" + dtXmlFileName.ToString("yyyyMMddHHmm") + "_HealthIndex.xml";
            var Mac = (from a in Monitor.ConnectionList
                       where a.MacID == sMacID
                       select a).FirstOrDefault();
            string dir = "../data";
            string dirName = dir + "/" + sXmlFileName; //目标机器接收文件后，储存的完整文件路径名
            int ret = Monitor.GlobalSession.HNC_NetFileSend("./" + sXmlFileName, dirName, Mac.ClientNO);
            if (ret == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool CreateXmlFile(string sGcodeName, DateTime dtXmlFileName, string SpdlHealthIndex, string XaxisHealthIndex, string YaxisHealthIndex, string ZaxisHealthIndex, string MacHealthIndex)
        {
            string sXmlFileName = sGcodeName + "_" + dtXmlFileName.ToString("yyyyMMddHHmm") + "_HealthIndex.xml";

            HealthIndex2NC HealthIndexTemp = new HealthIndex2NC();
            HealthIndexTemp.GcodeName = sGcodeName;
            HealthIndexTemp.MacHealthIndex = MacHealthIndex;
            HealthIndexTemp.SampleTime = dtXmlFileName.ToString("yyyy-MM-dd HH:mm:ss");
            HealthIndexTemp.SpdlHealthIndex = SpdlHealthIndex;
            HealthIndexTemp.XaxisHealthIndex = XaxisHealthIndex;
            HealthIndexTemp.YaxisHealthIndex = YaxisHealthIndex;
            HealthIndexTemp.ZaxisHealthIndex = ZaxisHealthIndex;


            XmlSerializerNamespaces ns = new XmlSerializerNamespaces();
            ns.Add("", "");
            XmlSerializer serializer = new XmlSerializer(typeof(HealthIndex2NC));
            string xmlstring = "";
            try
            {
                MemoryStream ms = new MemoryStream();
                XmlTextWriter xmlWriter = new XmlTextWriter(ms, Encoding.UTF8);

                serializer.Serialize(xmlWriter, HealthIndexTemp, ns);
                xmlWriter.Close();
                xmlstring = Encoding.UTF8.GetString(ms.ToArray());
                //xmlstring = xmlstring.Replace("\r\n", "\n");

                FileStream f = new FileStream(sXmlFileName, FileMode.Create);
                StreamWriter sw = new StreamWriter(f, Encoding.GetEncoding("UTF-8"));
                //serializer.Serialize(sw, HealthIndexTemp, ns);
                sw.Write(xmlstring);
                sw.Close();
                f.Close();
            }
            catch
            {
                return false;
            }
            return true;
        }

        public CRTDataForWeb GetSummaryData(string MacID)
        {
            CRTDataForWeb cRTDataForWeb = new CRTDataForWeb();

            cRTDataForWeb.axisList = GetAxisInfo(MacID);
            cRTDataForWeb.axismovement = GetAxisMotion(MacID);
            cRTDataForWeb.channelValue = GetHncChannelValue(MacID);
            var varMac = Monitor.ConnectionList.Where(t => t.MacID == MacID).FirstOrDefault();
            if (null != varMac)
            {
                cRTDataForWeb.state = varMac.m_nMachineState;
                cRTDataForWeb.DescText = (cRTDataForWeb.state == 0 || cRTDataForWeb.state == 2) ? "空闲" : ((cRTDataForWeb.state == 1) ? "运行" : ((cRTDataForWeb.state == 3) ? "报警" : "离线"));
            }

            //        var state = resService.GetSysValue("", HncSystem.HNC_SYS_ACCESS_LEVEL);
            //                var state =resService.GetSysValue("",HncSystem.HNC_SYS_ACCESS_LEVEL); // .GetNowMachineState(macID);
            //var stateItem =dbnew.MacStateLog.OrderByDescending(t => t.ID).FirstOrDefault(t => t.MacID == macID);
            //var workMode = dbnew.WorkModeDic.Where(t => t.WorkMode == stateItem.WorkMode).FirstOrDefault();
            cRTDataForWeb.Gmodelist = GetGmode(MacID);

            //string spdlState;
            string KeepTimeStr = "";
            //if (actSpdlSpeed == 0)
            //{
            //    spdlState = "停止";
            //}
            //else
            //{
            //    spdlState = "转动";
            //}
            //if (channelValue.GcodeName != gCodeName2)
            //{
            //    var lastRunEnd = dbnew.MacStateLog.Where(t => t.MacID == macID && t.State == 5 && t.GcodeName == channelValue.GcodeName).OrderByDescending(t => t.ID).FirstOrDefault();
            //    if (axismovement.actFeedSpeed == 0 && axismovement.actSpdlSpeed == 0) //判断暂停了
            //    {
            //        KeepTimeStr = StopTime;

            //    }
            //    else
            //    {
            //        var lastRunStart = dbnew.MacStateLog.Where(t => t.MacID == macID && t.State == 1 && t.GcodeName == channelValue.GcodeName).OrderByDescending(t => t.ID).FirstOrDefault();
            //        progKeepTime = DateTime.Now - lastRunStart.CreateTime;
            //        KeepTimeStr = progKeepTime.Hours + "时" + progKeepTime.Minutes + "分" + progKeepTime.Seconds + "秒";

            //    }


            //}
            //  StopTime = KeepTimeStr;
            //  gCodeName2 = channelValue.GcodeName;
            Dictionary<int, string> toolBase = new Dictionary<int, string>();
            toolBase.Add(0, "平刀");
            toolBase.Add(1, "粗加工刀");
            toolBase.Add(2, "精加工刀");
            toolBase.Add(3, "切槽刀");
            toolBase.Add(4, "丝锥");
            toolBase.Add(5, "铣刀");
            toolBase.Add(6, "圆弧刀");
            toolBase.Add(7, "钻头");
            toolBase.Add(8, "钻头");
            //var toolType = from a in toolBase where a.Key == channelValue.ToolType select a.Value;
            //var toolImg = from a in toolBase where a.Key == channelValue.ToolType select a.Value.ToString() + ".png";
            int[] regs = new int[] { 480, 482, 483 };
            var regValues = GetRegsValuesMore(MacID, "Y", regs.ToList());
            //int? Regvalue480 =  resService.GetRegValueOne(macID, "Y", 480);
            //int? Regvalue483 =resService.GetRegValueOne(macID, "Y", 483);
            // int? Regvalue482 =0; //resService.GetRegValueOne(macID, "Y", 482);
            Dictionary<string, int?> RegvalueArray = new Dictionary<string, int?>();
            for (int i = 0; i < regs.Length; i++)
            {
                RegvalueArray.Add(regs[i].ToString(), regValues[i]);

            }
            // var Regvaluelist = RegvalueArray.ToArray();


            //cRTDataForWeb.toolType = toolType;
            //cRTDataForWeb.toolImg = toolImg;
            //cRTDataForWeb.Gmodelist = Gmodelist;
            //cRTDataForWeb.spdlState = spdlState;
            cRTDataForWeb.KeepTimeStr = KeepTimeStr;
            cRTDataForWeb.RegvalueArray = RegvalueArray;
            return cRTDataForWeb;
        }

        public GcodeDataForWeb GetGcodeData(string MacID)
        {
            GcodeDataForWeb gcodeDataForWeb = new GcodeDataForWeb();


            gcodeDataForWeb.GcodeName = GetHncChannelValue(MacID).GcodeName;
            gcodeDataForWeb.GcodeRunNow = GetHncChannelValue(MacID).GcodeRunRow;
            gcodeDataForWeb.Gmodelist = GetGmode(MacID);
            return gcodeDataForWeb;
        }

        [Serializable]
        public class HncData : MarshalByRefObject, IHncData
        {
            public String FirstName, LastName, Body, Title;
            public String GetformattedResume()
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                sb.Append("*" + GetformattedName() + "*\n");
                sb.Append("--" + Title + "--\n");
                sb.Append("--------------------------\n");
                sb.Append(Body);
                return sb.ToString();
            }
            public String GetformattedName()
            {
                return FirstName + " " + LastName;
            }
        }//END OF Resume Object

        [XmlRoot("HealthIndex")]
        public class HealthIndex2NC
        {
            [XmlAttribute("SampleTime")]
            public string SampleTime { get; set; }

            [XmlAttribute("GcodeName")]
            public string GcodeName { get; set; }

            [XmlAttribute("SpdlHealthIndex")]
            public string SpdlHealthIndex { get; set; }

            [XmlAttribute("XaxisHealthIndex")]
            public string XaxisHealthIndex { get; set; }

            [XmlAttribute("YaxisHealthIndex")]
            public string YaxisHealthIndex { get; set; }

            [XmlAttribute("ZaxisHealthIndex")]
            public string ZaxisHealthIndex { get; set; }

            [XmlAttribute("MacHealthIndex")]
            public string MacHealthIndex { get; set; }

        }
    }
}
