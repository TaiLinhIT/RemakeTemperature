using GalaSoft.MvvmLight.Messaging;
using HslCommunication.ModBus;
using MaterialDesignThemes.Wpf;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Globalization;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Reflection.PortableExecutable;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Xml;
using ToolTemp.WPF.Configs;
using ToolTemp.WPF.Constants;
using ToolTemp.WPF.Core;
using ToolTemp.WPF.Interfaces;
using ToolTemp.WPF.Models;
using ToolTemp.WPF.Services;
using ToolTemp.WPF.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;
using ValidationResult = System.ComponentModel.DataAnnotations.ValidationResult;

namespace ToolTemp.WPF.MVVM.ViewModel
{
    public class MainViewModel : BaseViewModel , INotifyPropertyChanged 
    {
        private readonly IToolService _toolService;
        private BaseViewModel _currentViewModel;
        private SettingViewModel _settingViewModel;
        private ToolViewModel _toolViewModel;
        private AppSettings _appSettings;
        public MySerialPortService _mySerialPort;
        private MyDbContext _context;
        string factory;


        //constructor
        public MainViewModel(AppSettings appSettings, SettingViewModel settingViewModel)
        {
            //LstFactory = DataModelConstant.FactoryConst.Values;
            LstAddress = DataModelConstant.AddressConst;
            _context = new MyDbContext();

            _mySerialPort = new MySerialPortService();
            //settingViewModel = new SettingViewModel(appSettings);

            _settingViewModel = new SettingViewModel(appSettings);

            // Lắng nghe sự kiện từ SettingViewModel
            _settingViewModel.NewButtonCreated += OnNewButtonCreated;


            _toolViewModel = new ToolViewModel();
            _toolService = new ToolService();

            CurrentFactory = appSettings.CurrentArea;
            CurrentViewModel = _settingViewModel;


            DeviceConfig = new DeviceConfig();


            DataFormCommand = new RelayCommand(ExecuteDataForm);
            SettingCommand = new RelayCommand(ExecuteSettingForm);
            SelectFactoryFormCommand = new RelayCommand(ExcuteSelectFactoryForm);

            _factoryButtons = new ObservableCollection<Button>();

            IsEnableBtnSaveNewMachine = true;

            LoadMachineDefaultAsync();

            Messenger.Default.Register<DeviceConfig>(this, "DeviceConfigMessage", HandleDeviceConfigMessage);

            Messenger.Default.Register<Factory>(this, "FactoryConfigMessage", HandleFactoryConfigMessage);
            //Hiển thị máy mới lên 

            _languageService = new LanguageService();
            UpdateTexts();

            LoadLanguage("en"); // Mặc định là tiếng Anh
            LoadDefaultMachine();
            ChangeLanguageCommand = new RelayCommand(ChangeLanguage);


            

        }
        private void OnNewButtonCreated(Button btnMachine)
        {

            btnMachine.Width = 100;
            btnMachine.Height = 30;
            btnMachine.Margin = new Thickness(5);
            btnMachine.Background = new SolidColorBrush(Colors.LightGreen);
            btnMachine.MouseRightButtonDown += BtnMachine_MouseRightButtonDown;
            // Thêm button mới
            FactoryButtons.Add(btnMachine);
        }

        private void LoadDefaultMachine()
        {
            var NameMachine =  _context.Machines.ToList();
            
            foreach (ToolTemp.WPF.Models.Machine Machine  in NameMachine)
            {
                ExecuteAddMachineButtonCommand(Machine);
            }
            
        }
        private void BtnMachine_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("Are you want to modify?", "Notification");
            
        }

        #region Language

        private readonly LanguageService _languageService;
        private string _selectedLanguage;
        public string SelectedLanguage
        {
            get => _selectedLanguage;
            set
            {
                if (_selectedLanguage != value)
                {
                    _selectedLanguage = value;
                    _toolViewModel.CurrentLanguage = value;
                    OnPropertyChanged(nameof(SelectedLanguage));
                    _languageService.ChangeLanguage(_selectedLanguage);
                    UpdateTexts(); // Cập nhật lại văn bản
                }
            }
        }
        public string SettingCommandText { get; private set; }
        public string HelpCommandText { get; private set; }
        public string MenuCommandText { get; private set; }
        
        private void UpdateTexts()
        {
            SettingCommandText = _languageService.GetString("Settings");
            HelpCommandText = _languageService.GetString("Helps");
            MenuCommandText = _languageService.GetString("Menu");
            AssemblingText = _languageService.GetString("Assembling");

            //SettingViewModel
            _settingViewModel.NameCommandText = _languageService.GetString("Name");
            _settingViewModel.MaxCommandText = _languageService.GetString("Max");
            _settingViewModel.MinCommandText = _languageService.GetString("Min");
            _settingViewModel.AddStyleCommandText = _languageService.GetString("AddStyle");
            _settingViewModel.DeleteStyleCommandText = _languageService.GetString("DeleteStyle");
            _settingViewModel.ChooseStyleCommandText = _languageService.GetString("ChooseStyle");
            _settingViewModel.ConnectCommandText = _languageService.GetString("Connect");
            _settingViewModel.CodeText = _languageService.GetString("Code");



            OnPropertyChanged(nameof(SettingCommandText));
            OnPropertyChanged(nameof(HelpCommandText));
            OnPropertyChanged(nameof(MenuCommandText));
            OnPropertyChanged(nameof(AssemblingText));

            OnPropertyChanged(nameof(_settingViewModel.NameCommandText));
            OnPropertyChanged(nameof(_settingViewModel.MaxCommandText));
            OnPropertyChanged(nameof(_settingViewModel.MinCommandText));
            OnPropertyChanged(nameof(_settingViewModel.AddStyleCommandText));
            OnPropertyChanged(nameof(_settingViewModel.DeleteStyleCommandText));
            OnPropertyChanged(nameof(_settingViewModel.ChooseStyleCommandText));
            OnPropertyChanged(nameof(_settingViewModel.ConnectCommandText));
            OnPropertyChanged(nameof(_settingViewModel.CodeText));
        }


        private Dictionary<string, string> _currentLanguage;
        public ICommand ChangeLanguageCommand { get; }

        public void LoadLanguage(string languageCode)
        {
            string filePath = System.IO.Path.Combine(Directory.GetCurrentDirectory(), $"Languages/{languageCode}.json");
            if (File.Exists(filePath))
            {
                var jsonData = File.ReadAllText(filePath);
                _currentLanguage = JsonConvert.DeserializeObject<Dictionary<string, string>>(jsonData);
                OnPropertyChanged(""); // Thông báo tất cả các thuộc tính thay đổi để cập nhật UI
            }
        }

        private void ChangeLanguage(object languageCode)
        {
            if (languageCode is string code)
            {
                LoadLanguage(code);
            }
        }

        private string GetLocalizedString(string key)
        {
            return _currentLanguage != null && _currentLanguage.ContainsKey(key) ? _currentLanguage[key] : key;
        }


        public class ComboBoxTagToStringConverter : IValueConverter
        {
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return value; // Hoặc logic chuyển đổi nếu cần
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                // Trả về Tag của ComboBoxItem tương ứng
                return (value as ComboBoxItem)?.Tag;
            }
        }
        #endregion

        private void HandleFactoryConfigMessage(Factory factory)
        {
            if (factory != null)
            {
                Factory = factory.FactoryCode;
            }
            
        }


        //Event open factory form
        private void ExcuteSelectFactoryForm(object obj)
        {
            
        }
        private string _currentFactory;
        public string CurrentFactory
        {
            get => _currentFactory;
            set
            {
                if (_currentFactory != value)
                {
                    _currentFactory = value;
                    OnPropertyChanged(nameof(CurrentFactory));
                }
            }
        }
        #region Khai báo propertiy Line
        private string _line;
        public string Line
        {
            get => _line;
            set
            {
                if (_line != value)
                {
                    _line = value;
                    OnPropertyChanged(nameof(Line));
                }
            }
        }

        private ObservableCollection<string> _lstLine;
        public ObservableCollection<string> LstLine
        {
            get => _lstLine;
            set
            {
                if (_lstLine != value)
                {
                    _lstLine = value;
                    OnPropertyChanged(nameof(LstLine));
                }
            }
        }


        #endregion

        #region Load Machine mặc định
        int address;
        public  void LoadMachineDefaultAsync()
        {
            try
            {
                
                var mappings = _toolService.GetListAssembling(CurrentFactory);
                if (mappings == null)
                {
                    Tool.Log("Mappings is null.");
                }
                else if (!mappings.Any())
                {
                    Tool.Log("Mappings is empty.");
                }
                else
                {
                    foreach (var mapping in mappings)
                    {
                        ExecuteAddFactoryButtonCommand(mapping);
                    }
                }
            }
            catch (Exception ex)
            {
                Tool.Log("Error Get Default machine: " + ex.Message);
            }
        }

        #endregion

        public ObservableCollection<Button> _factoryButtons = new ObservableCollection<Button>();
        public ObservableCollection<Button> FactoryButtons
        {
            get { return _factoryButtons; }
            set
            {
                _factoryButtons = value;
                OnPropertyChanged(nameof(FactoryButtons));
            }
        }

        #region Language
        private string _assemblingText;
        public string AssemblingText
        {
            get => _assemblingText;
            set
            {
                _assemblingText = value;
                OnPropertyChanged(nameof(AssemblingText));
                UpdateFactoryButtonTexts();  // Gọi hàm cập nhật button khi AssemblingText thay đổi
            }
        }
        #endregion

        // Hàm để cập nhật lại text của các button
        private void UpdateFactoryButtonTexts()
        {
            foreach (var button in FactoryButtons)
            {
                if (button.CommandParameter is Factory model)
                {
                    if (button.Content is TextBlock textBlock)
                    {
                        textBlock.Text = AssemblingText + " " + model.Line;  // Cập nhật nội dung button
                    }
                }
            }
        }

        private void ExecuteAddMachineButtonCommand(object obj)
        {
            if (obj != null)
            {
                ToolTemp.WPF.Models.Machine myMachine =  (ToolTemp.WPF.Models.Machine)obj;

                Button btn_Machine = new Button
                {
                    Content = new TextBlock
                    {
                        Text = myMachine.Name,
                        Width = 90,
                        Height = 30,
                    },
                    Margin = new Thickness(5),
                    Command = new RelayCommand(OpenTools),
                    Background = new SolidColorBrush(Colors.LightGreen)
                };
                // Thêm button mới vào danh sách FactoryButtons
                FactoryButtons.Add(btn_Machine);

                btn_Machine.MouseRightButtonDown += Btn_Machine_MouseRightButtonDown;
            }
        }

        private void Btn_Machine_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBoxResult result = MessageBox.Show("Ban co muon sua khong?", "Notification", MessageBoxButton.YesNo);
            if (result == MessageBoxResult.Yes)
            {
                MessageBox.Show("thuc hien tiep");
            }
            
        }

        private void ExecuteAddFactoryButtonCommand(object obj)
        {
            // Kiểm tra xem nút với Address đã tồn tại trên giao diện chưa
            bool isButtonExists = FactoryButtons.Any(button =>
            {
                return button.Tag?.ToString() == Address.ToString();
            });

            if (isButtonExists)
            {
                // Nếu button đã tồn tại, không tạo thêm mới
                return;
            }

            if (obj != null)
            {
                Factory model = (Factory)obj;
                Button newButton = new Button
                {
                    Content = new TextBlock
                    {
                        Text = AssemblingText + " " + model.Line,  // Sử dụng AssemblingText hiện tại để tạo nội dung button
                        VerticalAlignment = VerticalAlignment.Center,
                        Width = 90,
                        Height = 30,
                    },
                    Margin = new Thickness(5),
                    CommandParameter = model,
                    Command = new RelayCommand(OpenTools),
                    Tag = model.Address,
                    Background = new SolidColorBrush(Colors.LightGreen)
                };

                newButton.MouseRightButtonDown += NewButton_MouseRightButtonDown;
                newButton.Click += MainButton_Click;

                // Thêm button mới vào danh sách FactoryButtons
                FactoryButtons.Add(newButton);
                
            }
        }

        private void NewButton_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show("ok");
        }

        private Button _previousButton; // Biến lưu Button trước đó

        public async void MainButton_Click(object sender, RoutedEventArgs e)
        {
            if (_deviceConfig.Port == "" || _deviceConfig.Port == null )
            {
                MessageBox.Show("Please connect to the device in setting form before!");
                CurrentViewModel = _settingViewModel;
                return;
            }
            else if (_deviceConfig.Max == 0 || _deviceConfig.Min == 0)
            {
                MessageBox.Show("Please choose Max and Min");
                CurrentViewModel = _settingViewModel;
                return;
            }
            else
            {
                CurrentViewModel = _toolViewModel;
                Line = "";

                // Lấy Button được nhấn
                var button = sender as Button;

                // Nếu Button trước đó khác Button hiện tại, trả Button trước đó về màu cũ
                if (_previousButton != null && _previousButton != button)
                {
                    _previousButton.Background = new SolidColorBrush(Colors.LightGreen); // Màu gốc
                }

                // Đổi màu Button hiện tại
                button.Background = new SolidColorBrush(Colors.AliceBlue);

                // Lưu Button hiện tại là Button trước đó
                _previousButton = button;

                // Tiếp tục logic của bạn, ví dụ: xử lý dữ liệu theo Tag của Button
                if (button?.Tag != null)
                {
                    if (_appSettings.CurrentArea != CurrentFactory)
                    {
                        //MessageBox.Show("Please choose correct current factory area to get data!");
                        int address = (int)button.Tag;
                        Line = await _toolService.GetLineByAddressAndFactoryAsync(address, CurrentFactory);
                        _toolViewModel.SetFactory(CurrentFactory, address);
                    }
                    else
                    {
                        if (button.Tag is Task<int> task && _toolViewModel._mySerialPort.IsOpen())
                        {
                            int address = await task;
                            Line = await _toolService.GetLineByAddressAndFactoryAsync(address, CurrentFactory);
                            await _toolViewModel.GetTempFromMachine(address);
                        }
                        else if (button.Tag is int address && _toolViewModel._mySerialPort.IsOpen())
                        {
                            Line = await _toolService.GetLineByAddressAndFactoryAsync(address, CurrentFactory);
                            await _toolViewModel.GetTempFromMachine(address);
                        }
                    }
                }
            }
        }




        private void OpenTools(object param)
        {

            if (param != null)
            {
                Factory fac = (Factory)param;
                _toolViewModel.SetFactory(fac.FactoryCode,fac.Address);
                
            }
            // Switch to the ToolViewModel
        }

        
        



        public ICommand btnMachineCommand { get; set; }


        #region Thêm dữ liệu vào combobox
        private List<int> _LstAddress;
        public List<int> LstAddress
        {
            get => _LstAddress;
            set
            {
                this._LstAddress = value;
                OnPropertyChanged(nameof(LstAddress));
            }
        }
        private int _Address;
        [Required(ErrorMessage ="Address is required")]
        public int Address
        {
            get => _Address;
            set
            {
                this._Address = value;
                OnPropertyChanged(nameof(Address));
            }
        }
        private List<string> _LstFactory;

        public List<string> LstFactory
        {
            get => _LstFactory;
            set
            {
                this._LstFactory = value;
                OnPropertyChanged(nameof(LstFactory));
            }
        }
        private string _Factory;

        [Required(ErrorMessage ="Factory is required")]
        public string Factory
        {
            get => _Factory;
            set
            {
                this._Factory = value;
                OnPropertyChanged(nameof(Factory));
            }
        }
        #endregion
        private DeviceConfig _deviceConfig;

        public DeviceConfig DeviceConfig
        {
            get { return _deviceConfig; }
            set { 
                _deviceConfig = value;
                OnPropertyChanged(nameof(DeviceConfig));
            }
        }

        
        private bool isEnableBtnSaveNewMachine;
        private bool IsEnableBtnSaveNewMachine { 
            get { return isEnableBtnSaveNewMachine; }
            set
            {
                isEnableBtnSaveNewMachine = value;
                OnPropertyChanged(nameof(IsEnableBtnSaveNewMachine));
            }
        }

        private bool isEnabledGetDataAsync;

        public bool IsEnabledGetDataAsync
        {
            get { return isEnabledGetDataAsync; }
            set
            {
                isEnabledGetDataAsync = value;
                OnPropertyChanged(nameof(IsEnabledGetDataAsync));
            }
        }



        public BaseViewModel CurrentViewModel
        {
            get { return _currentViewModel; }
            set
            {
                _currentViewModel = value;
                OnPropertyChanged(nameof(CurrentViewModel));
            }
        }

        private ObservableCollection<MenuItemViewModel> _menuItems;
        public ObservableCollection<MenuItemViewModel> MenuItems
        {
            get { return _menuItems; }
            set
            {
                _menuItems = value;
                OnPropertyChanged(nameof(MenuItems));
            }
        }



        public ICommand DataFormCommand { get; set; }//Open data form
        public ICommand SelectFactoryFormCommand { get; set; }//Open select factory form
        public ICommand SettingCommand { get; set; }




        public event PropertyChangedEventHandler PropertyChanged;
        public event EventHandler<DataErrorsChangedEventArgs>? ErrorsChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        private void HandleDeviceConfigMessage(DeviceConfig message)
        {
            if (message != null)
            {
                DeviceConfig = message;
                _toolViewModel.Port = message.Port;
                _toolViewModel.Baudrate = message.Baudrate;
                //_toolViewModel.Address = message.AddressMachine;
                
            }
        }


        

        

        private void ExecuteDataForm(object parameter)
        {
            _toolViewModel.StopTimer();
           // CurrentViewModel = _addmiachineViewModel;
        }
        private void ExecuteSettingForm(object parameter)
        {
            _toolViewModel.StopTimer();
            
            CurrentViewModel = _settingViewModel;
        }



        /// <summary>
        /// Send function to machine single
        /// </summary>
        /// <param name="parameter"></param>
        //private void ModbusSendFunction(string decimalString)
        //{
        //    var hexWithCRC = Helper.CalculateCRC(decimalString);
        //        var message = hexWithCRC.Replace(" ", "");
        //        _mySerialPort.Write(message);

        //}
        

        #region LogCheck
        private readonly int _maxMessages = 8;
        private Queue<string> _messagesQueue = new Queue<string>();
        private string messagesLogConnect;
        public string MessagesLogConnect
        {
            get { return messagesLogConnect; }
            set
            {
                messagesLogConnect = value;
                OnPropertyChanged(nameof(MessagesLogConnect));
            }
        }


        public string this[string columnName] => throw new NotImplementedException();

        private void AddMessage(string message)
        {
            string prefix = DateTime.Now.ToString("yyyy-MM-dd_HH:mm:ss");
            message = $"{prefix}-{message}";
            // Add the new message to the queue
            _messagesQueue.Enqueue(message);

            // Remove the oldest message if the limit is reached
            if (_messagesQueue.Count > _maxMessages)
            {
                _messagesQueue.Dequeue();
            }

            // Rebuild the message content
            var data = string.Join("\n", _messagesQueue);
            MessagesLogConnect = data;
        }

        public IEnumerable GetErrors(string? propertyName)
        {
            throw new NotImplementedException();
        }

        public  void OnFactorySelectionChanged()
        {
            // Lấy danh sách các Line từ _toolService
            var listOfLines =  _toolService.GetListAssembling(Factory).Select(x => x.Line).ToList();
            

            // Chuyển đổi thành ObservableCollection<string>
            LstLine = new ObservableCollection<string>(listOfLines);
        }

        #endregion

    }
}
