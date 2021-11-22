using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net.Sockets;
using System.Threading;

namespace Scan2
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }
        //自定义变量
        private int port;//记录当前扫描的端口号
        private string Address;//记录扫描的系统地址
        private bool[] done = new bool[65536];//记录端口是否已经扫描
        private int start;//记录扫描的起始端口
        private int end;//记录扫描的结束端口
        private bool OK;
        private Thread scanThread; 
        private void button1_Click(object sender, EventArgs e)
        {
            label4.Text = textBox2.Text;//设定进度条的起始端口
            label5.Text = textBox3.Text;//设置进度条的终止端口
            //创建线程
            Thread procss = new Thread(new ThreadStart(PortScan));
            procss.Start();
            //设定进度条的范围
            progressBar1.Minimum = Int32.Parse(textBox2.Text);
            progressBar1.Maximum = Int32.Parse(textBox3.Text);
            //显示框的初始化
            listBox1.Items.Clear();
            listBox1.Items.Add("端口扫描器 v1.0");
            listBox1.Items.Add(" ");
        }
        private void PortScan()
        {
            start = Int32.Parse(textBox2.Text);
            end = Int32.Parse(textBox3.Text);
            //检查端口的合法性
            if ((start >= 0 && start <= 65536) && (end >= 0 && end <= 65536) && (start <= end))
            {

                Invoke(new Action(() => {//在线程里修改界面
                    listBox1.Items.Add("开始扫描：这个过程可能需要等待几分钟！");
                }));
                Address = textBox1.Text;
                for (int i = start; i <= end; i++)
                {
                    port = i;
                    //对该端口进行扫描的线程
                    scanThread = new Thread(Scan);
                    scanThread.Start();
                    //使线程睡眠
                    System.Threading.Thread.Sleep(100);

                    Invoke(new Action(() => {//在线程里修改界面
                        progressBar1.Value = i;
                        label6.Text = i.ToString();
                    }));
                }
                //未完成时情况
                while (!OK)
                {
                    OK = true;
                    for (int i = start; i <= end; i++)
                    {
                        if (!done[i])
                        {
                            OK = false;
                            break;
                        }
                    }
                }

                Invoke(new Action(() => {//在线程里修改界面
                    listBox1.Items.Add("扫描结束！");
                }));
                System.Threading.Thread.Sleep(1000);
            }
            else
            {
                Invoke(new Action(() => {//在线程里修改界面
                    MessageBox.Show("输入错误，端口范围为[0,65536]");
                }));

            }
        }
        private void Scan()
        {
            int portnow = port;
            //创建线程变量
            Thread Threadnow = scanThread;
            done[portnow] = true;
            //创建TcpClient对象，TcpClient用于TCP网络服务提供客户端连接
            TcpClient objTCP = null;
            //扫描端口，成功就写入信息
            try
            {
                objTCP = new TcpClient(Address, portnow);
                Invoke(new Action(() => {//在线程里修改界面
                    listBox1.Items.Add("端口" + portnow.ToString() + "开放！");
                }));
                objTCP.Close();
            }
            catch
            {

            }
        }
    }
}
