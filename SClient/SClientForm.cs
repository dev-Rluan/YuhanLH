using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SClient
{
    public partial class SClientForm : Form
    {
        public static SClientForm sclientform;
        public static SessionManager sessionManager;
        public static PacketManager packetManager;
        public SClientForm()
        {
            InitializeComponent();
            sclientform = this;
            sessionManager = new SessionManager();
            packetManager = new PacketManager();
        }

        private void SClientForm_Load(object sender, EventArgs e)
        {
            IPAddress ip = IPAddress.Parse("49.247.149.125");
            IPEndPoint endPoint = new IPEndPoint(ip, 7777);
            Connector connector = new Connector();
            connector.Connect(endPoint, () => { return sessionManager.Generate(); });
            
            
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            sessionManager.LoginSend();

        }

        private void btnSend_Click(object sender, EventArgs e)
        {
            byte[] img = ScreenCopy.Copy();
            sessionManager.ImgSend(img);
            Bitmap bmp;
            bmp = ScreenCopy.GetBitmap(img);
            ptBox.Image = bmp;

        }
    }
}
