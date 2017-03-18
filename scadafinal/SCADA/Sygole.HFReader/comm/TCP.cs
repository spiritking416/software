using System;
using System.Collections.Generic;
using System.Text;
using System.Net;
using System.Net.Sockets;
using System.Threading;

namespace Sygole.HFReader
{
    public class TCP : Communication
    {
        NetworkStream tcpDataStream;
        TcpClient client = new TcpClient();
        ManualResetEvent TimeoutObject = new ManualResetEvent(false);
        bool IsConnectionSuccessful = false;
        // 它有自己的被处理过标记 
        private bool _disposed = false;

        ~TCP()
        {
            Dispose(true);
        }

        //释放资源
        protected override void Dispose(bool isDisposing)
        {
            // 不要多次处理 
            if (_disposed)
            {
                return;
            }

            try
            {
                tcpDataStream.Close();
                tcpDataStream.Dispose();
                client.Close();
            }
            catch
            {
            }

            // 让基类释放自己的资源。基类负责调用GC.SuppressFinalize( ) 
            base.Dispose(isDisposing);

            // 设置衍生类的被处理过标记 
            _disposed = true;
        }

        //连接状态
        public override bool ConnectState()
        {
            return client.Client.Connected;
        }

        //建立TCP连接
        //连接成功返回true，出现异常失败返回false;
        public override bool Connect(string IP, int port)
        {
            try
            {
                IPAddress serverIP = IPAddress.Parse(IP);

                //采用非阻塞的方式建立TCP连接，超时时间为2000ms
                client.BeginConnect(IP, port, new AsyncCallback(CallBackMethod), client);
                if (!TimeoutObject.WaitOne(2000, false) || !IsConnectionSuccessful)
                {
                    client.Close();
                    return false;
                }

                //client.Connect(serverIP, port);
                tcpDataStream = client.GetStream();
                return StartReceiveThread();
            }
            catch (Exception exp)
            {
                exp.ToString();
                return false;
            }
        }

        private void CallBackMethod(IAsyncResult asyncresult)
        {
            try
            {
                IsConnectionSuccessful = false;
                TcpClient tcpclient = asyncresult.AsyncState as TcpClient;

                if (tcpclient.Client != null)
                {
                    tcpclient.EndConnect(asyncresult);
                    IsConnectionSuccessful = true;
                }
            }
            catch (Exception ex)
            {
                ex.ToString();
                IsConnectionSuccessful = false;
            }
            finally
            {
                TimeoutObject.Set();
            }
        }

        //从端口获取数据
        protected override int read(ref byte[] data, int off, int size)
        {
            try
            {
                if (tcpDataStream.DataAvailable)
                {
                    return tcpDataStream.Read(data, off, size);
                }
                else
                {
                    return 0;
                }
            }
            catch
            {
                return 0;
            }
        }

        //数据发送函数
        protected override bool write(byte[] data, int off, int size)
        {
            try
            {
                tcpDataStream.Write(data, off, size);
            }
            catch (Exception exp)
            {
                exp.ToString();
                return false;
            }
            return true;
        }
    }
}
