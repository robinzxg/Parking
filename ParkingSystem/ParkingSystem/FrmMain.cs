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
using System.Diagnostics;
using System.Net;
using ParkingSystem;

namespace ParkingSystem
{
    public partial class FrmMain : Form
    {
        private static TcpListener tcpListener = null;
        private static TcpClient client = new TcpClient();
        private Thread serverThread;
        public static ParkingSystem.ParkingRemoteTCP wapper = new ParkingRemoteTCP();
        public FrmMain()
        {
            InitializeComponent();
            EthCommInit();
            ParkingOriginalPacket.EvProcessReceivedPacket += sp_ProcessReceivedPacket;
            this.btn_SerStart.Enabled = true;
            this.btn_SerExit.Enabled = false;
        }
        #region 属性
        public static List<string> clientList = new List<string>();//在线客户端集合
        private bool isConnect = false;//连接状态
        #endregion

        #region 初始化窗体
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (this.tabControl1.SelectedTab != this.tabPage1 && isConnect == false)
            {
                MessageBox.Show("Comm not open or Ethernet not connect ok,please open comm or connet Ethernet first!");
                this.tabControl1.SelectedTab = this.tabPage1;
                return;
            }
        }
        #endregion
        #region 自动加载ip地址
        private void EthCommInit()
        {
            string addresses = GetLocalAddresses();
            cmb_SerIP.Items.Clear();
            if (addresses.Length > 0)
            {

                cmb_SerIP.Items.Add(addresses);

                cmb_SerIP.Text = (string)cmb_SerIP.Items[0];
            }
        }
        public string GetLocalAddresses()
        {
            // 获取主机名
            string strHostName = Dns.GetHostName();
            System.Net.IPAddress addr;
            addr = new System.Net.IPAddress(Dns.GetHostByName(Dns.GetHostName()).AddressList[0].Address);
            return addr.ToString();
        }
        #endregion

        #region 窗体移动

        private Point mouseOff;//鼠标移动位置变量
        private bool leftFlag;//标签是否为左键
        private void Frm_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                mouseOff = new Point(-e.X, -e.Y); //得到变量的值
                leftFlag = true;                  //点击左键按下时标注为true;
            }
        }
        private void Frm_MouseMove(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                Point mouseSet = Control.MousePosition;
                mouseSet.Offset(mouseOff.X, mouseOff.Y);  //设置移动后的位置
                Location = mouseSet;
            }
        }
        private void Frm_MouseUp(object sender, MouseEventArgs e)
        {
            if (leftFlag)
            {
                leftFlag = false;//释放鼠标后标注为false;
            }
        }

        #endregion

        #region 关闭窗体
        private void btn_CloseSys_Click(object sender, EventArgs e)
        {
            ParkingOriginalPacket.EvProcessReceivedPacket -= sp_ProcessReceivedPacket;

            Application.Exit();
            Process.GetCurrentProcess().Kill();
        }
        #endregion

        #region 开启和关闭服务

        //开启服务
        private void btn_SerStart_Click(object sender, EventArgs e)
        {
            try
            {
                tcpListener = new TcpListener(IPAddress.Parse(this.cmb_SerIP.Text.Trim()), int.Parse(this.cmb_SerPort.Text.Trim()));
                tcpListener.Start();
                serverThread = new Thread(new ThreadStart(ReceiveAccept));
                serverThread.Start();
                serverThread.IsBackground = true;

                this.lst_Info.Items.Add(DateTime.Now.ToString());
                this.lst_Info.Items.Add("Server started successfully！");
                isConnect = true;
                this.btn_SerStart.Enabled = false;
                this.btn_SerExit.Enabled = true;
            }
            catch (Exception ex)
            {
                this.lst_Info.Items.Add(ex.Message);
            }

        }

        //关闭服务
        private void btn_SerExit_Click(object sender, EventArgs e)
        {
            try
            {
                tcpListener.Stop();
                tcpListener = null;
                serverThread.Abort();
                serverThread = null;
                client.Close();
                this.lst_Info.Items.Add(DateTime.Now.ToString());
                this.lst_Info.Items.Add("The server is shut down！");

                this.btn_SerStart.Enabled = true;
                this.btn_SerExit.Enabled = false;
            }
            catch (Exception ex)
            {
                this.lst_Info.Items.Add(ex.Message);
            }

        }
        #endregion

        #region 客户端在线列表的更新
        private void ReceiveAccept()
        {
            while (true)
            {
                try
                {
                    client = tcpListener.AcceptTcpClient();
                    wapper = new ParkingRemoteTCP(client);
                    string socketClient = client.Client.RemoteEndPoint.ToString();
                    clientList.Add(socketClient);
                    UpdateOnlie(socketClient, true);
                }

                catch (Exception)
                {


                }
            }
        }
       
            public void UpdateOnlie(string client, bool operate)
            {
                if (!this.lst_OnlieList.InvokeRequired)
                {
                    if (operate)
                    {
                        this.lst_OnlieList.Items.Add(client);
                        this.lst_Info.Items.Add(DateTime.Now.ToString() + "客户端:" + client + "连接！");
                    }
                    else
                    {
                        foreach (string item in this.lst_OnlieList.Items)
                        {
                            if (item == client)
                            {
                                this.lst_OnlieList.Items.Remove(item);
                                this.lst_Info.Items.Add(DateTime.Now.ToString() + "客户端:" + client + "断开！");
                                break;
                            }
                        }
                    }
                }
                else
                {
                    Invoke(new Action(() =>
                    {
                        if (operate)
                        {
                            this.lst_OnlieList.Items.Add(client);
                            this.lst_Info.Items.Add(DateTime.Now.ToString() + "客户端:" + client + "连接！");
                        }
                        else
                        {
                            foreach (string item in this.lst_OnlieList.Items)
                            {
                                if (item == client)
                                {
                                    this.lst_OnlieList.Items.Remove(item);
                                    this.lst_Info.Items.Add(DateTime.Now.ToString() + "客户端:" + client + "断开！");
                                    break;
                                }
                            }
                        }
                    }));
                }
            }

        #endregion

        private void sp_ProcessReceivedPacket(baseReceivedPacket pk)
        {
            throw new NotImplementedException();
        }


    }
}
