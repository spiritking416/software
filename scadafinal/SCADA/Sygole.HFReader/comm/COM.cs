using System;
using System.Collections.Generic;
using System.Text;
using System.IO.Ports;

namespace Sygole.HFReader
{
    public class COM : Communication
    {
        SerialPort SerialPort = new SerialPort();
        // 它有自己的被处理过标记 
        private bool _disposed = false;

        ~COM()
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
                SerialPort.Close();
                SerialPort.Dispose();
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
            return SerialPort.IsOpen;
        }

        //建立串口连接
        //连接成功返回true，出现异常失败返回false;
        public override bool Connect(string COM, int baud)
        {
            try
            {
                SerialPort.PortName = COM;
                SerialPort.BaudRate = baud;
                SerialPort.DataBits = 8;
                SerialPort.StopBits = System.IO.Ports.StopBits.One;
                SerialPort.Parity = System.IO.Ports.Parity.None;

                SerialPort.Open();
                return StartReceiveThread();
            }
            catch
            {
                return false;
            }
        }

        //从端口获取数据
        protected override int read(ref byte[] data, int off, int size)
        {
            try
            {
                int len = SerialPort.BytesToRead;

                if (len == 0)
                {
                    return 0;
                }
                else if (len > size)
                {
                    len = size;
                }
                return SerialPort.Read(data, off, len);
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
                SerialPort.Write(data, off, size);
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
