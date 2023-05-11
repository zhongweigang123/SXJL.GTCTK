using System;
using System.Windows;
using SXJL.GTCTK.UI.ViewModel;

namespace SXJL.GTCTK.UI
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private readonly MainViewModel ViewModel = new MainViewModel();
        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void Min_Click(object sender, RoutedEventArgs e)
        {
            count = 0;
            WindowState = WindowState.Minimized;
        }

        private void Close_Click(object sender, RoutedEventArgs e)
        {
            if (MessageBox.Show("退出后PLC数据无法采集，数据会丢失，确定要退出吗？", "警告", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
            {
                ViewModel.Exit();
                Close();
                Application.Current.Shutdown();
                Environment.Exit(0);
            }
        }

        private int count = 0;
        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            count++;
            if (count == 10)
            {
                Min_Click(sender, null);
            }
        }
    }
}
