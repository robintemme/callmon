using System;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.IO;
using System.ComponentModel;


namespace callmon
{
    public partial class frmMain : Form
    {
        public frmMain()
        {
            InitializeComponent();
        }

        IPAddress ip = IPAddress.Parse("192.168.178.1");
        int port = 1012;

        TcpClient client = new TcpClient();

        NetworkStream stream;

        bool stillringing { get; set; }
        string caller = string.Empty;

        frmPopup notification = new frmPopup();

        private void frmMain_Load(object sender, EventArgs e)
        {
            try
            {
                client.Connect(ip, port);

                stream = client.GetStream();

                Thread ReceiveThread = new Thread(callmonaction);
                ReceiveThread.IsBackground = true;
                ReceiveThread.Start();

            }
            catch (Exception ex)
            {
                addtolog(ex);
            }
        }

        private void callmonaction()
        {
            StreamReader reader = new StreamReader(stream);
            string fbstatus = string.Empty;
            string[] currentline = new string[10];

            while (client.Connected)        
                {
                if (stream.DataAvailable)
                {
                    fbstatus = reader.ReadLine();

                    addtolog(fbstatus);

                    currentline = fbstatus.Split(new string[] { ";" }, StringSplitOptions.RemoveEmptyEntries);

                    if (currentline.Length != 1)
                    {
                        NewCallmonEvent(Convert.ToString(currentline.GetValue(0)),
                            Convert.ToString(currentline.GetValue(1)),
                            Convert.ToString(currentline.GetValue(3)));
                    }
                }
                Thread.Sleep(50);
            } 
        }

        private void NewCallmonEvent(string eventtime, string whathappened, string callerid)
        {
            notification.NumberOrName = callerid;
            notification.TimeStamp = eventtime;

            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string, string, string>(NewCallmonEvent), eventtime, whathappened, callerid);
            }
            else
            {
                switch (whathappened)
                {
                    case "RING":
                        notification.Show(this);
                        break;
                    case "CALL":

                        break;
                    case "CONNECT":

                        break;
                    case "DISCONNECT":
                        notification.Close();
                        break;
                }
            }
        }

        private void addtolog(object what)
        {
            if (this.InvokeRequired) this.Invoke(new Action<object>(addtolog),what);
            else
            {
                tbxOutput.Text += what;
                tbxOutput.Text += "\r\n";
            }
        }
    }
}
