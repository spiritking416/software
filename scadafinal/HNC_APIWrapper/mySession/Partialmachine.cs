using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace HNCAPI
{
    internal partial class Machine : ISubscriber
    {
        private const Int32 byteSize = 1;
        private const Int32 shortSize = 2;
        private const Int32 intSize = 4;
        private const Int32 doubleSize = 8;
        public Int32 HNC_NetFileSend(String localNamme, String dstName)
        {
            return HncApi.HNC_NetFileSend(localNamme, dstName, _ClientNo);
        }
        public Int32 HNC_NetFileGet(String localNamme, String dstName)
        {
            return HncApi.HNC_NetFileGet(localNamme, dstName, _ClientNo);
        }
        public UInt16 HNC_NetIsConnect()
        {
            return HncApi.HNC_NetIsConnect(_ClientNo);
        }
        public Int32 HNC_NetFileCheck(String localNamme, String dstName)
        {
            return HncApi.HNC_NetFileCheck(localNamme, dstName, _ClientNo);
        }
        public Int32 HNC_NetFileRemove(String dstName)
        {
            return HncApi.HNC_NetFileRemove(dstName, _ClientNo);
        }
        public Int32 HNC_NetMakeDir(String dir)
        {
            return HncApi.HNC_NetMakeDir(dir, _ClientNo);
        }
        public Int32 HNC_NetRemoveDir(String dir)
        {
            return HncApi.HNC_NetRemoveDir(dir, _ClientNo);
        }
        public Int32 HNC_RegSetBit(Int32 type, Int32 index, Int32 bit)
        {
            return HncApi.HNC_RegSetBit(type, index, bit, _ClientNo);
        }
        public Int32 HNC_RegClrBit(Int32 type, Int32 index, Int32 bit)
        {
            return HncApi.HNC_RegClrBit(type, index, bit, _ClientNo);
        }
        public Int32 HNC_RegGetNum(Int32 type, ref Int32 num)
        {
            return HncApi.HNC_RegGetNum(type, ref num, _ClientNo);
        }
        public Int32 HNC_RegGetFGBase(Int32 baseType, ref Int32 value)
        {
            return HncApi.HNC_RegGetFGBase(baseType, ref value, _ClientNo);
        }
        public Int32 HNC_VarSetBit(Int32 type, Int32 no, Int32 index, Int32 bit)
        {
            return HncApi.HNC_VarSetBit(type, no, index, bit, _ClientNo);
        }
        public Int32 HNC_VarClrBit(Int32 type, Int32 no, Int32 index, Int32 bit)
        {
            return HncApi.HNC_VarClrBit(type, no, index, bit, _ClientNo);
        }
        public Int32 HNC_MacroVarGetValue(Int32 no, ref SDataUnion var)
        {
            return HncApi.HNC_MacroVarGetValue(no, ref var, _ClientNo);
        }
        public Int32 HNC_ParamanLoad(String lpFileName)
        {
            return HncApi.HNC_ParamanLoad(lpFileName, _ClientNo);
        }
        public Int32 HNC_ParamanSave()
        {
            return HncApi.HNC_ParamanSave(_ClientNo);
        }
        public Int32 HNC_ParamanSaveAs(String lpFileName)
        {
            return HncApi.HNC_ParamanSaveAs(lpFileName, _ClientNo);
        }
        public Int32 HNC_ParamanGetTotalRowNum(ref Int32 rowNum)
        {
            return HncApi.HNC_ParamanGetTotalRowNum(ref rowNum, _ClientNo);
        }
        public Int32 HNC_ParamanTransRow2Index(Int32 fileNo, Int32 subNo, Int32 rowNo, ref Int32 index, ref Int16 dupNum, ref Int16 dupNo)
        {
            return HncApi.HNC_ParamanTransRow2Index(fileNo, subNo, rowNo, ref index, ref dupNum, ref dupNo, _ClientNo);
        }
        public Int32 HNC_ParamanTransRowx2Row(Int32 rowx, ref Int32 fileNo, ref Int32 subNo, ref Int32 row)
        {
            return HncApi.HNC_ParamanTransRowx2Row(rowx, ref fileNo, ref subNo, ref row, _ClientNo);
        }
        public Int32 HNC_ParamanTransId2Rowx(Int32 parmId, ref Int32 rowx)
        {
            return HncApi.HNC_ParamanTransId2Rowx(parmId, ref rowx, _ClientNo);
        }
        public Int32 HNC_ParamanRewriteSubClass(Int32 fileNo, Int32 subNo)
        {
            return HncApi.HNC_ParamanRewriteSubClass(fileNo, subNo, _ClientNo);
        }
        public Int32 HNC_ParamanSaveStrFile()
        {
            return HncApi.HNC_ParamanSaveStrFile(_ClientNo);
        }
        public Int32 HNC_ParamanGetI32(Int32 fileNo, Int32 subNo, Int32 index, ref Int32 value)
        {
            return HncApi.HNC_ParamanGetI32(fileNo, subNo, index, ref value, _ClientNo);
        }
        public Int32 HNC_ParamanSetI32(Int32 fileNo, Int32 subNo, Int32 index, Int32 value)
        {
            return HncApi.HNC_ParamanSetI32(fileNo, subNo, index, value, _ClientNo);
        }
        public Int32 HNC_ParamanGetFloat(Int32 fileNo, Int32 subNo, Int32 index, ref Double value)
        {
            return HncApi.HNC_ParamanGetFloat(fileNo, subNo, index, ref value, _ClientNo);
        }
        public Int32 HNC_ParamanSetFloat(Int32 fileNo, Int32 subNo, Int32 index, Double value)
        {
            return HncApi.HNC_ParamanSetFloat(fileNo, subNo, index, value, _ClientNo);
        }
        public Int32 HNC_ParamanSetStr(Int32 fileNo, Int32 subNo, Int32 index, String value)
        {
            return HncApi.HNC_ParamanSetStr(fileNo, subNo, index, value, _ClientNo);
        }
        public Int32 HNC_SystemGetUserRealTimeData(Byte[] info)
        {
            return HncApi.HNC_SystemGetUserRealTimeData(info, _ClientNo);
        }
        public Int32 HNC_SystemSetUserRealTimeData(Byte[] info)
        {
            return HncApi.HNC_SystemSetUserRealTimeData(info, _ClientNo);
        }
        public Int32 HNC_CrdsGetMaxNum(Int32 type)
        {
            return HncApi.HNC_CrdsGetMaxNum(type, _ClientNo);
        }
        public Int32 HNC_CrdsLoad()
        {
            return HncApi.HNC_CrdsLoad(_ClientNo);
        }
        public Int32 HNC_CrdsSave()
        {
            return HncApi.HNC_CrdsSave(_ClientNo);
        }
        public Int32 HNC_ToolMagSave()
        {
            return HncApi.HNC_ToolMagSave(_ClientNo);
        }
        public Int32 HNC_ToolGetMaxMagNum()
        {
            return HncApi.HNC_ToolGetMaxMagNum(_ClientNo);
        }
        public Int32 HNC_ToolGetMagHeadBase()
        {
            return HncApi.HNC_ToolGetMagHeadBase(_ClientNo);
        }
        public Int32 HNC_ToolGetPotDataBase()
        {
            return HncApi.HNC_ToolGetPotDataBase(_ClientNo);
        }
        public Int32 HNC_ToolGetMagBase(Int32 magNo, Int32 index, ref Int32 value)
        {
            return HncApi.HNC_ToolGetMagBase(magNo, index, ref value, _ClientNo);
        }
        public Int32 HNC_ToolSetMagBase(Int32 magNo, Int32 index, Int32 value)
        {
            return HncApi.HNC_ToolSetMagBase(magNo, index, value, _ClientNo);
        }
        public Int32 HNC_ToolMagGetToolNo(Int32 magNo, Int32 potNo, ref Int32 toolNo)
        {
            return HncApi.HNC_ToolMagGetToolNo(magNo, potNo, ref toolNo, _ClientNo);
        }
        public Int32 HNC_ToolMagSetToolNo(Int32 magNo, Int32 potNo, Int32 toolNo)
        {
            return HncApi.HNC_ToolMagSetToolNo(magNo, potNo, toolNo, _ClientNo);
        }
        public Int32 HNC_ToolGetPotAttri(Int32 magNo, Int32 potNo, ref Int32 potAttri)
        {
            return HncApi.HNC_ToolGetPotAttri(magNo, potNo, ref potAttri, _ClientNo);
        }
        public Int32 HNC_ToolSetPotAttri(Int32 magNo, Int32 potNo, Int32 potAttri)
        {
            return HncApi.HNC_ToolSetPotAttri(magNo, potNo, potAttri, _ClientNo);
        }
        public Int32 HNC_ToolLoad()
        {
            return HncApi.HNC_ToolLoad(_ClientNo);
        }
        public Int32 HNC_ToolSave()
        {
            return HncApi.HNC_ToolSave(_ClientNo);
        }
        public Int32 HNC_ToolGetMaxToolNum()
        {
            return HncApi.HNC_ToolGetMaxToolNum(_ClientNo);
        }
        //采样

        public Int32 HNC_AlarmGetNum(Int32 type, Int32 level, ref Int32 num)
        {
            return HncApi.HNC_AlarmGetNum(type, level, ref num, _ClientNo);
        }
        public Int32 HNC_AlarmGetHistoryNum(ref Int32 num)
        {
            return HncApi.HNC_AlarmGetHistoryNum(ref num, _ClientNo);
        }
        public Int32 HNC_AlarmRefresh()
        {
            return HncApi.HNC_AlarmRefresh(_ClientNo);
        }
        public Int32 HNC_AlarmSaveHistory()
        {
            return HncApi.HNC_AlarmSaveHistory(_ClientNo);
        }
        public Int32 HNC_AlarmClrHistory()
        {
            return HncApi.HNC_AlarmClrHistory(_ClientNo);
        }
        public Int32 HNC_AlarmClr(Int32 type, Int32 level)
        {
            return HncApi.HNC_AlarmClr(type, level, _ClientNo);
        }
        public Int32 HNC_FprogRandomInit(Int32 ch)
        {
            return HncApi.HNC_FprogRandomInit(ch, _ClientNo);
        }
        public Int32 HNC_FprogRandomLoad(Int32 line)
        {
            return HncApi.HNC_FprogRandomLoad(line, _ClientNo);
        }
        public Int32 HNC_FprogRandomWriteback(Int32 line, Byte flag)
        {
            return HncApi.HNC_FprogRandomWriteback(line, flag, _ClientNo);
        }
        public Int32 HNC_SysCtrlSkipToRow(Int32 ch, Int32 row)
        {
            return HncApi.HNC_SysCtrlSkipToRow(ch, row, _ClientNo);
        }
        public Int32 HNC_FprogRandomExit()
        {
            return HncApi.HNC_FprogRandomExit(_ClientNo);
        }
        public Int32 HNC_SysCtrlSelectProg(Int32 ch, String name)
        {
            return HncApi.HNC_SysCtrlSelectProg(ch, name, _ClientNo);
        }
        public Int32 HNC_SysCtrlLoadProg(Int32 ch, String name)
        {
            return HncApi.HNC_SysCtrlLoadProg(ch, name, _ClientNo);
        }
        public Int32 HNC_SysCtrlResetProg(Int32 ch)
        {
            return HncApi.HNC_SysCtrlResetProg(ch, _ClientNo);
        }
        public Int32 HNC_SysCtrlStopProg(Int32 ch)
        {
            return HncApi.HNC_SysCtrlStopProg(ch, _ClientNo);
        }
        public Int32 HNC_SysBackup(Int32 flag, String PathName)
        {
            return HncApi.HNC_SysBackup(flag, PathName, _ClientNo);
        }
        public Int32 HNC_SysUpdate(Int32 flag, String PathName)
        {
            return HncApi.HNC_SysUpdate(flag, PathName, _ClientNo);
        }
        public Int32 HNC_NetDiskMount(String ip, String progAddr, String name, String pass)
        {
            return HncApi.HNC_NetDiskMount(ip, progAddr, name, pass, _ClientNo);
        }
        public Int32 HNC_ActivationGetExpDate(ref Int32 flag, Int32[] pDate)
        {
            return HncApi.HNC_ActivationGetExpDate(ref flag, pDate, _ClientNo);
        }
        public Int32 HNC_ActivationGetLastday(ref Int32 flag, ref Int32 day)
        {
            return HncApi.HNC_ActivationGetLastday(ref flag, ref day, _ClientNo);
        }

        //MDI
        public Int32 HNC_SysCtrlMdiTry(Int32 ch)
        {
            return HncApi.HNC_SysCtrlMdiTry(ch, _ClientNo);
        }
        public Int32 HNC_SysCtrlMdiReq(Int32 ch)
        {
            return HncApi.HNC_SysCtrlMdiReq(ch, _ClientNo);
        }
        public Int32 HNC_SysCtrlMdiOpen()
        {
            return HncApi.HNC_SysCtrlMdiOpen(_ClientNo);
        }
        public Int32 HNC_SysCtrlMdiClear()
        {
            return HncApi.HNC_SysCtrlMdiClear(_ClientNo);
        }
        public Int32 HNC_SysCtrlMdiStop()
        {
            return HncApi.HNC_SysCtrlMdiStop(_ClientNo);
        }
        public Int32 HNC_SysCtrlMdiClose()
        {
            return HncApi.HNC_SysCtrlMdiClose(_ClientNo);
        }
        public Int32 HNC_FprogMdiConfirm()
        {
            return HncApi.HNC_FprogMdiConfirm(_ClientNo);
        }
        public Int32 HNC_FprogMdiSetContent(String txt, Int32 txtLen)
        {
            return HncApi.HNC_FprogMdiSetContent(txt, txtLen, _ClientNo);
        }
        public Int32 HNC_SysCtrlMdiUpdate(Int32 rowNum)
        {
            return HncApi.HNC_SysCtrlMdiUpdate(rowNum, _ClientNo);
        }
        public Int32 HNC_VerifyGetCurveType(Int32 ch, ref Int32 curtype)
        {
            return HncApi.HNC_VerifyGetCurveType(ch, ref curtype, _ClientNo);
        }
        public Int32 HNC_VerifyGetCurveSpos(Int32 ch, Int32 ax, ref Int32 spos)
        {
            return HncApi.HNC_VerifyGetCurveSpos(ch, ax, ref spos, _ClientNo);
        }

        public Int32 HNC_VerifyGetCurveEpos(Int32 ch, Int32 ax, ref Int32 epos)
        {
            return HncApi.HNC_VerifyGetCurveEpos(ch, ax, ref epos, _ClientNo);
        }
        public Int32 HNC_VerifyGetLinePos(Int32 ch, Int32[] pos, ref Int32 flag)
        {
            return HncApi.HNC_VerifyGetLinePos(ch, pos, ref flag, _ClientNo);
        }
        public Int32 HNC_VerifyClearCurve(Int32 ch)
        {
            return HncApi.HNC_VerifyClearCurve(ch, _ClientNo);
        }
        public Int32 HNC_VerifyGetCurvePoint(Int32 ch, Int32[] pos, ref Int32 vflag)
        {
            return HncApi.HNC_VerifyGetCurvePoint(ch, pos, ref vflag, _ClientNo);
        }
        public Int32 HNC_VerifyCalcuCyclePara(Int32 ch, ref Byte vflag)
        {
            return HncApi.HNC_VerifyCalcuCyclePara(ch, ref vflag, _ClientNo);
        }
        public Int32 HNC_VerifySetChCmdPos(Int32 ch, Int32 ax, Int32 pos)
        {
            return HncApi.HNC_VerifySetChCmdPos(ch, ax, pos, _ClientNo);
        }
        public Int32 HNC_VerifyGetCmdPos(Int32 ax, ref Int32 pos)
        {
            return HncApi.HNC_VerifyGetCmdPos(ax, ref pos, _ClientNo);
        }

        //net api
        public Int32 HNC_NetGetDllVer(ref String dllVer)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCNET.VERSION_LEN);
            ret = HncApi.HNC_NetGetDllVer(ptr);
            dllVer = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        public Int32 HNC_NetGetSDKVer(ref String dllPlusVer)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCNET.VERSION_LEN);
            ret = HncApi.HNC_NetGetSDKVer(ptr);
            dllPlusVer = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        //重载函数
        public Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Byte value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.byteSize);
            ret = HncApi.HNC_RegGetValue(type, index, ptr, _ClientNo);
            value = Marshal.ReadByte(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Int16 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.shortSize);
            ret = HncApi.HNC_RegGetValue(type, index, ptr, _ClientNo);
            value = Marshal.ReadInt16(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            ret = HncApi.HNC_RegGetValue(type, index, ptr, _ClientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_RegSetValue(Int32 type, Int32 index, Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HncApi.HNC_RegSetValue(type, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_RegSetValue(Int32 type, Int32 index, Int16 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.shortSize);
            Marshal.WriteInt16(ptr, value);
            ret = HncApi.HNC_RegSetValue(type, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_RegSetValue(Int32 type, Int32 index, Byte value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.byteSize);
            Marshal.WriteByte(ptr, value);
            ret = HncApi.HNC_RegSetValue(type, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ChannelGetValue(Int32 type, Int32 ch, Int32 index, ref String value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.doubleSize);
            ret = HncApi.HNC_ChannelGetValue(type, ch, index, ptr, _ClientNo);
            value = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ChannelGetValue(Int32 type, Int32 ch, Int32 index, ref Double value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.doubleSize);
            ret = HncApi.HNC_ChannelGetValue(type, ch, index, ptr, _ClientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        public Int32 HNC_ChannelGetValue(Int32 type, Int32 ch, Int32 index, ref Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            ret = HncApi.HNC_ChannelGetValue(type, ch, index, ptr, _ClientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ChannelSetValue(Int32 type, Int32 ch, Int32 index, Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HncApi.HNC_ChannelSetValue(type, ch, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_AxisGetValue(Int32 type, Int32 ax, ref String value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCAXIS.MOTOR_TYPE_LEN);
            ret = HncApi.HNC_AxisGetValue(type, ax, ptr, _ClientNo);
            value = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_AxisGetValue(Int32 type, Int32 ax, ref Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            ret = HncApi.HNC_AxisGetValue(type, ax, ptr, _ClientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_AxisGetValue(Int32 type, Int32 ax, ref Double value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.doubleSize);
            ret = HncApi.HNC_AxisGetValue(type, ax, ptr, _ClientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_CrdsGetValue(Int32 type, Int32 ax, ref Double value, Int32 ch, Int32 crds)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.doubleSize);
            ret = HncApi.HNC_CrdsGetValue(type, ax, ptr, ch, crds, _ClientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_CrdsGetValue(Int32 type, Int32 ax, ref Int32 value, Int32 ch, Int32 crds)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            ret = HncApi.HNC_CrdsGetValue(type, ax, ptr, ch, crds, _ClientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_CrdsSetValue(Int32 type, Int32 ax, Double value, Int32 ch, Int32 crds)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.doubleSize);
            Marshal.StructureToPtr(value, ptr, true);
            ret = HncApi.HNC_CrdsSetValue(type, ax, ptr, ch, crds, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_CrdsSetValue(Int32 type, Int32 ax, Int32 value, Int32 ch, Int32 crds)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HncApi.HNC_CrdsSetValue(type, ax, ptr, ch, crds, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ToolGetToolPara(Int32 toolNo, Int32 index, ref Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            ret = HncApi.HNC_ToolGetToolPara(toolNo, index, ptr, _ClientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ToolGetToolPara(Int32 toolNo, Int32 index, ref Double value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.doubleSize);
            ret = HncApi.HNC_ToolGetToolPara(toolNo, index, ptr, _ClientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ToolSetToolPara(Int32 toolNo, Int32 index, Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HncApi.HNC_ToolSetToolPara(toolNo, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ToolSetToolPara(Int32 toolNo, Int32 index, Double value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.doubleSize);
            Marshal.StructureToPtr(value, ptr, true);
            ret = HncApi.HNC_ToolSetToolPara(toolNo, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_VarGetValue(Int32 type, Int32 no, Int32 index, ref Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            ret = HncApi.HNC_VarGetValue(type, no, index, ptr, _ClientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_VarGetValue(Int32 type, Int32 no, Int32 index, ref Double value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.doubleSize);
            ret = HncApi.HNC_VarGetValue(type, no, index, ptr, _ClientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_VarSetValue(Int32 type, Int32 no, Int32 index, Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HncApi.HNC_VarSetValue(type, no, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_VarSetValue(Int32 type, Int32 no, Int32 index, Double value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.doubleSize);
            Marshal.StructureToPtr(value, ptr, true);
            ret = HncApi.HNC_VarSetValue(type, no, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_SystemGetValue(Int32 type, ref String value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCSYS.MAX_SYS_STR_LEN);
            ret = HncApi.HNC_SystemGetValue(type, ptr, _ClientNo);
            value = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_SystemGetValue(Int32 type, ref Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            ret = HncApi.HNC_SystemGetValue(type, ptr, _ClientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_SystemSetValue(Int32 type, String value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCSYS.MAX_SYS_STR_LEN);
            ptr = Marshal.StringToCoTaskMemAnsi(value);
            ret = HncApi.HNC_SystemSetValue(type, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_SystemSetValue(Int32 type, Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.intSize);
            Marshal.WriteInt32(ptr, value);
            ret = HncApi.HNC_SystemSetValue(type, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, ref Int32 propValue)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HncApi.HNC_ParamanGetParaPropEx(parmId, propType, ptr, _ClientNo);
            propValue = Marshal.ReadInt32((IntPtr)(ptr.ToInt32() + HncApi.intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, ref Double propValue)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HncApi.HNC_ParamanGetParaPropEx(parmId, propType, ptr, _ClientNo);
            propValue = (Double)Marshal.PtrToStructure((IntPtr)(ptr.ToInt32() + HncApi.intSize), typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, SByte[] propValue)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HncApi.HNC_ParamanGetParaPropEx(parmId, propType, ptr, _ClientNo);
            Int32 length = propValue.Length < HNCDATATYPE.PARAM_STR_LEN ? propValue.Length : HNCDATATYPE.PARAM_STR_LEN;

            if (ret == 0)
            {
                for (Int32 i = 0; i < length; ++i)
                {
                    propValue[i] = (SByte)Marshal.ReadByte((IntPtr)(ptr.ToInt32() + HncApi.intSize + i));
                }
            }
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, ref String propValue)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HncApi.HNC_ParamanGetParaPropEx(parmId, propType, ptr, _ClientNo);
            propValue = Marshal.PtrToStringAnsi((IntPtr)(ptr.ToInt32() + HncApi.intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, Int32 propValue)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 1);
            Marshal.WriteInt32((IntPtr)(ptr.ToInt32() + HncApi.intSize), propValue);
            ret = HncApi.HNC_ParamanSetParaPropEx(parmId, propType, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, Double propValue)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 2);
            Marshal.StructureToPtr(propValue, (IntPtr)(ptr.ToInt32() + HncApi.intSize), true);
            ret = HncApi.HNC_ParamanSetParaPropEx(parmId, propType, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, SByte[] propValue)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 11);
            for (Int32 i = 0; i < propValue.Length; ++i)
            {
                Marshal.WriteByte((IntPtr)(ptr.ToInt32() + HncApi.intSize + i), (Byte)propValue[i]);
            }

            ret = HncApi.HNC_ParamanSetParaPropEx(parmId, propType, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, String propValue)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 5);

            Byte[] tempArray = Encoding.Default.GetBytes(propValue);
            Byte[] strArray = new Byte[tempArray.Length + 1];
            tempArray.CopyTo(strArray, 0);
            strArray[strArray.Length - 1] = 0;
            Marshal.Copy(strArray, 0, (IntPtr)(ptr.ToInt32() + HncApi.intSize), strArray.Length);

            ret = HncApi.HNC_ParamanSetParaPropEx(parmId, propType, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, ref Int32 prop_value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HncApi.HNC_ParamanGetParaProp(filetype, subid, index, prop_type, ptr, _ClientNo);
            prop_value = Marshal.ReadInt32((IntPtr)(ptr.ToInt32() + HncApi.intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, ref Double prop_value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HncApi.HNC_ParamanGetParaProp(filetype, subid, index, prop_type, ptr, _ClientNo);
            prop_value = (Double)Marshal.PtrToStructure((IntPtr)(ptr.ToInt32() + HncApi.intSize), typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, SByte[] prop_value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HncApi.HNC_ParamanGetParaProp(filetype, subid, index, prop_type, ptr, _ClientNo);
            Int32 length = prop_value.Length < HNCDATATYPE.PARAM_STR_LEN ? prop_value.Length : HNCDATATYPE.PARAM_STR_LEN;

            if (ret == 0)
            {
                for (Int32 i = 0; i < length; ++i)
                {
                    prop_value[i] = (SByte)Marshal.ReadByte((IntPtr)(ptr.ToInt32() + HncApi.intSize + i));
                }
            }
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, ref String prop_value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HncApi.HNC_ParamanGetParaProp(filetype, subid, index, prop_type, ptr, _ClientNo);
            prop_value = Marshal.PtrToStringAnsi((IntPtr)(ptr.ToInt32() + HncApi.intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, Int32 prop_value)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 1);
            Marshal.WriteInt32((IntPtr)(ptr.ToInt32() + HncApi.intSize), prop_value);
            ret = HncApi.HNC_ParamanSetParaProp(filetype, subid, index, prop_type, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, Double prop_value)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 2);
            Marshal.StructureToPtr(prop_value, (IntPtr)(ptr.ToInt32() + HncApi.intSize), true);
            ret = HncApi.HNC_ParamanSetParaProp(filetype, subid, index, prop_type, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, SByte[] prop_value)
        {
            Int32 ret = -1;

            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 11);
            for (Int32 i = 0; i < prop_value.Length; ++i)
            {
                Marshal.WriteByte((IntPtr)(ptr.ToInt32() + HncApi.intSize + i), (Byte)prop_value[i]);
            }
            ret = HncApi.HNC_ParamanSetParaProp(filetype, subid, index, prop_type, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, String prop_value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            Marshal.WriteInt32(ptr, 5);

            Byte[] tempArray = Encoding.Default.GetBytes(prop_value);
            Byte[] strArray = new Byte[tempArray.Length + 1];
            tempArray.CopyTo(strArray, 0);
            strArray[strArray.Length - 1] = 0;
            Marshal.Copy(strArray, 0, (IntPtr)(ptr.ToInt32() + HncApi.intSize), strArray.Length);

            ret = HncApi.HNC_ParamanSetParaProp(filetype, subid, index, prop_type, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetSubClassProp(Int32 fileNo, Byte propType, ref Int32 propValue)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HncApi.HNC_ParamanGetSubClassProp(fileNo, propType, ptr, _ClientNo);
            propValue = Marshal.ReadInt32((IntPtr)(ptr.ToInt32() + HncApi.intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetSubClassProp(Int32 fileNo, Byte propType, ref String propValue)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HncApi.HNC_ParamanGetSubClassProp(fileNo, propType, ptr, _ClientNo);
            propValue = Marshal.PtrToStringAnsi((IntPtr)(ptr.ToInt32() + HncApi.intSize));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_MacroVarSetValue(Int32 no, SDataUnion var)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SDataUnion)));
            Marshal.StructureToPtr(var, ptr, true);
            ret = HncApi.HNC_MacroVarSetValue(no, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, ref Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            ret = HncApi.HNC_ParamanGetItem(fileNo, subNo, index, ptr, _ClientNo);
            value = Marshal.ReadInt32(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, ref Double value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            ret = HncApi.HNC_ParamanGetItem(fileNo, subNo, index, ptr, _ClientNo);
            value = (Double)Marshal.PtrToStructure(ptr, typeof(Double));
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, SByte[] value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            ret = HncApi.HNC_ParamanGetItem(fileNo, subNo, index, ptr, _ClientNo);
            Int32 length = value.Length < HNCDATATYPE.PARAM_STR_LEN ? value.Length : HNCDATATYPE.PARAM_STR_LEN;

            if (ret == 0)
            {
                for (Int32 i = 0; i < length; ++i)
                {
                    value[i] = (SByte)Marshal.ReadByte((IntPtr)(ptr.ToInt32() + i));
                }
            }
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, ref String value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            ret = HncApi.HNC_ParamanGetItem(fileNo, subNo, index, ptr, _ClientNo);
            value = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, Int32 value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            Marshal.WriteInt32(ptr, value);
            ret = HncApi.HNC_ParamanSetItem(fileNo, subNo, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, Double value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            Marshal.StructureToPtr(value, ptr, true);
            ret = HncApi.HNC_ParamanSetItem(fileNo, subNo, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, SByte[] value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            for (Int32 i = 0; i < value.Length; ++i)
            {
                Marshal.WriteByte((IntPtr)(ptr.ToInt32() + i), (Byte)value[i]);
            }
            ret = HncApi.HNC_ParamanSetItem(fileNo, subNo, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, String value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SParamValue)));
            Byte[] tempArray = Encoding.Default.GetBytes(value);
            Byte[] strArray = new Byte[tempArray.Length + 1];
            tempArray.CopyTo(strArray, 0);
            strArray[strArray.Length - 1] = 0;

            Marshal.Copy(strArray, 0, ptr, strArray.Length);
            ret = HncApi.HNC_ParamanSetItem(fileNo, subNo, index, ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_NetGetIpaddr(ref String ip, ref UInt16 port)
        {
            return HncApi.HNC_NetGetIpaddr(ref ip, ref port, _ClientNo);
        }



        public Int32 HNC_ParamanGetStr(Int32 fileNo, Int32 subNo, Int32 index, ref String value)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HncApi.PAR_PROP_DATA_LEN);
            ret = HncApi.HNC_ParamanGetStr(fileNo, subNo, index, ptr, _ClientNo);
            value = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_FprogGetFullName(Int32 ch, ref String progName)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCFPROGMAN.PROG_PATH_SIZE);
            ret = HncApi.HNC_FprogGetFullName(ch, ptr, _ClientNo);
            progName = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_ParamanGetFileName(Int32 fileNo, ref String buf)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCDATADEF.PARAMAN_LIB_TITLE_SIZE);
            ret = HncApi.HNC_ParamanGetFileName(fileNo, ptr, _ClientNo);
            buf = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_AlarmGetData(Int32 type, Int32 level, Int32 index, ref Int32 alarmNo, ref String alarmText)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCALARM.ALARM_TXT_LEN);
            ret = HncApi.HNC_AlarmGetData(type, level, index, ref alarmNo, ptr, _ClientNo);
            alarmText = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }

        public Int32 HNC_NetFileGetDirInfo(String dirname, ncfind_t[] info, ref UInt16 num)
        {
            Int32 size = Marshal.SizeOf(typeof(ncfind_t));
            IntPtr ptr = Marshal.AllocHGlobal(size * info.Length);
            Int32 ret = -1;
            ret = HncApi.HNC_NetFileGetDirInfo(dirname, ptr, ref num, _ClientNo);

            if (ret == 0)
            {
                for (Int32 i = 0; i < num; ++i)
                {
                    info[i] = (ncfind_t)Marshal.PtrToStructure((IntPtr)(ptr.ToInt32() + i * size), typeof(ncfind_t));
                }
            }

            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        public Int32 HNC_EventPut(SEventElement ev)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(SEventElement)));
            Marshal.StructureToPtr(ev, ptr, true);
            ret = HncApi.HNC_EventPut(ptr, _ClientNo);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }
        public Int32 HNC_AlarmGetHistoryData(Int32 index, ref Int32 count, AlarmHisData[] data)
        {
            Int32 MAX_ALARM_HISDATA_LEN = HncApi.ALARM_HISTORY_MAX_NUM * Marshal.SizeOf(typeof(AlarmHisData));
            IntPtr ptr = Marshal.AllocHGlobal(MAX_ALARM_HISDATA_LEN);
            Int32 ret = HncApi.HNC_AlarmGetHistoryData(index, ref count, ptr, _ClientNo);
            if (ret == 0)
            {
                for (Int32 i = 0; i < count; i++)
                {
                    data[i] = (AlarmHisData)Marshal.PtrToStructure((IntPtr)(ptr.ToInt32() + i * Marshal.SizeOf(typeof(AlarmHisData))), typeof(AlarmHisData));
                }
            }

            Marshal.FreeHGlobal(ptr);

            return ret;
        }
        public Int32 HNC_SysCtrlMdiGetBlk(Int32[] pos, Int32[] msft, Int32[] ijkr)
        {
            Int32 ret = -1;

            IntPtr posPtr = Marshal.AllocHGlobal(HNCDATADEF.CHAN_AXES_NUM * HncApi.intSize);
            IntPtr msftPtr = Marshal.AllocHGlobal(HNCSYSCTRL.MSFTCOUNT * HncApi.intSize);
            IntPtr ijkrPtr = Marshal.AllocHGlobal(HNCSYSCTRL.IJKRCOUNT * HncApi.intSize);
            ret = HncApi.HNC_SysCtrlMdiGetBlk(posPtr, msftPtr, ijkrPtr, _ClientNo);

            if (ret == 0)
            {
                Marshal.Copy(posPtr, pos, 0, HNCDATADEF.CHAN_AXES_NUM);
                Marshal.Copy(msftPtr, msft, 0, HNCSYSCTRL.MSFTCOUNT);
                Marshal.Copy(ijkrPtr, ijkr, 0, HNCSYSCTRL.IJKRCOUNT);
            }

            Marshal.FreeHGlobal(posPtr);
            Marshal.FreeHGlobal(msftPtr);
            Marshal.FreeHGlobal(ijkrPtr);

            return ret;
        }


        public Int32 HNC_FprogGetProgPathByIdx(Int32 pindex, ref String progName)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCFPROGMAN.PROG_PATH_SIZE);
            ret = HncApi.HNC_FprogGetProgPathByIdx(pindex, ptr, _ClientNo);
            progName = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);

            return ret;
        }
        public Int32 HNC_SamplReset(Int32 ch)
        {
            return HncApi.HNC_SamplReset(ch, _ClientNo);
        }
        public Int32 HNC_SamplTriggerOn()
        {
            return HncApi.HNC_SamplTriggerOn(_ClientNo);
        }
        public Int32 HNC_SamplTriggerOff()
        {
            return HncApi.HNC_SamplTriggerOff(_ClientNo);
        }
       
    }
}
