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
        public SClientForm()
        {
            InitializeComponent();
            sclientform = this;
            sessionManager = new SessionManager();
        }

        private void SClientForm_Load(object sender, EventArgs e)
        {
            String host = Dns.GetHostName();
            IPHostEntry ipHost = Dns.GetHostEntry(host);
            IPAddress ipAddr = ipHost.AddressList[0];
            IPEndPoint endPoint = new IPEndPoint(ipAddr, 7773);
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
