using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using ExpRuntimeApp.ExpTask;
using ExpRuntimeApp.ViewModules;
namespace ExpRuntimeApp.Pages.MeasureAndControlPages
{
    /// <summary>
    /// ExpTaskerPage.xaml 的交互逻辑
    /// </summary>
    public partial class ExpTaskerPage : Page
    {
        public ExpTaskerPage()
        {
            InitializeComponent();
            var vm = this.DataContext as ExperimentViewModule;
            vm.Taskers.TaskerRun += Taskers_TaskerRun;
        }

        private void TaskerGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (TaskerGrid.SelectedItem != null)
            {

            }
        }

        private void AddBtn_Click(object sender, RoutedEventArgs e)
        {
            var tasker = TaskerGrid.SelectedItem as Tasker;
            var vm = this.DataContext as ExperimentViewModule;
            if (tasker != null)
            {
                vm.Taskers.Add(tasker.Clone());
            }
            else
            {
                vm.Taskers.Add(vm.BaseTasker.Clone());
            }
        }

        private void InsertBtn_Click(object sender, RoutedEventArgs e)
        {
            var tasker = TaskerGrid.SelectedItem as Tasker;
            var vm = this.DataContext as ExperimentViewModule;
            if (tasker != null)
            {
                vm.Taskers.Insert(TaskerGrid.SelectedIndex, tasker.Clone());
            }
            else
            {
                vm.Taskers.Insert(0, vm.BaseTasker.Clone());
            }
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var tasker = TaskerGrid.SelectedItem as Tasker;
            var vm = this.DataContext as ExperimentViewModule;
            if (tasker != null)
            {
                vm.Taskers.Remove(tasker);
            }
        }

        private void UpSwapBtn_Click(object sender, RoutedEventArgs e)
        {
            var tasker = TaskerGrid.SelectedItem as Tasker;
            var vm = this.DataContext as ExperimentViewModule;
            if (tasker != null)
            {
                int cIndex = vm.Taskers.IndexOf(tasker);
                if (cIndex > 0)
                {
                    vm.Taskers.Move(cIndex, cIndex - 1);
                }
            }
        }

        private void DownSwapBtn_Click(object sender, RoutedEventArgs e)
        {
            var tasker = TaskerGrid.SelectedItem as Tasker;
            var vm = this.DataContext as ExperimentViewModule;
            if (tasker != null)
            {
                int cIndex = vm.Taskers.IndexOf(tasker);
                int count = vm.Taskers.Count;
                if (cIndex < count - 1)
                {
                    vm.Taskers.Move(cIndex, cIndex + 1);
                }
            }
        }

        private void RunBtn_Click(object sender, RoutedEventArgs e)
        {
            var tasker = TaskerGrid.SelectedItem as Tasker;
            var vm = this.DataContext as ExperimentViewModule;
            if (tasker != null)
            {
                if (!vm.IsExpRunning)
                {
                    vm.IsExpRunning = true;
                }
                vm.Taskers.RunOneTasker(tasker);
            }
        }

        private void StopBtn_Click(object sender, RoutedEventArgs e)
        {
            var tasker = TaskerGrid.SelectedItem as Tasker;
            var vm = this.DataContext as ExperimentViewModule;
            if (tasker != null)
            {
                vm.Taskers.Stop();
            }
        }

        private void Taskers_TaskerRun(object sender, TaskerRunEventArgs e)
        {
            var ts = sender as TaskerCollection;
            if (ts != null)
            {
                this.Dispatcher.Invoke(() =>
                {
                    RunBtn.IsEnabled = !ts.IsRunning;
                    StopBtn.IsEnabled = !RunBtn.IsEnabled;
                });
            }
        }
        
    }
}
