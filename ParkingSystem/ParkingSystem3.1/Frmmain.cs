using ParkingSys.Models;
using ParkingSystem;
using ParkingSystem.BLL;
using ParkingSystemDAL;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ParkingSystem3._1
{
    public partial class FrmMain : Form
    {
        private static TcpListener tcpListener = null;
        private static TcpClient client = new TcpClient();
        private Thread serverThread;
        public static ParkingSystem.ParkingRemoteTCP wapper = new ParkingRemoteTCP();
        private Parking parking = new Parking();
        private SensorData sensorData = new SensorData();
        public static ParkingSystem.ParkingSerialPort serialPort = new ParkingSerialPort();//创建一个串口对象
        public static ParkingSystem.ParkingSerialPort serialPort2 = new ParkingSerialPort();//创建串口2对象        
        protected override CreateParams CreateParams
        {
            get
            {
                const int WS_MINIMIZEBOX = 0x00020000;  // Winuser.h中定义  
                CreateParams cp = base.CreateParams;
                cp.Style = cp.Style | WS_MINIMIZEBOX;   // 允许最小化操作  
                return cp;
            }
        }
        public FrmMain()
        {
            InitializeComponent();
            this.Load += FrmMain_Load;
            EthCommInit();
            this.btn_SerStart.Enabled = true;
            this.btn_SerExit.Enabled = false;
            this.btn_Connect1.Enabled = true;
            this.btn_Close1.Enabled = false;
            this.btn_Connect2.Enabled = true;
            this.btn_Close2.Enabled = false;
            this.dateTimePicker1.Value.ToString("yyyy-MM-dd HH:mm:ss");
            this.dateTimePicker2.Value.ToString("yyyy-MM-dd HH:mm:ss");
            notifyIcon1.Visible = true;
            this.timerLed.Enabled = false;
            tabControl1.TabPages.Remove(tabPage5);
            this.dgv_LedListInfo.AutoGenerateColumns = false;
        }


        #region 属性
        public static Dictionary<string, TcpClient> dicClient = new Dictionary<string, TcpClient>();//在线客户端集合
        private bool isConnect = false;//服务器连接状态
        private bool comConnect1 = false;//串口1的连接状态
        private bool comConnect2 = false;//串口2的连接状态
        private bool serverConnect = false;

        public Dictionary<string, Label> dicSensorLbl = new Dictionary<string, Label>();//传感器Lable标签集合<ID,lable标签>

        public Dictionary<string, ListBox> dicSensorLst = new Dictionary<string, ListBox>();//传感器信息listbox集合<ID,listbox控件>

        public Dictionary<string, Sensor> dicSensor = new Dictionary<string , Sensor>();//<节点编号，传感器>

        public List<LedInfo> ledInfos = new List<LedInfo>();//led信息集合


        private List<DetectInfo> queryDetect = null;
        private List<ParkingInfo> queryParking = null;
        private List<Sensor> querySensors = null;
        private List<SensorList> queSensorList = null;
        #endregion

        #region 初始化窗体
        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (this.tabControl1.SelectedTab != this.tabPage1 && isConnect == false && comConnect1 == false)
            {
                MessageBox.Show("Comm not open or Ethernet not connect ok,please open comm or connet Ethernet first!");
                this.tabControl1.SelectedTab = this.tabPage1;
                return;
            }
            if (this.tabControl1.SelectedTab == this.tabPage6)
            {
                if (serverConnect == false)
                {
                    regainServer();//先从数据库恢复数据
                }
            }
        }
        private void FrmMain_Load(object sender, EventArgs e)
        {
            //初始化端口
            string[] portList1 = SerialPort.GetPortNames();//获取计算机中的端口
            string[] portList2 = SerialPort.GetPortNames();//获取计算机中的端口
            if (portList1.Length > 0)
            {
                this.cmb_PortName1.DataSource = portList1;

            }
            if (portList2.Length > 0)
            {
                this.cmb_PortName2.DataSource = portList2;
            }
            //设置波特率
            this.cmb_BaudRate1.DataSource = new string[] { "115200" };
            this.cmb_BaudRate2.DataSource = new string[] { "115200" };


        }

        //退出事件
        private void FrmMain_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                if (this.WindowState == FormWindowState.Minimized)
                {
                    this.WindowState = FormWindowState.Normal;
                }
                e.Cancel = true;
                DialogResult result = MessageBox.Show("Are you sure to log out of the parking system?", "warring", MessageBoxButtons.OKCancel);
                if (result == DialogResult.OK)
                {
                    if (isConnect == true)
                    {
                        tcpListener.Stop();
                        tcpListener = null;
                        serverThread.Abort();
                        serverThread = null;
                        client.Close();
                    }
                    notifyIcon1.Icon.Dispose();
                    Process.GetCurrentProcess().Kill();
                }
            }
        }

        //最小化
        private void btn_Min_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
        }

        //双击系统图标显示窗体
        private void notifyIcon1_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
            }
            if (this.WindowState == FormWindowState.Normal)
            {
                this.TopMost = true;
            }
            this.TopMost = false;

        }

        #region 系统图标的操作
        //显示
        private void toolStripMenuItem1_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Minimized)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Normal;
            }
        }
        //隐藏
        private void Hide_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }
            //this.Visible = false;
        }
        //退出
        private void toolStripMenuItem2_Click(object sender, EventArgs e)
        {
            DialogResult result = MessageBox.Show("Are you sure to log out of the parking system?", "warring", MessageBoxButtons.OKCancel);
            if (result == DialogResult.OK)
            {
                Process.GetCurrentProcess().Kill();
            }
        }
        #endregion

        #region 隐藏窗体到系统托盘
        private void btn_CloseSys_Click(object sender, EventArgs e)
        {
            if (this.WindowState == FormWindowState.Normal)
            {
                this.Visible = true;
                this.WindowState = FormWindowState.Minimized;
                this.Hide();
            }

        }
        #endregion

        #endregion

        #region datagridview自动添加行号
        private void dgv_Data_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                  e.RowBounds.Location.Y,
                  dgv_Data.RowHeadersWidth - 4,
                  e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dgv_Data.RowHeadersDefaultCellStyle.Font,
                rectangle,
               dgv_Data.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }
        private void dgv_LedListInfo_RowPostPaint(object sender, DataGridViewRowPostPaintEventArgs e)
        {
            Rectangle rectangle = new Rectangle(e.RowBounds.Location.X,
                  e.RowBounds.Location.Y,
                  dgv_LedListInfo.RowHeadersWidth - 4,
                  e.RowBounds.Height);
            TextRenderer.DrawText(e.Graphics, (e.RowIndex + 1).ToString(),
                dgv_LedListInfo.RowHeadersDefaultCellStyle.Font,
                rectangle,
               dgv_LedListInfo.RowHeadersDefaultCellStyle.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.HorizontalCenter);
        }
        #endregion

        #region 时间控件的启用      
        /// <summary>
        /// 时间控件的启用
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cmb_DateType_SelectedIndexChanged(object sender, EventArgs e)
        {
            switch (this.cmb_DateType.SelectedIndex)
            {
                case 0:
                    this.dateTimePicker1.Enabled = true;
                    this.dateTimePicker2.Enabled = true;
                    break;
                case 1:
                    this.dateTimePicker1.Enabled = true;
                    this.dateTimePicker2.Enabled = true;
                    break;
                case 2:
                    this.dateTimePicker1.Enabled = false;
                    this.dateTimePicker2.Enabled = false;
                    break;
                case 3:
                    this.dateTimePicker1.Enabled = false;
                    this.dateTimePicker2.Enabled = false;
                    break;
                default:
                    break;
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

        #region 串口的操作
        //打开串口1
        private void btn_Connect1_Click(object sender, EventArgs e)
        {
            try
            {
                //设置串口属性
                serialPort = new ParkingSerialPort(this.cmb_PortName1.Text, int.Parse(this.cmb_BaudRate1.Text));
                //打开串口
                serialPort.Open();
                comConnect1 = true;
                this.btn_Connect1.Enabled = false;
                this.btn_Close1.Enabled = true;
                this.lst_Info.Items.Add(DateTime.Now.ToString());
                this.lst_Info.Items.Add("port: " + this.cmb_PortName1.Text + " open successfully！");
            }
            catch (Exception ex)
            {
                serialPort.Close();
                comConnect1 = false;
                this.btn_Connect1.Enabled = true;
                this.lst_Info.Items.Add(DateTime.Now.ToString());
                this.lst_Info.Items.Add(" port: " + this.cmb_PortName1.Text + " open fail！");
                this.lst_Info.Items.Add(ex.ToString());
            }

        }

        //关闭串口1
        private void btn_Close1_Click(object sender, EventArgs e)
        {
            try
            {
                if (comConnect1 == true)
                {
                    serialPort.Close();
                    comConnect1 = false;
                    this.btn_Connect1.Enabled = true;
                    this.btn_Close1.Enabled = false;
                    this.lst_Info.Items.Add(DateTime.Now.ToString());
                    this.lst_Info.Items.Add(" port: " + serialPort.PortName + " Close ！");
                }
            }
            catch (Exception)
            {

                throw;
            }
        }

        /// <summary>
        /// 打开串口2
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Connect2_Click(object sender, EventArgs e)
        {
            try
            {
                //设置串口属性
                serialPort2 = new ParkingSerialPort(this.cmb_PortName2.Text, int.Parse(this.cmb_BaudRate2.Text));
                //打开串口
                serialPort2.Open();
                comConnect2 = true;
                this.btn_Connect2.Enabled = false;
                this.btn_Close2.Enabled = true;
                this.lst_Info.Items.Add(DateTime.Now.ToString());
                this.lst_Info.Items.Add("port: " + this.cmb_PortName2.Text + " open successfully！");
            }
            catch (Exception ex)
            {
                serialPort.Close();
                comConnect2 = false;
                this.btn_Connect2.Enabled = true;
                this.lst_Info.Items.Add(DateTime.Now.ToString());
                this.lst_Info.Items.Add(" port: " + this.cmb_PortName2.Text + " open fail！");
                this.lst_Info.Items.Add(ex.ToString());
            }
        }

        private void btn_Close2_Click(object sender, EventArgs e)
        {
            try
            {
                if (comConnect2 == true)
                {
                    serialPort.Close();
                    comConnect2 = false;
                    this.btn_Connect2.Enabled = true;
                    this.btn_Close2.Enabled = false;
                    this.lst_Info.Items.Add(DateTime.Now.ToString());
                    this.lst_Info.Items.Add(" port: " + serialPort2.PortName + " Close ！");
                }
            }
            catch (Exception)
            {

                throw;
            }
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
                this.lst_Info.Items.Add(DateTime.Now.ToString());
                this.lst_Info.Items.Add("Server startup failure!!!！");
                this.lst_Info.Items.Add(" erro：" + ex.Message);

            }

        }
        private static void WriteLog(string msg)
        {
            FileStream fs = new FileStream("Log1.txt", FileMode.Append);
            StreamWriter sw = new StreamWriter(fs);
            sw.WriteLine("[{0}]  erro：{1}", DateTime.Now.ToString(), msg);
            sw.Close();
            fs.Close();
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
                isConnect = false;
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
                    string socketClient = client.Client.RemoteEndPoint.ToString();
                    wapper = new ParkingRemoteTCP(client);
                    if (!dicClient.ContainsKey(socketClient))
                    {
                        dicClient.Add(socketClient, client);
                    }
                    else
                    {
                        dicClient[socketClient] = client;
                    }
                    UpdateOnlie(socketClient, true);
                }
                catch (Exception ex)
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
                    if (this.lst_OnlieList.Items.Contains(client))
                    {
                        this.lst_Info.Items.Add(DateTime.Now.ToString());
                        this.lst_Info.Items.Add("Client:" + client + "connection！");
                    }
                    else
                    {
                        this.lst_OnlieList.Items.Add(client);
                        this.lst_Info.Items.Add(DateTime.Now.ToString());
                        this.lst_Info.Items.Add("Client:" + client + "connection！");
                    }
                }
                else
                {
                    foreach (string item in this.lst_OnlieList.Items)
                    {
                        if (item == client)
                        {
                            this.lst_OnlieList.Items.Remove(item);
                            this.lst_Info.Items.Add(DateTime.Now.ToString());
                            this.lst_Info.Items.Add("Client:" + client + "break！");
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
                        if (this.lst_OnlieList.Items.Contains(client))
                        {

                            this.lst_Info.Items.Add(DateTime.Now.ToString());
                            this.lst_Info.Items.Add("Client:" + client + "reconnection！");
                        }

                        else
                        {
                            this.lst_OnlieList.Items.Add(client);
                            this.lst_Info.Items.Add(DateTime.Now.ToString());
                            this.lst_Info.Items.Add("Client:" + client + "connection！");
                        }
                    }
                    else
                    {
                        foreach (string item in this.lst_OnlieList.Items)
                        {
                            if (item == client)
                            {
                                this.lst_OnlieList.Items.Remove(item);
                                this.lst_Info.Items.Add(DateTime.Now.ToString());
                                this.lst_Info.Items.Add("Client:" + client + "break！");
                                break;
                            }
                        }
                    }
                }));
            }
        }
        #endregion

        #region 数据包解析

        /// <summary>
        /// 解析数据包
        /// </summary>
        /// <param name="pk"></param>
        private void sp_ProcessReceivedPacket(baseReceivedPacket pk)
        {
            try
            {
                UInt16 revType = pk.type_ver;
                this.Invoke((EventHandler)delegate
                {
                    #region 心跳包的解析

                    if (pk is SensorHBeat)
                    {
                        SensorHBeat hb = (SensorHBeat)pk;
                        showMsg(hb.recRawData, true, this.rtb_recMsg);
                        Sensor sensor = new Sensor
                        {
                            SensorId = (hb.WPSD_ID).ToString("X2").PadLeft(8, '0'),
                            SensorWdcId = (hb.WDC_ID).ToString("X2").PadLeft(8, '0'),
                            SensorSoftver = "v" + int.Parse(hb.APP_VER.ToString("X2").Substring(0, 1)).ToString() + "." + int.Parse(hb.APP_VER.ToString("X2").Substring(1, 1)).ToString().PadLeft(2, '0'),
                            SensorHardver = "v" + ((int)(hb.HARD_VER) + 10).ToString().Substring(0, 1) + "." + ((int)(hb.HARD_VER) + 10).ToString().Substring(1, 1),
                            SensorVoltage = (Math.Round((decimal)hb.VOLT / 10, 2)).ToString() + "V",
                            SensorRssi = ((Int16)hb.RSSI - 30).ToString(),
                            SensorHB = hb.HB_PERIOD.ToString(),
                            DateTimeRec = hb.rxtime,
                            SensorModel = GetDevName(hb.DEV_TYPE),
                            SensorSerial = hb.SN,
                            sensorState = hb.CAR_STATE
                        };

                        if (dicSensorLbl.ContainsKey(sensor.SensorId))
                        {
                            sensor.SensorRunTime = (sensor.DateTimeRec - dicSensor[sensor.SensorId].DateTimeRec).Days + "D" + (sensor.DateTimeRec - dicSensor[sensor.SensorId].DateTimeRec).Hours + "H" + (sensor.DateTimeRec - dicSensor[sensor.SensorId].DateTimeRec).Minutes + "m" + (sensor.DateTimeRec - dicSensor[sensor.SensorId].DateTimeRec).Seconds + "s";

                            SensorUpdate(sensor);
                        }
                        else
                        {
                            AddSensor(sensor, p_SensorPannel);
                        }
                        sensorData.AddSensorHeartData(sensor);//添加实时数据到数据库                                          
                    }
                    #endregion

                    #region 检测包的解析
                    if (pk is SensorDetect)
                    {
                        SensorDetect sensorDetect = (SensorDetect)pk;
                        showMsg(sensorDetect.recRawData, true, this.rtb_recMsg);

                        Sensor sensor = new Sensor
                        {
                            SensorId = (sensorDetect.WPSD_ID).ToString("X2").PadLeft(8, '0'),
                            SensorWdcId = (sensorDetect.WDC_ID).ToString("X2").PadLeft(8, '0'),
                            DateTimeRec = sensorDetect.rxtime,
                            SensorModel = GetDevName(sensorDetect.DEV_TYPE),
                            SensorSerial = sensorDetect.SN,
                            sensorState = sensorDetect.CAR_STATE
                        };
                        if (dicSensorLbl.ContainsKey(sensor.SensorId))
                        {
                            sensor.SensorRunTime = (sensor.DateTimeRec - dicSensor[sensor.SensorId].DateTimeRec).Days+ "D"+(sensor.DateTimeRec - dicSensor[sensor.SensorId].DateTimeRec).Hours+"H"+ (sensor.DateTimeRec - dicSensor[sensor.SensorId].DateTimeRec).Minutes+"m"+ (sensor.DateTimeRec - dicSensor[sensor.SensorId].DateTimeRec).Seconds+"s";
                            SensorUpdate(sensor);
                        }
                        else
                        {
                            AddSensor(sensor, p_SensorPannel);
                        }
                        sensorData.AddSensorHeartData(sensor);//添加实时数据到数据库
                    }
                    #endregion
                });
            }
            catch (Exception ex)
            {

            }
        }
        #endregion

        #region 节点型号
        /// <summary>
        /// 节点型号
        /// </summary>
        /// <param name="byteName"></param>
        /// <returns></returns>
        private string GetDevName(byte byteName)
        {
            string devname = "";
            switch (byteName)
            {
                case 0x01:
                    devname = "WDC-4003";
                    break;
                case 0x02:
                    devname = "WDC-4005";
                    break;
                case 0x03:
                    devname = "WDC-4008";
                    break;
                case 0x04:
                    devname = "WDC-4007";
                    break;
                case 0x05:
                    devname = "WPSD-340S3";
                    break;
                case 0x06:
                    devname = "WPSD-340S5";
                    break;
                case 0x07:
                    devname = "WPSD-340S8";
                    break;
                case 0x08:
                    devname = "WPSD-340S7";
                    break;
                case 0x09:
                    devname = "WPSD-340E3";
                    break;
                case 0x0A:
                    devname = "WPSD-340E5";
                    break;
                case 0x0B:
                    devname = "WPSD-340E8";
                    break;
                case 0x0C:
                    devname = "WPSD-340E7";
                    break;

                default:
                    devname = "WDC-400x";
                    break;
            }
            return devname;
        }
        #endregion

        #region 接收信息显示
        //信息显示
        public void showMsg(byte[] msg, bool source, RichTextBox richTextBox)
        {
            string restr = "";
            if (msg != null)
            {
                for (int i = 0; i < msg.Length; i++)
                {
                    restr += msg[i].ToString("X2");
                    restr += " ";
                }
            }
            if (source)
            {
                richTextBox.AppendText(System.DateTime.Now.ToString() + "[Received]:  " + restr + Environment.NewLine);
            }
            else
            {
                richTextBox.AppendText(System.DateTime.Now.ToString() + "[SendInfo]:  " + restr + Environment.NewLine);
            }
            richTextBox.ScrollToCaret();
        }
        #endregion

        #region 添加传感器标签以及实时状态的改变

        //添加传感器
        private void AddSensor(Sensor sensor, Panel panel)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new MethodInvoker(delegate { AddSensor(sensor, panel); }));
                return;
            }
            //创建lable控件

            Label lbl = new Label
            {
                BorderStyle = System.Windows.Forms.BorderStyle.None,
                Font = new System.Drawing.Font("宋体", 15F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(134))),
                Name = sensor.SensorId,
                Size = new System.Drawing.Size(106, 64),

                TextAlign = System.Drawing.ContentAlignment.BottomCenter
            };
            if (sensor.SensorNum > 0)
            {
                lbl.Location = new System.Drawing.Point(10 + (130 * ((sensor.SensorNum - 1) % 8)), 8 + 90 * (int)Math.Floor((double)(sensor.SensorNum - 1) / 8));
                lbl.Text = sensor.SensorNum.ToString();
            }
            else
            {
                lbl.Location = new System.Drawing.Point(10 + (130 * ((panel.Controls.Count / 2) % 8)), 8 + 90 * (int)Math.Floor((double)(panel.Controls.Count / 2) / 8));
                lbl.Text = (panel.Controls.Count / 2 + 1).ToString();
                SensorList sensorList = new SensorList
                {
                    SensorID = sensor.SensorId,
                    SensorNum = int.Parse(lbl.Text),
                    AddDateTime = DateTime.Now
                };
                sensorData.AddSensorList(sensorList);//插入车位号到数据库               
            }
            if (sensor.SensorState == "occupy")
            {
                lbl.Image = Image.FromFile("havecar.png");
                lbl.Tag = 1;
            }
            if (sensor.SensorState == "vacant")
            {
                lbl.Image = Image.FromFile("nocar.png");
                lbl.Tag = 0;
            }
            ParkingAdd((int)lbl.Tag);
            ledAddParking(int.Parse(lbl.Text), (int)lbl.Tag);
            lbl.MouseClick += label13_MouseClick;//绑定控件单击事件

            //创建listbox控件
            ListBox listBox = new ListBox
            {
                Font = new System.Drawing.Font("宋体", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(134))),
                FormattingEnabled = true,
                ItemHeight = 14,
                Location = lbl.Location,
                //listBox.Location= new System.Drawing.Point(584, 47);
                Name = lbl.Text,
                Size = new System.Drawing.Size(120, 90),
                Visible = false,
                Text = sensor.SensorId
            };


            listBox.Items.Add("ID:" + sensor.SensorId);
            listBox.Items.Add("state:" + sensor.SensorState);
            // listBox.Items.Add("hw:" + sensor.SensorHardver);
            //listBox.Items.Add("sw:" + sensor.SensorSoftver);
            listBox.Items.Add("vlote:" + sensor.SensorVoltage);
            listBox.Items.Add("Rssi:" + sensor.SensorRssi);
            listBox.Items.Add("hb:" + sensor.SensorHB);
            listBox.Items.Add("serial:" + sensor.SensorSerial);
            listBox.Items.Add(sensor.DateTimeRec);
            listBox.MouseLeave += lst_SensorInfo_MouseLeave;//绑定鼠标离开事件

            //哪个控件先添加 哪个控件就在最前面
            panel.Controls.Add(listBox);
            panel.Controls.Add(lbl);

            dicSensorLbl.Add(sensor.SensorId, lbl);
            dicSensorLst.Add(sensor.SensorId, listBox);
            dicSensor.Add(sensor.SensorId, sensor);

        }

        //传感器实时更新状态
        private void SensorUpdate(Sensor sensor)
        {
            Label label = dicSensorLbl[sensor.SensorId];
            // dicSensor[int.Parse(label.Text)] = sensor;
            if (sensor.SensorState == "occupy")
            {
                if ((int)label.Tag == 0)
                {
                    dicSensorLbl[sensor.SensorId].Image = Image.FromFile("havecar.png");
                    dicSensorLbl[sensor.SensorId].Tag = 1;
                    ParkingUpdate((int)label.Tag);
                    ledUpdate(int.Parse(label.Text), (int)label.Tag);
                    sensorData.AddSensorData(sensor);//更新检测数据到数据库,如果状态改变就上传到对应的数据库

                }
            }
            if (sensor.SensorState == "vacant")
            {
                if ((int)label.Tag == 1)
                {
                    dicSensorLbl[sensor.SensorId].Image = Image.FromFile("nocar.png");
                    dicSensorLbl[sensor.SensorId].Tag = 0;
                    ParkingUpdate((int)label.Tag);
                    ledUpdate(int.Parse(label.Text), (int)label.Tag);
                    sensorData.AddSensorData(sensor);//更新检测数据到数据库,如果状态改变就上传到对应的数据库
                }
            }

            dicSensorLst[sensor.SensorId].Items.Clear();
            dicSensorLst[sensor.SensorId].Items.Add("ID:" + sensor.SensorId);
            dicSensorLst[sensor.SensorId].Items.Add("state:" + sensor.SensorState);
            // dicSensorLst[sensor.SensorId].Items.Add("hw:" + sensor.SensorHardver);
            //dicSensorLst[sensor.SensorId].Items.Add("sw:" + sensor.SensorSoftver);
            dicSensorLst[sensor.SensorId].Items.Add("vlote:" + sensor.SensorVoltage);
            dicSensorLst[sensor.SensorId].Items.Add("Rssi:" + sensor.SensorRssi);
            dicSensorLst[sensor.SensorId].Items.Add("hb:" + sensor.SensorHB);
            dicSensorLst[sensor.SensorId].Items.Add("serial:" + sensor.SensorSerial);
            dicSensorLst[sensor.SensorId].Items.Add(sensor.DateTimeRec);

           dicSensor[sensor.SensorId] = sensor;//更新集合里面的信息
        }
        #endregion

        #region 鼠标移动显示
        /// <summary>
        /// 鼠标离开
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lst_SensorInfo_MouseLeave(object sender, EventArgs e)
        {
            ListBox listBox = (ListBox)sender;
            listBox.Hide();
        }

        /// <summary>
        /// 鼠标单击显示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void label13_MouseClick(object sender, MouseEventArgs e)
        {
            Label lbl = (Label)sender;
            dicSensorLst[lbl.Name].Show();
            this.txt_senorID.Text = lbl.Name;
            this.txt_ParkingNum.Text = lbl.Text;
        }
        #endregion

        #region 车位数量信息
        /// <summary>
        /// 增加车位
        /// </summary>
        /// <param name="tag"></param>
        private void ParkingAdd(int tag)
        {
            parking.ParkingSum += 1;
            if (tag == 0)
            {
                parking.ParkingVacant += 1;
            }
            if (tag == 1)
            {
                parking.ParkingOccupy += 1;
            }
            this.txt_Space.Text = parking.ParkingSum.ToString();
            this.txt_occupy.Text = parking.ParkingOccupy.ToString();
            this.txt_Vacant.Text = parking.ParkingVacant.ToString();


            ParkingInfo parkingInfo = new ParkingInfo
            {
                ParkingSum = parking.ParkingSum,
                ParkingOccupy = parking.ParkingOccupy,
                ParkingVacant = parking.ParkingVacant,
                DateTimeUpdate = DateTime.Now
            };
            //更新停车位数据到数据库
            sensorData.AddParkingData(parkingInfo);


        }

        /// <summary>
        /// 车位状态更新
        /// </summary>
        /// <param name="tag"></param>
        private void ParkingUpdate(int tag)
        {
            if (tag == 0)
            {
                parking.ParkingOccupy -= 1;
                parking.ParkingVacant += 1;
            }
            if (tag == 1)
            {
                parking.ParkingOccupy += 1;
                parking.ParkingVacant -= 1;
            }
            this.txt_Space.Text = parking.ParkingSum.ToString();
            this.txt_occupy.Text = parking.ParkingOccupy.ToString();
            this.txt_Vacant.Text = parking.ParkingVacant.ToString();


            ParkingInfo parkingInfo = new ParkingInfo
            {
                ParkingSum = parking.ParkingSum,
                ParkingOccupy = parking.ParkingOccupy,
                ParkingVacant = parking.ParkingVacant,
                DateTimeUpdate = DateTime.Now
            };
            //更新停车位数据到数据库
            sensorData.AddParkingData(parkingInfo);

        }
        #endregion

        #region 停车场设置

        /// <summary>
        /// 车位查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_queSensor_Click(object sender, EventArgs e)
        {
            QueParking();
        }
        private void QueParking()
        {
            string sensorID = this.txt_senorID.Text.Trim();
            string parkingNum = this.txt_ParkingNum.Text.Trim();
            if (dicSensorLbl.ContainsKey(sensorID))
            {
                this.txt_ParkingNum.Text = dicSensorLbl[sensorID].Text.Trim();
            }

            if (sensorID.Length == 0 && parkingNum.Length > 0)
            {
                foreach (Label item in dicSensorLbl.Values)
                {
                    if (item.Text == parkingNum)
                    {
                        this.txt_senorID.Text = item.Name;
                    }
                }
            }
        }

        /// <summary>
        /// 车位设置
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ParkingSet_Click(object sender, EventArgs e)
        {
            ParkingSet();
        }
        private void ParkingSet()
        {
            if (this.txt_senorID.Text != "" && this.txt_ParkingNum.Text != "")
            {
                string sensorID = this.txt_senorID.Text.Trim();
                int parkingNum = int.Parse(this.txt_ParkingNum.Text.Trim());
                if (dicSensorLbl.ContainsKey(sensorID) && parkingNum > 0)
                {
                    dicSensorLbl[sensorID].Text = parkingNum.ToString();
                    //dicSensorLbl[sensorID].Location = new System.Drawing.Point(10 + (130 * ((parkingNum - 1) % 8)), 8 + 90 * (int)Math.Floor((double)(parkingNum - 1) / 8));
                    //dicSensorLst[sensorID].Location = new System.Drawing.Point(10 + (130 * ((parkingNum - 1) % 8)), 8 + 90 * (int)Math.Floor((double)(parkingNum - 1) / 8));


                    SensorList sensorList = new SensorList()
                    {
                        SensorID = sensorID,
                        SensorNum = parkingNum,
                        AddDateTime = DateTime.Now
                    };
                    sensorData.AddSensorList(sensorList);//插入停车场节点编号


                }
            }
        }

        /// <summary>
        /// 删除车位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ParkingDel_Click(object sender, EventArgs e)
        {
            string sensorID = this.txt_senorID.Text.Trim();
            if (dicSensorLbl.ContainsKey(sensorID))
            {
                DialogResult result = MessageBox.Show("Are you sure you want to remove this sensor?", "warring", MessageBoxButtons.OKCancel);
                {
                    if (result == DialogResult.OK)
                    {
                        parking.ParkingSum -= 1;
                        if ((int)dicSensorLbl[sensorID].Tag == 0)
                        {
                            parking.ParkingVacant -= 1;
                        }
                        if ((int)dicSensorLbl[sensorID].Tag == 1)
                        {
                            parking.ParkingOccupy -= 1;
                        }
                        p_SensorPannel.Controls.Remove(dicSensorLbl[sensorID]);
                        p_SensorPannel.Controls.Remove(dicSensorLst[sensorID]);
                        dicSensorLbl.Remove(sensorID);
                        dicSensorLst.Remove(sensorID);

                        this.txt_Space.Text = parking.ParkingSum.ToString();
                        this.txt_occupy.Text = parking.ParkingOccupy.ToString();
                        this.txt_Vacant.Text = parking.ParkingVacant.ToString();
                    }
                }
            }
        }

        /// <summary>
        /// 清除全部停车位
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_ParkingClear_Click(object sender, EventArgs e)
        {
            parkingClear();
        }
        private void parkingClear()
        {
            DialogResult result = MessageBox.Show("Are you sure you want to clear all sensor?", "warring", MessageBoxButtons.OKCancel);
            {
                if (result == DialogResult.OK)
                {
                    p_SensorPannel.Controls.Clear();
                    dicSensorLbl.Clear();
                    dicSensorLst.Clear();
                    parking.ParkingSum = 0;
                    parking.ParkingOccupy = 0;
                    parking.ParkingVacant = 0;
                    this.txt_Space.Text = parking.ParkingSum.ToString();
                    this.txt_occupy.Text = parking.ParkingOccupy.ToString();
                    this.txt_Vacant.Text = parking.ParkingVacant.ToString();
                    this.txt_senorID.Text = "";
                    this.txt_ParkingNum.Text = "";
                }
            }
        }
        //传感器根据序号排序
        private void btn_sensorList_Click(object sender, EventArgs e)
        {
            foreach (Label lbl in dicSensorLbl.Values)
            {
                lbl.Location = new System.Drawing.Point(10 + (130 * ((int.Parse(lbl.Text) - 1) % 8)), 8 + 90 * (int)Math.Floor((double)(int.Parse(lbl.Text) - 1) / 8));
                dicSensorLst[lbl.Name].Location = lbl.Location;
            }

        }
        #endregion

        #region 查询数据库
        /// <summary>
        /// 数据库查询
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_QuerySer_Click(object sender, EventArgs e)
        {

            string dateTime1 = this.dateTimePicker1.Text;
            string dateTime2 = this.dateTimePicker2.Text;
            string sensorID = this.txt_queID.Text.Trim();
            try
            {
                //查询车位数量信息
                if (this.cmb_DateType.SelectedIndex == 0)
                {

                    queryParking = sensorData.QueryParking(dateTime1, dateTime2);
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = queryParking;
                    this.dgv_Data.DataSource = null;
                    this.dgv_Data.DataSource = bindingSource;
                }
                //查询检测数据
                if (this.cmb_DateType.SelectedIndex == 1)
                {

                    queryDetect = sensorData.QueryDetect(sensorID, dateTime1, dateTime2);
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = queryDetect;
                    this.dgv_Data.DataSource = null;
                    this.dgv_Data.DataSource = bindingSource;
                }
                //查询所有实时数据
                if (this.cmb_DateType.SelectedIndex == 2)
                {

                    //querySensors = sensorData.QueryData(sensorID);
                    querySensors = sensorData.QueryNewSensor();
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = querySensors;
                    this.dgv_Data.DataSource = null;
                    this.dgv_Data.DataSource = bindingSource;
                }
                //查询节点对应车位号
                if (this.cmb_DateType.SelectedIndex == 3)
                {

                    queSensorList = sensorData.SensorList(sensorID);
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = queSensorList;
                    this.dgv_Data.DataSource = null;
                    this.dgv_Data.DataSource = bindingSource;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Database query failed:" + ex.ToString());
            }
        }
        #endregion

        #region 数据库恢复信息
        private void regainServer()
        {
            DialogResult result = MessageBox.Show("Do you want to recover node information from the database?", "warring", MessageBoxButtons.OKCancel);
            {
                if (result == DialogResult.OK)
                {
                    ParkingOriginalPacket.EvProcessReceivedPacket -= sp_ProcessReceivedPacket;
                    parkingClear();
                    querySensors = sensorData.QueryNewSensor();
                    foreach (Sensor sensor in querySensors)
                    {
                        AddSensor(sensor, p_SensorPannel);
                    }
                }
                ParkingOriginalPacket.EvProcessReceivedPacket += sp_ProcessReceivedPacket;
                serverConnect = true;
            }
        }
        #endregion

        //#region 保存车位信息       
        ///// <summary>
        ///// 保存车位信息到数据库
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void btn_SaveServer_Click(object sender, EventArgs e)
        //{
        //    foreach (Label label in dicSensorLbl.Values)
        //    {
        //        SensorList sensorList = new SensorList
        //        {
        //            SensorID = label.Name,
        //            SensorNum = int.Parse(label.Text)
        //        };
        //        sensorData.AddSensorList(sensorList);//插入车位号到数据库
        //    }
        //    //foreach (KeyValuePair<int, Sensor> dic in dicSensor)
        //    //{
        //    //    SensorList sensorList = new SensorList
        //    //    {
        //    //        SensorID = dic.Value.SensorId,
        //    //        SensorNum = dic.Key
        //    //    };
        //    //    sensorData.AddSensorList(sensorList);//插入车位号到数据库
        //    //}
        //}
        //#endregion

        //#region 更新数据库的节点
        //public void UpdateSensorData(Sensor sensor)
        //{

        //    int count = sensorData.QueSen(sensor);
        //    if (count > 0)
        //    {
        //        sensorData.UpdateSensor(sensor);//更新节点数据库信息
        //    }
        //    else
        //    {
        //        sensorData.AddSensorHeartData(sensor);//添加实时数据到数据库
        //    }

        //}
        //#endregion

        #region 连接多个显示屏     

        #region 连接显示屏     
        /// <summary>
        /// 连接总显示屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetSum_Click(object sender, EventArgs e)
        {
            SetParking(this.txt_LedIpSum.Text.Trim(), int.Parse(this.txt_StartNumSum.Text.Trim()), int.Parse(this.txt_EndNumSum.Text.Trim()), txt_ParkingSumSpace, txt_ParkingSumOccupy, txt_ParkingSumVacant);

        }

        /// <summary>
        /// 连接A区显示屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetParkingA_Click(object sender, EventArgs e)
        {
            SetParking(this.txt_LedIpA.Text.Trim(), int.Parse(this.txt_StartNumA.Text.Trim()), int.Parse(this.txt_EndNumA.Text.Trim()), txt_ParkingASpace, txt_ParkingAOccupy, txt_ParkingAVacant);
        }

        /// <summary>
        /// 连接B区显示屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetParkingB_Click(object sender, EventArgs e)
        {
            SetParking(this.txt_LedIpB.Text.Trim(), int.Parse(this.txt_StartNumB.Text.Trim()), int.Parse(this.txt_EndNumB.Text.Trim()), txt_ParkingBSpace, txt_ParkingBOccupy, txt_ParkingBVacant);
        }

        /// <summary>
        /// 连接C区显示屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_SetParkingC_Click(object sender, EventArgs e)
        {
            SetParking(this.txt_LedIpC.Text.Trim(), int.Parse(this.txt_StartNumC.Text.Trim()), int.Parse(this.txt_EndNumC.Text.Trim()), txt_ParkingCSpace, txt_ParkingCOccupy, txt_ParkingCVacant);
        }

        /// <summary>
        /// 设置显示屏显示的连接
        /// </summary>
        /// <param name="LedIp"></param>
        /// <param name="StartNum"></param>
        /// <param name="EndNum"></param>
        private void SetParking(string LedIp, int StartNum, int EndNum, TextBox txt_sum, TextBox txt_occupy, TextBox txt_vacant)
        {
            int lbltext;
            int space = 0;
            int occupy = 0;
            int vacant = 0;
            bool ledConnect = false;

            try
            {
                if (dicClient.ContainsKey(LedIp))//判断ledIP
                {
                    foreach (LedInfo item in ledInfos)
                    {
                        if (item.LedIP == LedIp && item.LedConnect == true)
                        {
                            rtb_LedInfo.AppendText(System.DateTime.Now.ToString() + " :" + LedIp + " Don't open it again!!!" + Environment.NewLine);
                            return;
                        }
                    }
                    foreach (Label lbl in dicSensorLbl.Values)
                    {
                        lbltext = int.Parse(lbl.Text);
                        if (lbltext >= StartNum && lbltext <= EndNum)//判断lbl标签的值是不是在范围之内
                        {
                            if ((int)lbl.Tag == 1)
                            {
                                occupy += 1;
                            }
                            if ((int)lbl.Tag == 0)
                            {
                                vacant += 1;
                            }
                            space += 1;
                        }
                    }
                    ledConnect = true;

                    this.timerLed.Start();
                    LedInfo ledInfo = new LedInfo()
                    {
                        LedIP = LedIp,
                        StartNum = StartNum,
                        EndNum = EndNum,
                        parkingSum = space,
                        parkingOccupy = occupy,
                        parkingVacant = vacant,
                        LedConnect = ledConnect
                    };
                    ledInfos.Add(ledInfo);
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = ledInfos;
                    this.dgv_LedListInfo.DataSource = null;
                    this.dgv_LedListInfo.DataSource = bindingSource;

                    txt_sum.DataBindings.Clear();
                    txt_occupy.DataBindings.Clear();
                    txt_vacant.DataBindings.Clear();

                    txt_sum.DataBindings.Add("Text", ledInfo, "parkingSum");
                    txt_occupy.DataBindings.Add("Text", ledInfo, "parkingOccupy");
                    txt_vacant.DataBindings.Add("Text", ledInfo, "parkingVacant");
                    rtb_LedInfo.AppendText(System.DateTime.Now.ToString() + " :" + LedIp + "  Set Succced!!!" + Environment.NewLine);

                }
                else
                {
                    rtb_LedInfo.AppendText(System.DateTime.Now.ToString() + " :" + LedIp + "  Set Fail!!!" + Environment.NewLine);
                }
            }

            catch (Exception ex)
            {
                rtb_LedInfo.AppendText(System.DateTime.Now.ToString() + "erro :" + "  Set Fail!!!" + Environment.NewLine);
                MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 增加不在界面显示的显示屏连接
        /// </summary>
        /// <param name="LedIp"></param>
        /// <param name="StartNum"></param>
        /// <param name="EndNum"></param>
        private void AddParking(string LedIp, int StartNum, int EndNum)
        {
            int lbltext;
            int space = 0;
            int occupy = 0;
            int vacant = 0;
            bool ledConnect;

            try
            {
                if (dicClient.ContainsKey(LedIp))//判断ledIP
                {
                    foreach (LedInfo item in ledInfos)
                    {
                        if (item.LedIP == LedIp && item.LedConnect == true)
                        {
                            rtb_LedInfo.AppendText(System.DateTime.Now.ToString() + " :" + LedIp + " Don't open it again!!!" + Environment.NewLine);
                            return;
                        }
                    }
                    foreach (Label lbl in dicSensorLbl.Values)
                    {
                        lbltext = int.Parse(lbl.Text);
                        if (lbltext >= StartNum && lbltext <= EndNum)//判断lbl标签的值是不是在范围之内
                        {
                            if ((int)lbl.Tag == 1)
                            {
                                occupy += 1;
                            }
                            if ((int)lbl.Tag == 0)
                            {
                                vacant += 1;
                            }
                            space += 1;
                        }
                    }
                    ledConnect = true;

                    LedInfo ledInfo = new LedInfo()
                    {
                        LedIP = LedIp,
                        StartNum = StartNum,
                        EndNum = EndNum,
                        parkingSum = space,
                        parkingOccupy = occupy,
                        parkingVacant = vacant,
                        LedConnect = ledConnect
                    };
                    ledInfos.Add(ledInfo);
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = ledInfos;
                    this.dgv_LedListInfo.DataSource = null;
                    this.dgv_LedListInfo.DataSource = bindingSource;

                    rtb_LedInfo.AppendText(System.DateTime.Now.ToString() + " :" + LedIp + "  Add Succced!!!" + Environment.NewLine);

                }
                else
                {
                    rtb_LedInfo.AppendText(System.DateTime.Now.ToString() + " :" + LedIp + "  Add Fail!!!" + Environment.NewLine);
                }
            }

            catch (Exception ex)
            {
                rtb_LedInfo.AppendText(System.DateTime.Now.ToString() + "erro :" + "  Set Fail!!!" + Environment.NewLine);
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion

        #region 显示屏信息更新

        /// <summary>
        /// 显示屏更新
        /// </summary>
        /// <param name="SensorNum"></param>
        /// <param name="tag"></param>
        private void ledUpdate(int SensorNum, int tag)
        {
            foreach (LedInfo item in ledInfos)
            {
                if (SensorNum >= item.StartNum && SensorNum <= item.EndNum)
                {
                    if (item.LedConnect == true)
                    {
                        if (tag == 0)
                        {
                            item.ParkingOccupy -= 1;
                            item.ParkingVacant += 1;
                        }
                        if (tag == 1)
                        {
                            item.ParkingOccupy += 1;
                            item.ParkingVacant -= 1;
                        }
                        SendLed(item);//给显示屏发送消息
                    }
                }
            }
        }

        private void ledAddParking(int SensorNum, int tag)
        {
            foreach (LedInfo item in ledInfos)
            {
                if (SensorNum >= item.StartNum && SensorNum <= item.EndNum)
                {
                    if (item.LedConnect == true)
                    {
                        if (tag == 0)
                        {
                            item.ParkingVacant += 1;
                        }
                        if (tag == 1)
                        {
                            item.ParkingOccupy += 1;
                        }
                        item.ParkingSum += 1;
                        SendLed(item);//给显示屏发送消息
                    }
                }
            }
        }
        #endregion

        #region 给显示屏发消息
        /// <summary>
        /// 给显示屏发送消息
        /// </summary>
        /// <param name="ledInfo"></param>
        private void SendLed(LedInfo ledInfo)
        {
            byte[] sendMsg = { 0x02, 0x01, 0x06, 0xa1, 0x20, 0x20, 0x30, 0x20, 0x20, 0x20, 0x03 };//定义发送的原始数据 
            try
            {
                if (ledInfo.LedConnect == true)
                {
                    string msg = ledInfo.parkingVacant.ToString();
                    if (ledInfo.parkingVacant == 0)
                    {
                        msg = "FULL";
                    }
                    byte[] send = Encoding.ASCII.GetBytes(msg);//把要发送的字符串转换成字节数组
                    for (int i = 0; i < send.Length; i++)
                    {
                        //sendMsg[5 + i] = send[i];
                        if (ledInfo.parkingVacant == 0)
                        {
                            sendMsg[5 + i] = send[i];
                        }
                        if (ledInfo.parkingVacant > 0 && ledInfo.parkingVacant < 10)
                        {
                            sendMsg[7 + i] = send[i];
                        }
                        if (ledInfo.parkingVacant >= 10)
                        {
                            sendMsg[6 + i] = send[i];
                        }
                    }

                    dicClient[ledInfo.LedIP].GetStream().Write(sendMsg, 0, sendMsg.Length);
                    //showMsg(sendMsg, false, this.rtb_LedInfo);
                    rtb_LedInfo.AppendText(System.DateTime.Now.ToString() + "[Send]:  " + ledInfo.LedIP + ":have parking space：" + msg + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                rtb_LedInfo.AppendText(System.DateTime.Now.ToString() + "[erro]:  " + ledInfo.LedIP + ": Connection exception!" + Environment.NewLine);
            }
        }

        /// <summary>
        /// 定时给显示屏发送数据
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void timerLed_Tick(object sender, EventArgs e)
        {
            //多个显示屏发送消息
            foreach (LedInfo item in ledInfos)
            {
                SendLed(item);
            }
        }
        #endregion

        #region 关闭显示屏连接
        //关闭显示屏连接
        private void setLedOff(TextBox textBox)
        {
            try
            {
                foreach (LedInfo item in ledInfos)
                {
                    if (item.LedIP == textBox.Text.Trim())
                    {
                        ledInfos.Remove(item);

                        this.rtb_LedInfo.AppendText(System.DateTime.Now.ToString() + " :" + item.LedIP + " Close!!!" + Environment.NewLine);
                    }
                    BindingSource bindingSource = new BindingSource();
                    bindingSource.DataSource = ledInfos;
                    this.dgv_LedListInfo.DataSource = null;
                    this.dgv_LedListInfo.DataSource = bindingSource;
                }
            }
            catch (Exception)
            {

            }

        }

        private void btn_ParkingSumOff_Click(object sender, EventArgs e)
        {
            setLedOff(this.txt_LedIpSum);
        }

        private void btn_ParkingAOff_Click(object sender, EventArgs e)
        {
            setLedOff(this.txt_LedIpA);
        }

        private void btn_ParkingBOff_Click(object sender, EventArgs e)
        {
            setLedOff(this.txt_LedIpB);
        }

        private void btn_ParkingCOff_Click(object sender, EventArgs e)
        {
            setLedOff(this.txt_LedIpC);
        }

        private void btn_offParking_Click(object sender, EventArgs e)
        {
            setLedOff(this.txt_AddLedIP);
        }
        #endregion

        #region 增加显示屏      
        /// <summary>
        /// 增加不在界面现实的显示屏
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_AddLed_Click(object sender, EventArgs e)
        {
            AddParking(this.txt_AddLedIP.Text.Trim(), int.Parse(this.txt_AddStartNum.Text.Trim()), int.Parse(this.txt_AddEndNum.Text.Trim()));
        }

        #endregion

        #endregion

    }
}
