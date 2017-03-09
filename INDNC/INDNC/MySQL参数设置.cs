using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MySql.Data.MySqlClient;
using MySql.Data;
using System.IO;

namespace INDNC
{
    public partial class MySQLParaSetting : Form
    {
        public  MySQLPara mysqlpara = new MySQLPara();
        
        public MySQLParaSetting()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            mysqlpara.MySQLID = textBox1.Text;
            mysqlpara.MySQLHostID = textBox2.Text;
            mysqlpara.MySQLPassword = textBox3.Text;
            mysqlpara.MySQLDatabase = textBox4.Text;

            string constr = "server=" + mysqlpara.MySQLID + ";User Id=" + mysqlpara.MySQLHostID + ";password=" + mysqlpara.MySQLPassword + ";Database="+ mysqlpara.MySQLDatabase;
            try
            {
                MySqlConnection mysqlconnection = new MySqlConnection(constr);
                if (mysqlconnection.Ping())  
                {
                    mysqlpara.connectvalid = true;
                    FileStream fs1 = new FileStream("../../MySQLPara.txt", FileMode.OpenOrCreate, FileAccess.Write);//创建写入文件 
                    StreamWriter sw = new StreamWriter(fs1);
                    sw.WriteLine(mysqlpara.MySQLID + " " + mysqlpara.MySQLHostID+ " " + mysqlpara.MySQLPassword+ " " + mysqlpara.MySQLDatabase);//开始写入值
                    sw.Close();
                    fs1.Close();
                  
                }
                else
                    mysqlpara.connectvalid = false;
            }
            catch(Exception ex)
            {
                MessageBox.Show("ERROR:" + ex.Message, "ERROR");
                mysqlpara.connectvalid = false;
            }
            finally
            {
                this.Close();
            }
        }
    }
}
