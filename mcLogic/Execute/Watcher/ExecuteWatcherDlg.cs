using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace LabMCESystem.Logic.Execute.Watcher
{
    public partial class ExecuteWatcherDlg : Form
    {
        public ExecuteWatcherDlg(ExecuteWatcher watcher)
        {
            InitializeComponent();

            _watcher = watcher;
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
            }

            RefreshBtn_Click(sender, e);
        }

        // 刷新数据
        private void RefreshBtn_Click(object sender, EventArgs e)
        {
            var sEv = EChart.Series.FindByName("ExeValue");
            var sFv = EChart.Series.FindByName("FedbackValue");
            var fe = _watcher.TargetExecuter as IDataFedback;
            foreach (var item in _watcher.WatchData)
            {
                sEv.Points.AddXY(item.Time, item.ExecuteValue);
                
                if (fe != null)
                {
                    sFv.Points.AddXY(item.Time, item.FedbackValue);
                }
            }
        }
    }
}
