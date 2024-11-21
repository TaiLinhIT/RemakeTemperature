using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO.Ports;
using System.Reflection.Emit;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using ToolTemp.WPF.Configs;
using ToolTemp.WPF.Constants;
using ToolTemp.WPF.Core;
using ToolTemp.WPF.Interfaces;
using ToolTemp.WPF.Models;
using ToolTemp.WPF.Services;
using ToolTemp.WPF.Utils;
using Button = System.Windows.Controls.Button;
using MessageBox = System.Windows.MessageBox;
using Style = ToolTemp.WPF.Models.Style;

namespace ToolTemp.WPF.MVVM.ViewModel
{
    public class SettingViewModel : BaseViewModel, INotifyPropertyChanged , IDataErrorInfo
    {
        private SerialPort _serialPort;
        private string _selectedPort;
        private readonly IToolService _toolService;

        private readonly MyDbContext _context;
        private MySerialPortService _myserialPort;

        public ObservableCollection<int> LstBaudrate { get; set; }
        public ObservableCollection<string> LstChooseAssembling { get; set; }

        #region Khai báo và lấy ra danh sách các post

        private List<string> _lstPost;
        public List<string> ListPost
        {
            get => _lstPost;
            set { 
         
                this._lstPost = value;
                OnPropertyChanged(nameof(_lstPost));
            }
        }

        public string port;
        public string Port
        {
            get => port;
            set
            {
                this.port = value;
                OnPropertyChanged(nameof(_lstPost));
            }
        }
        public void GetPorts()
        {
            string[] ArryPort = SerialPort.GetPortNames();
            ListPost = ArryPort.ToList<string>();
        }
        #endregion

        #region Lấy ra danh sách các tốc độ truyền

        private List<int> _lstBaute;
        public List<int> lstBaute
        {
            get => _lstBaute;
            set
            {
                this._lstBaute = value;
                OnPropertyChanged(nameof(lstBaute));
            }
        }
        public int baudrate;
        public int Baudrate
        {
            get => baudrate;
            set
            {
                this.baudrate = value;
                OnPropertyChanged(nameof(Baudrate));
            }
        }
        #endregion

        #region GetPortName
        public ObservableCollection<string> LstPort { get; set; } = new ObservableCollection<string>();


        public void GetPortName()
        {
            string[] lstPort = SerialPort.GetPortNames();
            foreach (var item in lstPort)
            {
                LstPort.Add(item);

            }
        }
        #endregion





        #region Entity

        private DeviceConfig _deviceConfig;

        public DeviceConfig DeviceConfig
        {
            get { return _deviceConfig; }
            set
            {
                _deviceConfig = value;
                OnPropertyChanged(nameof(DeviceConfig));
            }
        }
        // Thuộc tính lưu trữ Baudrate được chọn
        private string _selectedBaudrate;
        public string SelectedBaudrate
        {
            get => _selectedBaudrate;
            set
            {
                if (_selectedBaudrate != value)
                {
                    _selectedBaudrate = value;
                    OnPropertyChanged(nameof(SelectedBaudrate));
                }
            }
        }

        //Thuoc tinh luu tru choose Assembling
        private string _selectedAssembling;
        public string SelectedAssembling
        {
            get => _selectedAssembling;
            set
            {
                if (_selectedAssembling != value)
                {
                    _selectedAssembling = value;
                    OnPropertyChanged(nameof(SelectedAssembling));
                }
            }
        }


        //Thuoc tinh luu tru port
        private string _selectPort;

        public string SelectedPort
        {
            get => _selectPort;
            set
            {
                if (_selectPort != value)
                {
                    _selectPort = value;
                    OnPropertyChanged(nameof(SelectedPort));
                }
            }
        }


        //Thuoc tinh luu tru LstAssembling
        private string _selectedChooseAssembling;

        public string SelectedChooseAssembling
        {
            get => _selectedChooseAssembling;
            set
            {
                if (_selectedChooseAssembling != value)
                {

                    _selectedChooseAssembling = value;
                    OnPropertyChanged(nameof(SelectedChooseAssembling));
                }
            }
        }


        // Thuộc tính lưu trữ NameMachine (TextBox)
        private string _nameMachine;
        public string NameMachine
        {
            get => _nameMachine;
            set
            {
                if (_nameMachine != value)
                {
                    _nameMachine = value;
                    OnPropertyChanged(nameof(NameMachine));
                }
            }
        }



        //Thuoc tinh luu tru Address
        private int _addressMachine;
        public int AddressMachine
        {
            get => _addressMachine;
            set
            {
                if (_addressMachine != value)
                {
                    _addressMachine = value;
                    OnPropertyChanged(nameof(AddressMachine));
                }
            }
        }



        public string _nameStyle;
        public string NameStyle
        {
            get => _nameStyle;
            set
            {
                this._nameStyle = value;
                OnPropertyChanged(nameof(NameStyle));
                IsEnabledBtnAddStyle = true;
            }
        }

        public int _max;
        public int Max
        {
            get => _max;
            set
            {
                this._max = value;
                OnPropertyChanged(nameof(Max));
            }
        }


        public int _min;
        public int Min
        {
            get => _min;
            set
            {
                this._min = value;
                OnPropertyChanged(nameof(Min));
            }
        }

        public List<int> _lstAddress;
        public List<int> ListAddress
        {
            get => _lstAddress;
            set
            {
                this._lstAddress = value;
                OnPropertyChanged(nameof(ListAddress));
            }
        }
        public int address;
        public int Address
        {
            get => address;
            set
            {
                this.address = value;
                OnPropertyChanged(nameof(Address));
            }
        }

        public string this[string columnName]
        {
            get
            {
                string error = null;
                switch (columnName)
                {
                    case nameof(Port):
                        if (string.IsNullOrEmpty(Port))
                            error = "Port is required.";
                        else if (Port == "COM1")
                            error = "Port is default! Choose anthore port.";
                        break;
                    case nameof(Baudrate):
                        if (string.IsNullOrEmpty(Baudrate.ToString()))
                            error = "Baudrate is required.";
                        else if (Baudrate != 115200)
                            error = "Baudrate is not correct.";
                        break;
                    
                }
                return error;
            }
        }

        public string Error => null;
        #endregion




        public RelayCommand SaveConfigCommand { get; }
        string selectedCompany;
        #region Khai báo các button
        public ICommand ConnectCommand { get; set; }
        public ICommand DisconnectCommand { get; set; }
        public ICommand AddStyleCommand { get; set; }
        public ICommand DeleteStyleCommand { get; set; }

        #endregion
        private MySerialPortService _mySerialPort;
        public ToolViewModel _toolViewModel;
        public AppSettings _appSetting;

        public event PropertyChangedEventHandler PropertyChanged;
        public event Action<Button> NewButtonCreated;

        private string _textBoxContent;
        public string TextBoxContent
        {
            get => _textBoxContent;
            set
            {
                _textBoxContent = value;
                OnPropertyChanged(nameof(TextBoxContent));
            }
        }
        public ICommand CreatedButtonCommand { get; }

        #region Ẩn hiển settings
        public event Action OnConnect;



        #endregion
        // constructor
        #region Language
        private string _connectCommandText;
        public string ConnectCommandText
        {
            get => _connectCommandText;
            set
            {
                _connectCommandText = value;
                OnPropertyChanged(nameof(ConnectCommandText));
            }
        }
        private string _deleteStyleCommandText;
        public string DeleteStyleCommandText
        {
            get => _deleteStyleCommandText;
            set
            {
                _deleteStyleCommandText = value;
                OnPropertyChanged(nameof(DeleteStyleCommandText));
            }
        }
        private string _chooseStyleCommandText;
        public string ChooseStyleCommandText
        {
            get => _chooseStyleCommandText;
            set
            {
                _chooseStyleCommandText = value;
                OnPropertyChanged(nameof(ChooseStyleCommandText));
            }
        }
        private string _addStyleCommandText;
        public string AddStyleCommandText
        {
            get => _addStyleCommandText;
            set
            {
                _addStyleCommandText = value;
                OnPropertyChanged(nameof(AddStyleCommandText));
            }
        }
        private string _nameCommandText;
        public string NameCommandText
        {
            get => _nameCommandText;
            set
            {
                _nameCommandText = value;
                OnPropertyChanged(nameof(NameCommandText));
            }
        }

        private string _maxCommandText;
        public string MaxCommandText
        {
            get => _maxCommandText;
            set
            {
                _maxCommandText = value;
                OnPropertyChanged(nameof(MaxCommandText));
            }
        }

        private string _minCommandText;
        public string MinCommandText
        {
            get => _minCommandText;
            set
            {
                _minCommandText = value;
                OnPropertyChanged(nameof(MinCommandText));
            }
        }
        #endregion

        #region Constructor
        public SettingViewModel(AppSettings appSettings)
        {
            _appSetting = appSettings;
            message.Port = _appSetting.Port;
            message.Baudrate = _appSetting.Baudrate;
            _toolViewModel = new ToolViewModel();

            _toolViewModel.Port = _appSetting.Port;
            _toolViewModel.Baudrate = _appSetting.Baudrate;

            _myserialPort = new MySerialPortService();

            IsEnabledBtnConnect = true;
            IsEnabledBtnAddStyle = true;
            IsEnabledBtnDelete = false;

            // list baudrate

            LstBaudrate = new ObservableCollection<int>()
            {
                9600,14400,19200,
            };

            // list choose Assembling

            LstChooseAssembling = new ObservableCollection<string>()
            {
                "Nong","Lanh"
            };


            _toolService = new ToolService();

            //Buttons
            ConnectCommand = new RelayCommand(ExecuteConnectCommand, CanConnect);
            DisconnectCommand = new RelayCommand(ExecuteDisConnectCommand, CanDisconnect);
            AddStyleCommand = new RelayCommand(AddButton, CanAddStyle);
            DeleteStyleCommand = new RelayCommand(ExecuteDeleteCommand, CanDeleteCommand);


            ButtonList = new ObservableCollection<Button>();

            //GetCompanyAddress();
            lstBaute = DataModelConstant.BaudrateConst;// khởi tạo và gán giá trị cho danh sách tốc độ truyền
           
            

            //OpenViewMainCommand = new RelayCommand(OpenViewMain);
            GetPorts();

            Messenger.Default.Register<DeviceConfig>(this, "DeviceConfigToChild", HandleDeviceConfigToChildMessage);

            LoadButtonsAsync();

            CreatedButtonCommand = new RelayCommand(Createbutton,canExecute);

            GetPortName();

            
        }

        #endregion

        private bool canExecute(object obj)
        {
            throw new NotImplementedException();
        }

        private void Createbutton(object obj)
        {
            // Kiểm tra nội dung hợp lệ
            if (!string.IsNullOrWhiteSpace(TextBoxContent))
            {
                // Tạo Button mới
                var newButton = new Button
                {
                    Content = TextBoxContent,
                    Width = 90,
                    Height = 30,
                    Margin = new Thickness(5),
                    Background = new SolidColorBrush(Colors.LightGreen)
                };

                // Gửi sự kiện
                NewButtonCreated?.Invoke(newButton);
            }
        }


        private void HandleDeviceConfigToChildMessage(DeviceConfig message)
        {
            Port = message.Port;
            Baudrate = message.Baudrate;
            Max = Convert.ToInt32(message.Max) ;
            Min = Convert.ToInt32(message.Min);
            NameStyle = message.NameStyle;
        }

        #region Style

        private string _codetext;
        public string CodeText
        {
            get => _codetext;
            set
            {
                _codetext = value;
                OnPropertyChanged(nameof(CodeText));
                UpdateStyleButtonTexts();
            }
        }
        private void UpdateStyleButtonTexts()
        {
            foreach (var button in ButtonList)
            {
                if (button.DataContext is Style model)
                {
                    if (button.Content is TextBlock textBlock)
                    {
                        textBlock.Text = CodeText + " " + model.NameStyle;  // Cập nhật nội dung button
                    }
                }
            }
        }

        // Danh sách chứa các button đã tạo
        public ObservableCollection<Button> _buttonList = new ObservableCollection<Button>();
        public ObservableCollection<Button> ButtonList
        {
            get { return _buttonList; }
            set
            {
                _buttonList = value;
                OnPropertyChanged(nameof(ButtonList));

            }
        }

        

        // Phương thức để thêm button mới vào danh sách
        private async void AddButton(object parameter)
        {
            Style style = new Style();
            style.NameStyle = NameStyle;
            style.Max = Max;
            style.Min = Min;
            // Save data asynchronously
            await Task.Run(async () =>
            {
                try
                {
                    await _toolService.InsertToStyle(style);
                }
                catch (Exception insertEx)
                {
                    Tool.Log($"Error saving data: {insertEx.Message}");
                    if (insertEx.StackTrace != null)
                        Tool.Log(insertEx.StackTrace.ToString());
                }
            });


            Button btn_Style = new Button
            {
                Content = new TextBlock
                {
                    Text = "Mã " + _toolService.GetListStyle().OrderByDescending(x => x.Id).Select(x => x.NameStyle)
                             .FirstOrDefault().ToString(),
                    VerticalAlignment = VerticalAlignment.Center,
                    Width = 90,
                    Height = 30,

                },
                Margin = new Thickness(5),
                DataContext = style,


                Background = new SolidColorBrush(Colors.LightGreen)
            };

            btn_Style.Click += btn_Style_Click;
            // Thêm vào ButtonList
            ButtonList.Add(btn_Style);



            // Reset các trường nhập liệu
            NameStyle = string.Empty;

        }
        public DeviceConfig message = new DeviceConfig();

        private void btn_Style_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Cast sender to Button
                Button clickedButton = (Button)sender;

                

                // Retrieve the Style object from DataContext
                if (clickedButton.DataContext is Style style)
                {
                    // Use the Style object to populate DeviceConfig
                    message.NameStyle = style.NameStyle;
                    message.Max = style.Max;
                    message.Min = style.Min;

                    Messenger.Default.Send(message, "DeviceConfigMessage");
                    _toolViewModel.Max = Convert.ToInt32(style.Max);
                    _toolViewModel.Min = Convert.ToInt32(style.Min);
                    _toolViewModel.NameStyle = style.NameStyle;

                    NameStyle = style.NameStyle;
                    Max = Convert.ToInt32(style.Max);
                    Min = Convert.ToInt32(style.Min);
                    IsEnabledBtnAddStyle = false;
                    IsEnabledBtnDelete = true;
                    
                    //_toolViewModel.Start();
                }
                else
                {
                    // Handle the case where DataContext is not a Style object
                    MessageBox.Show("The DataContext of the button is not of type Style.");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Connection error! " + ex.Message);
            }
        }



        private async Task LoadButtonsAsync()
        {
            try
            {
                // Retrieve the styles from the database
                var styles =  _toolService.GetAllStyles();

                // Clear existing buttons if necessary
                ButtonList.Clear();

                foreach (var style in styles)
                {
                    Button btn_Style = new Button
                    {
                        Content = new TextBlock
                        {
                            Text = "Mã " + style.NameStyle,
                            VerticalAlignment = VerticalAlignment.Center,
                            Width = 90,
                            Height = 30
                        },
                        Margin = new Thickness(5),
                        Background = new SolidColorBrush(Colors.LightGreen),
                        DataContext = style // Set DataContext to the Style object
                    };

                    btn_Style.Click += btn_Style_Click;
                    ButtonList.Add(btn_Style); // Add the button to the list
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error loading buttons: " + ex.Message);
            }
        }



        // Điều kiện để cho phép thêm button (ComboBox không rỗng)
        private bool CanAddStyle(object parameter)
        {
            try
            {
                return !string.IsNullOrEmpty(NameStyle) && !string.IsNullOrWhiteSpace(Max.ToString()) && !string.IsNullOrWhiteSpace(Min.ToString()) && IsEnabledBtnAddStyle;

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
            // Chỉ cho phép thêm khi các trường Name, Max, Min không rỗng
        }



        public async void ExecuteConnectCommand(object parameter)
        {
            //if (string.IsNullOrWhiteSpace(Port) || !DataModelConstant.BaudrateConst.Contains(Baudrate))
            //{
            //    MessageBox.Show("Please connect to the device before");
            //    return;
            //}
            //if (Port == "COM1" || Baudrate != 2400)
            //{
            //    MessageBox.Show("Please choose correct connection");
            //    return;
            //}

            try
            {
                //message.Port = this.Port;
                //message.Baudrate = this.Baudrate;

                

                Messenger.Default.Send(message, "DeviceConfigMessage");
                // set port for _toolViewModel
                
                
                _toolViewModel.Start();
                IsEnabledBtnConnect = false;
                MessageBox.Show("Connection successful!");

                Machine machine = new Machine();
                machine.Name = NameMachine;
                machine.Port = SelectedPort;
                machine.Baudrate = SelectedBaudrate;
                machine.Address = AddressMachine;


                await _toolService.InsertToMachine(machine);

                Button btn_Machine = new Button()
                {
                    Content = NameMachine,
                    
                };
                // Gửi Button qua sự kiện
                NewButtonCreated?.Invoke(btn_Machine);

            }
            catch (Exception ex)
            {
                IsEnabledBtnConnect = true;
                MessageBox.Show("Connection erro!" + ex.Message);
            }
        }
        private bool CanConnect(object parameter)
        {
            return IsEnabledBtnConnect;
        }


        private async void ExecuteDisConnectCommand(object parameter)
        {
            try
            {
                
                _toolViewModel.Close();
                
                IsEnabledBtnConnect = true;
                MessageBox.Show("Disconnected !");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error disconnecting!" + ex.Message);
                IsEnabledBtnConnect = false;
            }
        }

        private bool CanDisconnect(object parameter)
        {
            return !IsEnabledBtnConnect;
        }


        private void ExecuteDeleteCommand (object parameter)
        {
            try
            {
                _toolService.DeleteStyleByName(NameStyle);
                LoadButtonsAsync();
                NameStyle = string.Empty;
                Max = 0;
                Min = 0;
                MessageBox.Show("Delete succesesfully!");
            }
            catch (Exception ex)
            {

                Tool.Log($"Errors delete: {ex.Message}");
                
            }
        }
        private bool CanDeleteCommand(object parameter)
        {
            return IsEnabledBtnDelete;
        }
        private bool isEnabledBtnConnect;

        public bool IsEnabledBtnConnect
        {
            get { return isEnabledBtnConnect; }
            set
            {
                isEnabledBtnConnect = value;
                OnPropertyChanged(nameof(IsEnabledBtnConnect));
            }
        }

        private bool isEnabledBtnAddStyle;
        public bool IsEnabledBtnAddStyle
        {
            get { return isEnabledBtnAddStyle; }
            set
            {
                isEnabledBtnAddStyle = value;
                OnPropertyChanged(nameof(IsEnabledBtnAddStyle));
            }
        }

        private bool isEnabledBtnDelete;

        public bool IsEnabledBtnDelete
        {
            get { return isEnabledBtnDelete; }
            set
            {
                isEnabledBtnDelete = value;
                OnPropertyChanged(nameof(IsEnabledBtnDelete));
            }
        }

        #endregion


        public event PropertyChangedEventHandler PropertyChangeds;

        protected void OnPropertyChanged(string name)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }
}
