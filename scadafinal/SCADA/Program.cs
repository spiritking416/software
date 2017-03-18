using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Threading;

namespace SCADA
{
    static class Program
    {
        /// <summary>
        /// 应用程序的主入口点。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.ThreadException += new ThreadExceptionEventHandler(MainUIThreadExceptionHandler);
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(MainUIUnhandledExceptionHandler);

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            /*System.Diagnostics.Process m_process = RunningInstance();
            if (m_process != null)
            {
                if (MessageBox.Show("已经有一个SCADA在运行，是否关闭正在运行的SCADA?", "警告", MessageBoxButtons.YesNo) == DialogResult.Yes)
                {
                    m_process.Kill();
                }
                else 
                {
                    return;
                }
            }*/

            Application.Run(new MainForm());
        }
        public static void MainUIThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            MessageBox.Show(e.Exception.Message, "线程异常:", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        public static void MainUIUnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            MessageBox.Show(e.ExceptionObject.ToString(), "未处理的异常:", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        /// <summary>
        /// 获取正在运行的实例，没有运行的实例返回null;
        /// </summary>
        public static System.Diagnostics.Process RunningInstance()
        {
            System.Diagnostics.Process current = System.Diagnostics.Process.GetCurrentProcess();
            System.Diagnostics.Process[] processes = System.Diagnostics.Process.GetProcessesByName(current.ProcessName);
            foreach (System.Diagnostics.Process process in processes)
            {
                if (process.Id != current.Id)
                {
                    if (System.Reflection.Assembly.GetExecutingAssembly().Location.Replace("/", "//") == current.MainModule.FileName)
                    {
                        return process;
                    }
                }
            }
            return null;
        }
    }
}
