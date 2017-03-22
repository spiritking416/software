using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Sygole.HFReader
{
    public class Communication : IDisposable
    {
        public delegate void DealDataEvent(byte[] data, int length);
        public DealDataEvent dealDataEvent;
//        private System.Threading.Timer timerDataReceive;
        private System.Threading.Thread ReceiveThread;
        private const int RECEIVE_INTERVAL = 5;
        private const int RECEIVE_BUF_LEN = 1024;
        private const int FRAME_LEN = 128;

        private byte[] ReceiveBuffer = new byte[RECEIVE_BUF_LEN];
        private byte[] A_Frame = new byte[128];
        private int GottenDataLen = 0;
        private int GetDataCntATime = 0;
        private byte DealDataStatus = Package.HEADER_POS;

        private bool ReceiveThreadExit = false;
        private bool ReceiveThreadStarted = false;

        // 已经被清理过的标记 
        private bool _alreadyDisposed = false;

        public Communication()
        {
            ReceiveThreadExit = false;
            ReceiveThread = new Thread(DataReceive);
            ReceiveThread.IsBackground = true;
            ReceiveThread.Start();

//            timerDataReceive = new System.Threading.Timer((TimerCallback)DataReceive, null, Timeout.Infinite, RECEIVE_INTERVAL);
        }
        ~Communication()
        {
            Dispose();
        }

        // IDisposable的实现 
        // 调用虚拟的Dispose方法。禁止Finalization（终结操作）
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        #region 虚函数
        // 虚拟的Dispose方法 
        protected virtual void Dispose(bool isDisposing)
        {
            // 不要多次处理 
            if (_alreadyDisposed)
            {
                return;
            }

            if (isDisposing)
            {
                dealDataEvent = null;
            }

            StopReceiveThread();
            ReceiveThreadExit = true;

            _alreadyDisposed = true;
        }
        //连接读写器
        public virtual bool Connect(string ip, int port)
        {
            return false;
        }
        //连接状态
        public virtual bool ConnectState()
        {
            return false;
        }
        //从端口获取数据
        protected virtual int read(ref byte[] data, int off, int size)
        {
            return 0;
        }
        //数据发送函数
        protected virtual bool write(byte[] data, int off, int pos)
        {
            return false;
        }
        #endregion

        //停止接收
        private void StopReceiveThread()
        {
            try
            {
                //停止定时器
//                timerDataReceive.Change(Timeout.Infinite, RECEIVE_INTERVAL);
                ReceiveThreadStarted = false;
            }
            catch (Exception exp)
            {
                exp.ToString();
            }
        }

        //启动接收,成功返回true,失败false
        protected bool StartReceiveThread()
        {
            try
            {
                //启动定时器
//                timerDataReceive.Change(0, RECEIVE_INTERVAL);
                ReceiveThreadStarted = true;
                return true;
            }
            catch (Exception exp)
            {
                exp.ToString();
                return false;
            }
        }

        //线程启动函数
        private void DataReceive(object obj)
        {
            while (!ReceiveThreadExit)
            {
                if (ReceiveThreadStarted)
                {
                    int ReceiveLength = 0;
                    lock (ReceiveBuffer)
                    {
                        try
                        {
                            for (int tmpI = 0; tmpI < ReceiveBuffer.Length; tmpI++)
                            {
                                ReceiveBuffer[tmpI] = 0xFF;
                            }
                            ReceiveLength = read(ref ReceiveBuffer, 0, RECEIVE_BUF_LEN);
                        }
                        catch
                        {
                            return;
                        }

                        for (int i = 0; i < ReceiveLength; i++)
                        {
                            int len = 0;
                            GetDataCntATime = 0;
                            len = GetAFrame(ReceiveBuffer, i, ReceiveLength);

                            if (len == 0)//没解出帧
                            {
                                break;
                            }
                            else
                            {
                                i += GetDataCntATime - 1;
                                if (dealDataEvent != null)
                                {
                                    dealDataEvent.Invoke(A_Frame, len);
                                    //dealDataEvent.BeginInvoke(A_Frame, len, null, null);
                                }
                            }
                        }
                    }
                }
                System.Threading.Thread.Sleep(RECEIVE_INTERVAL);
            }
        }

        //遍历整个data,直到解出一帧数据
        //此种解析方式存在漏洞，如：一个正常帧插在一个看似正常的异常帧中，则无法解析出来
        private int GetAFrame(byte[] data, int offset, int length)
        {
            int i = offset;
            for (i = offset; i < length; i++)
            {
                switch (DealDataStatus)
                {
                    case Package.HEADER_POS:
                        if (data[i] == Package.Header)
                        {
                            for (int tmpI = 0; tmpI < A_Frame.Length; tmpI++)
                            {
                                A_Frame[tmpI] = 0xFF;
                            }
                            DealDataStatus++;
                            A_Frame[Package.HEADER_POS] = Package.Header;
                        }
                        break;
                    case Package.CMD_POS:
                        if (data[i] != Package.Header)
                        {
                            DealDataStatus++;
                            A_Frame[Package.CMD_POS] = data[i];
                        }
                        else
                        {
                            A_Frame[Package.HEADER_POS] = Package.Header;
                        }
                        break;
                    case Package.ID_POS:
                        DealDataStatus++;
                        A_Frame[Package.ID_POS] = data[i];
                        break;
                    case Package.STATUS_POS:
                        DealDataStatus++;
                        A_Frame[Package.STATUS_POS] = data[i];
                        break;
                    case Package.OPCODE_POS:
                        DealDataStatus++;
                        A_Frame[Package.OPCODE_POS] = data[i];
                        break;
                    case Package.DATALEN_POS:
                        DealDataStatus++;
                        if (data[i] > 100)
                        {
                            DealDataStatus = Package.HEADER_POS;
                        }
                        A_Frame[Package.DATALEN_POS] = data[i];
                        GottenDataLen = 0;
                        break;
                    case Package.DATA_POS:
                        if ((length - i - 1) >= (A_Frame[Package.DATALEN_POS] - GottenDataLen))
                        {
                            int j;
                            for (j = 0; j < (A_Frame[Package.DATALEN_POS] - GottenDataLen); j++)
                            {
                                A_Frame[Package.DATA_POS + j + GottenDataLen] = data[i + j];
                            }
                            i += j - 1;
                            DealDataStatus++;
                        }
                        else
                        {
                            for (; i < length; i++)
                            {
                                A_Frame[Package.DATA_POS + GottenDataLen] = data[i];
                                GottenDataLen++;
                            }
                        }
                        break;
                    case Package.BCC_POS:
                        DealDataStatus = Package.HEADER_POS;

                        //BCC校验正确，解出一帧
                        if (data[i] == tool.GetBCC(A_Frame, Package.DATA_POS, A_Frame[Package.DATALEN_POS]))
                        {
                            A_Frame[Package.DATA_POS + A_Frame[Package.DATALEN_POS]] = data[i];
                            GetDataCntATime = i + 1 - offset;// for循环中直接返回，故需要给i加上1,指向buffer末尾。
                            return (A_Frame[Package.DATALEN_POS] + 7);
                        }
                        break;
                    default:
                        break;
                }
            }
            GetDataCntATime = i - offset;
            return 0;
        }

        //网络端口发送函数
        public bool SendData(byte[] data, int offset,int len)
        {
            return write(data, offset, len);
        }
    }
}
