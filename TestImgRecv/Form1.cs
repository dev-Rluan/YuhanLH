using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace TestImgRecv
{
    public partial class Form1 : Form
    {
        Socket conn_socket;
        public static Form1 form1;
        public Form1()
        {
            InitializeComponent();
            form1 = this;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            String host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7776);

            conn_socket = new Socket(endPoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp);
            conn_socket.Connect(endPoint);
            
            


        }
        private void time()
        {
            while (true)
            {
                
                Thread.Sleep(500);
            }
        }

        private void btnRecv_Click(object sender, EventArgs e)
        {
            
            byte[] size = new byte[8];
            conn_socket.Receive(size);
            int a = BitConverter.ToInt32(size);
            byte[] img = new byte[a];
            Bitmap bmp = ScreenCopy.GetBitmap(img);

            ptBox.Image = bmp;
            

        }
    }
}
