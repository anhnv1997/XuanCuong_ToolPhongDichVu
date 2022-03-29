using CryptorEngines;
using Databases;
using Extensions;
using FileXMLs;
using iParking;
using Microsoft.Win32;
using SQLConns;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using ToolLed_XuanCuong.Databases;
using ToolLed_XuanCuong.Objects;

namespace ToolLed_XuanCuong
{
    public partial class frmDisplayData : Form
    {

        private CancellationTokenSource cts;
        ManualResetEvent ForceLoopIteration;
        SQLConn[] sqls = null;
        private int scrollIndex = 0;
        private int currentDay = 1;

        public frmDisplayData()
        {
            InitializeComponent();
            //RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            //if (!IsStartupItem())
            //    // Add the value in the registry so that the application runs at startup
            //    rkApp.SetValue("KZTEK_LED_TOOL_CHECK", Application.ExecutablePath.ToString());

            //LogHelper.Logger_Info("Start Application: Startup Path: " + Application.ExecutablePath.ToString());
            currentDay = DateTime.Now.Day;
        }
        private bool IsStartupItem()
        {
            // The path to the key where Windows looks for startup applications
            RegistryKey rkApp = Registry.CurrentUser.OpenSubKey("SOFTWARE\\Microsoft\\Windows\\CurrentVersion\\Run", true);

            if (rkApp.GetValue("KZTEK_LED_TOOL") == null)
                // The value doesn't exist, the application is not set to run at startup
                return false;
            else
                // The value exists, the application is set to run at startup
                return true;
        }
        private void frmDisplayData_Load(object sender, EventArgs e)
        {
            try
            {
                if (File.Exists(Application.StartupPath + "\\SQLConn.xml"))
                {
                    FileXML.ReadXMLSQLConn(Application.StartupPath + "\\SQLConn.xml", ref sqls);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("frmConnectionConfig: " + ex.Message);
            }

            ConnectToSQLServer();
            panelData.ToggleDoubleBuffered(true);
            //this.TopMost = true;
            InitData();

            PollingStart();
        }

        private void InitData()
        {
            ucDisplayItem ucTittle = new ucDisplayItem();
            ucTittle.Dock = DockStyle.Top;
            ucTittle.Size = new Size(this.Width - 10, ucTittle.Height);
            panelData.Controls.Add(ucTittle);
            panelData.Controls.SetChildIndex(ucTittle, 1);

            List<EventData> eventDatas = tblEvent.GetData();
            if (eventDatas.Count > 0)
            {
                foreach (EventData eventData in eventDatas)
                {
                    ucDisplayItem ucData = new ucDisplayItem(panelData.Controls.Count, eventData.ServiceCode, eventData.ChinaPlateNumber, eventData.VietNamPlateNumber, eventData.ParkingPosition, eventData.Group, eventData.Status);
                    ucData.Dock = DockStyle.Top;
                    ucData.Size = new Size(this.Width - 10, ucData.Height);

                    ucData.ID = eventData.ID;
                    ucData.STT = panelData.Controls.Count;
                    panelData.Controls.Add(ucData);
                    panelData.Controls.SetChildIndex(ucData, panelData.Controls.Count);
                }
            }
        }

        private void PollingStart()
        {
            cts = new CancellationTokenSource();
            ForceLoopIteration = new ManualResetEvent(false);
            Task.Run(() =>
                DisplayTop6Data(cts.Token), cts.Token
            );
        }

        private async Task DisplayTop6Data(CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(3000);
                    if (DateTime.Now.Day != this.currentDay)
                    {
                        this.currentDay = DateTime.Now.Day;
                        this.scrollIndex = 0;
                        panelData?.Invoke(new Action(() =>
                        {
                            panelData.Controls.Clear();
                            InitData();
                        }));
                        continue;
                    }

                    UpdataData();
                    ScrollData();

                }
                catch (Exception ex)
                {
                    LogHelper.Logger_Error(ex.Message);
                }
            }
        }

        private void UpdataData()
        {
            List<EventData> eventDatas = tblEvent.GetData();
            for (int i = 0; i < eventDatas.Count; i++)
            {
                if (i >= panelData.Controls.Count - 1)
                {
                    panelData?.Invoke(new Action(() =>
                    {
                        EventData eventData = eventDatas[i];
                        ucDisplayItem ucData = new ucDisplayItem(panelData.Controls.Count, eventData.ServiceCode, eventData.ChinaPlateNumber, eventData.VietNamPlateNumber, eventData.ParkingPosition, eventData.Group, eventData.Status);
                        ucData.Dock = DockStyle.Top;
                        ucData.Size = new Size(this.Width - 10, ucData.Height);
                        ucData.ID = eventData.ID;
                        ucData.STT = panelData.Controls.Count;
                        panelData.Controls.Add(ucData);
                        panelData.Controls.SetChildIndex(ucData, panelData.Controls.Count - 1 - scrollIndex);
                    }));

                }
                else
                {
                    foreach (ucDisplayItem control in panelData.Controls.OfType<ucDisplayItem>())
                    {
                        if (control.ID == eventDatas[i].ID)
                        {
                            control?.Invoke(new Action(() =>
                            {
                                {
                                    EventData eventData = eventDatas[i];
                                    control.updateInfor(control.STT, eventData.ServiceCode, eventData.ChinaPlateNumber, eventData.VietNamPlateNumber, eventData.ParkingPosition, eventData.Group, eventData.Status);
                                }
                            }));
                            break;
                        }


                    }
                }
            }
        }

        private void ScrollData()
        {
            panelData?.Invoke(new Action(() =>
            {
                if (panelData.Controls.Count >= 3)
                {
                    if (scrollIndex < panelData.Controls.Count - 2)
                    {
                        scrollIndex++;
                    }
                    else
                    {
                        scrollIndex = 0;
                    }
                    panelData.Controls.SetChildIndex(panelData.Controls[1], panelData.Controls.Count);

                    for (int i = 2; i < panelData.Controls.Count; i++)
                    {
                        panelData.Controls.SetChildIndex(panelData.Controls[i], i);
                    }
                }
            }));
        }

        private void button1_Click(object sender, EventArgs e)
        {


        }

        private void ConnectToSQLServer()
        {
            if (sqls != null && sqls.Length > 0)
            {
                string cbSQLServerName = sqls[0].SQLServerName;
                string cbSQLDatabaseName = sqls[0].SQLDatabase;
                string cbSQLAuthentication = sqls[0].SQLAuthentication;
                string txtSQLUserName = sqls[0].SQLUserName;
                string txtSQLPassword = CryptorEngine.Decrypt(sqls[0].SQLPassword, true);
                StaticPool.mdb = new MDB(cbSQLServerName, cbSQLDatabaseName, cbSQLAuthentication, txtSQLUserName, txtSQLPassword);
            }
        }

        private void frmDisplayData_FormClosed(object sender, FormClosedEventArgs e)
        {
            var p = new Process();
            string path = Application.ExecutablePath;
            p.StartInfo.FileName = path;  // just for example, you can use yours.
            p.Start();
        }
    }
}
