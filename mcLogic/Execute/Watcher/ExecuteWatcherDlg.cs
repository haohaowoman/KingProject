using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace mcLogic.Execute.Watcher
{
    public partial class ExecuteWatcherDlg : Form
    {
        public ExecuteWatcherDlg(ExecuteWatcher watcher)
        {
            InitializeComponent();

            _watcher = watcher;
            _watcher.WatchDataChanged += _watcher_WatchDataChanged;
        }

        private ExecuteWatcher _watcher;

        private void ExecuteWatcherDlg_Load(object sender, EventArgs e)
        {
            ETypeTextBox.Text = _watcher.TargetExecuter.GetType().FullName;

            ENameTextBox.Text = nameof(_watcher.TargetExecuter);

            var pro = _watcher.TargetExecuter as IPeriodExecute;
            if (pro != null)
            {
                PeriodTextBox.Text = pro.PeriodInterval.ToString();
            }
            else
            {
                PeriodTextBox.Text = "";
            }

            var pid = _watcher.TargetExecuter as PIDExecuterBase;

            if (pid != null)
            {
                PIDKpTextBox.Text = pid.Param.Kp.ToString();
                PIDKiTextBox.Text = pid.Param.Ti.ToString();
                PIDTdTextBox.Text = pid.Param.Td.ToString();
                TsTextBox.Text = pid.Param.Ts.ToString();
                TargetValueTextBox.Text = pid.TargetVal.ToString();
            }            
            RefreshBtn_Click(sender, e);
        }

        // 刷新数据
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            var sEv = EChart.Series.FindByName("ExeValue");
            var sFv = EChart.Series.FindByName("FedbackValue");
            var fe = _watcher.TargetExecuter as IDataFeedback;
            int wcount = _watcher.WatchData.Count;
            if (sEv.Points.Count < wcount)
            {
                for (int i = sEv.Points.Count; i < wcount; i++)
                {
                    sEv.Points.AddXY(_watcher.WatchData[i].Time, _watcher.WatchData[i].ExecuteValue);
                    
                    if (fe != null)
                    {
                        sFv.Points.AddXY(_watcher.WatchData[i].Time, _watcher.WatchData[i].FedbackValue);
                        
                    }
                }
            }            
        }

        private void _watcher_WatchDataChanged(object sender, List<EWatcherData> e)
        {
            this.Invoke(new Action(delegate () { RefreshBtn_Click(this, null); }));
        }

        private void ExecuteWatcherDlg_FormClosed(object sender, FormClosedEventArgs e)
        {
            _watcher.WatchDataChanged -= _watcher_WatchDataChanged;
        }

        private void EOverBtn_Click(object sender, EventArgs e)
        {
            _watcher?.TargetExecuter?.ExecuteOver();
        }

        private void EBegainBtn_Click(object sender, EventArgs e)
        {
            _watcher?.TargetExecuter?.ExecuteBegin();
        }

        private void ResetBtn_Click(object sender, EventArgs e)
        {
            PIDExecuterBase pidExe = _watcher?.TargetExecuter as PIDExecuterBase;
            if (pidExe != null)
            {
                PIDParam pidParam = new PIDParam();
                pidParam.Kp = Convert.ToDouble(PIDKpTextBox.Text);
                pidParam.Ti = Convert.ToDouble(PIDKiTextBox.Text);
                pidParam.Td = Convert.ToDouble(PIDTdTextBox.Text);
                pidParam.Ts = Convert.ToDouble(TsTextBox.Text);
                pidExe.PeriodInterval = Convert.ToDouble(PeriodTextBox.Text);
                pidExe.TargetVal = Convert.ToDouble(TargetValueTextBox.Text);
                if (pidExe.Enabled)
                {
                    pidExe.ExecuteOver();
                    pidExe.Param = pidParam;
                    pidExe.ExecuteBegin();
                }
                else
                {
                    pidExe.Param = pidParam;
                }
                                
            }

        }

        private void ResetButton_Click(object sender, EventArgs e)
        {
            _watcher?.TargetExecuter?.Reset();
        }
    }
}
