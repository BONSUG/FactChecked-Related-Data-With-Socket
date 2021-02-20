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
using mshtml;

namespace client
{
    public partial class Form1 : Form
    {
        Thread ReceiveThread=null;
        bool Conneted;
        public Socket sender1;
        byte[] bytes = new byte[1024];
        public int port;
        public IPAddress ip;
        public string server_data = null;
        string temp=null;
        RelatedData r = new RelatedData();

        public Form1()
        {
            InitializeComponent();
            webBrowser1.ScriptErrorsSuppressed = true;
            webBrowser1.Navigate("www.naver.com");
            webBrowser1.IsWebBrowserContextMenuEnabled = false;
            webBrowser1.ContextMenuStrip = contextMenuStrip1;
        }
        private void button1_Click(object sender, EventArgs e)
        {
            PrintToTextBox();
            
        }

        private void button2_Click(object sender, EventArgs e)
        {
            port = 6000;

            ip = IPAddress.Parse("192");

            Start_Server();

            Thread thread_server;

            thread_server = new Thread(new ThreadStart(Receive));

            thread_server.Start();
        }

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            Conneted = false;

            ReceiveThread.Abort();
        }
        private void Start_Server()
        {
            try
            {
                IPEndPoint remoteEP = new IPEndPoint(ip, port);
                sender1 = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                try{
                    sender1.Connect(remoteEP);   //서버 연결
                    Conneted = true;
                }
                catch{

                }
            }
            catch
            {
            }
        }

        public void PrintToTextBox(){
            //byte[] msg = Encoding.UTF8.GetBytes(textBox1.Text);
            //int bytesSent = sender1.Send(msg);
        }

        public void Receive()
        {
            server_data = null;

            try
            {
                while (Conneted)
                {
                    int server_receive = sender1.Receive(bytes);
                    if (server_receive != 0)
                    {
                        server_data += Encoding.UTF8.GetString(bytes, 0, server_receive);
                        if (server_data.Equals(temp)) continue;
                        //textBox2.Text = "";
                        //textBox2.Text=(server_data + "\r\n");
                        //temp = server_data;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            try
            {
                Conneted = false;

                ReceiveThread.Abort();
            }
            catch { }
        }

        private void button3_Click(object sender, EventArgs e)
        {
            webBrowser1.GoBack();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            webBrowser1.GoForward();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string url = textBox3.Text;
            webBrowser1.Navigate(url);
        }

        private void textBox3_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) {
                button5_Click(sender, e);
            }
        }

        private void webBrowser1_DocumentCompleted(object sender, WebBrowserDocumentCompletedEventArgs e)
        {
            
            this.Text = webBrowser1.DocumentTitle;
            textBox3.Text=webBrowser1.Url.ToString();
        }
        public void Receive2()
        {
            server_data = null;

            try
            {
                while (Conneted)
                {
                    int server_receive = sender1.Receive(bytes);
                    if (server_receive != 0)
                    {
                        server_data = Encoding.UTF8.GetString(bytes, 0, server_receive);
                        
                        if (server_data.Equals(temp)) return;
                        r.OutputData = server_data;
                        temp = server_data;
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        private void 판별ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            IHTMLDocument2 htmlDocument = webBrowser1.Document.DomDocument as IHTMLDocument2;

            IHTMLSelectionObject currentSelection = htmlDocument.selection;

            if (currentSelection != null)
            {
                IHTMLTxtRange range = currentSelection.createRange() as IHTMLTxtRange;

                if (range != null)
                {
                    //MessageBox.Show(range.text);
                    r.InputData = range.text.ToString();
                }
            }
            //send
            byte[] msg = Encoding.UTF8.GetBytes(r.InputData);
            int bytesSent = sender1.Send(msg);

            //recv
            Receive2();
            r.StripData(r);
            MessageBox.Show("주제 : " + r.Title+"\n"+ "분류 : " + r.Type + "\n"+ "정확도 : " + r.Accuracy + "\n"+ "링크 : " + r.Link ) ;
        }
    }
}
