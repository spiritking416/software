using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StackExchange.Redis;
using HNCAPI;
using HNCAPI.Data;
using Newtonsoft;

namespace HncDCAgentDll_CS
{
    public class DCAgentApi
    {
        private ConnectionMultiplexer _LocalConnection;
        private static DCAgentApi _DCAgentApi;
        private String _RedisIP;
        //private long lastTimestamp = 0;
        private const int CONNECT_TIMEOUT = 3;
        private const int DB_SUM = 24;
        private Int16 dbNo = 0;

        private DCAgentApi()
        {
            _RedisIP = "127.0.0.1";
        }

        private DCAgentApi(String RedisIP)
        {
            //_LocalDB = LocalRedisDB;
            _RedisIP = RedisIP;
        }

        /// <summary>
        /// 获取指定IP上的Redis数据库
        /// </summary>
        /// <param name="RedisIP"></param>
        /// <returns></returns>
        public static DCAgentApi GetInstance(String RedisIP)
        {
            if (_DCAgentApi == null)
            {
                _DCAgentApi = new DCAgentApi(RedisIP);
            }
            return _DCAgentApi;
        }

        /// <summary>
        /// 获取本地IP(127.0.0.1)的Redis数据库
        /// </summary>
        /// <param name="RedisIP"></param>
        /// <returns></returns>
        public static DCAgentApi GetInstance()
        {
            if (_DCAgentApi == null)
            {
                _DCAgentApi = new DCAgentApi();
            }
            return _DCAgentApi;
        }

        private StackExchange.Redis.ConnectionMultiplexer RedisConnectLocal
        {
            get
            {
                try
                {
                    if (_LocalConnection == null || !_LocalConnection.IsConnected)
                    {
                        _LocalConnection = ConnectionMultiplexer.Connect(_RedisIP + ":6379,allowAdmin=true");

                    }
                    return _LocalConnection;
                }
                catch (Exception ex)
                {
                    Console.WriteLine("RedisConnectLocal: " + ex.Message);
                    return null;
                }
            }
        }

        private StackExchange.Redis.IDatabase LocalDatabase
        {
            get
            {
                if (RedisConnectLocal != null)
                {
                    return RedisConnectLocal.GetDatabase(0);
                }
                return null;
            }
        }

        private StackExchange.Redis.IServer LocalServer
        {
            get
            {
                if (RedisConnectLocal != null)
                {
                    return RedisConnectLocal.GetServer(_RedisIP, 6379);
                }
                return null;
            }
        }

        private int GetKeyValueString(int dbNo, String key, ref String value)
        {
            int ret = -1;
            try
            {
                if (RedisConnectLocal != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).KeyExists(key))
                    {
                        value = this.RedisConnectLocal.GetDatabase(dbNo).StringGet(key);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetKeyValue" + ":" + ex.Message);
            }
            return ret;
        }

        private int GetHashKeyValueString(Int32 dbNo, String key, String hashField, ref String value)
        {
            int ret = -1;
            try
            {
                if (RedisConnectLocal != null)
                {
                    if (this.RedisConnectLocal.GetDatabase(dbNo).HashExists(key, hashField))
                    {
                        value = this.RedisConnectLocal.GetDatabase(dbNo).HashGet(key, hashField).ToString();
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("GetHashKeyValueString" + ":" + ex.Message);
            }
            return ret;
        }

        /// <summary>
        /// 0~255：连接成功，返回值为分配的clientNo
        /// </summary>
        /// <param name="ip"></param>
        /// <param name="port"></param>
        /// <returns></returns>
        public Int16 HNC_NetConnect(String ip, UInt16 port)
        {
            //int ret = -1;
            for (Int16 i = 0; i < DB_SUM; i++)
            {
                String IpInDb = "";
                if (GetKeyValueString(i, "IP", ref IpInDb) == 0)
                {
                    if (ip.Equals(IpInDb))
                    {
                        dbNo = i;
                        return dbNo;
                    }
                }
            }
            return -1;
        }

        /// <summary>
        /// 判断NC是否在线，仅在本地RedisIP处于本地时有效
        /// </summary>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public bool HNC_NetIsConnect(Int16 clientNo)
        {
            bool result = false;
            try
            {
                if (RedisConnectLocal != null && clientNo >= 0)
                {
                    long timestamp = Convert.ToInt64(RedisConnectLocal.GetDatabase(clientNo).StringGet("TimeStamp"));
                    TimeSpan span = System.DateTime.Now - System.DateTime.FromBinary(timestamp);
                    if (span.TotalSeconds < CONNECT_TIMEOUT)
                        result = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_NetIsConnect: " + ex.Message);
            }
            return result;
        }

        //--------------寄存器---------------//
        public int HNC_RegClrBit(Int32 type, Int32 index, Int32 bit, Int16 clientNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "Register";
            m.Index = NCMessageFunction.REG_CLR;
            String regType = "";
            switch (type)
            {
                case (int)HncRegType.REG_TYPE_X:
                    {
                        regType = "X";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_Y:
                    {
                        regType = "Y";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_F:
                    {
                        regType = "F";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_G:
                    {
                        regType = "G";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_R:
                    {
                        regType = "R";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_W:
                    {
                        regType = "W";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_D:
                    {
                        regType = "D";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_B:
                    {
                        regType = "B";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_P:
                    {
                        regType = "P";
                    }
                    break;
                default: break;
            }
            m.Value = "{\"regType\":\"" + regType + "\",\"index\": " + index +
               ",\"value\": " + bit + "}";
            String message = m.ToString();

            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(clientNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_RegClrBit" + ":" + ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// 将寄存器数据某一位置1, 返回值 0 ：成功 -1 ：失败
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="bit"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_RegSetBit(Int32 type, Int32 index, Int32 bit, Int16 clientNo)
        {
            return HNC_RegSetValue(type, index, (Int32)bit, clientNo);
        }

        /// <summary>
        /// 设置寄存器的值, 返回值 0 ：成功 -1 ：失败
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="bit"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_RegSetValue(Int32 type, Int32 index, Int16 value, Int16 clientNo)
        {
            return HNC_RegSetValue(type, index, (Int32)value, clientNo);
        }

        /// <summary>
        /// 设置寄存器的值, 返回值 0 ：成功 -1 ：失败
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="bit"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_RegSetValue(Int32 type, Int32 index, Byte value, Int16 clientNo)
        {
            return HNC_RegSetValue(type, index, (Int32)value, clientNo);
        }

        /// <summary>
        /// 设置寄存器的值, 返回值 0 ：成功 -1 ：失败
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="bit"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_RegSetValue(Int32 type, Int32 index, Int32 value, Int16 clientNo)
        {
            int ret = -1;
            NCMessage m = new NCMessage();
            m.Type = "Register";
            m.Index = NCMessageFunction.REG_SET;
            String regType = "";
            switch (type)
            {
                case (int)HncRegType.REG_TYPE_X:
                    {
                        regType = "X";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_Y:
                    {
                        regType = "Y";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_F:
                    {
                        regType = "F";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_G:
                    {
                        regType = "G";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_R:
                    {
                        regType = "R";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_W:
                    {
                        regType = "W";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_D:
                    {
                        regType = "D";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_B:
                    {
                        regType = "B";
                    }
                    break;
                case (int)HncRegType.REG_TYPE_P:
                    {
                        regType = "P";
                    }
                    break;
                default: break;
            }
            m.Value = "{\"regType\":\"" + regType + "\",\"index\": " + index +
               ",\"value\": " + value + "}";
            String message = m.ToString();

            try
            {
                if (RedisConnectLocal != null)
                {
                    String MachineSN = "";
                    if (GetKeyValueString(clientNo, "Machine", ref MachineSN) == 0)
                    {
                        ISubscriber sub = RedisConnectLocal.GetSubscriber();
                        sub.Publish(MachineSN + ":SetValue", message);
                        ret = 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("HNC_RegSetBit" + ":" + ex.Message);
            }

            return ret;
        }

        /// <summary>
        /// 获取寄存器的值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_RegGetValue(Int32 type, Int32 index, out int value, Int32 clientNo)
        {
            Int32 ret = -1;
            String RegType = "XYFGRWDBP";
            String key = "Register:" + RegType[type];
            String valueStr = "";
            ret = GetHashKeyValueString(clientNo, key, index.ToString("D4"), ref valueStr);
            if (ret == 0)
            {
                switch (type)
                {
                    case (int)HncRegType.REG_TYPE_X://x
                    case (int)HncRegType.REG_TYPE_Y://y
                    case (int)HncRegType.REG_TYPE_R://r
                        //byte value8 = 0;
                        //ret = Instance().HNC_RegGetValue(type, index, ref value8, clientNo);
                        value = byte.Parse(valueStr);
                        break;
                    case (int)HncRegType.REG_TYPE_F://f
                    case (int)HncRegType.REG_TYPE_G://g
                    case (int)HncRegType.REG_TYPE_W://w
                        //Int16 value16 = 0;
                        //ret = Instance().HNC_RegGetValue(type, index, ref value16, clientNo);
                        value = Int16.Parse(valueStr);
                        break;
                    case (int)HncRegType.REG_TYPE_D://d
                    case (int)HncRegType.REG_TYPE_B://b
                    case (int)HncRegType.REG_TYPE_P://p
                        //Int32 value32 = 0;
                        //ret = Instance().HNC_RegGetValue(type, index, ref value32, clientNo);
                        value = Int32.Parse(valueStr);
                        break;
                    default:
                        value = -1;
                        break;
                }
            }
            else
            {
                value = -1;
            }
            return ret;
        }

        /// <summary>
        /// 获取寄存器的值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Byte value, Int16 clientNo)
        {
            Int32 ret = -1;
            String RegType = "XYFGRWDBP";
            String key = "Register:" + RegType[type];
            String valueStr = "";
            ret = GetHashKeyValueString(clientNo, key, index.ToString("D4"), ref valueStr);
            if (ret == 0) {
                value = Byte.Parse(valueStr);
            }
            return ret;
        }

        /// <summary>
        /// 获取寄存器的值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Int16 value, Int16 clientNo)
        {
            Int32 ret = -1;
            String RegType = "XYFGRWDBP";
            String key = "Register:" + RegType[type];
            String valueStr = "";
            ret = GetHashKeyValueString(clientNo, key, index.ToString("D4"), ref valueStr);
            if (ret == 0)
            {
                value = Int16.Parse(valueStr);
            }
            return ret;
        }

        /// <summary>
        /// 获取寄存器的值
        /// </summary>
        /// <param name="type"></param>
        /// <param name="index"></param>
        /// <param name="value"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_RegGetValue(Int32 type, Int32 index, ref Int32 value, Int16 clientNo)
        {
            Int32 ret = -1;
            String RegType = "XYFGRWDBP";
            String key = "Register:" + RegType[type];
            String valueStr = "";
            ret = GetHashKeyValueString(clientNo, key, index.ToString("D4"), ref valueStr);
            if (ret == 0)
            {
                value = Int32.Parse(valueStr);
            }
            return ret;
        }

        /// <summary>
        /// 获取程序的完整名（含路径）
        /// </summary>
        /// <param name="ch">默认值填0</param>
        /// <param name="progName"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_FprogGetFullName(Int32 ch, ref String progName, Int16 clientNo)
        {
            int ret = -1;
            String key = "Channel:" + ch;
            if (GetHashKeyValueString(clientNo, key, "CHAN_RUN_PROG", ref progName) == 0)
            {
                ret = 0;
            }
            return ret;
        }

        /// <summary>
        /// 按索引号获取宏变量的值
        /// </summary>
        /// <param name="no"></param>
        /// <param name="var"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_MacroVarGetValue(Int32 no, ref SDataUnion var, Int16 clientNo)
        {
            int ret = -1;
            String key = "";
            String value = "";
            if (no >= VariablesDef.CHAN_MACRO_START_INDEX && no <= VariablesDef.CHAN_MACRO_END_INDEX)
            {
                key = "MacroVariables:CHAN";
            }
            else if (no >= VariablesDef.SYS_MACRO_START_INDEX && no <= VariablesDef.SYS_MACRO_END_INDEX)
            {
                key = "MacroVariables:System";
            }
            else if (no >= VariablesDef.AXIS_MACRO_START_INDEX && no <= VariablesDef.AXIS_MACRO_END_INDEX)
            {
                key = "MacroVariables:Axis";
            }
            else if (no >= VariablesDef.TOOL_MACRO_START_INDEX && no <= VariablesDef.TOOL_MACRO_END_INDEX)
            {
                key = "MacroVariables:Tool";
            }
            if (GetHashKeyValueString(clientNo, key, no.ToString(), ref value) == 0)
            {
                Variables<SDataUnion> macroVariables = Newtonsoft.Json.JsonConvert.DeserializeObject<Variables<SDataUnion>>(value);
                var = macroVariables.Value;
            }
            return ret;
        }

        /// <summary>
        /// 从下位机加载G代码程序
        /// </summary>
        /// <param name="ch"></param>
        /// <param name="name"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_SysCtrlSelectProg(Int32 ch, String name, Int16 clientNo)
        {
            int ret = -1;
            if (name != null)
            {
                if (name.Length > 0)
                {
                    //if (SetContentToRemoteRedisDB(database, key, hashField, content))
                    {
                        HNCAPI.Data.NC_SetValueMessage m = new HNCAPI.Data.NC_SetValueMessage();
                        m.Type = "SysCtrlSelectProg";
                        m.Index = 0;
                        m.Value = "{\"ch\":\"" + ch + "\","+"\"name\":\"" + name + "\"}";
                        String message = m.ToString();
                        if (RedisConnectLocal != null)
                        {
                            String MachineSN = "";
                            GetKeyValueString(clientNo, "Machine", ref MachineSN);
                            if (MachineSN.Length > 0)
                            {
                                ISubscriber sub = RedisConnectLocal.GetSubscriber();
                                sub.Publish(MachineSN + ":SetValue", message);
                                ret = 0;
                            }
                        }
                    }
                }
            }
            return ret;
        }

        /// <summary>
        /// 按索引号设置宏变量的值
        /// </summary>
        /// <param name="no"></param>
        /// <param name="var"></param>
        /// <param name="clientNo"></param>
        /// <returns></returns>
        public Int32 HNC_MacroVarSetValue(Int32 no, SDataUnion var, Int16 clientNo)
        {
            int ret = -1;
            //if (var != null)
            {
                //if (name.Length > 0)
                {
                    //if (SetContentToRemoteRedisDB(database, key, hashField, content))
                    {
                        HNCAPI.Data.NC_SetValueMessage m = new HNCAPI.Data.NC_SetValueMessage();
                        m.Type = "MacroVariable";
                        m.Index = 0;
                        m.Value = "{\"no\":\"" + no + "\"," + "\"var\":" + Newtonsoft.Json.JsonConvert.SerializeObject(var) + "}";
                        String message = m.ToString();
                        if (RedisConnectLocal != null)
                        {
                            String MachineSN = "";
                            GetKeyValueString(clientNo, "Machine", ref MachineSN);
                            if (MachineSN.Length > 0)
                            {
                                ISubscriber sub = RedisConnectLocal.GetSubscriber();
                                sub.Publish(MachineSN + ":SetValue", message);
                                ret = 0;
                            }
                        }
                    }
                }
            }
            return ret;
        }
    }
}
