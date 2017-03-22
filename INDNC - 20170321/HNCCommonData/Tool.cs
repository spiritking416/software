using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HNCAPI
{
    [Serializable]
    public class Tool
    {
        public String TOOL_TYPE { get; set; }      //刀具所属类型 Bit32
        public String GTOOL_DIR { get; set; }         //方向
        public String GTOOL_LEN1 { get; set; }       //长度1(铣：刀具长度；车：X偏置)
        public String GTOOL_LEN2 { get; set; }       //长度2(车：Y偏置)
        public String GTOOL_LEN3 { get; set; }       //长度3(车：Z偏置)
        public String GTOOL_LEN4 { get; set; }       //长度4
        public String GTOOL_LEN5 { get; set; }       //长度5
        public String GTOOL_RAD1 { get; set; }        //半径1(铣：刀具半径；车：刀尖半径)
        public String GTOOL_RAD2 { get; set; }        //半径2
        public String GTOOL_ANG1 { get; set; }        //角度1
        public String GTOOL_ANG2 { get; set; }        //角度2
        public String WTOOL_LEN1 { get; set; }        //(铣：长度磨损；车：Z磨损)
        public String WTOOL_LEN2 { get; set; }        //长度2
        public String WTOOL_LEN3 { get; set; }        //长度3
        public String WTOOL_LEN4 { get; set; }        //长度4
        public String WTOOL_LEN5 { get; set; }        //长度5
        public String WTOOL_RAD1 { get; set; }        //半径1(铣：半径磨损；车：X磨损)
        public String WTOOL_RAD2 { get; set; }        //半径2
        public String WTOOL_ANG1 { get; set; }        //角度1
        public String WTOOL_ANG2 { get; set; }        //角度2
        public String TETOOL_PARA0 { get; set; }      //刀具工艺相关参数索引
        public String TETOOL_PARA1 { get; set; }      //刀具工艺相关参数索引
        public String TETOOL_PARA2 { get; set; }      //刀具工艺相关参数索引
        public String TETOOL_PARA3 { get; set; }      //刀具工艺相关参数索引
        public String TETOOL_PARA4 { get; set; }      //刀具工艺相关参数索引
        public String TETOOL_PARA5 { get; set; }      //刀具工艺相关参数索引
        public String TETOOL_PARA6 { get; set; }      //刀具工艺相关参数索引
        public String TETOOL_PARA7 { get; set; }      //刀具工艺相关参数索引
        public String TETOOL_PARA8 { get; set; }      //刀具工艺相关参数索引
        public String TETOOL_PARA9 { get; set; }      //刀具工艺相关参数索引
        public String EXTOOL_S_LIMIT { get; set; }    //S转速限制
        public String EXTOOL_F_LIMIT { get; set; }    //F转速限制
        public String EXTOOL_LARGE_LEFT { get; set; } //大刀具干涩左刀位(半刀位)数目
        public String EXTOOL_LARGE_RIGHT { get; set; }//大刀具干涩右刀位(半刀位)数目
        public String MOTOOL_TYPE { get; set; }       //刀具监控类型，按位有效，寿命/计件/磨损，可选多种监控方式同时监控
        public String MOTOOL_SEQU { get; set; }       //优先级
        public String MOTOOL_MULTI { get; set; }      //倍率  
        public String MOTOOL_MAX_LIFE { get; set; }   //最大寿命
        public String MOTOOL_ALM_LIFE { get; set; }   //预警寿命     
        public String MOTOOL_ACT_LIFE { get; set; }   //实际寿命
        public String MOTOOL_MAX_COUNT { get; set; }  //最大计件数
        public String MOTOOL_ALM_COUNT { get; set; }  //预警计件数
        public String MOTOOL_ACT_COUNT { get; set; }  //实际计件数
        public String METOOL_PARA0 { get; set; }      //刀具测量参数个数
        public String METOOL_PARA1 { get; set; }      //刀具测量参数个数
        public String METOOL_PARA2 { get; set; }      //刀具测量参数个数
        public String METOOL_PARA3 { get; set; }      //刀具测量参数个数
        public String METOOL_PARA4 { get; set; }      //刀具测量参数个数
        public String METOOL_PARA5 { get; set; }      //刀具测量参数个数
        public String METOOL_PARA6 { get; set; }      //刀具测量参数个数
        public String METOOL_PARA7 { get; set; }      //刀具测量参数个数
        public String METOOL_PARA8 { get; set; }      //刀具测量参数个数
        public String METOOL_PARA9 { get; set; }      //刀具测量参数个数
        public String INFTOOL_ID { get; set; }        //刀具索引号 Bit32
        public String INFTOOL_MAGZ { get; set; }      //刀具所属刀库号 Bit32
        public String INFTOOL_CH { get; set; }        //刀具所属通道号 Bit32
        public String INFTOOL_STATE { get; set; }     //刀具刀具状态字 Bit32
        public String INFTOOL_G64MODE { get; set; }   //刀具高速高精模式
        public String MOTOOL_GROUP { get; set; }      // 刀具所属分组号
        

        public override String ToString()
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(this);
        }
    }
    
}
