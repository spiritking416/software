using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;

namespace HNCAPI
{
    class SampleData
    {
        private Int32 _num;
        private Int32[] _data;
        public SampleData(Int32 num,Int32[] data)
        {
            _num = num;
            _data=new Int32[data.Length];
            data.CopyTo(_data,0);
        }
        public Int32 Num
        {
            get { return _num; }
            set { _num = value; }
        }
        public Int32[] Data
        {
            get { return _data; }
            set { _data = value; }
        }
        
    }
   public class Session :ISubscriber
    {

       private static System.Collections.Generic.Dictionary<Int32, Machine> _MachineDic=null;

       public delegate void CloseHandler(Session currentSession);
       public event CloseHandler CloseEvent;
       public delegate void Eventhandler(SEventElement e, Int16 clientNo);
       public event Eventhandler EventAvailable;
       public delegate void GCodeHandler(String key, String GCodeText);
       public event GCodeHandler EventGCodeAvailable;
       public delegate void JobFinished(String jobId, String GCodeName);
       private SampleSet _SampleSet;
       public SampleSet SmpSet
       {
           get { return _SampleSet; }
           set { _SampleSet = value; }
       }

       internal Session()
       {
            if (null == _MachineDic)//add by leomei on20160723
            {
                _MachineDic = new Dictionary<int, Machine>();
            }
       }
       private Int16 MachineExist(String Ip, UInt16 port)
       {
           foreach (Int32 key in _MachineDic.Keys)
           {
               Machine mac = _MachineDic[key];
               if (mac.Ip == Ip && mac.Port == port)
               {
                   if(mac.HNC_NetIsConnect()==0)
                        return mac.ClientNo;
                   else
                   {
                        _MachineDic[key].StopUpload();
                       _MachineDic.Remove(key);
                          return -1;
                   }
               }
           }
           return -1;

       }
       public void Close()
       {
            foreach(Int32 key in _MachineDic.Keys)
            {
                _MachineDic[key].StopUpload();
            }
           if (CloseEvent != null)
           {
               CloseEvent(this);
           }
       }
       public Int16 HNC_NetConnect(String ip, UInt16 port,bool NeedSample)
       {
           lock (_MachineDic)
           {
               Int16 clientNo = MachineExist(ip, port);
               if (clientNo > -1) return clientNo;
               Int16 ret = HncApi.HNC_NetConnect(ip, port);
               if (ret == -1)
               {
                   return ret;
               }
               Machine mac = new Machine(ret, ip, port, _SampleSet, NeedSample);
               mac.EventAvailable += new Machine.Eventhandler(mac_EventAvailable);
               _MachineDic.Add(ret, mac);
               return ret;
           }
       }
       void mac_EventGCodeAvailable(string key, string GCodeText)
       {
           if (EventGCodeAvailable != null)
           {
               EventGCodeAvailable(key, GCodeText);
           }
       }
       void mac_EventAvailable(SEventElement e, short clientNo)
       {
           if (EventAvailable != null)
           {
               EventAvailable(e, clientNo);
           }
           if (e.code == EVENTDEF.ncEvtDisConnect)
           {
               _MachineDic.Remove(clientNo);
           }
       }
       #region old function
       //NET
       
        public Int32 HNC_NetGetClientNo(String ip, UInt16 port, ref Int16 clientNo)
        {
            foreach (Int32 key in _MachineDic.Keys)
            {
                Machine mac = _MachineDic[key];
                if (mac.Ip == ip && mac.Port == port)
                {
                    clientNo = mac.ClientNo;
                    return 0;
                }
            }
            return -1;
        }
        public Int32 HNC_NetFileSend(String localNamme, String dstName, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_NetFileSend(localNamme, dstName);
        }
        public Int32 HNC_NetFileGet(String localNamme, String dstName, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_NetFileGet(localNamme, dstName);
        }
        
        public bool HNC_NetIsConnect(Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return false;Machine mac = _MachineDic[clientNo];
            return mac.HNC_NetIsConnect()==0;
        }
        public Int32 HNC_NetFileCheck(String localNamme, String dstName, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_NetFileCheck(localNamme, dstName);
        }
        public Int32 HNC_NetFileRemove(String dstName, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_NetFileRemove(dstName);
        }
        public Int32 HNC_NetMakeDir(String dir, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_NetMakeDir(dir);
        }
        public Int32 HNC_NetRemoveDir(String dir, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_NetRemoveDir(dir);
        }
        public Int32 HNC_RegSetBit(Int32 type, Int32 index, Int32 bit, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_RegSetBit(type, index, bit);
        }
        public Int32 HNC_RegClrBit(Int32 type, Int32 index, Int32 bit, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_RegClrBit(type, index, bit);
        }
        public Int32 HNC_RegGetNum(Int32 type, ref Int32 num, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_RegGetNum(type, ref num);
        }
        public Int32 HNC_RegGetFGBase(Int32 baseType, ref Int32 value, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_RegGetFGBase(baseType, ref value);
        }
        public Int32 HNC_VarSetBit(Int32 type, Int32 no, Int32 index, Int32 bit, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_VarSetBit(type, no, index, bit);
        }
        public Int32 HNC_VarClrBit(Int32 type, Int32 no, Int32 index, Int32 bit, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_VarClrBit(type, no, index, bit);
        }
        public Int32 HNC_MacroVarGetValue(Int32 no, ref SDataUnion var, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_MacroVarGetValue(no, ref var);
        }
        public Int32 HNC_ParamanLoad(String lpFileName, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanLoad(lpFileName);
        }
        public Int32 HNC_ParamanSave(Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSave();
        }
        public Int32 HNC_ParamanSaveAs(String lpFileName, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSaveAs(lpFileName);
        }
        public Int32 HNC_ParamanGetTotalRowNum(ref Int32 rowNum, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1;Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetTotalRowNum(ref rowNum);
        }
        public Int32 HNC_ParamanTransRow2Index(Int32 fileNo, Int32 subNo, Int32 rowNo, ref Int32 index, ref Int16 dupNum, ref Int16 dupNo, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanTransRow2Index(fileNo, subNo, rowNo, ref index, ref dupNum, ref dupNo);
        }
        public Int32 HNC_ParamanTransRowx2Row(Int32 rowx, ref Int32 fileNo, ref Int32 subNo, ref Int32 row, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanTransRowx2Row(rowx, ref fileNo, ref subNo, ref row);
        }
        public Int32 HNC_ParamanTransId2Rowx(Int32 parmId, ref Int32 rowx, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanTransId2Rowx(parmId, ref rowx);
        }
        public Int32 HNC_ParamanRewriteSubClass(Int32 fileNo, Int32 subNo, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanRewriteSubClass(fileNo, subNo);
        }
        public Int32 HNC_ParamanSaveStrFile(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSaveStrFile();
        }
        public Int32 HNC_ParamanGetI32(Int32 fileNo, Int32 subNo, Int32 index, ref Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetI32(fileNo, subNo, index, ref value);
        }
        public Int32 HNC_ParamanSetI32(Int32 fileNo, Int32 subNo, Int32 index, Int32 value, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetI32(fileNo, subNo, index, value);
        }
        public Int32 HNC_ParamanGetFloat(Int32 fileNo, Int32 subNo, Int32 index, ref Double value, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetFloat(fileNo, subNo, index, ref value);
        }
        public Int32 HNC_ParamanSetFloat(Int32 fileNo, Int32 subNo, Int32 index, Double value, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetFloat(fileNo, subNo, index, value);
        }
        public Int32 HNC_ParamanSetStr(Int32 fileNo, Int32 subNo, Int32 index, String value, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetStr(fileNo, subNo, index, value);
        }
        public Int32 HNC_SystemGetUserRealTimeData(Byte[] info, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SystemGetUserRealTimeData(info);
        }
        public Int32 HNC_SystemSetUserRealTimeData(Byte[] info, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SystemSetUserRealTimeData(info);
        }
        public Int32 HNC_CrdsGetMaxNum(Int32 type, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_CrdsGetMaxNum(type);
        }
        public Int32 HNC_CrdsLoad(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_CrdsLoad();
        }
        public Int32 HNC_CrdsSave(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_CrdsSave();
        }
        public Int32 HNC_ToolMagSave(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolMagSave();
        }
        public Int32 HNC_ToolGetMaxMagNum(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolGetMaxMagNum();
        }
        public Int32 HNC_ToolGetMagHeadBase(Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolGetMagHeadBase();
        }
        public Int32 HNC_ToolGetPotDataBase(Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolGetPotDataBase();
        }
        public Int32 HNC_ToolGetMagBase(Int32 magNo, Int32 index, ref Int32 value, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolGetMagBase(magNo, index, ref value);
        }
        public Int32 HNC_ToolSetMagBase(Int32 magNo, Int32 index, Int32 value, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolSetMagBase(magNo, index, value);
        }
        public Int32 HNC_ToolMagGetToolNo(Int32 magNo, Int32 potNo, ref Int32 toolNo, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolMagGetToolNo(magNo, potNo, ref toolNo);
        }
        public Int32 HNC_ToolMagSetToolNo(Int32 magNo, Int32 potNo, Int32 toolNo, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolMagSetToolNo(magNo, potNo, toolNo);
        }
        public Int32 HNC_ToolGetPotAttri(Int32 magNo, Int32 potNo, ref Int32 potAttri, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolGetPotAttri(magNo, potNo, ref potAttri);
        }
        public Int32 HNC_ToolSetPotAttri(Int32 magNo, Int32 potNo, Int32 potAttri, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolSetPotAttri(magNo, potNo, potAttri);
        }
        public Int32 HNC_ToolLoad(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolLoad();
        }
        public Int32 HNC_ToolSave(Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolSave();
        }
        public Int32 HNC_ToolGetMaxToolNum(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolGetMaxToolNum();
        }
        public Int32 HNC_AlarmGetNum(Int32 type, Int32 level, ref Int32 num, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_AlarmGetNum(type, level, ref num);
        }
        public Int32 HNC_AlarmGetHistoryNum(ref Int32 num, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_AlarmGetHistoryNum(ref num);
        }
        public Int32 HNC_AlarmRefresh(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_AlarmRefresh();
        }
        public Int32 HNC_AlarmSaveHistory(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_AlarmSaveHistory();
        }
        public Int32 HNC_AlarmClrHistory(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_AlarmClrHistory();
        }
        public Int32 HNC_AlarmClr(Int32 type, Int32 level, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_AlarmClr(type, level);
        }
        public Int32 HNC_FprogRandomInit(Int32 ch, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_FprogRandomInit(ch);
        }
        public Int32 HNC_FprogRandomLoad(Int32 line, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_FprogRandomLoad(line);
        }
        public Int32 HNC_FprogRandomWriteback(Int32 line, Byte flag, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_FprogRandomWriteback(line, flag);
        }
        public Int32 HNC_SysCtrlSkipToRow(Int32 ch, Int32 row, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlSkipToRow(ch, row);
        }
        public Int32 HNC_FprogRandomExit(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_FprogRandomExit();
        }
        public Int32 HNC_SysCtrlSelectProg(Int32 ch, String name, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlSelectProg(ch, name);
        }
        public Int32 HNC_SysCtrlLoadProg(Int32 ch, String name, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlLoadProg(ch, name);
        }
        public Int32 HNC_SysCtrlResetProg(Int32 ch, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlResetProg(ch);
        }
        public Int32 HNC_SysCtrlStopProg(Int32 ch, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlStopProg(ch);
        }
        public Int32 HNC_SysBackup(Int32 flag, String PathName, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysBackup(flag, PathName);
        }
        public Int32 HNC_SysUpdate(Int32 flag, String PathName, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysUpdate(flag, PathName);
        }
        public Int32 HNC_NetDiskMount(String ip, String progAddr, String name, String pass, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_NetDiskMount(ip, progAddr, name, pass);
        }
        public Int32 HNC_ActivationGetExpDate(ref Int32 flag, Int32[] pDate, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ActivationGetExpDate(ref flag, pDate);
        }
        public Int32 HNC_ActivationGetLastday(ref Int32 flag, ref Int32 day, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ActivationGetLastday(ref flag, ref day);
        }

        //MDI
        public Int32 HNC_SysCtrlMdiTry(Int32 ch, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlMdiTry(ch);
        }
        public Int32 HNC_SysCtrlMdiReq(Int32 ch, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlMdiReq(ch);
        }
        public Int32 HNC_SysCtrlMdiOpen(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlMdiOpen();
        }
        public Int32 HNC_SysCtrlMdiClear(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlMdiClear();
        }
        public Int32 HNC_SysCtrlMdiStop(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlMdiStop();
        }
        public Int32 HNC_SysCtrlMdiClose(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlMdiClose();
        }
        public Int32 HNC_FprogMdiConfirm(Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_FprogMdiConfirm();
        }
        public Int32 HNC_FprogMdiSetContent(String txt, Int32 txtLen, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_FprogMdiSetContent(txt, txtLen);
        }
        public Int32 HNC_SysCtrlMdiUpdate(Int32 rowNum, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlMdiUpdate(rowNum);
        }
        public Int32 HNC_VerifyGetCurveType(Int32 ch, ref Int32 curtype, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VerifyGetCurveType(ch, ref curtype);
        }
        public Int32 HNC_VerifyGetCurveSpos(Int32 ch, Int32 ax, ref Int32 spos, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VerifyGetCurveSpos(ch, ax, ref spos);
        }

        public Int32 HNC_VerifyGetCurveEpos(Int32 ch, Int32 ax, ref Int32 epos, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VerifyGetCurveEpos(ch, ax, ref epos);
        }
        public Int32 HNC_VerifyGetLinePos(Int32 ch, Int32[] pos, ref Int32 flag, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VerifyGetLinePos(ch, pos, ref  flag);
        }
        public Int32 HNC_VerifyClearCurve(Int32 ch, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VerifyClearCurve(ch);
        }
        public Int32 HNC_VerifyGetCurvePoint(Int32 ch, Int32[] pos, ref Int32 vflag, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VerifyGetCurvePoint(ch, pos, ref vflag);
        }
        public Int32 HNC_VerifyCalcuCyclePara(Int32 ch, ref Byte vflag, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VerifyCalcuCyclePara(ch, ref vflag);
        }
        public Int32 HNC_VerifySetChCmdPos(Int32 ch, Int32 ax, Int32 pos, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VerifySetChCmdPos(ch, ax, pos);
        }
        public Int32 HNC_VerifyGetCmdPos(Int32 ax, ref Int32 pos, Int16 clientNo) 
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VerifyGetCmdPos(ax, ref pos); 
        }

        //net api
        public  Int32 HNC_NetGetDllVer(ref String dllVer)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCNET.VERSION_LEN);
            ret = HncApi.HNC_NetGetDllVer(ptr);
            dllVer = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        public  Int32 HNC_NetGetSDKVer(ref String dllPlusVer)
        {
            Int32 ret = -1;
            IntPtr ptr = Marshal.AllocHGlobal(HNCNET.VERSION_LEN);
            ret = HncApi.HNC_NetGetSDKVer(ptr);
            dllPlusVer = Marshal.PtrToStringAnsi(ptr);
            Marshal.FreeHGlobal(ptr);
            return ret;
        }

        //÷ÿ‘ÿ∫Ø ˝
        public  Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Byte value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_RegGetValue(type, index, ref value);
        }

        public  Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Int16 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return  mac.HNC_RegGetValue(type, index, ref value);

        }

        public  Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_RegGetValue(type, index, ref value);
        }

        public  Int32 HNC_RegSetValue(Int32 type, Int32 index, Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_RegSetValue(type, index, value);
        }

        public  Int32 HNC_RegSetValue(Int32 type, Int32 index, Int16 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_RegSetValue(type, index, value);
        }

        public  Int32 HNC_RegSetValue(Int32 type, Int32 index, Byte value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_RegSetValue(type, index, value);
        }

        public  Int32 HNC_ChannelGetValue(Int32 type, Int32 ch, Int32 index, ref String value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
           return mac.HNC_ChannelGetValue(type, ch, index, ref value);
        }

        public  Int32 HNC_ChannelGetValue(Int32 type, Int32 ch, Int32 index, ref Double value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ChannelGetValue(type, ch, index, ref value);
        }

        public  Int32 HNC_ChannelGetValue(Int32 type, Int32 ch, Int32 index, ref Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ChannelGetValue(type, ch, index, ref value);

        }

        public  Int32 HNC_ChannelSetValue(Int32 type, Int32 ch, Int32 index, Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ChannelSetValue(type, ch, index, value);
        }

        public  Int32 HNC_AxisGetValue(Int32 type, Int32 ax, ref String value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_AxisGetValue(type, ax, ref value);
        }

        public  Int32 HNC_AxisGetValue(Int32 type, Int32 ax, ref Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_AxisGetValue(type, ax, ref value);
        }

        public  Int32 HNC_AxisGetValue(Int32 type, Int32 ax, ref Double value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_AxisGetValue(type, ax, ref value);
        }

        public  Int32 HNC_CrdsGetValue(Int32 type, Int32 ax, ref Double value, Int32 ch, Int32 crds, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_CrdsGetValue(type, ax, ref value, ch, crds);
        }

        public  Int32 HNC_CrdsGetValue(Int32 type, Int32 ax, ref Int32 value, Int32 ch, Int32 crds, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_CrdsGetValue(type, ax, ref value, ch, crds);
        }

        public  Int32 HNC_CrdsSetValue(Int32 type, Int32 ax, Double value, Int32 ch, Int32 crds, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_CrdsSetValue(type, ax, value, ch, crds);
        }

        public  Int32 HNC_CrdsSetValue(Int32 type, Int32 ax, Int32 value, Int32 ch, Int32 crds, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_CrdsSetValue(type, ax, value, ch, crds);
        }

        public  Int32 HNC_ToolGetToolPara(Int32 toolNo, Int32 index, ref Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolGetToolPara(toolNo, index, ref value);
        }

        public  Int32 HNC_ToolGetToolPara(Int32 toolNo, Int32 index, ref Double value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolGetToolPara(toolNo, index, ref value);
        }

        public  Int32 HNC_ToolSetToolPara(Int32 toolNo, Int32 index, Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolSetToolPara(toolNo, index, value);
        }

        public  Int32 HNC_ToolSetToolPara(Int32 toolNo, Int32 index, Double value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ToolSetToolPara(toolNo, index, value);
        }

        public  Int32 HNC_VarGetValue(Int32 type, Int32 no, Int32 index, ref Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VarGetValue(type, no, index,ref value);
        }

        public  Int32 HNC_VarGetValue(Int32 type, Int32 no, Int32 index, ref Double value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VarGetValue(type, no, index, ref value);
        }

        public  Int32 HNC_VarSetValue(Int32 type, Int32 no, Int32 index, Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VarSetValue(type, no, index,value);
         }

        public  Int32 HNC_VarSetValue(Int32 type, Int32 no, Int32 index, Double value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_VarSetValue(type, no, index, value);
        }

        public  Int32 HNC_SystemGetValue(Int32 type, ref String value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SystemGetValue(type, ref value);
        }

        public  Int32 HNC_SystemGetValue(Int32 type, ref Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SystemGetValue(type, ref value);
        }

        public  Int32 HNC_SystemSetValue(Int32 type, String value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SystemSetValue(type, value);
        }

        public  Int32 HNC_SystemSetValue(Int32 type, Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SystemSetValue(type, value);
        }

        public  Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, ref Int32 propValue, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetParaPropEx(parmId, propType, ref propValue);
        }

        public  Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, ref Double propValue, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetParaPropEx(parmId, propType, ref propValue);
        }

        public  Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, SByte[] propValue, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetParaPropEx(parmId, propType, propValue);
            
        }

        public  Int32 HNC_ParamanGetParaPropEx(Int32 parmId, Byte propType, ref String propValue, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetParaPropEx(parmId, propType, ref propValue);
        }

        public  Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, Int32 propValue, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return  mac.HNC_ParamanSetParaPropEx(parmId, propType, propValue);
        }

        public  Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, Double propValue, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetParaPropEx(parmId, propType, propValue);
        }

        public  Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, SByte[] propValue, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetParaPropEx(parmId, propType,propValue);
        }

        public  Int32 HNC_ParamanSetParaPropEx(Int32 parmId, Byte propType, String propValue, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetParaPropEx(parmId, propType, propValue);
        }

        public  Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, ref Int32 prop_value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetParaProp(filetype, subid, index, prop_type, ref prop_value);
            
        }

        public  Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, ref Double prop_value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetParaProp(filetype, subid, index, prop_type, ref prop_value);
           
        }

        public  Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, SByte[] prop_value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetParaProp(filetype, subid, index, prop_type, prop_value);
           
        }

        public  Int32 HNC_ParamanGetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, ref String prop_value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetParaProp(filetype, subid, index, prop_type, ref prop_value);
            
        }

        public  Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, Int32 prop_value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetParaProp(filetype, subid, index, prop_type, prop_value);
        }

        public  Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, Double prop_value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetParaProp(filetype, subid, index, prop_type, prop_value);
            
        }

        public  Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, SByte[] prop_value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetParaProp(filetype, subid, index, prop_type, prop_value);
         }

        public  Int32 HNC_ParamanSetParaProp(Int32 filetype, Int32 subid, Int32 index, Byte prop_type, String prop_value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetParaProp(filetype, subid, index, prop_type, prop_value);
        }

        public  Int32 HNC_ParamanGetSubClassProp(Int32 fileNo, Byte propType, ref Int32 propValue, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetSubClassProp(fileNo, propType, ref propValue);
            
        }

        public  Int32 HNC_ParamanGetSubClassProp(Int32 fileNo, Byte propType, ref String propValue, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetSubClassProp(fileNo, propType, ref propValue);
        }

        public  Int32 HNC_MacroVarSetValue(Int32 no, SDataUnion var, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_MacroVarSetValue(no, var);
        }

        public  Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, ref Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetItem(fileNo, subNo, index, ref value);
        }

        public  Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, ref Double value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetItem(fileNo, subNo, index, ref value);
            
        }

        public  Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, SByte[] value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetItem(fileNo, subNo, index, value);
            
        }

        public  Int32 HNC_ParamanGetItem(Int32 fileNo, Int32 subNo, Int32 index, ref String value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetItem(fileNo, subNo, index, ref value);
        }

        public  Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, Int32 value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetItem(fileNo, subNo, index, value);
        }

        public  Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, Double value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetItem(fileNo, subNo, index, value);
        }

        public  Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, SByte[] value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetItem(fileNo, subNo, index, value);
        }

        public  Int32 HNC_ParamanSetItem(Int32 fileNo, Int32 subNo, Int32 index, String value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanSetItem(fileNo, subNo, index, value);
        }

        public  Int32 HNC_NetGetIpaddr(ref String ip, ref UInt16 port, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_NetGetIpaddr(ref ip, ref port);
        }
        public  Int32 HNC_ParamanGetStr(Int32 fileNo, Int32 subNo, Int32 index, ref String value, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetStr(fileNo, subNo, index, ref value);
        }

        public  Int32 HNC_FprogGetFullName(Int32 ch, ref String progName, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_FprogGetFullName(ch, ref progName);
        }

        public  Int32 HNC_ParamanGetFileName(Int32 fileNo, ref String buf, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_ParamanGetFileName(fileNo, ref buf);
         }

        public  Int32 HNC_AlarmGetData(Int32 type, Int32 level, Int32 index, ref Int32 alarmNo, ref String alarmText, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
           return mac.HNC_AlarmGetData(type, level, index, ref alarmNo, ref alarmText);
        }

        public  Int32 HNC_NetFileGetDirInfo(String dirname, ncfind_t[] info, ref UInt16 num, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_NetFileGetDirInfo(dirname, info, ref num);

        }

        public  Int32 HNC_EventPut(SEventElement ev, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_EventPut(ev);
            
        }
        public  Int32 HNC_AlarmGetHistoryData(Int32 index, ref Int32 count, AlarmHisData[] data, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_AlarmGetHistoryData(index, ref count, data);
           
        }
        public  Int32 HNC_SysCtrlMdiGetBlk(Int32[] pos, Int32[] msft, Int32[] ijkr, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SysCtrlMdiGetBlk(pos, msft, ijkr);

        }


        public  Int32 HNC_FprogGetProgPathByIdx(Int32 pindex, ref String progName, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_FprogGetProgPathByIdx(pindex, ref progName);
           
        }

       #endregion
        public Int32 HNC_SamplReset(Int32 ch, Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SamplReset(ch);
        }
        public Int32 HNC_SamplTriggerOn(Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SamplTriggerOn();
        }
        public Int32 HNC_SamplTriggerOff(Int16 clientNo)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return -1; Machine mac = _MachineDic[clientNo];
            return mac.HNC_SamplTriggerOff();
        }
        public void ReceiveEvent(SEventElement evt, Int16 ClientNo)
        {
            if (_MachineDic.ContainsKey(ClientNo) == false) return; Machine mac = _MachineDic[ClientNo];
            mac.ReceiveEvent(evt, ClientNo);
   
        }
        public void ReceiveSample(List<List<Int32>> data, Int16 ClientNo)
        {
            if (_MachineDic.ContainsKey(ClientNo) == false) return; Machine mac = _MachineDic[ClientNo];
            mac.ReceiveSample(data,ClientNo);
            //Console.WriteLine(mac.Ip + ":" + data[0].Count);
     
        }
        public void NewJobAvaliable(Int16 ClientNo,String Path)
        {
             if (_MachineDic.ContainsKey(ClientNo) == false) return; Machine mac = _MachineDic[ClientNo];
             mac.NewJobAvaliable(Path);
        }
        public void SetToolBroken(String MacSN, Int32 ToolId)
        {
            foreach (Int32 key in _MachineDic.Keys)
            {
                if (_MachineDic[key].MachineSN == MacSN)
                {
                    _MachineDic[key].SetToolBroken(ToolId);
                    break;
                }
            }
        }
       public void SetToolBroken(Int16 clientNo,Int32 toolid)
        {
            if (_MachineDic.ContainsKey(clientNo) == false) return; Machine mac = _MachineDic[clientNo];
            mac.SetToolBroken(toolid);
        }
       public void CancelToolBroken(Int16 ClientNo,bool bIsTrueBroken)
        {

            if (_MachineDic.ContainsKey(ClientNo) == false) return; Machine mac = _MachineDic[ClientNo];
            mac.CancelToolBroken(bIsTrueBroken);
        }

    }
}
