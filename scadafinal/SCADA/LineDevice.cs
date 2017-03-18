using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net;
using System.Collections;
using System.Text.RegularExpressions;
using ScadaHncData;
using HNCAPI;
using Collector;

namespace LineDevice
{
    #region//HNC系统网络初始化及退出

    #region //CNC机床类定义
    public class CNC:IDisposable
    {
        public HNCData HCNCShareData;//数据采集模块的共享内存

        public UInt16 NC8_chang = 0;//通道
        public UInt16 serial;//机床serial
        public String roomNo = "";//车间号
        public String lineNo = "";//产线号
        public String BujianID = "";//部件ID
        public String JiTaiHao = "";//机台号
        public String OP_CODE = "";//机台能加工的工序编码
        public String remark;//CNC附加配置信息

        public String ip = "";//IP地址
        public UInt16 port = 0;//IP端口      

        public Int32 productedNum= 0;//已生产数量
        public String ncName="N/A";//运行或加载的G代码名称

        public String plcVer = "";//plc版本
        public String drvVer = "";//驱动版本
        public String cncVer = "";//cnc版本
        public String nckVer = "";//NCK版本

        public int alarm_num = 0;//当前报警数
        public alarm alarmtmp;//用于获取报警信息        
        public ArrayList alarmdata = new ArrayList();//当前报警内容
        public struct alarm
        {//报警data由报警号和报警内容组成
            public int alarm_ID;
            public String alarmID;
            public String alarmText;
        }

        public int alarm_history_num = 0;//历史报警数
        public ArrayList alarmhistorydata = new ArrayList();//历史报警内容  
        public alarm_history alarm_historytmp;
        private AlarmHisData[] alarmhistoryData = new AlarmHisData[HNCALARM.ALARM_HISTORY_MAX_NUM];  
        public struct alarm_history
        {//报警data由报警号和报警内容组成
            public String alarmID;
            public String alarmText;
            public String timeBegin;
            public String timeEnd;
        }

        public struct SetCMDStruct
        {
            public SetCMDStruct(Int32 SetValue = 0)
            {
                this.SignalList1 = -1;
                this.SignalList2 = -1;
                this.SetValue = SetValue;
            }
            public Int32 SignalList1;
            public Int32 SignalList2;
            public Int32 SetValue;//需要设置的值
        }
        public SetCMDStruct SetCNCCMDRe;//CNC切入切出控制变量

        /// <summary>
        /// 控制模式：复位、自动、手动、单段、增量、手摇、回零、PMC、未连接
        /// </summary>
        public enum ControlMode
        {            
            CHAN_MODE_RESET=0,		    //	复位
            CHAN_MODE_AUTO,				//	自动
            CHAN_MODE_JOG,				//	手动
            CHAN_MODE_STEP,				//	增量
            CHAN_MODE_MPG,				//	手摇
            CHAN_MODE_HOME,				//	回零
            CHAN_MODE_PMC,				//	PMC
            CHAN_MODE_SBL,			    //	手动
            DISCONNECT,
            UNKNOW
        }
        
        public enum RunningState//运行状态：停止、运行、暂停
        {
            MDI, PROGSEL, RUNNING, PAUSE, STOP, ESTOP, RESETTING, HOMING, REWINDED, DISCONNECT
        }

        public enum CNCState
        {
            DISCON,//离线
            IDLE,//空闲
            ALARM,//报警
            RUNING,//运行
        }

        //--------------------------------------------------------------------------------------------------
        public String[] reportChanPingXuLieHao = new String[2];
        public MES_DISPATCH mesDispatch = new MES_DISPATCH();  //2015.11.30 为报工单
        public bool OneInOneOut = true;
        public SCADA.NCTaskDel NcTaskManage;

        public Int16 InitCNCParam(String serial, String roomNo, String lineNo, String ip, String port, String OP_CODE,
            String BujianID, String remark, String NCTaskFileName)
        {
            NcTaskManage = new SCADA.NCTaskDel(NCTaskFileName, BujianID);
            this.serial = ushort.Parse(serial);
            this.roomNo = roomNo;
            this.lineNo = lineNo;
            this.ip = ip;
            this.port = ushort.Parse(port);
            this.OP_CODE = OP_CODE;
            this.BujianID = BujianID;
            if (BujianID.Length > 3)
            {
                JiTaiHao = BujianID.Substring(BujianID.Length - 3, 3);
            }
            else
            {
                JiTaiHao = BujianID;
            }
            this.remark = remark;
            SetCNCCMDRe = new SetCMDStruct();

            System.Data.DataRow node;
            NcTaskManage.GetDISPATCHSendOrRepor(false, out node);
            if (node != null && NcTaskManage.GetTaskDbName() == "1")
            {
                SetCNCCMDRe.SetValue = 1;
            }

            reportChanPingXuLieHao[0] = null;
            reportChanPingXuLieHao[1] = null;
            CNCchanDataEventHandler = new EventHandler<int[]>(this.CNCchanDataEventHandlerFuc);
            return 0;
        }

        public enum CNCCMDRegValueType//PLC中CNC切入切出值
        {
            IN = 0,
            OUT
        }
        public void CNCCMDChange(String CMD)
        {
            if (CMD == ((int)LineDevice.CNC.CNCCMDRegValueType.OUT).ToString())//停机
            {
                if (SetCNCCMDRe.SetValue != (int)CNCCMDRegValueType.OUT)
                {
                    SetCNCCMDRe.SetValue = (int)CNCCMDRegValueType.OUT;
                    NcTaskManage.SetTaskDbName(CMD);
                }
            }
            else if (CMD == ((int)LineDevice.CNC.CNCCMDRegValueType.OUT).ToString())//开机
            {
                if (SetCNCCMDRe.SetValue != (int)CNCCMDRegValueType.OUT)
                {
                    System.Data.DataRow node;
                    NcTaskManage.GetDISPATCHSendOrRepor(false, out node);
                    if (node != null)
                    {
                        SetCNCCMDRe.SetValue = (int)CNCCMDRegValueType.IN;
                    }
                    NcTaskManage.SetTaskDbName(CMD);
                }
            }
        }

        /// <summary>
        /// 获取CNC当前状态        
        /// </summary>
        /// <returns>当前状态</returns>
        int old_CNCState = -100;
        public bool Checkcnc_state(ref CNCState CNCStatei)
        {
            CNCState state ;
            if (HCNCShareData != null && HCNCShareData.sysData != null && HCNCShareData.sysData.isConnect)
            {
                state = CNCState.IDLE;
                if (HCNCShareData != null && HCNCShareData.chanDataLst.Count > 0)
                {
                    if (HCNCShareData.chanDataLst[NC8_chang].isEstop == 1)
                    {
                        state = CNCState.ALARM;
                    }
                    else if (HCNCShareData.chanDataLst[NC8_chang].isCycling == 1)
                    {
                        state = CNCState.RUNING;
                    }
                }
            }
            else
            {
                state = CNCState.DISCON;
            }
            CNCStatei = state;
            if((int)state != old_CNCState)
            {
                old_CNCState = (int)state;
                return true;
            }
            else 
            {
                return false;
            }
        }


       /// <summary>
       /// 获取系统加载的G代码名称        
       /// </summary>
       /// <returns>G代码名称</returns>
        public String get_ncName()
        {
            
            if (isConnected())
            {
                ncName = HCNCShareData.chanDataLst[NC8_chang].gCodeName;
            }
            return ncName;
        }


        /// <summary>
        /// 获取系统寄存器的个数      
        /// </summary>
        /// <returns>寄存器个数值</returns>
        public int get_reg_num(Int32 type)
        {
            int val = 0;
            switch (type)
            {
                case (int)HncRegType.REG_TYPE_X:
                    val = 512;
                    break;
                case (int)HncRegType.REG_TYPE_Y:
                    val = 512;
                    break;
                case (int)HncRegType.REG_TYPE_F:
                    val = 3120;
                    break;
                case (int)HncRegType.REG_TYPE_G:
                    val = 3120;
                    break;
                case (int)HncRegType.REG_TYPE_R:
                    val = 2288;
                    break;
                case (int)HncRegType.REG_TYPE_B:
                    val = 1722;
                    break;
            }
            return val;
        }


        /// <summary>
        /// 获取系统寄存器的值      
        /// </summary>
        /// <returns></returns>
//         public int get_reg_val(Int32 type, Int32 index, ref Int32 value)
//         {
//             int ret = -1;
//             if (isConnected())
//             {
// //                 ret = CollectShare.Instance().HNC_RegGetValue(type, index, ref value, HCNCShareData.sysData.clientNo);
//                 switch (type)
//                 {
//                     case (int)HncRegType.REG_TYPE_X:
//                         value = xreg_val_Arr.reg_val_arr[index];
//                         break;
//                     case (int)HncRegType.REG_TYPE_Y:
//                         value = yreg_val_Arr.reg_val_arr[index];
//                         break;
//                     case (int)HncRegType.REG_TYPE_F:
//                         value = freg_val_Arr.reg_val_arr[index];
//                         break;
//                     case (int)HncRegType.REG_TYPE_G:
//                         value = greg_val_Arr.reg_val_arr[index];
//                         break;
//                     case (int)HncRegType.REG_TYPE_R:
//                         value = rreg_val_Arr.reg_val_arr[index];
//                         break;
//                     case (int)HncRegType.REG_TYPE_B:
//                         value = breg_val_Arr.reg_val_arr[index];
//                         break;
//                 }
//             }
//             return ret;
//         }

         /**
	     * 获取本次生产数量
	     */	
	    public Int32 getCurrentProductedNum() {
            if (isConnected())
            {
//                 int ret = CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_PART_CNTR, ch, 0, ref productedNum, HCNCShareData.sysData.clientNo);
// 		        if(ret ==0){
//                     return productedNum;
//                 }
                return HCNCShareData.chanDataLst[NC8_chang].partNum;
            }
		    return -1;
	    }


        /**
        * 获取系统长度分辨率
        */
        public int get_sys_move_unit()
        {
            int value = -1;
            if (isConnected())
            {
                return HCNCShareData.sysData.moveUnit;

            }
            return value;
        }

        /**
        * 获取系统角度分辨率
        */
        public int get_sys_turn_unit()
        {
            int value = -1;
            if (isConnected())
            {
                return HCNCShareData.sysData.turnUnit;
            }
            return value;
        }

         /**
	     * 获取PLC版本
	     */
	
	    public String getPlcVer() {
            if (isConnected())
            {
                CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_PLC_VER, ref plcVer, HCNCShareData.sysData.clientNo);
                return plcVer;
		    }
            return "";
	    }

	    /**
	     * 获取DRV版本
	     */
	
	    public String getDrvVer() {
            if (isConnected())
            {
                CollectShare.Instance().HNC_SystemGetValue((int)HncSystem.HNC_SYS_DRV_VER, ref drvVer, HCNCShareData.sysData.clientNo);
                return drvVer;
            }
            return "";
	    }

	    /**
	     * 获取CNC版本
	     */
	
	    public String getCncVer() 
        {
            return HCNCShareData.sysData.sysver.ncu;
	    }

	    /**
	     * 获取NCK版本
	     */
	
	    public String getNckVer() 
        {
            return HCNCShareData.sysData.sysver.ncu;
	    }

        /**
	     * 从上位机选择加载G代码
	     */	
	    public int sendGCode(String gCodeFilename) {
            int ret = -1;
            if (isConnected())
            {
                //二次开发接口没有对gCodeFilename对象进行验证，如果是gCodeFilename为null，二次开发接口库崩溃 （20151114）
//                 ret = CollectShare.Instance().HNC_SysCtrlLoadProg(ch, gCodeFilename, HCNCShareData.sysData.clientNo);
		    }
            return ret;
	    }

        /**
	     * 从上位机发送文件到CNC
	     */
        public int sendFile(String localName,String dstName)
        {
            int ret = -4;
            if (isConnected())
            {
                ret = CollectShare.Instance().HNC_NetFileSend(localName, dstName, HCNCShareData.sysData.clientNo);
            }
            return ret;
        }

        /// <summary>
        /// 发送NC代码并选择为当前代码
        /// </summary>
        /// <param name="localName"></param>
        /// <param name="dstName"></param>
        /// <param name="ch"></param>
        /// <param name="progSelect"></param>
        /// <returns></returns>
        public int sendFile(String localName, String dstName, int ch = 0, bool progSelect = false)  //2015.10.26
        {
            int ret = -4;
            if (isConnected())
            {
                ret = CollectShare.Instance().HNC_NetFileSend(localName, dstName, HCNCShareData.sysData.clientNo);
                if (ret == 0 && progSelect == true)
                {
                    ret = CollectShare.Instance().HNC_SysCtrlSelectProg(ch, dstName, HCNCShareData.sysData.clientNo);
                }
            }
            return ret;
//             return 0;
        }
        /**
	     * 获取目标机器文件
	     */
        public int netFileGet(String localName, String dstName)
        {
            int ret = -4;
            if (isConnected())
            {
                ret = CollectShare.Instance().HNC_NetFileGet(localName, dstName, HCNCShareData.sysData.clientNo);
            }
            return ret;
        }


        /**
	     * 删除下位机文件
	     */
        public int netFileRemove(String dstName)
        {
            int ret = -4;
            if (isConnected())
            {
                ret = CollectShare.Instance().HNC_NetFileRemove(dstName, HCNCShareData.sysData.clientNo);
            }
            return ret;
        }

        /**
	     * 比较上、下位机文件是否一致
	     */
        public int netFileCheck(String localName, String dstName)
        {
            int ret = -4;
            if (isConnected())
            {
                ret = CollectShare.Instance().HNC_NetFileCheck(localName, dstName, HCNCShareData.sysData.clientNo);
            }
            return ret;
        }


        /**
	     * 获取系统当前运行模式，自动、手动、单段、增量等
         * UNKNOW, AUTORUN, STEP_OVER, INCHING, INCREMENT, DISCONNECT
	     */
        public ControlMode getControlMode() {
            if (!isConnected())
            {
			    return ControlMode.DISCONNECT;
		    }
            int mode = -1;
//             CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_MODE, ch, 0, ref mode, HCNCShareData.sysData.clientNo);
            mode = HCNCShareData.chanDataLst[NC8_chang].workMode;
            switch (mode)
            {
                case -1:
                    return ControlMode.UNKNOW;
                case 0:
                    return ControlMode.CHAN_MODE_RESET; 
                case 1:
                    return ControlMode.CHAN_MODE_AUTO; 
                case 2:
                    return ControlMode.CHAN_MODE_JOG;
                case 3:
                    return ControlMode.CHAN_MODE_STEP;
                case 4:
                    return ControlMode.CHAN_MODE_MPG;
                case 5:
                    return ControlMode.CHAN_MODE_HOME;
                case 6:
                    return ControlMode.CHAN_MODE_PMC;
                case 7:
                    return ControlMode.CHAN_MODE_SBL;

            }
		    return ControlMode.UNKNOW;
	    }




        /**
        * 获取系统当前运行状态：
        * RunningState//运行状态：
        * MDI(0), PROGSEL(1), RUNNING(2), PAUSE(3), STOP(4), ESTOP(5), RESETTING(6), HOMING(7), REWINDED(8),DISCONNECT(9)
        */
        public int getRunningState()
        {
            int  val = 0;
            if (HCNCShareData != null)
            {
                val |= 0x0200;
            }
            else
            {
                if (HCNCShareData.chanDataLst[NC8_chang].isMdi == 1)
                {
                    val |= 0x0001;
                }
                if (HCNCShareData.chanDataLst[NC8_chang].isProgSel == 1)
                {
                    val |= 0x0002;
                }
                if (HCNCShareData.chanDataLst[NC8_chang].isCycling == 1)
                {
                    val |= 0x0004;
                }
                if (HCNCShareData.chanDataLst[NC8_chang].isHolding == 1)
                {
                    val |= 0x0008;
                }
                if (HCNCShareData.chanDataLst[NC8_chang].isEstop == 1)
                {
                    val |= 0x0008;
                }
                if (HCNCShareData.chanDataLst[NC8_chang].isReseting == 1)
                {
                    val |= 0x0010;
                }
                if (HCNCShareData.chanDataLst[NC8_chang].isHoming == 1)
                {
                    val |= 0x0020;
                }
                if (HCNCShareData.chanDataLst[NC8_chang].isRewinded == 1)
                {
                    val |= 0x0080;
                }
            }
            return val;            
        }


        /**
         * 判断网络是否连接
         */
        public bool isConnected()
        {
            if (HCNCShareData != null && HCNCShareData.sysData != null)
            {
                return HCNCShareData.sysData.isConnect;
            }
            else
            {
                return false;
            }
        }


        

        #region 获取通道数据
        //获取加工件数
        public int get_partNum()
        {
            int value = 0;
            if (HCNCShareData.chanDataLst != null && HCNCShareData.chanDataLst.Count > NC8_chang)
            {
                value = HCNCShareData.chanDataLst[NC8_chang].partNum;
            }
            return value;
        }


        public int get_toolUse()
        {
            int value = 0;
            if (HCNCShareData.chanDataLst != null && HCNCShareData.chanDataLst.Count > NC8_chang)
            {
                value = HCNCShareData.chanDataLst[NC8_chang].toolUse;
            }
            return value;
        }

        public String get_gCodeName()
        {
            String value = "";
            if (HCNCShareData != null && HCNCShareData.chanDataLst != null && HCNCShareData.chanDataLst.Count > NC8_chang)
            {
                value = HCNCShareData.chanDataLst[NC8_chang].gCodeName;
            }
            return value;
        }

        /**
	     * 获取系统进给修调值
	     */
        public int get_feed_override()
        {
            int value = 0;

            if (HCNCShareData.chanDataLst != null && HCNCShareData.chanDataLst.Count > NC8_chang)
            {
//                 CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_FEED_OVERRIDE, ch, 0, ref feed_override, clientNo);
//                 return feed_override;
                value = HCNCShareData.chanDataLst[NC8_chang].feedOverride;
		    }
            return value;
	    }

        /**
	     * 获取系统快移修调值
	     */
        public int get_rapid_override()
        {
            int value = 0;
            if (HCNCShareData.chanDataLst != null && HCNCShareData.chanDataLst.Count > NC8_chang)
            {
//                 CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_RAPID_OVERRIDE, ch, 0, ref rapid_override, clientNo);
//                 return rapid_override;
                value = HCNCShareData.chanDataLst[NC8_chang].rapidOverride;
            }
            return value;
        }

        /**
	     * 获取主轴修调值
	     */
        public int get_spdl_override()
        {
            int value = 0;
            if (HCNCShareData.chanDataLst != null && HCNCShareData.chanDataLst.Count > NC8_chang)
            {
//                 CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_SPDL_OVERRIDE, ch, 0, ref spdl_override, HCNCShareData.sysData.clientNo);
//                 return spdl_override;
                value = HCNCShareData.chanDataLst[NC8_chang].spindleOverride;
            }
            return value;
        }


        /**
	     * 获取系统指令进给速度
	     */
        public double get_cmd_feedrate()
        {
            double value = 0;
            if (HCNCShareData.chanDataLst != null && HCNCShareData.chanDataLst.Count > NC8_chang)
            {
                value = HCNCShareData.chanDataLst[NC8_chang].cmdFeedrate;
            }
            return value;
        }

        /**
        * 获取系统实际进给速度
        */
        public double get_act_feedrate()
        {
            double value = 0;
            if (HCNCShareData.chanDataLst != null && HCNCShareData.chanDataLst.Count > NC8_chang)
            {
//                 CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_ACT_FEEDRATE, ch, 0, ref act_feedrate, HCNCShareData.sysData.clientNo);
//                 return act_feedrate;
                value = HCNCShareData.chanDataLst[NC8_chang].actFeedrate;
            }
            return value;
        }

        /**
        * 获取系统主轴实际进给速度
        */
        public double get_act_spdl_speed()
        {
            double value = 0;
            if (HCNCShareData.chanDataLst != null && HCNCShareData.chanDataLst.Count > NC8_chang)
            {
//                  CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_ACT_SPDL_SPEED, ch, 0, ref act_spdl_speed, HCNCShareData.sysData.clientNo);
//                 return act_spdl_speed;
                value = HCNCShareData.chanDataLst[NC8_chang].spdlSpeed;
            }
            return value;
        }

        /**
         * 获取系统当前运行行号
         */
        public int get_run_row()
        {
            int value = 0;

            if (HCNCShareData.chanDataLst != null && HCNCShareData.chanDataLst.Count > NC8_chang)
            {
//                 CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_RUN_ROW, ch, 0, ref run_row, HCNCShareData.sysData.clientNo);
//                 return run_row;
                value = HCNCShareData.chanDataLst[NC8_chang].runLine;
            }
            return value;
        }

        /**
       * 获取系统当前使用刀具
       */
        public int get_tool_use()
        {
            int value = 0;
            if (HCNCShareData.chanDataLst != null && HCNCShareData.chanDataLst.Count > NC8_chang)
            {
//                 CollectShare.Instance().HNC_ChannelGetValue((int)HncChannel.HNC_CHAN_TOOL_USE, ch, 0, ref tool_use, HCNCShareData.sysData.clientNo);
//                 return tool_use;
                value = HCNCShareData.chanDataLst[NC8_chang].toolUse;
            }
            return value;
        }
#endregion 


        #region 获取轴相关数据


        /**
         * 获取轴名称
         */
        public String get_axis_name(int ax)
        {
            String name = "";
//             if (isConnected())
            {
//                 int ret = CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_NAME, ax, ref name, HCNCShareData.sysData.clientNo);
//                 if (ret == 0)
//                 {
//                     return name;
//                 } 
                AxisData Axis = HCNCShareData.axlist.Find(
                delegate(AxisData temp)
                {
                    return (temp.axisNo == ax);
                }
                );
                if (Axis != null)
                {
                    name = Axis.axisName;
                }
            }
            return name;
        }

        /**
         * 获取进给轴实际位置值
         */
        public double get_axis_act_pos(int ax)
        {
            double pos = -1;
//             if (isConnected())
//             {
//                 CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_POS, ax, ref pos, HCNCShareData.sysData.clientNo);
//                 return pos;
//             }
            AxisData Axis = HCNCShareData.axlist.Find(
            delegate(AxisData temp)
            {
                return (temp.axisNo == ax);
            }
            );
            if (Axis != null)
            {
                pos = Axis.actPos;
            }

            return pos;
        }

        /**
        * 获取进给轴指令位置值
        */
        public double get_axis_cmd_pos(int ax)
        {
//             int pos = 0;
//             if (isConnected())
//             {
//                 CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_POS, ax, ref pos, HCNCShareData.sysData.clientNo);
//                 return pos;
//             }
            double pos = -1;
            AxisData Axis = HCNCShareData.axlist.Find(
            delegate(AxisData temp)
            {
                return (temp.axisNo == ax);
            }
            );
            if (Axis != null)
            {
                pos = Axis.cmdPos;
            }

            return pos;
        }


        /**
         * 获取进给轴实际速度值
         */
//         public double get_axis_act_vel(int ax)
//         {
//             int pos = 0;
//             if (isConnected())
//             {
//                 CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_ACT_VEL, ax, ref pos, HCNCShareData.sysData.clientNo);
//                 return pos;
//             }
//             return -1;
//         }

        /**
        * 获取进给轴指令位置值
        */
//         public double get_axis_cmd_vel(int ax)
//         {
//             int pos = 0;
//             if (isConnected())
//             {
//                 CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_CMD_VEL, ax, ref pos, HCNCShareData.sysData.clientNo);
//                 return pos;
//             }
//             return -1;
//         }

        /**
        * 获取进给轴跟踪误差值
        */
        public double get_axis_fllow_err(int ax)
        {
//             int pos = 0;
//             if (isConnected())
//             {
//                 CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_FOLLOW_ERR, ax, ref pos, HCNCShareData.sysData.clientNo);
//                 return pos;
//             }
//             return -1;
            double pos = -1;
            AxisData Axis = HCNCShareData.axlist.Find(
            delegate(AxisData temp)
            {
                return (temp.axisNo == ax);
            }
            );
            if (Axis != null)
            {
                pos = Axis.followErr;
            }

            return pos;

        }

        /**
       * 获取进给轴电机转速
       */
//         public double get_axis_motor_rev(int ax)
//         {
//             double pos = 0;
//             if (isConnected())
//             {
//                 CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_MOTOR_REV, ax, ref pos, HCNCShareData.sysData.clientNo);
//                 return pos;
//             }
//             return -1;
//         }

       /**
      * 获取进给轴驱动单元电流
      */
//         public double get_axis_drive_cur(int ax)
//         {
//             double val = 0;
//             if (isConnected())
//             {
//                 CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_DRIVE_CUR, ax, ref val, HCNCShareData.sysData.clientNo);
//                 
//             }
//             return val;

//         }

        /**
          * 获取进给轴额定电流
          */
        public double get_axis_load_cur(int ax)
        {
//             double val = 0;
//             if (isConnected())
//             {
//                 CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_LOAD_CUR, ax, ref val, HCNCShareData.sysData.clientNo);
// 
//             }
//             return val;
            double pos = -1;
            AxisData Axis = HCNCShareData.axlist.Find(
            delegate(AxisData temp)
            {
                return (temp.axisNo == ax);
            }
            );
            if (Axis != null)
            {
                pos = Axis.loadCurrent;

            }

            return pos;

        }

        /**
        * 获取进给轴负载电流
        */
        public double get_axis_rated_cur(int ax)
        {
//             double val = 0;
//             if (isConnected())
//             {
//                 CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_RATED_CUR, ax, ref val, HCNCShareData.sysData.clientNo);                
//             }
//             return val;
                double pos = -1;
            AxisData Axis = HCNCShareData.axlist.Find(
            delegate(AxisData temp)
            {
                return (temp.axisNo == ax);
            }
            );
            if (Axis != null)
            {
                pos = Axis.svCurrent;
            }

            return pos;

        }

        /**
       * 获取进给轴伺服驱动版本
       */
//         public double get_axis_drive_ver(int ax)
//         {
//             double pos = 0;
//             if (isConnected())
//             {
//                 CollectShare.Instance().HNC_AxisGetValue((int)HncAxis.HNC_AXIS_DRIVE_VER, ax, ref pos, HCNCShareData.sysData.clientNo);
//                 return pos;
//             }
//             return -1;
//         }


        #endregion


        #region 获取报警数据
        /**
          * 获取当前报警的数目
          */
        public int get_alarm_num()
        {
            int jj = 0;
            if (isConnected())
            {
//                 CollectShare.Instance().HNC_AlarmRefresh(HCNCShareData.sysData.clientNo);
//                 CollectShare.Instance().HNC_AlarmGetNum((int)AlarmType.ALARM_TYPE_ALL, (int)AlarmLevel.ALARM_ERR, ref alarm_num, HCNCShareData.sysData.clientNo);
//                 return alarm_num;
//                 for (int ii = 0; ii < HCNCShareData.almlist.Count; ii++)
//                 {
//                     if (HCNCShareData.almlist[ii].isOnOff == 1)
//                     {
//                         jj++;
//                     }
//                 }
                jj = HCNCShareData.currentAlarmList.Count;
            }
            return jj;
        }

        /**
          * 获取当前报警的报警号和报警文本
          */
        public ArrayList get_alarm_data()
        {
            alarmdata.Clear();
            if (isConnected())
            {
//                 CollectShare.Instance().HNC_AlarmRefresh(HCNCShareData.sysData.clientNo);
//                 CollectShare.Instance().HNC_AlarmGetNum((int)AlarmType.ALARM_TYPE_ALL, (int)AlarmLevel.ALARM_LEVEL_ALL, ref alarm_num, HCNCShareData.sysData.clientNo);
//                 for (int i = 0; i < alarm_num;i++ )
//                 {
//                     CollectShare.Instance().HNC_AlarmGetData((int)AlarmType.ALARM_TYPE_ALL,  //获取所有类型的报警
//                                              (int)AlarmLevel.ALARM_LEVEL_ALL, //仅获取error
//                                              i,
//                                              ref alarmtmp.alarm_ID,     //获取此报警的唯一ID，可用于报警识别
//                                              ref alarmtmp.alarmText,             //报警文本
//                                              HCNCShareData.sysData.clientNo);
//                     alarmtmp.alarmID = TransAlarmNoToStr(alarmtmp.alarm_ID);
//                     alarmdata.Add(alarmtmp);
//                 }
                for (int ii = 0; ii < HCNCShareData.currentAlarmList.Count; ii++)
                {
                    alarmtmp.alarm_ID = HCNCShareData.currentAlarmList[ii].alarmNo;
                    alarmtmp.alarmText = HCNCShareData.currentAlarmList[ii].alarmTxt;
                    alarmtmp.alarmID = TransAlarmNoToStr(alarmtmp.alarm_ID);
                    alarmdata.Add(alarmtmp);
                }

            }
            return alarmdata;
        }


        /**
          * 获取报警历史的数目
          */
        public int get_alarm_history_num()
        {
            int jj = 0;
            if (isConnected())
            {
//                 CollectShare.Instance().HNC_AlarmGetHistoryNum(ref alarm_history_num, HCNCShareData.sysData.clientNo);
//                 return alarm_history_num;
                for (int ii = 0; ii < HCNCShareData.alarmList.Count; ii++)
                {
                    if (HCNCShareData.alarmList[ii].isOnOff == 0)
                    {
                        jj++;
                    }
                }

            }
            return jj;
        }

        /**
          * 获取报警历史的报警内容
          */
        public ArrayList get_alarm_histroy_data()
        {
            alarmhistorydata.Clear();
            if (HCNCShareData.sysData.isConnect)
            {
//                 CollectShare.Instance().HNC_AlarmGetHistoryNum(ref alarm_history_num, HCNCShareData.sysData.clientNo);                
//                 CollectShare.Instance().HNC_AlarmGetHistoryData(0, //从第index个报警历史开始获取
//                                                    ref alarm_history_num, //（传入）共获取count个报警历史, //（传出）实际获取的报警历史个数
//                                                    alarmhistoryData,//历史报警内容：包括报警号、产生时间、消除时间和文本
//                                                HCNCShareData.sysData.clientNo);
//                 for (int i = 0; i < alarm_history_num; i++)
//                 {
//                     alarm_historytmp.alarmID = TransAlarmNoToStr(alarmhistoryData[i].alarmNo);
//                     alarm_historytmp.alarmText = alarmhistoryData[i].text;
//                     alarm_historytmp.timeBegin = alarmhistoryData[i].timeBegin.year + "-" + alarmhistoryData[i].timeBegin.month
//                         + "-" + alarmhistoryData[i].timeBegin.day + " " + alarmhistoryData[i].timeBegin.hour + ":" + alarmhistoryData[i].timeBegin.minute
//                         + ":" + alarmhistoryData[i].timeBegin.second;
//                     alarm_historytmp.timeEnd = alarmhistoryData[i].timeEnd.year + "-" + alarmhistoryData[i].timeEnd.month
//                         + "-" + alarmhistoryData[i].timeEnd.day + " " + alarmhistoryData[i].timeEnd.hour + ":" + alarmhistoryData[i].timeEnd.minute
//                         + ":" + alarmhistoryData[i].timeEnd.second;
// 
//                     alarmhistorydata.Add(alarm_historytmp);
//                 }
                for (int ii = 0; ii < HCNCShareData.alarmList.Count; ii++)
                {
                    if (HCNCShareData.alarmList[ii].isOnOff == 0)
                    {
                        alarm_historytmp.alarmID = TransAlarmNoToStr(HCNCShareData.alarmList[ii].alarmNo);
                        alarm_historytmp.alarmText = HCNCShareData.alarmList[ii].alarmTxt;
//                         alarm_historytmp.timeBegin = HCNCShareData.almlist[ii].time.Year + "-" + HCNCShareData.almlist[ii].time.Month
//                         + "-" + HCNCShareData.almlist[ii].time.Day + " " + HCNCShareData.almlist[ii].time.Hour + ":" + HCNCShareData.almlist[ii].time.Minute
//                         + ":" + HCNCShareData.almlist[ii].time.Second;
                        alarm_historytmp.timeBegin = "";
                        alarm_historytmp.timeEnd = HCNCShareData.alarmList[ii].time.Year + "-" + HCNCShareData.alarmList[ii].time.Month
                        + "-" + HCNCShareData.alarmList[ii].time.Day + " " + HCNCShareData.alarmList[ii].time.Hour + ":" + HCNCShareData.alarmList[ii].time.Minute
                        + ":" + HCNCShareData.alarmList[ii].time.Second;

                        alarmhistorydata.Add(alarm_historytmp);
                    }
                }
            }
            return alarmhistorydata;
        }


        private List<DateTime> HisAlarmShowDataTable_CoPLis = new List<DateTime>();
        private System.Data.DataTable HisAlarmShowDataSource = new System.Data.DataTable();
        private List<DateTime> AlarmShowDataTable_CoPLis = new List<DateTime>();
        private System.Data.DataTable AlarmShowDataSource = new System.Data.DataTable();
        public object HisAlarmShowDataSource_Lok = new object();
        public object AlarmShowDataSource_Lok = new object();

        public void UpHisAlarmShowDataTable(ref System.Data.DataTable DataSource) 
        {
            lock (HisAlarmShowDataSource_Lok)
            {
                if (HisAlarmShowDataSource.Columns.Count == 0)
                {
                    HisAlarmShowDataSource.Columns.Add("序号", typeof(string));
                    HisAlarmShowDataSource.Columns.Add("报警号", typeof(string));
                    HisAlarmShowDataSource.Columns.Add("报警内容", typeof(string));
                }
                while (HisAlarmShowDataSource.Rows.Count > HCNCShareData.alarmList.Count)
                {
                    HisAlarmShowDataTable_CoPLis.RemoveAt(HisAlarmShowDataTable_CoPLis.Count - 1);
                    HisAlarmShowDataSource.Rows.RemoveAt(HisAlarmShowDataSource.Rows.Count - 1);
                }
                int jj = 0;
                for (int ii = 0; ii < HCNCShareData.alarmList.Count; ii++)
                {
                    if (HCNCShareData.alarmList[ii].isOnOff == 0)
                    {
                        if (HisAlarmShowDataTable_CoPLis.Count < jj + 1)
                        {
                            System.Data.DataRow r;
                            r = HisAlarmShowDataSource.NewRow();
                            r[0] = jj.ToString();
                            HisAlarmShowDataSource.Rows.Add(r);
                            HisAlarmShowDataTable_CoPLis.Add(DateTime.Now);
                        }
                        if (HisAlarmShowDataTable_CoPLis[jj] != HCNCShareData.alarmList[ii].time)
                        {
                            if (HisAlarmShowDataSource.Rows.Count < jj + 1)
                            {
                                System.Windows.Forms.MessageBox.Show("AlarmShowDataSource.Rows[jj][1]");
                                return;
                            }
                            HisAlarmShowDataSource.Rows[jj][1] = TransAlarmNoToStr(HCNCShareData.alarmList[ii].alarmNo);
                            HisAlarmShowDataSource.Rows[jj][2] = HCNCShareData.alarmList[ii].time.Year + "-" + HCNCShareData.alarmList[ii].time.Month
                                + "-" + HCNCShareData.alarmList[ii].time.Day + " " + HCNCShareData.alarmList[ii].time.Hour + ":" +
                                HCNCShareData.alarmList[ii].time.Minute + ":" + HCNCShareData.alarmList[ii].time.Second + ":" + HCNCShareData.alarmList[ii].alarmTxt;
                            HisAlarmShowDataTable_CoPLis[jj] = HCNCShareData.alarmList[ii].time;
                        }
                        jj++;
                    }
                }
                DataSource = HisAlarmShowDataSource;
            }
        }
        public byte BDuanDaoBaojing_Old = 0;
        public void UpAlarmShowDataTable(ref System.Data.DataTable DataSource)
        {
            lock (AlarmShowDataSource_Lok)
            {
                if (AlarmShowDataSource.Columns.Count == 0)
                {
                    AlarmShowDataSource.Columns.Add("序号", typeof(string));
                    AlarmShowDataSource.Columns.Add("报警号", typeof(string));
                    AlarmShowDataSource.Columns.Add("报警内容", typeof(string));
                }
                while (AlarmShowDataSource.Rows.Count > HCNCShareData.currentAlarmList.Count)
                {
                    AlarmShowDataTable_CoPLis.RemoveAt(AlarmShowDataTable_CoPLis.Count - 1);
                    AlarmShowDataSource.Rows.RemoveAt(AlarmShowDataSource.Rows.Count - 1);
                }

                byte BDuanDaoBaojing = 1;
                for (int ii = 0; ii < HCNCShareData.currentAlarmList.Count; ii++)
                {
                    if (AlarmShowDataTable_CoPLis.Count < ii + 1)
                    {
                        System.Data.DataRow r;
                        r = AlarmShowDataSource.NewRow();
                        r[0] = ii.ToString();
                        AlarmShowDataSource.Rows.Add(r);
                        AlarmShowDataTable_CoPLis.Add(DateTime.Now);
                    }
                    if (AlarmShowDataTable_CoPLis[ii] != HCNCShareData.currentAlarmList[ii].time)
                    {
                        AlarmShowDataSource.Rows[ii][1] = TransAlarmNoToStr(HCNCShareData.currentAlarmList[ii].alarmNo);
                        AlarmShowDataSource.Rows[ii][2] = HCNCShareData.currentAlarmList[ii].time.Year + "-" + HCNCShareData.currentAlarmList[ii].time.Month
                            + "-" + HCNCShareData.currentAlarmList[ii].time.Day + " " + HCNCShareData.currentAlarmList[ii].time.Hour + ":" +
                            HCNCShareData.currentAlarmList[ii].time.Minute + ":" + HCNCShareData.currentAlarmList[ii].time.Second + ":" + HCNCShareData.currentAlarmList[ii].alarmTxt;
                        AlarmShowDataTable_CoPLis[ii] = HCNCShareData.currentAlarmList[ii].time;
                    }
                    if (HCNCShareData.currentAlarmList[ii].alarmNo == 100000064)//断刀报警
                    {
                        BDuanDaoBaojing = 2;
                    }
                }
                if ((BDuanDaoBaojing_Old == 0 || BDuanDaoBaojing_Old != BDuanDaoBaojing) && SCADA.CncForm.DuanDaoEven != null)
                {
                    BDuanDaoBaojing_Old = BDuanDaoBaojing;
                    if (BDuanDaoBaojing == 2)
                    {
                        SCADA.CncForm.DuanDaoEven.BeginInvoke(this, 0, null, null);
                    }
                    else if (BDuanDaoBaojing == 1)
                    {
                        SCADA.CncForm.DuanDaoEven.BeginInvoke(this, 1, null, null);
                    }
                }

                DataSource = AlarmShowDataSource;
            }
        }




        private String TransAlarmNoToStr(int alarmNo) 
        {
		    // 报警号共计9位，
		    // 通道、语法：(1位报警类型)+(1位报警级别)+(3位通道号)+(4位报警内容编号)；
		    // 轴、伺服 ：(1位报警类型)+(1位报警级别)+(3位轴号) +(4位报警内容编号)；
		    // 其它 ：(1位报警类型)+(1位报警级别)+(7位报警内容编号)
		    int type = alarmNo / 100000000;
		    int level = (alarmNo % 100000000) / 10000000;
		    String[] typeStr = { "系统", "通道", "轴", "伺服", "PLC", "设备", "语法", "PLC",
				    "HMI" };
            String[] levelStr = { "报警", "提示" };
		    String str = "";
		    switch (type) {
		    case 1:
		    case 6:
			    int chNo = (alarmNo % 10000000) / 10000;
			    str = typeStr[type] + levelStr[level] + "： " + "通道" + chNo + "_"
					    + alarmNo % 10000;
			    break;
		    case 2:
		    case 3:
			    int axNo = (alarmNo % 10000000) / 10000;
			    str = typeStr[type] + levelStr[level] + "： " + "轴" + axNo + "_"
					    + alarmNo % 10000;
			    break;
		    default:
			    str = typeStr[type] + levelStr[level] + "： " + alarmNo % 10000000;
			    break;
		    }
		    return str;
	    }

        #endregion

        /// <summary>
        /// 获取派工单的工序号
        /// </summary>
        /// <returns></returns>
        public Int32 GetOP_CODE_Index()
        {
            Int32 GongXu_i = 0;
            switch (OP_CODE)//派工工序编码
            {
                case "J01":
                    GongXu_i = 1;
                    break;
                case "J02":
                    GongXu_i = 2;
                    break;
                case "J03":
                    GongXu_i = 3;
                    break;
                case "J04":
                    GongXu_i = 4;
                    break;
                case "J05":
                    GongXu_i = 5;
                    break;
                case "J06":
                    GongXu_i = 6;
                    break;
                case "J07":
                    GongXu_i = 7;
                    break;
                case "J08":
                    GongXu_i = 8;
                    break;
                case "J09":
                    GongXu_i = 9;
                    break;
                case "J10":
                    GongXu_i = 10;
                    break;
                case "J11":
                    GongXu_i = 11;
                    break;
                case "J12":
                    GongXu_i = 12;
                    break;
                case "J13":
                    GongXu_i = 13;
                    break;
                case "J14":
                    GongXu_i = 14;
                    break;
                case "J15":
                    GongXu_i = 15;
                    break;
                case "J16":
                    GongXu_i = 16;
                    break;
                default:
                    GongXu_i = 0;
                    break;
            }
            return GongXu_i;
        }

        int old_partNum = -100;
        object CNCReport_LockObj = new object();
        public EventHandler<int[]> CNCchanDataEventHandler;//
        public enum CNCchanDataEventType
        {
            CNCReport = 0,
            CNCStateChage
        }
        public void CNCchanDataEventHandlerFuc(object obj, int[] num)
        {
            if(num != null && num.Length == 2)
            {
                switch (num[0])
                {
                    case (int)CNCchanDataEventType.CNCReport://加工数据变化
                        CNCReport(null,num[1]);
                        break;
                    case (int)CNCchanDataEventType.CNCStateChage://CNC 状态变化
                        CNCStateChage(num[1]);
                        break;
                    case 3:// isCycling
                        break;
                    default :
                        break;
                }
            }
        }

        public void CNCStateChage(int m_CNCState)
        {
            EQUIP_STATE m_EQUIP_STATE = new EQUIP_STATE();
            m_EQUIP_STATE.EQUIP_TYPE = 0;
            m_EQUIP_STATE.EQUIP_CODE = BujianID;// VARCHAR2(50),设备ID
            m_EQUIP_STATE.EQUIP_CODE_CNC = HCNCShareData.sysData.macSN; // VARCHAR2(50),cnc:SN号

            switch (m_CNCState)
            {
                case (int)CNCState.DISCON:
                    m_EQUIP_STATE.STATE_VALUE = -1; // FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
                    break;
                case (int)CNCState.ALARM:
                    m_EQUIP_STATE.STATE_VALUE = 3; // FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
                    break;
                case (int)CNCState.IDLE:
                    m_EQUIP_STATE.STATE_VALUE = 0; // FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
                    break;
                case (int)CNCState.RUNING:
                    m_EQUIP_STATE.STATE_VALUE = 1; // FLOAT(126),-1：离线状态，0：一般状态（空闲状态），1：循环启动（运行状态），2：进给保持（空闲状态），3：急停状态（报警状态）
                    break;
                default:
                    break;
            }
            if (SCADA.MainForm.m_CheckHander != null && SCADA.MainForm.m_CheckHander.StateChageEvenHandle != null)
            {
                SCADA.MainForm.m_CheckHander.StateChageEvenHandle.BeginInvoke(this, m_EQUIP_STATE, null, null);
            }

        }

        /// <summary>
        /// 新报工函数
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="clientNo"></param>
        /// <param name="buf"></param>
        public void CNCReport(object obj,int num)
        {
            lock (CNCReport_LockObj)
            {
                if ((old_partNum + 1) != num)
                {
                    SCADA.LogData.EventHandlerSendParm SendParm1 = new SCADA.LogData.EventHandlerSendParm();
                    SendParm1.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                    SendParm1.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                    SendParm1.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                    SendParm1.Keywords = BujianID + "加工完成数异常";
                    SendParm1.EventData += "old = " + old_partNum.ToString() + "  new = " + num;
                    SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm1, null, null);
                }
                old_partNum = num;
                SCADA.LogData.EventHandlerSendParm SendParm = new SCADA.LogData.EventHandlerSendParm();
                SendParm.Node1NameIndex = (int)SCADA.LogData.Node1Name.Equipment_CNC;
                SendParm.LevelIndex = (int)SCADA.LogData.Node2Level.MESSAGE;
                SendParm.EventID = ((int)SCADA.LogData.Node2Level.MESSAGE).ToString();
                SendParm.Keywords = BujianID + "加工完成数 = " + num.ToString();

                System.Data.DataRow mesDispatch = null;
                NcTaskManage.GetDISPATCHSendOrRepor(false, out mesDispatch);
                if (HCNCShareData != null && HCNCShareData.sysData != null && mesDispatch != null)
                {
                    PROD_REPORT machinedata = new PROD_REPORT();
                    machinedata.END_TIME = System.DateTime.Now;
                    machinedata.START_TIME = HCNCShareData.sysData.lastProgStartTime;
                    machinedata.EQUIP_CODE = BujianID;
                    machinedata.ASSIGN_CODE = mesDispatch[(int)SCADA.NCTaskDel.NodeName.生产订单].ToString();
                    if (mesDispatch[(int)SCADA.NCTaskDel.NodeName.操作标示].ToString().Length > 0)
                    {
                        machinedata.FLAG = sbyte.Parse(mesDispatch[(int)SCADA.NCTaskDel.NodeName.操作标示].ToString());
                    }
                    machinedata.OP_CODE = mesDispatch[(int)SCADA.NCTaskDel.NodeName.工序编码].ToString();//工序编码
                    machinedata.MATERIAL_CODE = mesDispatch[(int)SCADA.NCTaskDel.NodeName.物料编码].ToString();//物料编码
                    if (mesDispatch[(int)SCADA.NCTaskDel.NodeName.SN].ToString().Length > 0)
                    {
                        machinedata.SN = decimal.Parse(mesDispatch[(int)SCADA.NCTaskDel.NodeName.SN].ToString());
                    }

                    machinedata.ASSIGN_CODE = mesDispatch[(int)SCADA.NCTaskDel.NodeName.派工单号].ToString();//派工单号

                    machinedata.MARK_DATE = mesDispatch[(int)SCADA.NCTaskDel.NodeName.日期].ToString();
                    machinedata.QTY = 1;//报工数
                    if (OneInOneOut)
                    {
                        if (reportChanPingXuLieHao[0] != null &&
                            reportChanPingXuLieHao[0].Length != 0)
                        {
                            machinedata.PRODUCT_SN = reportChanPingXuLieHao[0];
                            HCNCShareData.reportList.Add(machinedata);
                            reportChanPingXuLieHao[0] = null;//清空
                            NcTaskManage.SetCNCReportNum();
                            SendParm.EventData += "一出一：" + machinedata.PRODUCT_SN;
                        }
                        else if (reportChanPingXuLieHao[1] != null &&
                            reportChanPingXuLieHao[1].Length != 0)
                        {
                            machinedata.PRODUCT_SN = reportChanPingXuLieHao[1];
                            HCNCShareData.reportList.Add(machinedata);
                            reportChanPingXuLieHao[1] = null;//清空
                            NcTaskManage.SetCNCReportNum();
                            SendParm.EventData += "一出一：" + machinedata.PRODUCT_SN;
                        }
                        else
                        {
                            SendParm.EventData += "RFID数据为空，报工失败！";
                        }
                    }
                    else
                    {
                        if (reportChanPingXuLieHao[0] != null &&
                            reportChanPingXuLieHao[0].Length != 0)
                        {
                            machinedata.PRODUCT_SN = reportChanPingXuLieHao[0];
                            HCNCShareData.reportList.Add(machinedata);
                            reportChanPingXuLieHao[0] = null;//清空
                            NcTaskManage.SetCNCReportNum();
                            SendParm.EventData += "一出一：" + machinedata.PRODUCT_SN;
                        }
                        else
                        {
                            SendParm.EventData += "RFID数据为空，报工失败！";
                        }
                        if (reportChanPingXuLieHao[1] != null &&
                            reportChanPingXuLieHao[1].Length != 0)
                        {
                            PROD_REPORT machinedata1 = new PROD_REPORT();
                            machinedata1.ASSIGN_CODE = machinedata.ASSIGN_CODE;
                            machinedata1.END_TIME = machinedata.END_TIME;
                            machinedata1.EQUIP_CODE = machinedata.EQUIP_CODE;
                            machinedata1.FLAG = machinedata.FLAG;
                            machinedata1.MARK_DATE = machinedata.MARK_DATE;
                            machinedata1.MARK_TIME = machinedata.MARK_TIME;
                            machinedata1.MATERIAL_CODE = machinedata.MATERIAL_CODE;
                            machinedata1.OP_CODE = machinedata.OP_CODE;
                            machinedata1.QTY = machinedata.QTY;
                            machinedata1.SN = machinedata.SN;
                            machinedata1.START_TIME = machinedata.START_TIME;
                            machinedata1.PRODUCT_SN = reportChanPingXuLieHao[1];
                            HCNCShareData.reportList.Add(machinedata1);
                            reportChanPingXuLieHao[1] = null;//清空
                            NcTaskManage.SetCNCReportNum();
                            SendParm.EventData += "一出一：" + machinedata.PRODUCT_SN;
                        }
                        else
                        {
                            SendParm.EventData += "RFID数据为空，报工失败！";
                        }

                    }

                    SCADA.MainForm.m_Log.AddLogMsgHandler.BeginInvoke(this, SendParm, null, null);

                    if (HCNCShareData.reportList.Count == HCNCShareData.MachLstLen)
                    {
                        HCNCShareData.reportList.RemoveRange(0, HCNCShareData.MachLstLen / 2);
                    }
                }
            }
        }

        public void CNCExit()
        {
            NcTaskManage.KillSaveData2Xml_threaFuc();
        }
    #endregion

        #region//升级、备份
        /// <summary>
        /// CNC系统升级
        /// </summary>
        /// <param name="flag">SELECT_BASE	0x0000	NA
        ///                        SELECT_BIN	0x0001	文件夹BIN
        ///                        SELECT_BMP	0x0002	文件夹BMP
        ///                        SELECT_DATA	0x0004	文件夹DATA
        ///                        SELECT_HLP	0x0008	文件夹HLP
        ///                        SELECT_N	0x0010	N文件
        ///                        SELECT_PARM	0x0020	文件夹PARM
        ///                        SELECT_PLC	0x0040	文件夹PLC
        ///                        SELECT_PROG	0x0080	文件夹PROG
        ///                        SELECT_TMP 	0x0100	文件夹TMP
        ///                         SELECT_ALLBTF	0x0200	所有文件夹
        ///                        </param>
        /// <param name="PathName">文件名</param>
        /// <returns></returns>
        public Int32 cnc_update(Int32 flag, String PathName)
        {
            int ret = -1;
            if (isConnected())
            {
                ret = CollectShare.Instance().HNC_SysUpdate(flag, PathName, HCNCShareData.sysData.clientNo);                
            }
            return ret ;
        }

        /// <summary>
        /// CNC系统备份
        /// </summary>
        /// <param name="flag">SELECT_BASE	0x0000	NA
        ///                        SELECT_BIN	0x0001	文件夹BIN
        ///                        SELECT_BMP	0x0002	文件夹BMP
        ///                        SELECT_DATA	0x0004	文件夹DATA
        ///                        SELECT_HLP	0x0008	文件夹HLP
        ///                        SELECT_N	0x0010	N文件
        ///                        SELECT_PARM	0x0020	文件夹PARM
        ///                        SELECT_PLC	0x0040	文件夹PLC
        ///                        SELECT_PROG	0x0080	文件夹PROG
        ///                        SELECT_TMP 	0x0100	文件夹TMP
        ///                         SELECT_ALLBTF	0x0200	所有文件夹
        ///                        </param>
        /// <param name="PathName">文件名</param>
        /// <returns></returns>
        public Int32 cnc_backup(Int32 flag, String PathName)
        {
            int ret = -1;
            if (isConnected())
            {
                ret = CollectShare.Instance().HNC_SysBackup(flag, PathName, HCNCShareData.sysData.clientNo);
            }
            return ret;
        }

        public void Dispose()
        {
            HisAlarmShowDataSource.Dispose();
            AlarmShowDataSource.Dispose();
    }


        #endregion



    }
    #endregion
}
