﻿using System;
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

namespace ExpRuntimeApp.Pages.MeasureAndControlPages
{
    /// <summary>
    /// ExpPointDataGridShowPage.xaml 的交互逻辑
    /// </summary>
    public partial class ExpPointDataGridShowPage : Page
    {
        public ExpPointDataGridShowPage()
        {
            InitializeComponent();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (epGrid != null)
            {
                CollectionView v = (CollectionView)CollectionViewSource.GetDefaultView(epGrid.ItemsSource);

                if (v.CanGroup)
                {
                    v.GroupDescriptions.Add(new PropertyGroupDescription("ExpArea"));
                }
            }

        }
    }
}
