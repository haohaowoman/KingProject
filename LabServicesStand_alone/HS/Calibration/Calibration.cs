using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using LabMCESystem.Servers.HS;
namespace LabMCESystem.Servers.HS.Calibration
{
    public partial class Calibration : Form
    {
        public Calibration()
        {
            InitializeComponent();
            _upTimer = new Timer();
            _upTimer.Interval = 500;
            _upTimer.Tick += _upTimer_Tick;
            for (int i = 0; i < 48; i++)
            {
                ChannelsCombox.Items.Add($"{i / 6 + 1:D2}_Ch{i % 6 + 1}");
            }
            _upTimer.Start();
        }


        private Timer _upTimer;

        public HS_MeasCtrlDevice HS_Device { get; set; }

        private void ChannelsCombox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (HS_Device != null && ChannelsCombox.SelectedIndex >= 0)
            {
                int index = ChannelsCombox.SelectedIndex;
                double sp0x = HS_Device.SamplePoints[index][0].X;
                double sp0y = HS_Device.SamplePoints[index][0].Y;
                X0TextBox.Text = sp0x.ToString("0.00");
                Y0TextBox.Text = sp0y.ToString("0.00");

                double sp1x = HS_Device.SamplePoints[index][1].X;
                double sp1y = HS_Device.SamplePoints[index][1].Y;
                X1TextBox.Text = sp1x.ToString("0.00");
                Y1TextBox.Text = sp1y.ToString("0.00");
            }
        }

        private void _upTimer_Tick(object sender, EventArgs e)
        {
            if (HS_Device != null && ChannelsCombox.SelectedIndex >= 0)
            {
                int index = ChannelsCombox.SelectedIndex;
                double cSrc = HS_Device.ADDeviceInteract.AllChannelsValue[index];
                double cCal = HS_Device.ADBoxDemarcaters[index].Demarcate(cSrc);

                CSrcTextBox.Text = cSrc.ToString("0.00");
                CalTextBox.Text = cCal.ToString("0.00");
            }
        }

        private void MemBtn1_Click(object sender, EventArgs e)
        {
            if (HS_Device != null && ChannelsCombox.SelectedIndex >= 0)
            {
                int index = ChannelsCombox.SelectedIndex;
                double sp0x = HS_Device.SamplePoints[index][0].X;
                double sp0y = HS_Device.SamplePoints[index][0].Y;
                try
                {
                    sp0x = double.Parse(X0TextBox.Text);
                    sp0y = double.Parse(Y0TextBox.Text);

                    HS_Device.SamplePoints[index][0] = new mcLogic.Data.SamplePoint(sp0x, sp0y);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n数据输入错误，请重新输入。");
                }

            }
        }

        private void MemBtn2_Click(object sender, EventArgs e)
        {
            if (HS_Device != null && ChannelsCombox.SelectedIndex >= 0)
            {
                int index = ChannelsCombox.SelectedIndex;
                double sp1x = HS_Device.SamplePoints[index][1].X;
                double sp1y = HS_Device.SamplePoints[index][1].Y;
                try
                {
                    sp1x = double.Parse(X1TextBox.Text);
                    sp1y = double.Parse(Y1TextBox.Text);

                    HS_Device.SamplePoints[index][1] = new mcLogic.Data.SamplePoint(sp1x, sp1y);
                }

                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message + "\n数据输入错误，请重新输入。");
                }

            }
        }

        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            if (HS_Device != null && ChannelsCombox.SelectedIndex >= 0)
            {
                int index = ChannelsCombox.SelectedIndex;
                HS_Device.ADBoxDemarcaters[index] = mcLogic.Demarcate.DemarcateFactory.LinerDemFromSamplePoints(HS_Device.SamplePoints[index]);
            }
        }

        private void SaveBtn_Click(object sender, EventArgs e)
        {
            if (HS_Device != null)
            {
                HS_Device.SaveCalibrations();
            }
        }

        private void CancleBtn_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void SampleBtn1_Click(object sender, EventArgs e)
        {
            if (HS_Device != null && ChannelsCombox.SelectedIndex >= 0)
            {
                int index = ChannelsCombox.SelectedIndex;
                double cSrc = HS_Device.ADDeviceInteract.AllChannelsValue[index];
                
                double sp0x = cSrc;
                X0TextBox.Text = sp0x.ToString("0.00");                
            }
        }

        private void SampleBtn2_Click(object sender, EventArgs e)
        {
            if (HS_Device != null && ChannelsCombox.SelectedIndex >= 0)
            {
                int index = ChannelsCombox.SelectedIndex;
                double cSrc = HS_Device.ADDeviceInteract.AllChannelsValue[index];

                double sp1x = cSrc;
                X1TextBox.Text = sp1x.ToString("0.00");
            }
        }
    }
}
