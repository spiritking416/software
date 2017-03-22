using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNCAPI
{
    public enum VariablesDataType
    {
        VAR_TYPE_BIT32 = 1,//{Get(Bit32), Set(Bit32)}
        VAR_TYPE_BIT64 = 2,// {Get(Bit64), Set(Bit64)}
    }

    /// <summary>
    /// 变量参数定义
    /// </summary>
    public class VariablesDef
    {
        public static int AXIS_NUM = 32;
        public static int CHAN_NUM = 4;
        public static int SYSTEM_NUM = 1;
        public static int SYSTEM_F_NUM = 1;

        public static int AXIS_MAX_IDNEX = 99;
        public static int CHAN_MAX_IDNEX = 1999;
        public static int SYSTEM_MAX_IDNEX = 9999;
        public static int SYSTEM_F_MAX_IDNEX = 9999;

        public static int CHAN_MACRO_START_INDEX = 0;
        public static int SYS_MACRO_START_INDEX = 40000;
        public static int AXIS_MACRO_START_INDEX = 60000;
        public static int TOOL_MACRO_START_INDEX = 70000;

        public static int CHAN_MACRO_END_INDEX = 2999;
        public static int SYS_MACRO_END_INDEX = 59999;
        public static int AXIS_MACRO_END_INDEX = 69999;
        public static int TOOL_MACRO_END_INDEX = 99999;
    }
}
