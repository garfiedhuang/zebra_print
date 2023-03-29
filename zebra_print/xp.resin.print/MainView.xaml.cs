using System;
using System.Windows;
using System.Windows.Input;
using Xp.Resin.Print.ViewsModels;

namespace Xp.Resin.Print
{
    /// <summary>
    /// MainView.xaml 的交互逻辑
    /// </summary>
    public partial class MainView : Window
    {
        public MainView()
        {
            InitializeComponent();

            this.tbTitle.Text = $"树脂车间产品打印客户端({GetClientVersion()})";
            this.DataContext = new MainViewModel();
        }

        public static string GetClientVersion()
        {
            return Application.ResourceAssembly.GetName().Version.ToString();
        }

        private void Border_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                DragMove();
            }
        }
        private void MinWin_click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void MaxWin_click(object sender, RoutedEventArgs e)
        {
            this.WindowState = (this.WindowState == WindowState.Maximized) ? WindowState.Normal : WindowState.Maximized;
        }

        private void CloseWin_click(object sender, RoutedEventArgs e)
        {
            //强制退出
            Environment.Exit(0);

            //this.Close();
        }

        private void btnMaintenance_Click(object sender, RoutedEventArgs e)
        {
            var viewModel = this.DataContext as MainViewModel;

            viewModel.BaseModel.IsMaintenanceMode = !viewModel.BaseModel.IsMaintenanceMode;

            if (viewModel.BaseModel.IsMaintenanceMode)
            {
                this.dgProductModels.Columns[7].Visibility = Visibility.Visible;
                this.dgProductModels.Columns[8].Visibility = Visibility.Collapsed;
            }
            else
            {
                this.dgProductModels.Columns[7].Visibility = Visibility.Collapsed;
                this.dgProductModels.Columns[8].Visibility = Visibility.Visible;
            }
        }
    }
}
