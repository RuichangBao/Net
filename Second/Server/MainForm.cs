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
using System.Net;
using System.Threading;

namespace Server
{
    public partial class MainForm : Form
    {
        static Socket server;
        Thread thread1;
        public MainForm()
        {
            InitializeComponent();
            server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            server.Bind(new IPEndPoint(IPAddress.Parse("127.0.0.1"), 6001));//绑定端口号和IP
        }

        private void button1_Click(object sender, EventArgs e)
        {
            thread1 = new Thread(new ThreadStart(Receive));  //Thread1是你新线程的函数
            thread1.Start();
        }
 
        private void Receive()
        {
            LogForm logForm = new LogForm();
            logForm.labLog.Text = "11111111111";
            logForm.ShowDialog();

            while (true)
            {
                EndPoint point = new IPEndPoint(IPAddress.Any, 0);//用来保存发送方的ip和端口号
                byte[] buffer = new byte[1024];
                int length = server.ReceiveFrom(buffer, ref point);//接收数据报
                string message = Encoding.UTF8.GetString(buffer, 0, length);
                string content = point.ToString() + message;
                Console.WriteLine(content);
                logForm.labLog.Text = content;
            }
        }

        private void btnEnd_Click(object sender, EventArgs e)
        {
            thread1 = null;
        }
    }
}
