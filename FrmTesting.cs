using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;


namespace SocketClient
{
    public partial class FrmTesting : Form
    {
        private TelNet tn;

        public FrmTesting()
        {
            InitializeComponent();
            Control.CheckForIllegalCrossThreadCalls = false;
        }

       

        /// <summary>
        /// 向服务器发送消息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void bt_send_Click(object sender, EventArgs e)
        {
            Thread thA = new Thread(new ThreadStart(RunTestFunc));
            thA.IsBackground = true;
            thA.Start();
        }

        private void RunTestFunc()
        {
            try
            {
                tn = new TelNet(txt_ip.Text, 23, 20);
                if (tn.Connect())
                {
                    lblResult.Text = "";
                    tn.SessionLog = "";
                    lbl_a.Text = "";
                    lbl_b.Text = "";
                    lbl_c.Text = "";
                    lbl_G.Text = "";
                    lbl_P.Text = "";
                    lbl_L4.Text = "";
                    lbl_tf.Text = "";
                    tbx_msg1.Text = "连接成功";

                    Thread.Sleep(100);
                    tn.WaitFor("login:");
                    tn.Send("root");
                    Thread.Sleep(100);
                    tn.WaitFor("Password:");
                    tn.Send("admin");
                    Thread.Sleep(200);

                    tn.WaitFor("#");
                    tn.SessionLog = "";
                    tn.Send("mfc -a");
                    tn.WaitFor("#");
                    tbx_msg1.AppendText("\r\n" + tn.SessionLog);
                    if (tn.SessionLog.IndexOf("FirmwareVersion:") > 0)
                        lbl_a.Text = tn.SessionLog.Substring(tn.SessionLog.IndexOf("FirmwareVersion:") + 16, 22);
                    tn.SessionLog = "";

                    tn.Send("mfc -b");
                    tn.WaitFor("#");
                    tbx_msg1.AppendText("\r\n" + tn.SessionLog);
                    if (tn.SessionLog.Contains("HardwareVersion:"))
                        lbl_b.Text = tn.SessionLog.Substring(tn.SessionLog.IndexOf("HardwareVersion:") + 17, 13);
                    tn.SessionLog = "";

                    tn.Send("mfc -c");
                    tn.WaitFor("#");
                    tbx_msg1.AppendText("\r\n" + tn.SessionLog);
                    if (tn.SessionLog.Contains("KernelVersion:"))
                        lbl_c.Text = tn.SessionLog.Substring(tn.SessionLog.IndexOf("KernelVersion:") + 14, 27);
                    tn.SessionLog = "";

                    tn.Send("mfc -G b_on");
                    tn.WaitFor("#");
                    tbx_msg1.AppendText("\r\n" + tn.SessionLog);
                    if (tn.SessionLog.Contains("LED_TEST:OK"))
                        lbl_G.Text = "OK";
                    tn.SessionLog = "";

                    tn.Send("mfc -R");
                    tn.WaitFor("#");
                    tbx_msg1.AppendText("\r\n" + tn.SessionLog);
                    if (tn.SessionLog.Contains("SD_CARD_CHECK:OK"))
                        lbl_tf.Text = "SD卡测试OK";
                    if (tn.SessionLog.Contains("SD_CARD_CHECK:ERROR"))
                        lbl_tf.Text = "SD卡测试失败,末找到SD卡";
                    tn.SessionLog = "";

                    tn.Send("mfc -L 4");
                    tn.WaitFor("#");
                    tbx_msg1.AppendText("\r\n" + tn.SessionLog);
                    if (tn.SessionLog.Contains("LTE: ERROR"))
                        lbl_L4.Text = "4G联网失败";
                    if (tn.SessionLog.Contains("LTE0_test:OK"))
                        lbl_L4.Text = "4G联网成功";
                    tn.SessionLog = "";

                    tn.Send("mfc -P");
                    tn.WaitFor("#");
                    tbx_msg1.AppendText("\r\n" + tn.SessionLog);
                    if (tn.SessionLog.Contains("button_test:OK"))
                        lbl_P.Text = "BUTTON测试 OK";
                    else
                        lbl_P.Text = "按一下RST按键，不能按住>=2秒";
                    tn.SessionLog = "";

                }
                else
                {
                    tbx_msg1.Text = "";
                    tbx_msg1.AppendText("正在连接" + txt_ip.Text + "无法打开到主机的连接。 在端口 23: 连接失败");
                    lblResult.Text = "FAIL";
                    lblResult.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                lblResult.Text = "FAIL";
                lblResult.ForeColor = Color.Red;
                MessageBox.Show("测试出出错，错误信息：" + ex.Message);
            }
            finally
            {
                Ini.INIWriteValue(@"D:\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "测试记录", DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), "测试" + label16.Text);
            }
        }


        private void Form1_Load(object sender, EventArgs e)
        {
            
            lblResult.Text = "";
            lbl_a.Text = "";
            lbl_b.Text = "";
            lbl_c.Text = "";
            lbl_G.Text = "";
            lbl_P.Text = "";
            lbl_L4.Text = "";
            lbl_tf.Text = "";
        }


        private void btnWrite_Click(object sender, EventArgs e)
        {
            Thread thB = new Thread(new ThreadStart(WriteMacAndSN));
            thB.IsBackground = true;
            thB.Start();
        }

        private void NewMACMethod(TextBox tb ,int n)
        {
            long mac = Convert.ToInt64(tbx2G.Text.Replace(":", ""),16);
            mac += n;
            string str = string.Format("{0:X}",mac).ToString().PadLeft(12,'0');
            str = str.Insert(2, ":");
            str = str.Insert(5, ":");
            str = str.Insert(8, ":");
            str = str.Insert(11, ":");
            str = str.Insert(14, ":");
            tb.Text = str;
        }

        private void WriteMacAndSN()
        {
            try
            {
                string msg = "";
                tn = new TelNet(txt_ip.Text, 23, 20);
                if (tn.Connect())
                {
                    tn.SessionLog = "";
                    tbx_msg2.Text = "";
                    tbxLan.Text = "";
                    tbxWan.Text = "";
                    lbl2G.Text = "";
                    lblLan.Text = "";
                    lblWan.Text = "";
                    lblSN.Text = "";
                    lblRest.Text = "";
                    lblWifiName.Text = "";

                    #region
                    Thread.Sleep(100);
                    tbx_msg2.AppendText("\r\n" + tn.WorkingData);
                    tn.WaitFor("login:");
                    tn.Send("root");
                    Thread.Sleep(100);
                    tn.WaitFor("Password:");
                    tn.Send("admin");
                    Thread.Sleep(500);
                    tbx_msg2.AppendText("\r\n" + tn.WorkingData);

                    tn.WaitFor("#");
                    //一下为写入信息
                    if (tbx2G.Text == "")
                    {
                        MessageBox.Show("2.4G MAC地址不能为空", "提示");
                        tbx2G.Focus();
                    }
                    else
                    {
                        NewMACMethod(tbxLan, 1);
                        NewMACMethod(tbxWan, 2);
                        msg = "mfc -C 7620 2G " + tbx2G.Text + " 0";
                        tn.Send(msg);
                        tbx_msg2.AppendText("\r\nSet_MAC_2G:" + tbx2G.Text);

                        Thread.Sleep(200);
                        tn.WaitFor("Set_MAC_2G:OK");
                        Thread.Sleep(200);
                        lbl2G.Text = tn.WorkingData;
                        tbx_msg2.AppendText(tn.WorkingData);
                    }

                    if (tbxLan.Text == "")
                    {
                        MessageBox.Show("LAN口 MAC地址不能为空", "提示");
                        tbxLan.Focus();
                    }
                    else
                    {
                        msg = "mfc -C 7620 LAN " + tbxLan.Text + " 0";
                        tn.Send(msg);
                        tbx_msg2.AppendText("\r\nSet_MAC_LAN:" + tbxLan.Text);
                        Thread.Sleep(200);
                        tn.WaitFor("Set_MAC_LAN:OK");
                        Thread.Sleep(200);
                        lblLan.Text = tn.WorkingData;
                        tbx_msg2.AppendText(tn.WorkingData);
                    }

                    if (tbxWan.Text == "")
                    {
                        MessageBox.Show("WAN口 MAC地址不能为空");
                        tbxWan.Focus();
                    }
                    else
                    {
                        msg = "mfc -C 7620 WAN " + tbxWan.Text + " 0";
                        tn.Send(msg);
                        tbx_msg2.AppendText("\r\nSet_MAC_WAN:" + tbxWan.Text);
                        Thread.Sleep(200);
                        tn.WaitFor("Set_MAC_WAN:OK");
                        Thread.Sleep(200);
                        lblWan.Text = tn.WorkingData;
                        tbx_msg2.AppendText(tn.WorkingData);
                    }

                    tn.Send("mfc -i 7620 3");
                    Thread.Sleep(200);
                    tn.WaitFor("Get_MAC_2G:");
                    Thread.Sleep(200);
                    lbl2G.Text = tn.WorkingData;
                    tbx_msg2.AppendText(tn.WorkingData);

                    tn.Send("mfc -i 7620 1");
                    Thread.Sleep(200);
                    tn.WaitFor("Get_MAC_LAN:");
                    Thread.Sleep(200);
                    lblLan.Text = tn.WorkingData;
                    tbx_msg2.AppendText(tn.WorkingData);

                    tn.Send("mfc -i 7620 2");
                    Thread.Sleep(100);
                    tn.WaitFor("Get_MAC_WAN:");
                    Thread.Sleep(200);
                    lblWan.Text = tn.WorkingData;
                    tbx_msg2.AppendText(tn.WorkingData);

                    //16进制SN
                    if (tbxSN.Text != "")
                    {
                        string sn = Convert.ToString(Convert.ToInt64(tbxSN.Text), 16);
                        if (sn.Length > 5)
                        {
                            tn.Send("mfc -V " + sn);
                            tbx_msg2.AppendText("\r\nSet_SN:" + sn);
                            Thread.Sleep(200);
                            tbx_msg2.AppendText(tn.WorkingData);
                            tn.WaitFor("Set_SN:");
                            Thread.Sleep(200);
                            lblSN.Text = tn.WorkingData.ToUpper();
                            tbx_msg2.AppendText(tn.WorkingData);

                            tn.Send("mfc -v");
                            Thread.Sleep(200);
                            tn.WaitFor("Get_SN:");
                            Thread.Sleep(200);
                            lblSN.Text = tn.WorkingData.ToUpper();
                            tbx_msg2.AppendText(tn.WorkingData);
                        }
                    }



                    tn.Send("mfc -M");
                    tbx_msg2.AppendText(tn.SessionLog);
                    Thread.Sleep(200);
                    tn.WaitFor("factory_reset:OK");
                    Thread.Sleep(200);
                    lblRest.Text = "RESET OK";
                    tbx_msg2.AppendText(tn.WorkingData);

                    tn.Send("mfc -d");
                    Thread.Sleep(200);
                    tn.WaitFor("Get_SSID_2G:");
                    Thread.Sleep(1000);
                    lblWifiName.Text = tn.WorkingData;
                    tbx_msg2.AppendText(tn.WorkingData);

                    tbx_msg2.AppendText("\r\n写入完成");
                    #endregion


                }
                else
                {
                    tbx_msg2.Text = "";
                    tbx_msg2.AppendText("正在连接" + txt_ip.Text + "无法打开到主机的连接。 在端口 23: 连接失败");
                    label16.Text = "FAIL";
                    label16.ForeColor = Color.Red;
                }
            }
            catch (Exception ex)
            {
                label16.Text = "FAIL";
                label16.ForeColor = Color.Red;
                MessageBox.Show("写号的时候出错了!错误信息:" + ex.Message);
            }
            finally
            {
                if(tbx2G.Text != "")
                    Ini.INIWriteValue(@"D:\" + DateTime.Now.ToString("yyyyMMdd") + ".txt", "写号记录", tbx2G.Text, "写入"+label16.Text + " " + DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"));
                
            }
        }

        private void tbx2G_TextChanged(object sender, EventArgs e)
        {
            if (tbx2G.Text.Length == 17)
            {
                Thread thB = new Thread(new ThreadStart(WriteMacAndSN));
                thB.IsBackground = true;
                thB.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (lbl_G.Text.Contains("OK") && lbl_tf.Text.Contains("OK") && lbl_L4.Text.Contains("成功") && lbl_P.Text.Contains("OK"))
            {
                lblResult.Text = "PASS";
                lblResult.ForeColor = Color.Green;
            }
            else
            {
                lblResult.Text = "FAIL";
                lblResult.ForeColor = Color.Red;
            }

            if (lbl2G.Text.Contains(tbx2G.Text) && lblLan.Text.Contains(tbxLan.Text) && lblWan.Text.Contains(tbxWan.Text) && lblRest.Text.Contains("OK"))
            {
                label16.Text = "PASS";
                tbx2G.Text = "";
                tbx2G.SelectAll();
                label16.ForeColor = Color.Green;
            }
            else
            {
                label16.Text = "FAIL";
                tbx2G.Focus();
                label16.ForeColor = Color.Red;
            }
        }

        private void tbx2G_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                Thread thB = new Thread(new ThreadStart(WriteMacAndSN));
                thB.IsBackground = true;
                thB.Start();
            }
        }

    }
}