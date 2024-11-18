using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using ToolTemp.WPF.Configs;
using ToolTemp.WPF.MVVM.ViewModel;
using ToolTemp.WPF.Services;

namespace ToolTemp.WPF
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public MainWindow(AppSettings appSettings)
        {
            InitializeComponent();

            DataContext = new MainViewModel(appSettings);
           

            // Đăng ký sự kiện đóng form
            Closing += MainWindow_Closing;

            
        }

        // Sự kiện đóng cửa sổ
        private void MainWindow_Closing(object? sender, CancelEventArgs e)
        {
            // Đóng toàn bộ ứng dụng khi MainWindow bị đóng
            Application.Current.Shutdown();
        }

        // Xử lý khi ComboBox thay đổi lựa chọn
        private void cbb_factory_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (DataContext is MainViewModel viewModel)
            {
                viewModel.OnFactorySelectionChanged();
            }
        }
    }
}
