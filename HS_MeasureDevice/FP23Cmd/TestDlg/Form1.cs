using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;
using FP23;


namespace TestDlg
{
    public partial class Form1 : Form
    {
        FP23.FP23Ctrl m_ctrl;
        bool bCom = false;
        bool bFix = false;
        bool bRun = false;
        public Form1()
        {
            InitializeComponent();
            textBox1.Text = "COM1";
        }
        private void button3_Click(object sender, EventArgs e)
        {
            string m_Com = textBox1.Text;
            m_ctrl = new FP23.FP23Ctrl(m_Com);
            if (!m_ctrl.InitCtrl())
            {
                MessageBox.Show("初始化失败！");
            }
        }
        private void button1_Click(object sender, EventArgs e)
        {
            if (!m_ctrl.SetTemp(Convert.ToSingle(textBox2.Text))) 
            {
                MessageBox.Show("设置温度失败！");
            }
        }
        private void button2_Click(object sender, EventArgs e)
        {
            float temp;
            if (m_ctrl.GetTemp(out temp))
            {
                textBox2.Text = string.Format("{0:f1}", temp);
            }
        }

        private void button4_Click(object sender, EventArgs e)
        {
            float temp;
            if (!m_ctrl.GetPVData(out temp)) 
            {
                MessageBox.Show("获取PV值失败！");
                return;
            }
            textBox3.Text = string.Format("{0:f1}", temp);
        }

        private void button5_Click(object sender, EventArgs e)
        {
            string str = textBox4.Text;
            if (!m_ctrl.SetOUT1Data(Convert.ToSingle(str))) 
            {
                MessageBox.Show("设置OUT1失败！");
                return;
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            float temp;
            if (!m_ctrl.GetOUT1Data(out temp))
            {
                MessageBox.Show("获取OUT1失败！");
                return;
            }
            textBox4.Text = string.Format("{0:f1}", temp);
        }

        private void button7_Click(object sender, EventArgs e)
        {
            if (!bCom)
            {
                if (m_ctrl.SetComStyle(true))
                {
                    bCom = true;
                    textBox5.Text = "设置COM控制成功";
                }
                else
                    textBox5.Text = "设置COM控制失败";
            }
            else 
            {
                if (m_ctrl.SetComStyle(false))
                {
                    bCom = false;
                    textBox5.Text = "设置LOC控制成功";
                }
                else
                    textBox5.Text = "设置LOC控制成功";
            }
        }

        private void button8_Click(object sender, EventArgs e)
        {
            if (!bFix)
            {
                if (m_ctrl.SetFixStyle(true))
                {
                    bFix = true;
                    textBox6.Text = "设置FIX模式成功";
                }
                else
                    textBox6.Text = "设置FIX模式失败";
            }
            else 
            {
                if (m_ctrl.SetFixStyle(false))
                {
                    bFix = false;
                    textBox6.Text = "设置PROG模式成功";
                }
                else
                    textBox6.Text = "设置PROG模式失败";

            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            int a = 0;
            if (!m_ctrl.GetFixStyle(out a)) 
            {
                textBox6.Text = "获取FIX模式失败";
            }
            else
            {
                if (a == 1)
                {
                    textBox6.Text = "FIX模式";
                }
                else if(a == 0)
                {
                    textBox6.Text = "PROG模式";
                }
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            float pb = Convert.ToSingle(textBox7.Text);
            int it = Convert.ToInt32(textBox8.Text);
            int dt = Convert.ToInt32(textBox9.Text);
            if (!m_ctrl.SetPID(1,pb,it,dt))
            {
                MessageBox.Show("设置PID参数失败！");
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            PIDInfo pid;
            if (!m_ctrl.GetPID(1,out pid))
            {
                MessageBox.Show("设置PID失败！");
                return;
            }
            textBox7.Text = string.Format("{0:f1}", pid.PB);
            textBox8.Text = string.Format("{0:d}", pid.IT);
            textBox9.Text = string.Format("{0:d}", pid.DT);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            m_ctrl.ReleaseCtrl();
        }

        private void button13_Click(object sender, EventArgs e)
        {
            if (!bRun)
            {
                if (m_ctrl.SetRun(true))
                {
                    bRun = true;
                    MessageBox.Show("设置RUN成功");
                }
                else
                    MessageBox.Show("设置RUN失败");
            }
            else
            {
                if (m_ctrl.SetRun(false))
                {
                    bRun = false;
                    MessageBox.Show("设置RST成功");
                }
                else
                    MessageBox.Show("设置RST失败");

            }
        }
    }
}
