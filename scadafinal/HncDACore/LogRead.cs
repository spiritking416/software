using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using HncDataInterfaces;

namespace HncDACore
{
    //定义结构体接纳所有日志文件头
    public class LogFileHead
    {
        public string fflag;		//文件验证码
        public int fnum;		//文件内容日志数目
    }

    //定义WORK.LOG和FileChange.LOG文件链表数据项
    public class ListData
    {
        //public int second;	// seconds - [0,59]    
        //public int minute;	// minutes - [0,59]
        //public int hour;	// hours   - [0,23]
        //public int hsecond; /* hundredths of seconds */
        //public int day;	// [1,31]
        //public int month;	// [0,11] (January = 0)
        //public int year;	// (current year minus 1900)
        //public int wday;	// Day of week, [0,6] (Sunday = 0)
        public string detail;//日志条目详细内容
        public short type;//日志类型
        /*
         * 加工日志类型代码             内容
         *       0                通道X开始加工程序 
         *       1                通道X暂停加工程序
         *       2                通道X结束加工程序
         *       3                X通道Y轴工件坐标零点由Z变为W 
         *       4                通道X进给修调由Y变为Z
         *       5                通道X快移修调由Y变为Z
         *       6                通道X主轴修调由Y变为Z
         *       7                通道X主轴转速S由Y变为Z
         *       8                通道X指令T由%Y变为Z
         *       9                通道X进给速度F由Y变为Z 
         *       10               通道X第Y组模态由Z变为W
         *       11               警告
         *       12               载入加工程序...
         *       13               工件坐标系G54.X轴Y的零点由Z改为W
         *                        工件坐标系GX轴Y的零点由Z改为W         **两个合为一个**
         *                                                              **GXX内容存在EventCodeContent中***
         *       999              未知类型
         */
        public int? num;  //通道号
        public string SecondData; //日志项目的内容，轴或坐标...等
        public string oldData; //from的数据
        public string newData; //to的数据 
        public DateTime CreateTime;
        public int ID;
        public string ProName;
        public string GXX;
    }
    //表述整个WORK.LOG日志文件对应的结构体
    public class LogData
    {
        public LogFileHead LogHead;
        public List<ListData> LogContent;
    }

    //定义EVENT.LOG文件链表数据项
    public class ListData_Event
    {
        //public int second;	// seconds - [0,59]    
        //public int minute;	// minutes - [0,59]
        //public int hour;	// hours   - [0,23]
        //public int hsecond; /* hundredths of seconds */
        //public int day;	// [1,31]
        //public int month;	// [0,11] (January = 0)
        //public int year;	// (current year minus 1900)
        //public int wday;	// Day of week, [0,6] (Sunday = 0)
        public int src; //事件来源
        /*
         *          事件来源           类型
         *          0                系统事件
         *          1                通道0事件
         *          2                MDI的事件
         *          3                键盘事件
         *          4                轴事件
         *          999                未知事件来源                                    
         */
        public string code; // 事件代码
        public string detail;	 // 整个汉字文本内容（面板上）
        public ushort type; //事件代码具体类别
        /*    
                           类型                                    代码
                         kbNoKey                                    45
                         kbSpace                                    46
                         kbBack                                     47 
                         kbTab                                      48
                         kbEnter                                    49
                         字符键                                     50
                         功能键shift                                51
                         功能键Ctrl                                 52
                         功能键alt                                  53
                         功能键pause                                54
                         功能键Capslk                               55
                         功能键Esc                                  56

                         功能键kbpgup                               57
                         功能键kbpgdn                               58
                         功能键kbend                                59
                         功能键kbhome                               60
                         功能键kbleft                               61
                         功能键kbup                                 62
                         功能键kbRight                              63
                         功能键kbDown                               64
                         功能键kbIns                                65
                         功能键kbDel                                66

                         功能键kbF1                                 67
                         功能键kbF2                                 68
                         功能键kbF3                                 69
                         功能键kbF4                                 70
                         功能键kbF5                                 71
                         功能键kbF6                                 72
                         功能键kbF7                                 73
                         功能键kbF8                                 74
                         功能键kbF9                                 75
                         功能键kbF10                                76
                         功能键kbF11                                77
                         功能键kbF12                                78

                         // ctrl组合键
                         CtrlBase+A                                  79
                         CtrlBase+B                                  80
                         CtrlBase+C                                  81
                         CtrlBase+D                                  82
                         CtrlBase+E                                  83
                         CtrlBase+F                                  84
                         CtrlBase+G                                  85
                         CtrlBase+H                                  86
                         CtrlBase+I                                  87
                         CtrlBase+J                                  88
                         CtrlBase+K                                  89
                         CtrlBase+L                                  90
                         CtrlBase+M                                  91
                         CtrlBase+N                                  92
                         CtrlBase+O                                  93
                         CtrlBase+P                                  94
                         CtrlBase+Q                                  95
                         CtrlBase+R                                  96
                         CtrlBase+S                                  97
                         CtrlBase+T                                  98
                         CtrlBase+U                                  99
                         CtrlBase+V                                  100
                         CtrlBase+W                                  101
                         CtrlBase+X                                  102
                         CtrlBase+Y                                  103
                         CtrlBase+Z                                  104


                         CtrlBase+kbF1                               105
                         CtrlBase+kbF2                               106
                         CtrlBase+kbF3                               107
                         CtrlBase+kbF4                               108
                         CtrlBase+kbF5                               109
                         CtrlBase+kbF6                               110
                         CtrlBase+kbF7                               111
                         CtrlBase+kbF8                               112
                         CtrlBase+kbF9                               113
                         CtrlBase+kbF10                              114
                         CtrlBase+kbF11                              115
                         CtrlBase+kbF12                              116

                         CtrlBase+kbSpace                            117
                         CtrlBase+kbCtrlPgUp                         118
                         CtrlBase+kbCtrlPgDn                         119
                         CtrlBase+kbEnd                              120
                         CtrlBase+kbCtrlHome                         121
                         CtrlBase+kbCtrlLeft                         122
                         CtrlBase+kbCtrlUp                           123
                         CtrlBase+kbCtrlRight                        124
                         CtrlBase+kbCtrlDown                         125
                         CtrlBase+kbCtrlIns                          126
                         CtrlBase+kbCtrlDel                          127
                         CtrlBase+kbCtrlBack                         128
                         CtrlBase+kbCtrlEnter                        129

                         // alt组合键
                         AltBase+kbSpace                             130
                         AltBase+A                                   131
                         AltBase+B                                   132
                         AltBase+C                                   133
                         AltBase+D                                   134
                         AltBase+E                                   135
                         AltBase+F                                   136
                         AltBase+G                                   137
                         AltBase+H                                   138
                         AltBase+I                                   139
                         AltBase+G                                   140
                         AltBase+K                                   141
                         AltBase+L                                   142
                         AltBase+M                                   143
                         AltBase+N                                   144
                         AltBase+O                                   145
                         AltBase+P                                   146
                         AltBase+Q                                   147
                         AltBase+R                                   148
                         AltBase+S                                   149
                         AltBase+T                                   150
                         AltBase+U                                   151
                         AltBase+V                                   152
                         AltBase+W                                   153
                         AltBase+X                                   154
                         AltBase+Y                                   155
                         AltBase+Z                                   156

                         AltBase+1                                   157
                         AltBase+2                                   158
                         AltBase+3                                   159
                         AltBase+4                                   160
                         AltBase+5                                   161
                         AltBase+6                                   162
                         AltBase+7                                   163
                         AltBase+8                                   164
                         AltBase+9                                   165
                         AltBase+0                                   166


                         AltBase+kbF1                                167
                         AltBase+kbF2                                168
                         AltBase+kbF3                                169
                         AltBase+kbF4                                170
                         AltBase+kbF5                                171
                         AltBase+kbF6                                172
                         AltBase+kbF7                                173
                         AltBase+kbF8                                174
                         AltBase+kbF9                                175
                         AltBase+kbF11                               176
                         AltBase+kbF9                                177
                         AltBase+kbF12                               178

                         AltBase+'-'                                 179
                         AltBase+'='                                 180
                         AltBase+kbUp                                181
                         AltBase+kbDown                              182
                         AltBase+kbLeft                              183
                         AltBase+kbRight                             184


                         // shift组合键
                         ShiftBase+kbF1                              185
                         ShiftBase+kbF2                              186
                         ShiftBase+kbF3                              187
                         ShiftBase+kbF4                              188
                         ShiftBase+kbF5                              189
                         ShiftBase+kbF6                              190
                         ShiftBase+kbF7                              191
                         ShiftBase+kbF8                              192
                         ShiftBase+kbF9                              193
                         ShiftBase+kbF10                             194
                         ShiftBase+kbF11                             195
                         ShiftBase+kbF12                             196

                         ShiftBase+kbIns                             197
                         ShiftBase+kbDel                             198
                         ShiftBase+kbTab                             199

                         kbIdle                                      200
                         kbRealRe                                    201


                         // 2. 定义通道事件

                         程序启动                                    202
                         程序结束                                    203
                         Hold完成                                    204
                         break完成                                   205
                         G92完成                                     206
                         上电复位完成                                207
                         重运行完成                                  208
                         MDI准备好                                   209
                         MDI退出                                     210
                         MDI行解释完成                               211
                         程序运行                                    212
                         任意行请求应答                              213
                         任意行准备好                                214
                         断点保存完成                                215
                         断点恢复完成                                216
                         执行到M92等待用户干预                       217
                         外部急停                                    218
                         程序加载完成                                219
                         第一类语法错【修改后可接着运行】            220
                         第二类语法错【修改后从头运行】              221
                         程序中的数据保存指令                        222
                         程序中的数据加载指令                        223
                         G代码修改了刀具数据                         224
                         G代码修改了坐标系数据                       225
                         通道轴组发生了改变                          226
                         通道提示                                    227
                         通道报警                                    228
                         sys_stop_prog完成                           229
                         故障中断                                    230
                         数据打包完成                                231

                         // 3. 定义轴事件
                         轴编码器初始位置过大                        232


                         // 4. 定义系统事件
                         系统断电                                    233
                         保存系统数据                                234
                         系统退出                                    235
                         用户自定义事件                              236
                         请求切换通道                                237
                         请求屏蔽通道                                238
                         event 100 对应用户按键调用指定程序          239
                         硬复位完成                                  241

                         未知代码!                                   999
          */
        public ushort OriginalSrc; //事件来源原始数据
        public ushort OriginalCode; //事件代码原始数据

        public DateTime CreateTime;
        public int ID;
    }
    //表述EVENT.LOG整个日志文件对应的结构体
    public class LogData_Event
    {
        public LogFileHead LogHead;
        public List<ListData_Event> LogContent;
    }

    //定义PANEL.LOG文件链表数据项
    public class ListData_Panel
    {
        //public int second;	// seconds - [0,59]    
        //public int minute;	// minutes - [0,59]
        //public int hour;	// hours   - [0,23]
        //public int hsecond; /* hundredths of seconds */
        //public int day;	// [1,31]
        //public int month;	// [0,11] (January = 0)
        //public int year;	// (current year minus 1900)
        //public int wday;	// Day of week, [0,6] (Sunday = 0)
        public int src; //事件来源
        /*
         *          事件来源           类型
         *          0                系统事件
         *          1                通道0事件
         *          2                MDI的事件
         *          3                键盘事件
         *          4                轴事件
         *          999              未知事件来源                                    
         */
        public string code; // 事件内容
        public string content;	 // 整个汉字文本内容
        public ushort type; //事件代码具体类别
        /*    
                           类型                                    代码
                         kbNoKey                                    45
                         kbSpace                                    46
                         kbBack                                     47 
                         kbTab                                      48
                         kbEnter                                    49
                         字符键                                     50
                         功能键shift                                51
                         功能键Ctrl                                 52
                         功能键alt                                  53
                         功能键pause                                54
                         功能键Capslk                               55
                         功能键Esc                                  56

                         功能键kbpgup                               57
                         功能键kbpgdn                               58
                         功能键kbend                                59
                         功能键kbhome                               60
                         功能键kbleft                               61
                         功能键kbup                                 62
                         功能键kbRight                              63
                         功能键kbDown                               64
                         功能键kbIns                                65
                         功能键kbDel                                66

                         功能键kbF1                                 67
                         功能键kbF2                                 68
                         功能键kbF3                                 69
                         功能键kbF4                                 70
                         功能键kbF5                                 71
                         功能键kbF6                                 72
                         功能键kbF7                                 73
                         功能键kbF8                                 74
                         功能键kbF9                                 75
                         功能键kbF10                                76
                         功能键kbF11                                77
                         功能键kbF12                                78

                         // ctrl组合键
                         CtrlBase+A                                  79
                         CtrlBase+B                                  80
                         CtrlBase+C                                  81
                         CtrlBase+D                                  82
                         CtrlBase+E                                  83
                         CtrlBase+F                                  84
                         CtrlBase+G                                  85
                         CtrlBase+H                                  86
                         CtrlBase+I                                  87
                         CtrlBase+J                                  88
                         CtrlBase+K                                  89
                         CtrlBase+L                                  90
                         CtrlBase+M                                  91
                         CtrlBase+N                                  92
                         CtrlBase+O                                  93
                         CtrlBase+P                                  94
                         CtrlBase+Q                                  95
                         CtrlBase+R                                  96
                         CtrlBase+S                                  97
                         CtrlBase+T                                  98
                         CtrlBase+U                                  99
                         CtrlBase+V                                  100
                         CtrlBase+W                                  101
                         CtrlBase+X                                  102
                         CtrlBase+Y                                  103
                         CtrlBase+Z                                  104


                         CtrlBase+kbF1                               105
                         CtrlBase+kbF2                               106
                         CtrlBase+kbF3                               107
                         CtrlBase+kbF4                               108
                         CtrlBase+kbF5                               109
                         CtrlBase+kbF6                               110
                         CtrlBase+kbF7                               111
                         CtrlBase+kbF8                               112
                         CtrlBase+kbF9                               113
                         CtrlBase+kbF10                              114
                         CtrlBase+kbF11                              115
                         CtrlBase+kbF12                              116

                         CtrlBase+kbSpace                            117
                         CtrlBase+kbCtrlPgUp                         118
                         CtrlBase+kbCtrlPgDn                         119
                         CtrlBase+kbEnd                              120
                         CtrlBase+kbCtrlHome                         121
                         CtrlBase+kbCtrlLeft                         122
                         CtrlBase+kbCtrlUp                           123
                         CtrlBase+kbCtrlRight                        124
                         CtrlBase+kbCtrlDown                         125
                         CtrlBase+kbCtrlIns                          126
                         CtrlBase+kbCtrlDel                          127
                         CtrlBase+kbCtrlBack                         128
                         CtrlBase+kbCtrlEnter                        129

                         // alt组合键
                         AltBase+kbSpace                             130
                         AltBase+A                                   131
                         AltBase+B                                   132
                         AltBase+C                                   133
                         AltBase+D                                   134
                         AltBase+E                                   135
                         AltBase+F                                   136
                         AltBase+G                                   137
                         AltBase+H                                   138
                         AltBase+I                                   139
                         AltBase+G                                   140
                         AltBase+K                                   141
                         AltBase+L                                   142
                         AltBase+M                                   143
                         AltBase+N                                   144
                         AltBase+O                                   145
                         AltBase+P                                   146
                         AltBase+Q                                   147
                         AltBase+R                                   148
                         AltBase+S                                   149
                         AltBase+T                                   150
                         AltBase+U                                   151
                         AltBase+V                                   152
                         AltBase+W                                   153
                         AltBase+X                                   154
                         AltBase+Y                                   155
                         AltBase+Z                                   156

                         AltBase+1                                   157
                         AltBase+2                                   158
                         AltBase+3                                   159
                         AltBase+4                                   160
                         AltBase+5                                   161
                         AltBase+6                                   162
                         AltBase+7                                   163
                         AltBase+8                                   164
                         AltBase+9                                   165
                         AltBase+0                                   166


                         AltBase+kbF1                                167
                         AltBase+kbF2                                168
                         AltBase+kbF3                                169
                         AltBase+kbF4                                170
                         AltBase+kbF5                                171
                         AltBase+kbF6                                172
                         AltBase+kbF7                                173
                         AltBase+kbF8                                174
                         AltBase+kbF9                                175
                         AltBase+kbF11                               176
                         AltBase+kbF9                                177
                         AltBase+kbF12                               178

                         AltBase+'-'                                 179
                         AltBase+'='                                 180
                         AltBase+kbUp                                181
                         AltBase+kbDown                              182
                         AltBase+kbLeft                              183
                         AltBase+kbRight                             184


                         // shift组合键
                         ShiftBase+kbF1                              185
                         ShiftBase+kbF2                              186
                         ShiftBase+kbF3                              187
                         ShiftBase+kbF4                              188
                         ShiftBase+kbF5                              189
                         ShiftBase+kbF6                              190
                         ShiftBase+kbF7                              191
                         ShiftBase+kbF8                              192
                         ShiftBase+kbF9                              193
                         ShiftBase+kbF10                             194
                         ShiftBase+kbF11                             195
                         ShiftBase+kbF12                             196

                         ShiftBase+kbIns                             197
                         ShiftBase+kbDel                             198
                         ShiftBase+kbTab                             199

                         kbIdle                                      200
                         kbRealRe                                    201


                         // 2. 定义通道事件

                         程序启动                                    202
                         程序结束                                    203
                         Hold完成                                    204
                         break完成                                   205
                         G92完成                                     206
                         上电复位完成                                207
                         重运行完成                                  208
                         MDI准备好                                   209
                         MDI退出                                     210
                         MDI行解释完成                               211
                         程序运行                                    212
                         任意行请求应答                              213
                         任意行准备好                                214
                         断点保存完成                                215
                         断点恢复完成                                216
                         执行到M92等待用户干预                       217
                         外部急停                                    218
                         程序加载完成                                219
                         第一类语法错【修改后可接着运行】            220
                         第二类语法错【修改后从头运行】              221
                         程序中的数据保存指令                        222
                         程序中的数据加载指令                        223
                         G代码修改了刀具数据                         224
                         G代码修改了坐标系数据                       225
                         通道轴组发生了改变                          226
                         通道提示                                    227
                         通道报警                                    228
                         sys_stop_prog完成                           229
                         故障中断                                    230
                         数据打包完成                                231

                         // 3. 定义轴事件
                         轴编码器初始位置过大                        232


                         // 4. 定义系统事件
                         系统断电                                    233
                         保存系统数据                                234
                         系统退出                                    235
                         用户自定义事件                              236
                         请求切换通道                                237
                         请求屏蔽通道                                238
                         event 100 对应用户按键调用指定程序          239
                         硬复位完成                                  241

                         未知代码!                                   999
          */
        public ushort OriginalSrc; //事件来源原始数据
        public ushort OriginalCode; //事件内容原始数据

        public string detail;//面板上显示的内容

        public DateTime CreateTime;
        public int ID;
    }
    //表述PANEL.LOG整个日志文件对应的结构体
    public class LogData_Panel
    {
        public LogFileHead LogHead;
        public List<ListData_Panel> LogContent;
    }

    //定义FileChange.LOG文件链表数据项
    public class ListData_FileChange
    {
        public DateTime CreateTime;
        public string detail;//日志条目详细内容
        public int ID;
    }
    //表述整个FileChange.LOG日志文件对应的结构体
    public class LogData_FileChange
    {
        public LogFileHead LogHead;
        public List<ListData_FileChange> LogContent;
    }

    public  class LogsRead
    {
        //WORK.LOG文件读取接口
        public static LogData LogRead(string filename)
        {
            LogData myLogData = new LogData { };
            List<ListData> ListLink = new List<ListData>();
            LogFileHead lfh = new LogFileHead { };
            byte[] datahead = new byte[64];
            FileStream aFile = new FileStream(filename, FileMode.Open);
            aFile.Read(datahead, 0, 16);
            string ss = Encoding.Default.GetString(datahead);
            ss = ss.Substring(0, ss.IndexOf('\0'));
            aFile.Read(datahead, 0, 4);
            int rizhishu = datahead[0] & 0xFF;
            rizhishu |= ((datahead[1] << 8) & 0xFF00);
            rizhishu |= ((datahead[2] << 16) & 0xFF0000);
            rizhishu |= (int)((datahead[3] << 24) & 0xFF000000);
            lfh.fnum = rizhishu;
            lfh.fflag = ss;
            myLogData.LogHead = lfh;
            int IDNo = 0;
            for (int j = 1; j <= rizhishu; j++) //现在是前10条记录，要输出全部10<-rizhishu替代
            {
                int[] DateTime = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    aFile.Read(datahead, 0, 4);
                    int num = datahead[0] & 0xFF;
                    num |= ((datahead[1] << 8) & 0xFF00);
                    num |= ((datahead[2] << 16) & 0xFF0000);
                    num |= (int)((datahead[3] << 24) & 0xFF000000);
                    DateTime[i] = num;
                }
                aFile.Read(datahead, 0, 64);
                string s = Encoding.Default.GetString(datahead);
                s = s.Substring(0, s.IndexOf('\0'));

                int? readNum = 999;  //通道号
                string readItem = null;   //日志某个项目的内容，轴或坐标...等
                string readOldData = null; //日志中存储的旧数据
                string readNewData = null; //日志中存储的新数据
                short tp = 999;  //日志类型
                string nameOfProgram = null; //载入加工程序的名字，只用于MyEx12
                string myGXX = null; //用于MyEx13中暂存GXX的数据

                Regex MyEx0 = new Regex(@"^通道(\d{1,})开始加工程序$");
                Regex MyEx1 = new Regex(@"^通道(\d{1,})暂停加工程序$");
                Regex MyEx2 = new Regex(@"^通道(\d{1,})结束加工程序$");
                Regex MyEx3 = new Regex(@"^(\d{1,})通道(.+)轴工件坐标零点由(.{1,})变为(.{1,})$");
                Regex MyEx4 = new Regex(@"^通道(\d{1,})进给修调由(\d{1,})变为(\d{1,})$");
                Regex MyEx5 = new Regex(@"^通道(\d{1,})快移修调由(\d{1,})变为(\d{1,})$");
                Regex MyEx6 = new Regex(@"^通道(\d{1,})主轴修调由(\d{1,})变为(\d{1,})$");
                Regex MyEx7 = new Regex(@"^通道(\d{1,})主轴转速S由(.+)变为(.+)$");
                Regex MyEx8 = new Regex(@"^通道(\d{1,})指令T由(\d{4})变为(\d{4})$");
                Regex MyEx9 = new Regex(@"^通道(\d{1,})进给速度F由(\w+)变为(\w+)$");
                Regex MyEx10 = new Regex(@"^通道(\d{1,})第(\d{1,})组模态由(.+)变为(.+)$");
                Regex MyEx11 = new Regex("^警告(.+)$");
                Regex MyEx12 = new Regex("^载入加工程序(.+)$");
                Regex MyEx13 = new Regex("^工件坐标系(.+)轴(.+)的零点由(.{1,})改为(.{1,})$");

                if (MyEx0.IsMatch(s))
                {
                    tp = 0;
                    Match m = MyEx0.Match(s);
                    readNum = Convert.ToInt32(Convert.ToString(m.Groups[1]));
                }
                else if (MyEx1.IsMatch(s))
                {
                    tp = 1;
                    Match m = MyEx1.Match(s);
                    readNum = Convert.ToInt32(Convert.ToString(m.Groups[1]));
                }
                else if (MyEx2.IsMatch(s))
                {
                    tp = 2;
                    Match m = MyEx2.Match(s);
                    readNum = Convert.ToInt32(Convert.ToString(m.Groups[1]));
                }
                else if (MyEx3.IsMatch(s))
                {
                    tp = 3;
                    Match m = MyEx3.Match(s);
                    readNum = Convert.ToInt32(Convert.ToString(m.Groups[1]));
                    readItem = Convert.ToString(m.Groups[2]);
                    readOldData = Convert.ToString(m.Groups[3]);
                    readNewData = Convert.ToString(m.Groups[4]);
                }
                else if (MyEx4.IsMatch(s))
                {
                    tp = 4;
                    Match m = MyEx4.Match(s);
                    readNum = Convert.ToInt32(Convert.ToString(m.Groups[1]));
                    readOldData = Convert.ToString(m.Groups[2]);
                    readNewData = Convert.ToString(m.Groups[3]);
                }
                else if (MyEx5.IsMatch(s))
                {
                    tp = 5;
                    Match m = MyEx5.Match(s);
                    readNum = Convert.ToInt32(Convert.ToString(m.Groups[1]));
                    readOldData = Convert.ToString(m.Groups[2]);
                    readNewData = Convert.ToString(m.Groups[3]);
                }
                else if (MyEx6.IsMatch(s))
                {
                    tp = 6;
                    Match m = MyEx6.Match(s);
                    readNum = Convert.ToInt32(Convert.ToString(m.Groups[1]));
                    readOldData = Convert.ToString(m.Groups[2]);
                    readNewData = Convert.ToString(m.Groups[3]);
                }
                else if (MyEx7.IsMatch(s))
                {
                    tp = 7;
                    Match m = MyEx7.Match(s);
                    readNum = Convert.ToInt32(Convert.ToString(m.Groups[1]));
                    readOldData = Convert.ToString(m.Groups[2]);
                    readNewData = Convert.ToString(m.Groups[3]);
                }
                else if (MyEx8.IsMatch(s))
                {
                    tp = 8;
                    Match m = MyEx8.Match(s);
                    readNum = Convert.ToInt32(Convert.ToString(m.Groups[1]));
                    readOldData = Convert.ToString(m.Groups[2]);
                    readNewData = Convert.ToString(m.Groups[3]);
                }
                else if (MyEx9.IsMatch(s))
                {
                    tp = 9;
                    Match m = MyEx9.Match(s);
                    readNum = Convert.ToInt32(Convert.ToString(m.Groups[1]));
                    readOldData = Convert.ToString(m.Groups[2]);
                    readNewData = Convert.ToString(m.Groups[3]);
                }
                else if (MyEx10.IsMatch(s))
                {
                    tp = 10;
                    Match m = MyEx10.Match(s);
                    readNum = Convert.ToInt32(Convert.ToString(m.Groups[1]));
                    readItem = Convert.ToString(m.Groups[2]);
                    readOldData = Convert.ToString(m.Groups[3]);
                    readNewData = Convert.ToString(m.Groups[4]);
                }
                else if (MyEx11.IsMatch(s))
                {
                    tp = 11;
                    readNum = null;
                }
                else if (MyEx12.IsMatch(s))
                {
                    tp = 12;
                    readNum = null;
                    Match m = MyEx12.Match(s);
                    nameOfProgram = Convert.ToString(m.Groups[1]);
                }
                else if (MyEx13.IsMatch(s))
                {
                    tp = 13;
                    readNum = null;
                    Match m = MyEx13.Match(s);
                    myGXX = Convert.ToString(m.Groups[1]);
                    readItem = Convert.ToString(m.Groups[2]);
                    readOldData = Convert.ToString(m.Groups[3]);
                    readNewData = Convert.ToString(m.Groups[4]);
                } 
                if (tp == 999)
                {
                    readItem = "unknow!";
                    readOldData = "unknow!";
                    readNewData = "unknow!";
                }
                IDNo++;
                ListLink.Add(new ListData()
                {
                    //second = DateTime[0],
                    //minute = DateTime[1],
                    //hour = DateTime[2],
                    //hsecond = DateTime[3],
                    //day = DateTime[4],
                    //month = DateTime[5],
                    //year = DateTime[6],
                    //wday = DateTime[7],
                    detail = s,
                    type = tp,
                    num = readNum,
                    SecondData = readItem,
                    oldData = readOldData,
                    newData = readNewData,
                    CreateTime = new DateTime(DateTime[6], DateTime[5], DateTime[4], DateTime[2], DateTime[1], DateTime[0]),
                    ID = IDNo,
                    ProName = nameOfProgram,
                    GXX = myGXX
                });
                myLogData.LogContent = ListLink;

            }
            aFile.Close();
            return myLogData;
        }
        //EVENT.LOG文件读取接口
        public static LogData_Event LogRead_Event(string filename)
        {
            #region
            // 一. 定义事件来源
            const ushort EV_SRC_SYS = 0x010;		// 系统事件
            const ushort EV_SRC_CH0 = 0x100;		// 通道0事件 0x100~0x10f
            const ushort EV_SRC_MDI = 0x110;		// MDI的事件 
            const ushort EV_SRC_KBD = 0x200;		// 键盘事件
            const ushort EV_SRC_AX0 = 0x300;		// 轴事件
            // 二. 定义事件代码
            // 1. 定义键盘代码
            //								0x  0	009
            //								   ---	---
            //						组号		|	|         子码
            //						------------	--------------
            //				0：单键组，标准ASCII码，目前包含两类，1）字符键，0x00yy
            //														2）功能键, 0x01yy
            //				1：ctrl组，组合键=0x1000+单键值
            //				2：alt组，组合键=0x2000+单键值
            //				3：shift组，组合键=0x3000+单键值
            //				7：其他
            //				暂不考虑三键组合键
            //

            //            const ushort kbNoKey=		0x0000;

            // 00---字符键，标准ASCA码定义：'A'~'Z','0'~'9','*','!'......

            const ushort kbSpace = 0x0020;
            const ushort kbBack = 0x0008;
            const ushort kbTab = 0x0009;
            const ushort kbEnter = 0x000d;
            // 01---功能键

            const ushort kbShift = 0x0110;
            const ushort kbCtrl = 0x0111;
            const ushort kbAlt = 0x0112;
            const ushort kbPause = 0x0113;
            const ushort kbCapsLk = 0x0114;
            const ushort kbEsc = 0x011b;

            const ushort kbPgUp = 0x0121;
            const ushort kbPgDn = 0x0122;
            const ushort kbEnd = 0x0123;
            const ushort kbHome = 0x0124;
            const ushort kbLeft = 0x0125;
            const ushort kbUp = 0x0126;
            const ushort kbRight = 0x0127;
            const ushort kbDown = 0x0128;
            const ushort kbIns = 0x012d;
            const ushort kbDel = 0x012e;

            const ushort kbF1 = 0x0170;
            const ushort kbF2 = 0x0171;
            const ushort kbF3 = 0x0172;
            const ushort kbF4 = 0x0173;
            const ushort kbF5 = 0x0174;
            const ushort kbF6 = 0x0175;
            const ushort kbF7 = 0x0176;
            const ushort kbF8 = 0x0177;
            const ushort kbF9 = 0x0178;
            const ushort kbF10 = 0x0179;
            const ushort kbF11 = 0x017a;
            const ushort kbF12 = 0x017b;

            // ctrl组合键
            const ushort CtrlBase = 0x1000;

            const ushort kbCtrlA = (CtrlBase + 'A');
            const ushort kbCtrlB = (CtrlBase + 'B');
            const ushort kbCtrlC = (CtrlBase + 'C');
            const ushort kbCtrlD = (CtrlBase + 'D');
            const ushort kbCtrlE = (CtrlBase + 'E');
            const ushort kbCtrlF = (CtrlBase + 'F');
            const ushort kbCtrlG = (CtrlBase + 'G');
            const ushort kbCtrlH = (CtrlBase + 'H');
            const ushort kbCtrlI = (CtrlBase + 'I');
            const ushort kbCtrlJ = (CtrlBase + 'J');
            const ushort kbCtrlK = (CtrlBase + 'K');
            const ushort kbCtrlL = (CtrlBase + 'L');
            const ushort kbCtrlM = (CtrlBase + 'M');
            const ushort kbCtrlN = (CtrlBase + 'N');
            const ushort kbCtrlO = (CtrlBase + 'O');
            const ushort kbCtrlP = (CtrlBase + 'P');
            const ushort kbCtrlQ = (CtrlBase + 'Q');
            const ushort kbCtrlR = (CtrlBase + 'R');
            const ushort kbCtrlS = (CtrlBase + 'S');
            const ushort kbCtrlT = (CtrlBase + 'T');
            const ushort kbCtrlU = (CtrlBase + 'U');
            const ushort kbCtrlV = (CtrlBase + 'V');
            const ushort kbCtrlW = (CtrlBase + 'W');
            const ushort kbCtrlX = (CtrlBase + 'X');
            const ushort kbCtrlY = (CtrlBase + 'Y');
            const ushort kbCtrlZ = (CtrlBase + 'Z');

            const ushort kbCtrlF1 = (CtrlBase + kbF1);
            const ushort kbCtrlF2 = (CtrlBase + kbF2);
            const ushort kbCtrlF3 = (CtrlBase + kbF3);
            const ushort kbCtrlF4 = (CtrlBase + kbF4);
            const ushort kbCtrlF5 = (CtrlBase + kbF5);
            const ushort kbCtrlF6 = (CtrlBase + kbF6);
            const ushort kbCtrlF7 = (CtrlBase + kbF7);
            const ushort kbCtrlF8 = (CtrlBase + kbF8);
            const ushort kbCtrlF9 = (CtrlBase + kbF9);
            const ushort kbCtrlF10 = (CtrlBase + kbF10);
            const ushort kbCtrlF11 = (CtrlBase + kbF11);
            const ushort kbCtrlF12 = (CtrlBase + kbF12);

            const ushort kbCtrlSpace = (CtrlBase + kbSpace);
            const ushort kbCtrlPgUp = (CtrlBase + kbPgUp);
            const ushort kbCtrlPgDn = (CtrlBase + kbPgDn);
            const ushort kbCtrlEnd = (CtrlBase + kbEnd);
            const ushort kbCtrlHome = (CtrlBase + kbHome);
            const ushort kbCtrlLeft = (CtrlBase + kbLeft);
            const ushort kbCtrlUp = (CtrlBase + kbUp);
            const ushort kbCtrlRight = (CtrlBase + kbRight);
            const ushort kbCtrlDown = (CtrlBase + kbDown);
            const ushort kbCtrlIns = (CtrlBase + kbIns);
            const ushort kbCtrlDel = (CtrlBase + kbDel);
            const ushort kbCtrlBack = (CtrlBase + kbBack);
            const ushort kbCtrlEnter = (CtrlBase + kbEnter);

            // alt组合键
            const ushort AltBase = 0x2000;

            const ushort kbAltSpace = (AltBase + kbSpace);

            const ushort kbAltA = (AltBase + 'A');
            const ushort kbAltB = (AltBase + 'B');
            const ushort kbAltC = (AltBase + 'C');
            const ushort kbAltD = (AltBase + 'D');
            const ushort kbAltE = (AltBase + 'E');
            const ushort kbAltF = (AltBase + 'F');
            const ushort kbAltG = (AltBase + 'G');
            const ushort kbAltH = (AltBase + 'H');
            const ushort kbAltI = (AltBase + 'I');
            const ushort kbAltJ = (AltBase + 'J');
            const ushort kbAltK = (AltBase + 'K');
            const ushort kbAltL = (AltBase + 'L');
            const ushort kbAltM = (AltBase + 'M');
            const ushort kbAltN = (AltBase + 'N');
            const ushort kbAltO = (AltBase + 'O');
            const ushort kbAltP = (AltBase + 'P');
            const ushort kbAltQ = (AltBase + 'Q');
            const ushort kbAltR = (AltBase + 'R');
            const ushort kbAltS = (AltBase + 'S');
            const ushort kbAltT = (AltBase + 'T');
            const ushort kbAltU = (AltBase + 'U');
            const ushort kbAltV = (AltBase + 'V');
            const ushort kbAltW = (AltBase + 'W');
            const ushort kbAltX = (AltBase + 'X');
            const ushort kbAltY = (AltBase + 'Y');
            const ushort kbAltZ = (AltBase + 'Z');

            const ushort kbAlt1 = (AltBase + '1');
            const ushort kbAlt2 = (AltBase + '2');
            const ushort kbAlt3 = (AltBase + '3');
            const ushort kbAlt4 = (AltBase + '4');
            const ushort kbAlt5 = (AltBase + '5');
            const ushort kbAlt6 = (AltBase + '6');
            const ushort kbAlt7 = (AltBase + '7');
            const ushort kbAlt8 = (AltBase + '8');
            const ushort kbAlt9 = (AltBase + '9');
            const ushort kbAlt0 = (AltBase + '0');

            const ushort kbAltF1 = (AltBase + kbF1);
            const ushort kbAltF2 = (AltBase + kbF2);
            const ushort kbAltF3 = (AltBase + kbF3);
            const ushort kbAltF4 = (AltBase + kbF4);
            const ushort kbAltF5 = (AltBase + kbF5);
            const ushort kbAltF6 = (AltBase + kbF6);
            const ushort kbAltF7 = (AltBase + kbF7);
            const ushort kbAltF8 = (AltBase + kbF8);
            const ushort kbAltF9 = (AltBase + kbF9);
            const ushort kbAltF10 = (AltBase + kbF10);
            const ushort kbAltF11 = (AltBase + kbF11);
            const ushort kbAltF12 = (AltBase + kbF12);

            const ushort kbAltMinus = (AltBase + '-');
            const ushort kbAltEqual = (AltBase + '=');

            const ushort kbAltUp = (AltBase + kbUp);
            const ushort kbAltDown = (AltBase + kbDown);
            const ushort kbAltLeft = (AltBase + kbLeft);
            const ushort kbAltRight = (AltBase + kbRight);

            // shift组合键
            const ushort ShiftBase = 0x3000;

            const ushort kbShiftF1 = (ShiftBase + kbF1);
            const ushort kbShiftF2 = (ShiftBase + kbF2);
            const ushort kbShiftF3 = (ShiftBase + kbF3);
            const ushort kbShiftF4 = (ShiftBase + kbF4);
            const ushort kbShiftF5 = (ShiftBase + kbF5);
            const ushort kbShiftF6 = (ShiftBase + kbF6);
            const ushort kbShiftF7 = (ShiftBase + kbF7);
            const ushort kbShiftF8 = (ShiftBase + kbF8);
            const ushort kbShiftF9 = (ShiftBase + kbF9);
            const ushort kbShiftF10 = (ShiftBase + kbF10);
            const ushort kbShiftF11 = (ShiftBase + kbF11);
            const ushort kbShiftF12 = (ShiftBase + kbF12);

            const ushort kbShiftIns = (ShiftBase + kbIns);
            const ushort kbShiftDel = (ShiftBase + kbDel);
            const ushort kbShiftTab = (ShiftBase + kbTab);

            const int kbIdle = (0x7fff);
            const int kbRealRe = (0x7ffe);	// 只要系统查询事件队列无条件执行

            //            const int kbReset	=	kbCtrlZ; 	


            // 2. 定义通道事件
            const int ncEvtPrgStart = 0xa001;	// 程序启动
            const int ncEvtPrgEnd = 0xa002;	// 程序结束
            const int ncEvtPrgHold = 0xa003;	// Hold完成	
            const int ncEvtPrgBreak = 0xa004;	// break完成	
            const int ncEvtG92Fin = 0xa005;	// G92完成
            const int ncEvtRstFin = 0xa006;	// 上电复位完成
            const int ncEvtRwndFin = 0xa007;	// 重运行完成
            const int ncEvtMdiRdy = 0xa008;	// MDI准备好
            const int ncEvtMdiExit = 0xa009;	// MDI退出
            const int ncEvtMdiAck = 0xa00a;	// MDI行解释完成
            const int ncEvtRunStart = 0xa00b;	// 程序运行

            const int ncEvtRunRowAck = 0xa00d;	// 任意行请求应答
            const int ncEvtRunRowRdy = 0xa00e;	// 任意行准备好

            const int ncEvtBpSaved = 0xa011;	// 断点保存完成
            const int ncEvtBpResumed = 0xa012;	// 断点恢复完成
            const int ncEvtIntvHold = 0xa013;	// 执行到M92等待用户干预
            const int ncEvtEstop = 0xa014;	// 外部急停
            const int ncEvtLoadOK = 0xa015;	// 程序加载完成

            const int ncEvtSyntax1 = 0xa016;	// 第一类语法错【修改后可接着运行】
            const int ncEvtSyntax2 = 0xa017;	// 第二类语法错【修改后从头运行】

            const int ncEvtGcodeSave = 0xa018;	// 程序中的数据保存指令
            const int ncEvtLoadData = 0xa019;	// 程序中的数据加载指令
            const int ncEvtChgTool = 0xa01a;	// G代码修改了刀具数据
            const int ncEvtChgCrds = 0xa01b;	// G代码修改了坐标系数据
            const int ncEvtChgAxes = 0xa01c;	// 通道轴组发生了改变

            const int ncEvtNckNote = 0xa01e;	// 通道提示
            const int ncEvtNckAlarm = 0xa01f;	// 通道报警
            const int ncEvtStopAck = 0xa020;// sys_stop_prog完成
            const int ncEvtFaultIrq = 0xa030;	// 故障中断
            const int ncEvtPackFin = 0xa040;	// 数据打包完成

            // 3. 定义轴事件
            const int ncEvtMaxEncPos = 0xa201;	// 轴编码器初始位置过大

            // 4. 定义系统事件

            const int ncEvtPoweroff = 0xa800;	// 系统断电
            const int ncEvtSaveData = 0xa801;	// 保存系统数据
            const int ncEvtSysExit = 0xa802;	// 系统退出
            const int ncEvtUserStart = 0xb000;	// 用户自定义事件
            const int ncEvtUserReqChn = (ncEvtUserStart + 1);	// 请求切换通道
            const int ncEvtUserReqMsk = (ncEvtUserStart + 2);	// 请求屏蔽通道
            const int ncEvtUserFunc1 = (ncEvtUserStart + 100);	// event 100 对应用户按键调用指定程序
            const int ncEvtUserFunc2 = (ncEvtUserStart + 101);	// event 100 对应用户按键调用指定程序
            const int ncEvtHardRstFin = (ncEvtUserStart + 102);	// 硬复位完成

            #endregion
            LogData_Event myLogData_Event = new LogData_Event { }; //定义待返回的数据结构
            List<ListData_Event> ListLink = new List<ListData_Event>(); //放入返回数据结构的部分数据成员---链表
            LogFileHead lfh = new LogFileHead { };//定义转入返回数据结构数据成员---文件头

            #region
            byte[] datahead = new byte[64];
            FileStream aFile = new FileStream(filename, FileMode.Open);
            aFile.Read(datahead, 0, 16);
            string ss = Encoding.Default.GetString(datahead);
            ss = ss.Substring(0, ss.IndexOf('\0'));
            aFile.Read(datahead, 0, 4);
            int rizhishu = datahead[0] & 0xFF;
            rizhishu |= ((datahead[1] << 8) & 0xFF00);
            rizhishu |= ((datahead[2] << 16) & 0xFF0000);
            rizhishu |= (int)((datahead[3] << 24) & 0xFF000000);
            lfh.fnum = rizhishu;
            lfh.fflag = ss;
            myLogData_Event.LogHead = lfh;  //文件头存入返回数据结构中
            #endregion

            int IDNo = 0;
            for (int j = 1; j <= rizhishu; j++) //现在是前10条记录，要输出全部10<-rizhishu替代
            {
                //读取时间
                int[] DateTime = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    aFile.Read(datahead, 0, 4);
                    int num = datahead[0] & 0xFF;
                    num |= ((datahead[1] << 8) & 0xFF00);
                    num |= ((datahead[2] << 16) & 0xFF0000);
                    num |= (int)((datahead[3] << 24) & 0xFF000000);
                    DateTime[i] = num;
                }
                //读取事件来源
                aFile.Read(datahead, 0, 2);
                int src = datahead[0] & 0xFF;
                src |= ((datahead[1] << 8) & 0xFF00);
                src |= ((datahead[2] << 16) & 0xFF0000);
                src |= (int)((datahead[3] << 24) & 0xFF000000);
                ushort SrcVal = (ushort)src;
                ushort myOriginalSrc = SrcVal;
                int mysrc;
                string myevent;
                if ((SrcVal >= EV_SRC_CH0) && (SrcVal <= EV_SRC_CH0 + 15))
                {
                    myevent = "通道0事件";
                    mysrc = 1;
                }
                else switch (SrcVal)
                    {
                        case EV_SRC_SYS: { myevent = "系统事件"; mysrc = 0; break; }
                        case EV_SRC_MDI: { myevent = "MDI的事件"; mysrc = 2; break; }
                        case EV_SRC_KBD: { myevent = "键盘事件"; mysrc = 3; break; }
                        case EV_SRC_AX0: { myevent = "轴事件"; mysrc = 4; break; }
                        default: { myevent = "未知事件来源！"; mysrc = 999; break; }
                    }
                aFile.Read(datahead, 0, 2);
                src = datahead[0] & 0xFF;
                src |= ((datahead[1] << 8) & 0xFF00);
                src |= ((datahead[2] << 16) & 0xFF0000);
                src |= (int)((datahead[3] << 24) & 0xFF000000);
                SrcVal = (ushort)src;
                ushort myOriginalCode = SrcVal;
                string mysource;
                ushort mytype;

                #region
                if ((SrcVal & 0xff00) == 0x0000)
                {
                    if ((SrcVal & 0x00ff) == 0x0020) { mysource = "kbSpace"; mytype = 46; }
                    else if ((SrcVal & 0x00ff) == 0x0008) { mysource = "kbBack"; mytype = 47; }
                    else if ((SrcVal & 0x00ff) == 0x0009) { mysource = "kbTab"; mytype = 48; }
                    else if ((SrcVal & 0x00ff) == 0x000d) { mysource = "kbEnter"; mytype = 49; }
                    else { mysource = Convert.ToString(Convert.ToChar(SrcVal)); mytype = 50; }
                    if (SrcVal == 0) { mysource = null; mytype = 45; }
                }
                else
                    switch (SrcVal)
                    {
                        // 01---功能键
                        case kbShift: { mysource = "功能键shift"; mytype = 51; break; }
                        case kbCtrl: { mysource = "功能键Ctrl"; mytype = 52; break; }
                        case kbAlt: { mysource = "功能键alt"; mytype = 53; break; }
                        case kbPause: { mysource = "功能键pause"; mytype = 54; break; }
                        case kbCapsLk: { mysource = "功能键Capslk"; mytype = 55; break; }
                        case kbEsc: { mysource = "功能键Esc"; mytype = 56; break; }

                        case kbPgUp: { mysource = "功能键kbpgup"; mytype = 57; break; }
                        case kbPgDn: { mysource = "功能键kbpgdn"; mytype = 58; break; }
                        case kbEnd: { mysource = "功能键kbend"; mytype = 59; break; }
                        case kbHome: { mysource = "功能键kbhome"; mytype = 60; break; }
                        case kbLeft: { mysource = "功能键kbleft"; mytype = 61; break; }
                        case kbUp: { mysource = "功能键kbup"; mytype = 62; break; }
                        case kbRight: { mysource = "功能键kbRight"; mytype = 63; break; }
                        case kbDown: { mysource = "功能键kbDown"; mytype = 64; break; }
                        case kbIns: { mysource = "功能键kbIns"; mytype = 65; break; }
                        case kbDel: { mysource = "功能键kbDel"; mytype = 66; break; }

                        case kbF1: { mysource = "功能键kbF1"; mytype = 67; break; }
                        case kbF2: { mysource = "功能键kbF2"; mytype = 68; break; }
                        case kbF3: { mysource = "功能键kbF3"; mytype = 69; break; }
                        case kbF4: { mysource = "功能键kbF4"; mytype = 70; break; }
                        case kbF5: { mysource = "功能键kbF5"; mytype = 71; break; }
                        case kbF6: { mysource = "功能键kbF6"; mytype = 72; break; }
                        case kbF7: { mysource = "功能键kbF7"; mytype = 73; break; }
                        case kbF8: { mysource = "功能键kbF8"; mytype = 74; break; }
                        case kbF9: { mysource = "功能键kbF9"; mytype = 75; break; }
                        case kbF10: { mysource = "功能键kbF10"; mytype = 76; break; }
                        case kbF11: { mysource = "功能键kbF11"; mytype = 77; break; }
                        case kbF12: { mysource = "功能键kbF12"; mytype = 78; break; }

                        // ctrl组合键
                        case kbCtrlA: { mysource = "CtrlBase+A"; mytype = 79; break; }
                        case kbCtrlB: { mysource = "CtrlBase+B"; mytype = 80; break; }
                        case kbCtrlC: { mysource = "CtrlBase+C"; mytype = 81; break; }
                        case kbCtrlD: { mysource = "CtrlBase+D"; mytype = 82; break; }
                        case kbCtrlE: { mysource = "CtrlBase+E"; mytype = 83; break; }
                        case kbCtrlF: { mysource = "CtrlBase+F"; mytype = 84; break; }
                        case kbCtrlG: { mysource = "CtrlBase+G"; mytype = 85; break; }
                        case kbCtrlH: { mysource = "CtrlBase+H"; mytype = 86; break; }
                        case kbCtrlI: { mysource = "CtrlBase+I"; mytype = 87; break; }
                        case kbCtrlJ: { mysource = "CtrlBase+J"; mytype = 88; break; }
                        case kbCtrlK: { mysource = "CtrlBase+K"; mytype = 89; break; }
                        case kbCtrlL: { mysource = "CtrlBase+L"; mytype = 90; break; }
                        case kbCtrlM: { mysource = "CtrlBase+M"; mytype = 91; break; }
                        case kbCtrlN: { mysource = "CtrlBase+N"; mytype = 92; break; }
                        case kbCtrlO: { mysource = "CtrlBase+O"; mytype = 93; break; }
                        case kbCtrlP: { mysource = "CtrlBase+P"; mytype = 94; break; }
                        case kbCtrlQ: { mysource = "CtrlBase+Q"; mytype = 95; break; }
                        case kbCtrlR: { mysource = "CtrlBase+R"; mytype = 96; break; }
                        case kbCtrlS: { mysource = "CtrlBase+S"; mytype = 97; break; }
                        case kbCtrlT: { mysource = "CtrlBase+T"; mytype = 98; break; }
                        case kbCtrlU: { mysource = "CtrlBase+U"; mytype = 99; break; }
                        case kbCtrlV: { mysource = "CtrlBase+V"; mytype = 100; break; }
                        case kbCtrlW: { mysource = "CtrlBase+W"; mytype = 101; break; }
                        case kbCtrlX: { mysource = "CtrlBase+X"; mytype = 102; break; }
                        case kbCtrlY: { mysource = "CtrlBase+Y"; mytype = 103; break; }
                        case kbCtrlZ: { mysource = "CtrlBase+Z"; mytype = 104; break; }


                        case kbCtrlF1: { mysource = "CtrlBase+kbF1"; mytype = 105; break; }
                        case kbCtrlF2: { mysource = "CtrlBase+kbF2"; mytype = 106; break; }
                        case kbCtrlF3: { mysource = "CtrlBase+kbF3"; mytype = 107; break; }
                        case kbCtrlF4: { mysource = "CtrlBase+kbF4"; mytype = 108; break; }
                        case kbCtrlF5: { mysource = "CtrlBase+kbF5"; mytype = 109; break; }
                        case kbCtrlF6: { mysource = "CtrlBase+kbF6"; mytype = 110; break; }
                        case kbCtrlF7: { mysource = "CtrlBase+kbF7"; mytype = 111; break; }
                        case kbCtrlF8: { mysource = "CtrlBase+kbF8"; mytype = 112; break; }
                        case kbCtrlF9: { mysource = "CtrlBase+kbF9"; mytype = 113; break; }
                        case kbCtrlF10: { mysource = "CtrlBase+kbF10"; mytype = 114; break; }
                        case kbCtrlF11: { mysource = "CtrlBase+kbF11"; mytype = 115; break; }
                        case kbCtrlF12: { mysource = "CtrlBase+kbF12"; mytype = 116; break; }

                        case kbCtrlSpace: { mysource = "CtrlBase+kbSpace"; mytype = 117; break; }
                        case kbCtrlPgUp: { mysource = "CtrlBase+kbCtrlPgUp"; mytype = 118; break; }
                        case kbCtrlPgDn: { mysource = "CtrlBase+kbCtrlPgDn"; mytype = 119; break; }
                        case kbCtrlEnd: { mysource = "CtrlBase+kbEnd"; mytype = 120; break; }
                        case kbCtrlHome: { mysource = "CtrlBase+kbCtrlHome"; mytype = 121; break; }
                        case kbCtrlLeft: { mysource = "CtrlBase+kbCtrlLeft"; mytype = 122; break; }
                        case kbCtrlUp: { mysource = "CtrlBase+kbCtrlUp"; mytype = 123; break; }
                        case kbCtrlRight: { mysource = "CtrlBase+kbCtrlRight"; mytype = 124; break; }
                        case kbCtrlDown: { mysource = "CtrlBase+kbCtrlDown"; mytype = 125; break; }
                        case kbCtrlIns: { mysource = "CtrlBase+kbCtrlIns"; mytype = 126; break; }
                        case kbCtrlDel: { mysource = "CtrlBase+kbCtrlDel"; mytype = 127; break; }
                        case kbCtrlBack: { mysource = "CtrlBase+kbCtrlBack"; mytype = 128; break; }
                        case kbCtrlEnter: { mysource = "CtrlBase+kbCtrlEnter"; mytype = 129; break; }

                        // alt组合键
                        case kbAltSpace: { mysource = "AltBase+kbSpace"; mytype = 130; break; }
                        case kbAltA: { mysource = "AltBase+A"; mytype = 131; break; }
                        case kbAltB: { mysource = "AltBase+B"; mytype = 132; break; }
                        case kbAltC: { mysource = "AltBase+C"; mytype = 133; break; }
                        case kbAltD: { mysource = "AltBase+D"; mytype = 134; break; }
                        case kbAltE: { mysource = "AltBase+E"; mytype = 135; break; }
                        case kbAltF: { mysource = "AltBase+F"; mytype = 136; break; }
                        case kbAltG: { mysource = "AltBase+G"; mytype = 137; break; }
                        case kbAltH: { mysource = "AltBase+H"; mytype = 138; break; }
                        case kbAltI: { mysource = "AltBase+I"; mytype = 139; break; }
                        case kbAltJ: { mysource = "AltBase+G"; mytype = 140; break; }
                        case kbAltK: { mysource = "AltBase+K"; mytype = 141; break; }
                        case kbAltL: { mysource = "AltBase+L"; mytype = 142; break; }
                        case kbAltM: { mysource = "AltBase+M"; mytype = 143; break; }
                        case kbAltN: { mysource = "AltBase+N"; mytype = 144; break; }
                        case kbAltO: { mysource = "AltBase+O"; mytype = 145; break; }
                        case kbAltP: { mysource = "AltBase+P"; mytype = 146; break; }
                        case kbAltQ: { mysource = "AltBase+Q"; mytype = 147; break; }
                        case kbAltR: { mysource = "AltBase+R"; mytype = 148; break; }
                        case kbAltS: { mysource = "AltBase+S"; mytype = 149; break; }
                        case kbAltT: { mysource = "AltBase+T"; mytype = 150; break; }
                        case kbAltU: { mysource = "AltBase+U"; mytype = 151; break; }
                        case kbAltV: { mysource = "AltBase+V"; mytype = 152; break; }
                        case kbAltW: { mysource = "AltBase+W"; mytype = 153; break; }
                        case kbAltX: { mysource = "AltBase+X"; mytype = 154; break; }
                        case kbAltY: { mysource = "AltBase+Y"; mytype = 155; break; }
                        case kbAltZ: { mysource = "AltBase+Z"; mytype = 156; break; }

                        case kbAlt1: { mysource = "AltBase+1"; mytype = 157; break; }
                        case kbAlt2: { mysource = "AltBase+2"; mytype = 158; break; }
                        case kbAlt3: { mysource = "AltBase+3"; mytype = 159; break; }
                        case kbAlt4: { mysource = "AltBase+4"; mytype = 160; break; }
                        case kbAlt5: { mysource = "AltBase+5"; mytype = 161; break; }
                        case kbAlt6: { mysource = "AltBase+6"; mytype = 162; break; }
                        case kbAlt7: { mysource = "AltBase+7"; mytype = 163; break; }
                        case kbAlt8: { mysource = "AltBase+8"; mytype = 164; break; }
                        case kbAlt9: { mysource = "AltBase+9"; mytype = 165; break; }
                        case kbAlt0: { mysource = "AltBase+0"; mytype = 166; break; }


                        case kbAltF1: { mysource = "AltBase+kbF1"; mytype = 167; break; }
                        case kbAltF2: { mysource = "AltBase+kbF2"; mytype = 168; break; }
                        case kbAltF3: { mysource = "AltBase+kbF3"; mytype = 169; break; }
                        case kbAltF4: { mysource = "AltBase+kbF4"; mytype = 170; break; }
                        case kbAltF5: { mysource = "AltBase+kbF5"; mytype = 171; break; }
                        case kbAltF6: { mysource = "AltBase+kbF6"; mytype = 172; break; }
                        case kbAltF7: { mysource = "AltBase+kbF7"; mytype = 173; break; }
                        case kbAltF8: { mysource = "AltBase+kbF8"; mytype = 174; break; }
                        case kbAltF9: { mysource = "AltBase+kbF9"; mytype = 175; break; }
                        case kbAltF10: { mysource = "AltBase+kbF11"; mytype = 176; break; }
                        case kbAltF11: { mysource = "AltBase+kbF9"; mytype = 177; break; }
                        case kbAltF12: { mysource = "AltBase+kbF12"; mytype = 178; break; }

                        case kbAltMinus: { mysource = "AltBase+'-'"; mytype = 179; break; }
                        case kbAltEqual: { mysource = "AltBase+'='"; mytype = 180; break; }
                        case kbAltUp: { mysource = "AltBase+kbUp"; mytype = 181; break; }
                        case kbAltDown: { mysource = "AltBase+kbDown"; mytype = 182; break; }
                        case kbAltLeft: { mysource = "AltBase+kbLeft"; mytype = 183; break; }
                        case kbAltRight: { mysource = "AltBase+kbRight"; mytype = 184; break; }


                        // shift组合键
                        case kbShiftF1: { mysource = "ShiftBase+kbF1"; mytype = 185; break; }
                        case kbShiftF2: { mysource = "ShiftBase+kbF2"; mytype = 186; break; }
                        case kbShiftF3: { mysource = "ShiftBase+kbF3"; mytype = 187; break; }
                        case kbShiftF4: { mysource = "ShiftBase+kbF4"; mytype = 188; break; }
                        case kbShiftF5: { mysource = "ShiftBase+kbF5"; mytype = 189; break; }
                        case kbShiftF6: { mysource = "ShiftBase+kbF6"; mytype = 190; break; }
                        case kbShiftF7: { mysource = "ShiftBase+kbF7"; mytype = 191; break; }
                        case kbShiftF8: { mysource = "ShiftBase+kbF8"; mytype = 192; break; }
                        case kbShiftF9: { mysource = "ShiftBase+kbF9"; mytype = 193; break; }
                        case kbShiftF10: { mysource = "ShiftBase+kbF10"; mytype = 194; break; }
                        case kbShiftF11: { mysource = "ShiftBase+kbF11"; mytype = 195; break; }
                        case kbShiftF12: { mysource = "ShiftBase+kbF12"; mytype = 196; break; }

                        case kbShiftIns: { mysource = "ShiftBase+kbIns"; mytype = 197; break; }
                        case kbShiftDel: { mysource = "ShiftBase+kbDel"; mytype = 198; break; }
                        case kbShiftTab: { mysource = "ShiftBase+kbTab"; mytype = 199; break; }

                        case kbIdle: { mysource = "kbIdle"; mytype = 200; break; }
                        case kbRealRe: { mysource = "kbRealRe"; mytype = 201; break; }


                        // 2. 定义通道事件

                        case ncEvtPrgStart: { mysource = "程序启动"; mytype = 202; break; }
                        case ncEvtPrgEnd: { mysource = "程序结束"; mytype = 203; break; }
                        case ncEvtPrgHold: { mysource = "Hold完成"; mytype = 204; break; }
                        case ncEvtPrgBreak: { mysource = "break完成"; mytype = 205; break; }
                        case ncEvtG92Fin: { mysource = "G92完成"; mytype = 206; break; }
                        case ncEvtRstFin: { mysource = "上电复位完成"; mytype = 207; break; }
                        case ncEvtRwndFin: { mysource = "重运行完成"; mytype = 208; break; }
                        case ncEvtMdiRdy: { mysource = "MDI准备好"; mytype = 209; break; }
                        case ncEvtMdiExit: { mysource = "MDI退出"; mytype = 210; break; }
                        case ncEvtMdiAck: { mysource = "MDI行解释完成"; mytype = 211; break; }
                        case ncEvtRunStart: { mysource = "程序运行"; mytype = 212; break; }
                        case ncEvtRunRowAck: { mysource = "任意行请求应答"; mytype = 213; break; }
                        case ncEvtRunRowRdy: { mysource = "任意行准备好"; mytype = 214; break; }
                        case ncEvtBpSaved: { mysource = "断点保存完成"; mytype = 215; break; }
                        case ncEvtBpResumed: { mysource = "断点恢复完成"; mytype = 216; break; }
                        case ncEvtIntvHold: { mysource = "执行到M92等待用户干预"; mytype = 217; break; }
                        case ncEvtEstop: { mysource = "外部急停"; mytype = 218; break; }
                        case ncEvtLoadOK: { mysource = "程序加载完成"; mytype = 219; break; }
                        case ncEvtSyntax1: { mysource = "第一类语法错【修改后可接着运行】"; mytype = 220; break; }
                        case ncEvtSyntax2: { mysource = "第二类语法错【修改后从头运行】"; mytype = 221; break; }
                        case ncEvtGcodeSave: { mysource = "程序中的数据保存指令"; mytype = 222; break; }
                        case ncEvtLoadData: { mysource = "程序中的数据加载指令"; mytype = 223; break; }
                        case ncEvtChgTool: { mysource = "G代码修改了刀具数据"; mytype = 224; break; }
                        case ncEvtChgCrds: { mysource = "G代码修改了坐标系数据"; mytype = 225; break; }
                        case ncEvtChgAxes: { mysource = "通道轴组发生了改变"; mytype = 226; break; }
                        case ncEvtNckNote: { mysource = "通道提示"; mytype = 227; break; }
                        case ncEvtNckAlarm: { mysource = "通道报警"; mytype = 228; break; }
                        case ncEvtStopAck: { mysource = "sys_stop_prog完成"; mytype = 229; break; }
                        case ncEvtFaultIrq: { mysource = "故障中断"; mytype = 230; break; }
                        case ncEvtPackFin: { mysource = "数据打包完成"; mytype = 231; break; }

                        // 3. 定义轴事件
                        case ncEvtMaxEncPos: { mysource = "轴编码器初始位置过大"; mytype = 232; break; }


                        // 4. 定义系统事件
                        case ncEvtPoweroff: { mysource = "系统断电"; mytype = 233; break; }
                        case ncEvtSaveData: { mysource = "保存系统数据"; mytype = 234; break; }
                        case ncEvtSysExit: { mysource = "系统退出"; mytype = 235; break; }
                        case ncEvtUserStart: { mysource = "用户自定义事件"; mytype = 236; break; }
                        case ncEvtUserReqChn: { mysource = "请求切换通道"; mytype = 237; break; }
                        case ncEvtUserReqMsk: { mysource = "请求屏蔽通道"; mytype = 238; break; }
                        case ncEvtUserFunc1: { mysource = "event 100 对应用户按键调用指定程序"; mytype = 239; break; }
                        case ncEvtUserFunc2: { mysource = " event 100 对应用户按键调用指定程序"; mytype = 239; break; }
                        case ncEvtHardRstFin: { mysource = "硬复位完成"; mytype = 241; break; }

                        default: { mysource = "未知代码!"; mytype = 999; break; }
                    }
                #endregion
                IDNo++;
                ListLink.Add(new ListData_Event()
                {
                    //second = DateTime[0],
                    //minute = DateTime[1],
                    //hour = DateTime[2],
                    //hsecond = DateTime[3],
                    //day = DateTime[4],
                    //month = DateTime[5],
                    //year = DateTime[6],
                    //wday = DateTime[7],
                    src = mysrc,
                    code = mysource,
                    detail = myevent + mysource,
                    type = mytype,
                    OriginalSrc = myOriginalSrc,
                    OriginalCode = myOriginalCode,
                    CreateTime = new DateTime(DateTime[6], DateTime[5], DateTime[4], DateTime[2], DateTime[1], DateTime[0]),
                    ID = IDNo
                });
                myLogData_Event.LogContent = ListLink;
            }
            aFile.Close();
            return myLogData_Event;
        }
        //PANEL.LOG文件读取接口
        public static LogData_Panel LogRead_Panel(string filename)
        {
            #region
            // 一. 定义事件来源
            const ushort EV_SRC_ONOFF = 0x0000;   //开关机事件，看做系统事件
            const ushort EV_SRC_SYS = 0x010;		// 系统事件
            const ushort EV_SRC_CH0 = 0x100;		// 通道0事件 0x100~0x10f
            const ushort EV_SRC_MDI = 0x110;		// MDI的事件 
            const ushort EV_SRC_KBD = 0x200;		// 键盘事件
            const ushort EV_SRC_AX0 = 0x300;		// 轴事件
            // 二. 定义事件代码
            // 1. 定义键盘代码
            //								0x  0	009
            //								   ---	---
            //						组号		|	|         子码
            //						------------	--------------
            //				0：单键组，标准ASCII码，目前包含两类，1）字符键，0x00yy
            //														2）功能键, 0x01yy
            //				1：ctrl组，组合键=0x1000+单键值
            //				2：alt组，组合键=0x2000+单键值
            //				3：shift组，组合键=0x3000+单键值
            //				7：其他
            //				暂不考虑三键组合键
            //

            //            const ushort kbNoKey=		0x0000;

            // 00---字符键，标准ASCA码定义：'A'~'Z','0'~'9','*','!'......

            const ushort kbSpace = 0x0020;
            const ushort kbBack = 0x0008;
            const ushort kbTab = 0x0009;
            const ushort kbEnter = 0x000d;
            // 01---功能键

            const ushort kbShift = 0x0110;
            const ushort kbCtrl = 0x0111;
            const ushort kbAlt = 0x0112;
            const ushort kbPause = 0x0113;
            const ushort kbCapsLk = 0x0114;
            const ushort kbEsc = 0x011b;

            const ushort kbPgUp = 0x0121;
            const ushort kbPgDn = 0x0122;
            const ushort kbEnd = 0x0123;
            const ushort kbHome = 0x0124;
            const ushort kbLeft = 0x0125;
            const ushort kbUp = 0x0126;
            const ushort kbRight = 0x0127;
            const ushort kbDown = 0x0128;
            const ushort kbIns = 0x012d;
            const ushort kbDel = 0x012e;

            const ushort kbF1 = 0x0170;
            const ushort kbF2 = 0x0171;
            const ushort kbF3 = 0x0172;
            const ushort kbF4 = 0x0173;
            const ushort kbF5 = 0x0174;
            const ushort kbF6 = 0x0175;
            const ushort kbF7 = 0x0176;
            const ushort kbF8 = 0x0177;
            const ushort kbF9 = 0x0178;
            const ushort kbF10 = 0x0179;
            const ushort kbF11 = 0x017a;
            const ushort kbF12 = 0x017b;

            // ctrl组合键
            const ushort CtrlBase = 0x1000;

            const ushort kbCtrlA = (CtrlBase + 'A');
            const ushort kbCtrlB = (CtrlBase + 'B');
            const ushort kbCtrlC = (CtrlBase + 'C');
            const ushort kbCtrlD = (CtrlBase + 'D');
            const ushort kbCtrlE = (CtrlBase + 'E');
            const ushort kbCtrlF = (CtrlBase + 'F');
            const ushort kbCtrlG = (CtrlBase + 'G');
            const ushort kbCtrlH = (CtrlBase + 'H');
            const ushort kbCtrlI = (CtrlBase + 'I');
            const ushort kbCtrlJ = (CtrlBase + 'J');
            const ushort kbCtrlK = (CtrlBase + 'K');
            const ushort kbCtrlL = (CtrlBase + 'L');
            const ushort kbCtrlM = (CtrlBase + 'M');
            const ushort kbCtrlN = (CtrlBase + 'N');
            const ushort kbCtrlO = (CtrlBase + 'O');
            const ushort kbCtrlP = (CtrlBase + 'P');
            const ushort kbCtrlQ = (CtrlBase + 'Q');
            const ushort kbCtrlR = (CtrlBase + 'R');
            const ushort kbCtrlS = (CtrlBase + 'S');
            const ushort kbCtrlT = (CtrlBase + 'T');
            const ushort kbCtrlU = (CtrlBase + 'U');
            const ushort kbCtrlV = (CtrlBase + 'V');
            const ushort kbCtrlW = (CtrlBase + 'W');
            const ushort kbCtrlX = (CtrlBase + 'X');
            const ushort kbCtrlY = (CtrlBase + 'Y');
            const ushort kbCtrlZ = (CtrlBase + 'Z');

            const ushort kbCtrlF1 = (CtrlBase + kbF1);
            const ushort kbCtrlF2 = (CtrlBase + kbF2);
            const ushort kbCtrlF3 = (CtrlBase + kbF3);
            const ushort kbCtrlF4 = (CtrlBase + kbF4);
            const ushort kbCtrlF5 = (CtrlBase + kbF5);
            const ushort kbCtrlF6 = (CtrlBase + kbF6);
            const ushort kbCtrlF7 = (CtrlBase + kbF7);
            const ushort kbCtrlF8 = (CtrlBase + kbF8);
            const ushort kbCtrlF9 = (CtrlBase + kbF9);
            const ushort kbCtrlF10 = (CtrlBase + kbF10);
            const ushort kbCtrlF11 = (CtrlBase + kbF11);
            const ushort kbCtrlF12 = (CtrlBase + kbF12);

            const ushort kbCtrlSpace = (CtrlBase + kbSpace);
            const ushort kbCtrlPgUp = (CtrlBase + kbPgUp);
            const ushort kbCtrlPgDn = (CtrlBase + kbPgDn);
            const ushort kbCtrlEnd = (CtrlBase + kbEnd);
            const ushort kbCtrlHome = (CtrlBase + kbHome);
            const ushort kbCtrlLeft = (CtrlBase + kbLeft);
            const ushort kbCtrlUp = (CtrlBase + kbUp);
            const ushort kbCtrlRight = (CtrlBase + kbRight);
            const ushort kbCtrlDown = (CtrlBase + kbDown);
            const ushort kbCtrlIns = (CtrlBase + kbIns);
            const ushort kbCtrlDel = (CtrlBase + kbDel);
            const ushort kbCtrlBack = (CtrlBase + kbBack);
            const ushort kbCtrlEnter = (CtrlBase + kbEnter);

            // alt组合键
            const ushort AltBase = 0x2000;

            const ushort kbAltSpace = (AltBase + kbSpace);

            const ushort kbAltA = (AltBase + 'A');
            const ushort kbAltB = (AltBase + 'B');
            const ushort kbAltC = (AltBase + 'C');
            const ushort kbAltD = (AltBase + 'D');
            const ushort kbAltE = (AltBase + 'E');
            const ushort kbAltF = (AltBase + 'F');
            const ushort kbAltG = (AltBase + 'G');
            const ushort kbAltH = (AltBase + 'H');
            const ushort kbAltI = (AltBase + 'I');
            const ushort kbAltJ = (AltBase + 'J');
            const ushort kbAltK = (AltBase + 'K');
            const ushort kbAltL = (AltBase + 'L');
            const ushort kbAltM = (AltBase + 'M');
            const ushort kbAltN = (AltBase + 'N');
            const ushort kbAltO = (AltBase + 'O');
            const ushort kbAltP = (AltBase + 'P');
            const ushort kbAltQ = (AltBase + 'Q');
            const ushort kbAltR = (AltBase + 'R');
            const ushort kbAltS = (AltBase + 'S');
            const ushort kbAltT = (AltBase + 'T');
            const ushort kbAltU = (AltBase + 'U');
            const ushort kbAltV = (AltBase + 'V');
            const ushort kbAltW = (AltBase + 'W');
            const ushort kbAltX = (AltBase + 'X');
            const ushort kbAltY = (AltBase + 'Y');
            const ushort kbAltZ = (AltBase + 'Z');

            const ushort kbAlt1 = (AltBase + '1');
            const ushort kbAlt2 = (AltBase + '2');
            const ushort kbAlt3 = (AltBase + '3');
            const ushort kbAlt4 = (AltBase + '4');
            const ushort kbAlt5 = (AltBase + '5');
            const ushort kbAlt6 = (AltBase + '6');
            const ushort kbAlt7 = (AltBase + '7');
            const ushort kbAlt8 = (AltBase + '8');
            const ushort kbAlt9 = (AltBase + '9');
            const ushort kbAlt0 = (AltBase + '0');

            const ushort kbAltF1 = (AltBase + kbF1);
            const ushort kbAltF2 = (AltBase + kbF2);
            const ushort kbAltF3 = (AltBase + kbF3);
            const ushort kbAltF4 = (AltBase + kbF4);
            const ushort kbAltF5 = (AltBase + kbF5);
            const ushort kbAltF6 = (AltBase + kbF6);
            const ushort kbAltF7 = (AltBase + kbF7);
            const ushort kbAltF8 = (AltBase + kbF8);
            const ushort kbAltF9 = (AltBase + kbF9);
            const ushort kbAltF10 = (AltBase + kbF10);
            const ushort kbAltF11 = (AltBase + kbF11);
            const ushort kbAltF12 = (AltBase + kbF12);

            const ushort kbAltMinus = (AltBase + '-');
            const ushort kbAltEqual = (AltBase + '=');

            const ushort kbAltUp = (AltBase + kbUp);
            const ushort kbAltDown = (AltBase + kbDown);
            const ushort kbAltLeft = (AltBase + kbLeft);
            const ushort kbAltRight = (AltBase + kbRight);

            // shift组合键
            const ushort ShiftBase = 0x3000;

            const ushort kbShiftF1 = (ShiftBase + kbF1);
            const ushort kbShiftF2 = (ShiftBase + kbF2);
            const ushort kbShiftF3 = (ShiftBase + kbF3);
            const ushort kbShiftF4 = (ShiftBase + kbF4);
            const ushort kbShiftF5 = (ShiftBase + kbF5);
            const ushort kbShiftF6 = (ShiftBase + kbF6);
            const ushort kbShiftF7 = (ShiftBase + kbF7);
            const ushort kbShiftF8 = (ShiftBase + kbF8);
            const ushort kbShiftF9 = (ShiftBase + kbF9);
            const ushort kbShiftF10 = (ShiftBase + kbF10);
            const ushort kbShiftF11 = (ShiftBase + kbF11);
            const ushort kbShiftF12 = (ShiftBase + kbF12);

            const ushort kbShiftIns = (ShiftBase + kbIns);
            const ushort kbShiftDel = (ShiftBase + kbDel);
            const ushort kbShiftTab = (ShiftBase + kbTab);

            const int kbIdle = (0x7fff);
            const int kbRealRe = (0x7ffe);	// 只要系统查询事件队列无条件执行

            //            const int kbReset	=	kbCtrlZ; 	


            // 2. 定义通道事件
            const int ncEvtPrgStart = 0xa001;	// 程序启动
            const int ncEvtPrgEnd = 0xa002;	// 程序结束
            const int ncEvtPrgHold = 0xa003;	// Hold完成	
            const int ncEvtPrgBreak = 0xa004;	// break完成	
            const int ncEvtG92Fin = 0xa005;	// G92完成
            const int ncEvtRstFin = 0xa006;	// 上电复位完成
            const int ncEvtRwndFin = 0xa007;	// 重运行完成
            const int ncEvtMdiRdy = 0xa008;	// MDI准备好
            const int ncEvtMdiExit = 0xa009;	// MDI退出
            const int ncEvtMdiAck = 0xa00a;	// MDI行解释完成
            const int ncEvtRunStart = 0xa00b;	// 程序运行

            const int ncEvtRunRowAck = 0xa00d;	// 任意行请求应答
            const int ncEvtRunRowRdy = 0xa00e;	// 任意行准备好

            const int ncEvtBpSaved = 0xa011;	// 断点保存完成
            const int ncEvtBpResumed = 0xa012;	// 断点恢复完成
            const int ncEvtIntvHold = 0xa013;	// 执行到M92等待用户干预
            const int ncEvtEstop = 0xa014;	// 外部急停
            const int ncEvtLoadOK = 0xa015;	// 程序加载完成

            const int ncEvtSyntax1 = 0xa016;	// 第一类语法错【修改后可接着运行】
            const int ncEvtSyntax2 = 0xa017;	// 第二类语法错【修改后从头运行】

            const int ncEvtGcodeSave = 0xa018;	// 程序中的数据保存指令
            const int ncEvtLoadData = 0xa019;	// 程序中的数据加载指令
            const int ncEvtChgTool = 0xa01a;	// G代码修改了刀具数据
            const int ncEvtChgCrds = 0xa01b;	// G代码修改了坐标系数据
            const int ncEvtChgAxes = 0xa01c;	// 通道轴组发生了改变

            const int ncEvtNckNote = 0xa01e;	// 通道提示
            const int ncEvtNckAlarm = 0xa01f;	// 通道报警
            const int ncEvtStopAck = 0xa020;// sys_stop_prog完成
            const int ncEvtFaultIrq = 0xa030;	// 故障中断
            const int ncEvtPackFin = 0xa040;	// 数据打包完成

            // 3. 定义轴事件
            const int ncEvtMaxEncPos = 0xa201;	// 轴编码器初始位置过大

            // 4. 定义系统事件

            const int ncEvtPoweroff = 0xa800;	// 系统断电
            const int ncEvtSaveData = 0xa801;	// 保存系统数据
            const int ncEvtSysExit = 0xa802;	// 系统退出
            const int ncEvtUserStart = 0xb000;	// 用户自定义事件
            const int ncEvtUserReqChn = (ncEvtUserStart + 1);	// 请求切换通道
            const int ncEvtUserReqMsk = (ncEvtUserStart + 2);	// 请求屏蔽通道
            const int ncEvtUserFunc1 = (ncEvtUserStart + 100);	// event 100 对应用户按键调用指定程序
            const int ncEvtUserFunc2 = (ncEvtUserStart + 101);	// event 100 对应用户按键调用指定程序
            const int ncEvtHardRstFin = (ncEvtUserStart + 102);	// 硬复位完成

            #endregion
            LogData_Panel myLogData_Panel = new LogData_Panel { }; //定义待返回的数据结构
            List<ListData_Panel> ListLink = new List<ListData_Panel>(); //放入返回数据结构的部分数据成员---链表
            LogFileHead lfh = new LogFileHead { };//定义转入返回数据结构数据成员---文件头

            #region
            byte[] datahead = new byte[64];
            FileStream aFile = new FileStream(filename, FileMode.Open);
            aFile.Read(datahead, 0, 16);
            string ss = Encoding.Default.GetString(datahead);
            ss = ss.Substring(0, ss.IndexOf('\0'));
            aFile.Read(datahead, 0, 4);
            int rizhishu = datahead[0] & 0xFF;
            rizhishu |= ((datahead[1] << 8) & 0xFF00);
            rizhishu |= ((datahead[2] << 16) & 0xFF0000);
            rizhishu |= (int)((datahead[3] << 24) & 0xFF000000);
            lfh.fnum = rizhishu;
            lfh.fflag = ss;
            myLogData_Panel.LogHead = lfh;  //文件头存入返回数据结构中
            #endregion
            int IDNo = 0;
            for (int j = 1; j <= rizhishu; j++) //现在是前10条记录，要输出全部10<-rizhishu替代
            {
                //读取时间
                int[] DateTime = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    aFile.Read(datahead, 0, 4);
                    int num = datahead[0] & 0xFF;
                    num |= ((datahead[1] << 8) & 0xFF00);
                    num |= ((datahead[2] << 16) & 0xFF0000);
                    num |= (int)((datahead[3] << 24) & 0xFF000000);
                    DateTime[i] = num;
                }
                //读取事件来源
                aFile.Read(datahead, 0, 2);
                int src = datahead[0] & 0xFF;
                src |= ((datahead[1] << 8) & 0xFF00);
                src |= ((datahead[2] << 16) & 0xFF0000);
                src |= (int)((datahead[3] << 24) & 0xFF000000);
                ushort SrcVal = (ushort)src;
                ushort myOriginalSrc = SrcVal;
                int mysrc;
                string myevent;
                if ((SrcVal >= EV_SRC_CH0) && (SrcVal <= EV_SRC_CH0 + 15))
                {
                    myevent = "通道0事件";
                    mysrc = 1;
                }
                else switch (SrcVal)
                    {
                        //开关机在Panel日志中源和代码字段是0000 0000，这里看做系统事件
                        case EV_SRC_ONOFF: { myevent = "系统事件"; mysrc = 0; break; }
                        case EV_SRC_SYS: { myevent = "系统事件"; mysrc = 0; break; }
                        case EV_SRC_MDI: { myevent = "MDI的事件"; mysrc = 2; break; }
                        case EV_SRC_KBD: { myevent = "键盘事件"; mysrc = 3; break; }
                        case EV_SRC_AX0: { myevent = "轴事件"; mysrc = 4; break; }
                        default: { myevent = "未知事件来源！"; mysrc = 999; break; }
                    }
                aFile.Read(datahead, 0, 2);
                src = datahead[0] & 0xFF;
                src |= ((datahead[1] << 8) & 0xFF00);
                src |= ((datahead[2] << 16) & 0xFF0000);
                src |= (int)((datahead[3] << 24) & 0xFF000000);
                SrcVal = (ushort)src;
                ushort myOriginalCode = SrcVal;
                string mysource;
                ushort mytype;
                #region
                if ((SrcVal & 0xff00) == 0x0000)
                {
                    if ((SrcVal & 0x00ff) == 0x0020) { mysource = "kbSpace"; mytype = 46; }
                    else if ((SrcVal & 0x00ff) == 0x0008) { mysource = "kbBack"; mytype = 47; }
                    else if ((SrcVal & 0x00ff) == 0x0009) { mysource = "kbTab"; mytype = 48; }
                    else if ((SrcVal & 0x00ff) == 0x000d) { mysource = "kbEnter"; mytype = 49; }
                    else { mysource = Convert.ToString(Convert.ToChar(SrcVal)); mytype = 50; }
                    if (SrcVal == 0) { mysource = null; mytype = 45; }
                }
                else
                    switch (SrcVal)
                    {
                        // 01---功能键
                        case kbShift: { mysource = "功能键shift"; mytype = 51; break; }
                        case kbCtrl: { mysource = "功能键Ctrl"; mytype = 52; break; }
                        case kbAlt: { mysource = "功能键alt"; mytype = 53; break; }
                        case kbPause: { mysource = "功能键pause"; mytype = 54; break; }
                        case kbCapsLk: { mysource = "功能键Capslk"; mytype = 55; break; }
                        case kbEsc: { mysource = "功能键Esc"; mytype = 56; break; }

                        case kbPgUp: { mysource = "功能键kbpgup"; mytype = 57; break; }
                        case kbPgDn: { mysource = "功能键kbpgdn"; mytype = 58; break; }
                        case kbEnd: { mysource = "功能键kbend"; mytype = 59; break; }
                        case kbHome: { mysource = "功能键kbhome"; mytype = 60; break; }
                        case kbLeft: { mysource = "功能键kbleft"; mytype = 61; break; }
                        case kbUp: { mysource = "功能键kbup"; mytype = 62; break; }
                        case kbRight: { mysource = "功能键kbRight"; mytype = 63; break; }
                        case kbDown: { mysource = "功能键kbDown"; mytype = 64; break; }
                        case kbIns: { mysource = "功能键kbIns"; mytype = 65; break; }
                        case kbDel: { mysource = "功能键kbDel"; mytype = 66; break; }

                        case kbF1: { mysource = "功能键kbF1"; mytype = 67; break; }
                        case kbF2: { mysource = "功能键kbF2"; mytype = 68; break; }
                        case kbF3: { mysource = "功能键kbF3"; mytype = 69; break; }
                        case kbF4: { mysource = "功能键kbF4"; mytype = 70; break; }
                        case kbF5: { mysource = "功能键kbF5"; mytype = 71; break; }
                        case kbF6: { mysource = "功能键kbF6"; mytype = 72; break; }
                        case kbF7: { mysource = "功能键kbF7"; mytype = 73; break; }
                        case kbF8: { mysource = "功能键kbF8"; mytype = 74; break; }
                        case kbF9: { mysource = "功能键kbF9"; mytype = 75; break; }
                        case kbF10: { mysource = "功能键kbF10"; mytype = 76; break; }
                        case kbF11: { mysource = "功能键kbF11"; mytype = 77; break; }
                        case kbF12: { mysource = "功能键kbF12"; mytype = 78; break; }

                        // ctrl组合键
                        case kbCtrlA: { mysource = "CtrlBase+A"; mytype = 79; break; }
                        case kbCtrlB: { mysource = "CtrlBase+B"; mytype = 80; break; }
                        case kbCtrlC: { mysource = "CtrlBase+C"; mytype = 81; break; }
                        case kbCtrlD: { mysource = "CtrlBase+D"; mytype = 82; break; }
                        case kbCtrlE: { mysource = "CtrlBase+E"; mytype = 83; break; }
                        case kbCtrlF: { mysource = "CtrlBase+F"; mytype = 84; break; }
                        case kbCtrlG: { mysource = "CtrlBase+G"; mytype = 85; break; }
                        case kbCtrlH: { mysource = "CtrlBase+H"; mytype = 86; break; }
                        case kbCtrlI: { mysource = "CtrlBase+I"; mytype = 87; break; }
                        case kbCtrlJ: { mysource = "CtrlBase+J"; mytype = 88; break; }
                        case kbCtrlK: { mysource = "CtrlBase+K"; mytype = 89; break; }
                        case kbCtrlL: { mysource = "CtrlBase+L"; mytype = 90; break; }
                        case kbCtrlM: { mysource = "CtrlBase+M"; mytype = 91; break; }
                        case kbCtrlN: { mysource = "CtrlBase+N"; mytype = 92; break; }
                        case kbCtrlO: { mysource = "CtrlBase+O"; mytype = 93; break; }
                        case kbCtrlP: { mysource = "CtrlBase+P"; mytype = 94; break; }
                        case kbCtrlQ: { mysource = "CtrlBase+Q"; mytype = 95; break; }
                        case kbCtrlR: { mysource = "CtrlBase+R"; mytype = 96; break; }
                        case kbCtrlS: { mysource = "CtrlBase+S"; mytype = 97; break; }
                        case kbCtrlT: { mysource = "CtrlBase+T"; mytype = 98; break; }
                        case kbCtrlU: { mysource = "CtrlBase+U"; mytype = 99; break; }
                        case kbCtrlV: { mysource = "CtrlBase+V"; mytype = 100; break; }
                        case kbCtrlW: { mysource = "CtrlBase+W"; mytype = 101; break; }
                        case kbCtrlX: { mysource = "CtrlBase+X"; mytype = 102; break; }
                        case kbCtrlY: { mysource = "CtrlBase+Y"; mytype = 103; break; }
                        case kbCtrlZ: { mysource = "CtrlBase+Z"; mytype = 104; break; }


                        case kbCtrlF1: { mysource = "CtrlBase+kbF1"; mytype = 105; break; }
                        case kbCtrlF2: { mysource = "CtrlBase+kbF2"; mytype = 106; break; }
                        case kbCtrlF3: { mysource = "CtrlBase+kbF3"; mytype = 107; break; }
                        case kbCtrlF4: { mysource = "CtrlBase+kbF4"; mytype = 108; break; }
                        case kbCtrlF5: { mysource = "CtrlBase+kbF5"; mytype = 109; break; }
                        case kbCtrlF6: { mysource = "CtrlBase+kbF6"; mytype = 110; break; }
                        case kbCtrlF7: { mysource = "CtrlBase+kbF7"; mytype = 111; break; }
                        case kbCtrlF8: { mysource = "CtrlBase+kbF8"; mytype = 112; break; }
                        case kbCtrlF9: { mysource = "CtrlBase+kbF9"; mytype = 113; break; }
                        case kbCtrlF10: { mysource = "CtrlBase+kbF10"; mytype = 114; break; }
                        case kbCtrlF11: { mysource = "CtrlBase+kbF11"; mytype = 115; break; }
                        case kbCtrlF12: { mysource = "CtrlBase+kbF12"; mytype = 116; break; }

                        case kbCtrlSpace: { mysource = "CtrlBase+kbSpace"; mytype = 117; break; }
                        case kbCtrlPgUp: { mysource = "CtrlBase+kbCtrlPgUp"; mytype = 118; break; }
                        case kbCtrlPgDn: { mysource = "CtrlBase+kbCtrlPgDn"; mytype = 119; break; }
                        case kbCtrlEnd: { mysource = "CtrlBase+kbEnd"; mytype = 120; break; }
                        case kbCtrlHome: { mysource = "CtrlBase+kbCtrlHome"; mytype = 121; break; }
                        case kbCtrlLeft: { mysource = "CtrlBase+kbCtrlLeft"; mytype = 122; break; }
                        case kbCtrlUp: { mysource = "CtrlBase+kbCtrlUp"; mytype = 123; break; }
                        case kbCtrlRight: { mysource = "CtrlBase+kbCtrlRight"; mytype = 124; break; }
                        case kbCtrlDown: { mysource = "CtrlBase+kbCtrlDown"; mytype = 125; break; }
                        case kbCtrlIns: { mysource = "CtrlBase+kbCtrlIns"; mytype = 126; break; }
                        case kbCtrlDel: { mysource = "CtrlBase+kbCtrlDel"; mytype = 127; break; }
                        case kbCtrlBack: { mysource = "CtrlBase+kbCtrlBack"; mytype = 128; break; }
                        case kbCtrlEnter: { mysource = "CtrlBase+kbCtrlEnter"; mytype = 129; break; }

                        // alt组合键
                        case kbAltSpace: { mysource = "AltBase+kbSpace"; mytype = 130; break; }
                        case kbAltA: { mysource = "AltBase+A"; mytype = 131; break; }
                        case kbAltB: { mysource = "AltBase+B"; mytype = 132; break; }
                        case kbAltC: { mysource = "AltBase+C"; mytype = 133; break; }
                        case kbAltD: { mysource = "AltBase+D"; mytype = 134; break; }
                        case kbAltE: { mysource = "AltBase+E"; mytype = 135; break; }
                        case kbAltF: { mysource = "AltBase+F"; mytype = 136; break; }
                        case kbAltG: { mysource = "AltBase+G"; mytype = 137; break; }
                        case kbAltH: { mysource = "AltBase+H"; mytype = 138; break; }
                        case kbAltI: { mysource = "AltBase+I"; mytype = 139; break; }
                        case kbAltJ: { mysource = "AltBase+G"; mytype = 140; break; }
                        case kbAltK: { mysource = "AltBase+K"; mytype = 141; break; }
                        case kbAltL: { mysource = "AltBase+L"; mytype = 142; break; }
                        case kbAltM: { mysource = "AltBase+M"; mytype = 143; break; }
                        case kbAltN: { mysource = "AltBase+N"; mytype = 144; break; }
                        case kbAltO: { mysource = "AltBase+O"; mytype = 145; break; }
                        case kbAltP: { mysource = "AltBase+P"; mytype = 146; break; }
                        case kbAltQ: { mysource = "AltBase+Q"; mytype = 147; break; }
                        case kbAltR: { mysource = "AltBase+R"; mytype = 148; break; }
                        case kbAltS: { mysource = "AltBase+S"; mytype = 149; break; }
                        case kbAltT: { mysource = "AltBase+T"; mytype = 150; break; }
                        case kbAltU: { mysource = "AltBase+U"; mytype = 151; break; }
                        case kbAltV: { mysource = "AltBase+V"; mytype = 152; break; }
                        case kbAltW: { mysource = "AltBase+W"; mytype = 153; break; }
                        case kbAltX: { mysource = "AltBase+X"; mytype = 154; break; }
                        case kbAltY: { mysource = "AltBase+Y"; mytype = 155; break; }
                        case kbAltZ: { mysource = "AltBase+Z"; mytype = 156; break; }

                        case kbAlt1: { mysource = "AltBase+1"; mytype = 157; break; }
                        case kbAlt2: { mysource = "AltBase+2"; mytype = 158; break; }
                        case kbAlt3: { mysource = "AltBase+3"; mytype = 159; break; }
                        case kbAlt4: { mysource = "AltBase+4"; mytype = 160; break; }
                        case kbAlt5: { mysource = "AltBase+5"; mytype = 161; break; }
                        case kbAlt6: { mysource = "AltBase+6"; mytype = 162; break; }
                        case kbAlt7: { mysource = "AltBase+7"; mytype = 163; break; }
                        case kbAlt8: { mysource = "AltBase+8"; mytype = 164; break; }
                        case kbAlt9: { mysource = "AltBase+9"; mytype = 165; break; }
                        case kbAlt0: { mysource = "AltBase+0"; mytype = 166; break; }


                        case kbAltF1: { mysource = "AltBase+kbF1"; mytype = 167; break; }
                        case kbAltF2: { mysource = "AltBase+kbF2"; mytype = 168; break; }
                        case kbAltF3: { mysource = "AltBase+kbF3"; mytype = 169; break; }
                        case kbAltF4: { mysource = "AltBase+kbF4"; mytype = 170; break; }
                        case kbAltF5: { mysource = "AltBase+kbF5"; mytype = 171; break; }
                        case kbAltF6: { mysource = "AltBase+kbF6"; mytype = 172; break; }
                        case kbAltF7: { mysource = "AltBase+kbF7"; mytype = 173; break; }
                        case kbAltF8: { mysource = "AltBase+kbF8"; mytype = 174; break; }
                        case kbAltF9: { mysource = "AltBase+kbF9"; mytype = 175; break; }
                        case kbAltF10: { mysource = "AltBase+kbF11"; mytype = 176; break; }
                        case kbAltF11: { mysource = "AltBase+kbF9"; mytype = 177; break; }
                        case kbAltF12: { mysource = "AltBase+kbF12"; mytype = 178; break; }

                        case kbAltMinus: { mysource = "AltBase+'-'"; mytype = 179; break; }
                        case kbAltEqual: { mysource = "AltBase+'='"; mytype = 180; break; }
                        case kbAltUp: { mysource = "AltBase+kbUp"; mytype = 181; break; }
                        case kbAltDown: { mysource = "AltBase+kbDown"; mytype = 182; break; }
                        case kbAltLeft: { mysource = "AltBase+kbLeft"; mytype = 183; break; }
                        case kbAltRight: { mysource = "AltBase+kbRight"; mytype = 184; break; }


                        // shift组合键
                        case kbShiftF1: { mysource = "ShiftBase+kbF1"; mytype = 185; break; }
                        case kbShiftF2: { mysource = "ShiftBase+kbF2"; mytype = 186; break; }
                        case kbShiftF3: { mysource = "ShiftBase+kbF3"; mytype = 187; break; }
                        case kbShiftF4: { mysource = "ShiftBase+kbF4"; mytype = 188; break; }
                        case kbShiftF5: { mysource = "ShiftBase+kbF5"; mytype = 189; break; }
                        case kbShiftF6: { mysource = "ShiftBase+kbF6"; mytype = 190; break; }
                        case kbShiftF7: { mysource = "ShiftBase+kbF7"; mytype = 191; break; }
                        case kbShiftF8: { mysource = "ShiftBase+kbF8"; mytype = 192; break; }
                        case kbShiftF9: { mysource = "ShiftBase+kbF9"; mytype = 193; break; }
                        case kbShiftF10: { mysource = "ShiftBase+kbF10"; mytype = 194; break; }
                        case kbShiftF11: { mysource = "ShiftBase+kbF11"; mytype = 195; break; }
                        case kbShiftF12: { mysource = "ShiftBase+kbF12"; mytype = 196; break; }

                        case kbShiftIns: { mysource = "ShiftBase+kbIns"; mytype = 197; break; }
                        case kbShiftDel: { mysource = "ShiftBase+kbDel"; mytype = 198; break; }
                        case kbShiftTab: { mysource = "ShiftBase+kbTab"; mytype = 199; break; }

                        case kbIdle: { mysource = "kbIdle"; mytype = 200; break; }
                        case kbRealRe: { mysource = "kbRealRe"; mytype = 201; break; }


                        // 2. 定义通道事件

                        case ncEvtPrgStart: { mysource = "程序启动"; mytype = 202; break; }
                        case ncEvtPrgEnd: { mysource = "程序结束"; mytype = 203; break; }
                        case ncEvtPrgHold: { mysource = "Hold完成"; mytype = 204; break; }
                        case ncEvtPrgBreak: { mysource = "break完成"; mytype = 205; break; }
                        case ncEvtG92Fin: { mysource = "G92完成"; mytype = 206; break; }
                        case ncEvtRstFin: { mysource = "上电复位完成"; mytype = 207; break; }
                        case ncEvtRwndFin: { mysource = "重运行完成"; mytype = 208; break; }
                        case ncEvtMdiRdy: { mysource = "MDI准备好"; mytype = 209; break; }
                        case ncEvtMdiExit: { mysource = "MDI退出"; mytype = 210; break; }
                        case ncEvtMdiAck: { mysource = "MDI行解释完成"; mytype = 211; break; }
                        case ncEvtRunStart: { mysource = "程序运行"; mytype = 212; break; }
                        case ncEvtRunRowAck: { mysource = "任意行请求应答"; mytype = 213; break; }
                        case ncEvtRunRowRdy: { mysource = "任意行准备好"; mytype = 214; break; }
                        case ncEvtBpSaved: { mysource = "断点保存完成"; mytype = 215; break; }
                        case ncEvtBpResumed: { mysource = "断点恢复完成"; mytype = 216; break; }
                        case ncEvtIntvHold: { mysource = "执行到M92等待用户干预"; mytype = 217; break; }
                        case ncEvtEstop: { mysource = "外部急停"; mytype = 218; break; }
                        case ncEvtLoadOK: { mysource = "程序加载完成"; mytype = 219; break; }
                        case ncEvtSyntax1: { mysource = "第一类语法错【修改后可接着运行】"; mytype = 220; break; }
                        case ncEvtSyntax2: { mysource = "第二类语法错【修改后从头运行】"; mytype = 221; break; }
                        case ncEvtGcodeSave: { mysource = "程序中的数据保存指令"; mytype = 222; break; }
                        case ncEvtLoadData: { mysource = "程序中的数据加载指令"; mytype = 223; break; }
                        case ncEvtChgTool: { mysource = "G代码修改了刀具数据"; mytype = 224; break; }
                        case ncEvtChgCrds: { mysource = "G代码修改了坐标系数据"; mytype = 225; break; }
                        case ncEvtChgAxes: { mysource = "通道轴组发生了改变"; mytype = 226; break; }
                        case ncEvtNckNote: { mysource = "通道提示"; mytype = 227; break; }
                        case ncEvtNckAlarm: { mysource = "通道报警"; mytype = 228; break; }
                        case ncEvtStopAck: { mysource = "sys_stop_prog完成"; mytype = 229; break; }
                        case ncEvtFaultIrq: { mysource = "故障中断"; mytype = 230; break; }
                        case ncEvtPackFin: { mysource = "数据打包完成"; mytype = 231; break; }

                        // 3. 定义轴事件
                        case ncEvtMaxEncPos: { mysource = "轴编码器初始位置过大"; mytype = 232; break; }


                        // 4. 定义系统事件
                        case ncEvtPoweroff: { mysource = "系统断电"; mytype = 233; break; }
                        case ncEvtSaveData: { mysource = "保存系统数据"; mytype = 234; break; }
                        case ncEvtSysExit: { mysource = "系统退出"; mytype = 235; break; }
                        case ncEvtUserStart: { mysource = "用户自定义事件"; mytype = 236; break; }
                        case ncEvtUserReqChn: { mysource = "请求切换通道"; mytype = 237; break; }
                        case ncEvtUserReqMsk: { mysource = "请求屏蔽通道"; mytype = 238; break; }
                        case ncEvtUserFunc1: { mysource = "event 100 对应用户按键调用指定程序"; mytype = 239; break; }
                        case ncEvtUserFunc2: { mysource = " event 100 对应用户按键调用指定程序"; mytype = 239; break; }
                        case ncEvtHardRstFin: { mysource = "硬复位完成"; mytype = 241; break; }

                        default: { mysource = "未知代码!"; mytype = 999; break; }
                    }
                #endregion

                aFile.Read(datahead, 0, 64);
                string mycontent = Encoding.Default.GetString(datahead);
                mycontent = mycontent.Substring(0, mycontent.IndexOf('\0'));
                IDNo++;
                ListLink.Add(new ListData_Panel()
                {
                    //second = DateTime[0],
                    //minute = DateTime[1],
                    //hour = DateTime[2],
                    //hsecond = DateTime[3],
                    //day = DateTime[4],
                    //month = DateTime[5],
                    //year = DateTime[6],
                    //wday = DateTime[7],
                    src = mysrc,
                    code = mysource,
                    content = myevent + mysource,
                    type = mytype,
                    OriginalSrc = myOriginalSrc,
                    OriginalCode = myOriginalCode,
                    detail = mycontent,
                    CreateTime = new DateTime(DateTime[6], DateTime[5], DateTime[4], DateTime[2], DateTime[1], DateTime[0]),
                    ID = IDNo
                });
                myLogData_Panel.LogContent = ListLink;
            }
            aFile.Close();
            return myLogData_Panel;
        }
        //FILECHANGE.LOG文件读取接口
        public static LogData_FileChange LogRead_FileChange(string filename)
        {
            LogData_FileChange myLogData = new LogData_FileChange { };
            List<ListData_FileChange> ListLink = new List<ListData_FileChange>();
            LogFileHead lfh = new LogFileHead { };
            byte[] datahead = new byte[64];
            FileStream aFile = new FileStream(filename, FileMode.Open);
            aFile.Read(datahead, 0, 16);
            string ss = Encoding.Default.GetString(datahead);
            ss = ss.Substring(0, ss.IndexOf('\0'));
            aFile.Read(datahead, 0, 4);
            int rizhishu = datahead[0] & 0xFF;
            rizhishu |= ((datahead[1] << 8) & 0xFF00);
            rizhishu |= ((datahead[2] << 16) & 0xFF0000);
            rizhishu |= (int)((datahead[3] << 24) & 0xFF000000);
            lfh.fnum = rizhishu;
            lfh.fflag = ss;
            myLogData.LogHead = lfh;

            int IDNo = 0;
            for (int j = 1; j <= rizhishu; j++) //现在是前10条记录，要输出全部10<-rizhishu替代
            {
                int[] DateTime = new int[8];
                for (int i = 0; i < 8; i++)
                {
                    aFile.Read(datahead, 0, 4);
                    int num = datahead[0] & 0xFF;
                    num |= ((datahead[1] << 8) & 0xFF00);
                    num |= ((datahead[2] << 16) & 0xFF0000);
                    num |= (int)((datahead[3] << 24) & 0xFF000000);
                    DateTime[i] = num;
                }
                aFile.Read(datahead, 0, 64);
                string s = Encoding.Default.GetString(datahead);
                s = s.Substring(0, s.IndexOf('\0'));
                ListLink.Add(new ListData_FileChange()
                {
                    detail = s,
                    CreateTime = new DateTime(DateTime[6], DateTime[5], DateTime[4], DateTime[2], DateTime[1], DateTime[0]),
                    ID = ++IDNo
                });
                myLogData.LogContent = ListLink;
            }
            aFile.Close();
            return myLogData;
        }
    }
}

