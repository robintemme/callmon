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

            while (client.Connected)        
                {
                if (stream.DataAvailable)
                {
                    fbstatus = reader.ReadLine();
                    if (fbstatus != "")
                    {
                        addtolog(fbstatus);

                        NewCallmonEvent(fbstatus);
                    }

                }
                Thread.Sleep(50);
            } 
        }

        private string GetImportantData(string[] rawdata, int index)
        {
            return string.Empty;
        }

        private void NewCallmonEvent(string statusline)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action<string>(NewCallmonEvent), statusline);
            }
            else
            {
                string[] splittedstatus = new string[7];

                splittedstatus = statusline.Split(new string[] { ";" }, StringSplitOptions.None);

                frmPopup notification = new frmPopup();

                notification.TimeStamp = splittedstatus[0];
                

                switch (splittedstatus[1])
                {
                    case "RING":
                        notification.Type = 0;
                        notification.From = splittedstatus[3];
                        notification.To = splittedstatus[4];
                        notification.SIP = splittedstatus[5];
                        
                        notification.Show(this);
                        break;
                    case "CALL":
                        notification.Type = 1;
                        notification.From = splittedstatus[4];
                        notification.To = splittedstatus[5];
                        notification.SIP = splittedstatus[6];
                        notification.Show(this);
                        break;
                    case "CONNECT":
                        notification = (frmPopup)Application.OpenForms["frmPopup"];
                        notification.Close();
                        notification = null;
                        break;
                    case "DISCONNECT":
                        notification = (frmPopup)Application.OpenForms["frmPopup"];
                        notification.Close();
                        notification = null;
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
