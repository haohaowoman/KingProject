using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ExpRuntimeApp.Modules;
using LabMCESystem.Servers.HS;

namespace ExpRuntimeApp.ExpWindows
{
    /// <summary>
    /// RLTempSetPropWnd.xaml 的交互逻辑
    /// </summary>
    public partial class RLTempSetPropWnd
    {
        public RLTempSetPropWnd()
        {
            InitializeComponent();
            _sBtns.Add(HR1);
            _sBtns.Add(HR2);
            _sBtns.Add(HR3);
            _sBtns.Add(HR4);
            _sBtns.Add(HR5);

            _rTimer.Interval = TimeSpan.FromMilliseconds(500);
            _rTimer.Tick += _rTimer_Tick;
            _rTimer.Start();
        }

        private void _rTimer_Tick(object sender, EventArgs e)
        {
            MdExperPoint ePoint = this.Tag as MdExperPoint;
            System.Diagnostics.Debug.Assert(ePoint != null);
            string showStr = string.Empty;

            if (ePoint.AOValue < HS_MeasCtrlDevice.HeaterControlBaseFlow)
            {
                showStr = $"当前热路流量的控制值没有达到加热器的启动条件，请取消后重新设置热路的空气流量控制值。";
                OKBtn.IsEnabled = false;
            }
            else
            {

                // 根据流量计算需要使用的加热器个数。
                int hCount = (int)ePoint.AOValue / (int)HS_MeasCtrlDevice.HeaterControlBaseFlow;
                int yu = (int)ePoint.AOValue % (int)HS_MeasCtrlDevice.HeaterControlBaseFlow;
                if (yu > 0)
                {
                    hCount++;
                }
                hCount = Math.Min(_sBtns.Count, hCount);

                int rAuto = 0;
                foreach (var item in _sBtns)
                {
                    if (item.IsChecked == true)
                    {
                        rAuto++;
                    }
                }
                int lCount = hCount - rAuto;
                if (lCount == 0)
                {
                    // 状态已对。可以进行确定。
                    showStr = $"当前需要将{hCount}个加热器设置为远程状态，已设置{rAuto}个，可以启动温度控制。";
                    OKBtn.IsEnabled = true;
                }
                else if (lCount > 0)
                {
                    showStr = $"当前需要将{hCount}个加热器设置为远程状态，已设置{rAuto}个，还需要设置{lCount}个。";
                    OKBtn.IsEnabled = false;
                }
                else
                {
                    showStr = $"当前需要将{hCount}个加热器设置为远程状态，已设置{rAuto}个，还需要取消{-lCount}个。";
                    OKBtn.IsEnabled = false;
                }
            }
            
            MsgShow.Text = showStr;
        }

        private List<ToggleButton> _sBtns = new List<ToggleButton>(5);

        DispatcherTimer _rTimer = new DispatcherTimer();

        private void OKBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder strb = new StringBuilder("");
            foreach (var tbtn in _sBtns)
            {
                if (tbtn.IsChecked == true)
                {
                    strb.AppendLine($"\t{tbtn.Content.ToString()}");
                }
            }
            RLTempSetWarnningWnd warnWnd = new RLTempSetWarnningWnd(strb.ToString());

            if (warnWnd.ShowDialog() == true)
            {
                DialogResult = true;
                Close();
            }            
        }

        private void CancelBtn_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = null;
            Close();
        }

        private void MetroWindow_Closed(object sender, EventArgs e)
        {
            _rTimer.Stop();
            
        }
    }
}
