using GalaSoft.MvvmLight.Messaging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Drawing2D;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using ToolTemp.WPF.Configs;
using ToolTemp.WPF.Core;
using ToolTemp.WPF.Interfaces;
using ToolTemp.WPF.Models;
using ToolTemp.WPF.Services;
using ToolTemp.WPF.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ToolTemp.WPF.MVVM.ViewModel
{
    public class ToolViewModel : BaseViewModel
    {
        private SettingViewModel _settingViewModel;
        private readonly MyDbContext _context;
        public string Port = string.Empty;
        public string Factory = string.Empty;
        public string NameStyle = string.Empty;//add new item
        public int Max = 0;//add new item
        public int Min = 0;//add new item
        public int Baudrate = 0;
        private DispatcherTimer _dispatcherTimer;
        private readonly IToolService _toolService;
        private readonly AppSettings _appSettings;
        public MySerialPortService _mySerialPort;
        public string CurrentFactory;


        public ObservableCollection<BusDataWithDevice> _temperatures;
        public ObservableCollection<BusDataWithDevice> Temperatures
        {
            get => _temperatures;
            set
            {
                _temperatures = value;
                OnPropertyChanged(nameof(Temperatures));
            }
        }
        public string FactoryCode { get; private set; }
        public int AddressCurrent { get; private set; }

        
        public void SetFactory(string factory,int address)
        {
            FactoryCode = factory;
            AddressCurrent = address;
            ReloadData(FactoryCode,AddressCurrent); // Trigger the data reload immediately with the new address
            StartTimer(); // Start the timer only after setting the address
        }
        //constructor
        public ToolViewModel(AppSettings appSettings,ToolService toolService)
        {
            CurrentLanguage = "en";
            _appSettings = appSettings;
            _toolService = toolService;
           


            Temperatures = new ObservableCollection<BusDataWithDevice>
            {                
                
                new BusDataWithDevice() {Channel = _appSettings.Channel},

            };
            _dispatcherTimer = new DispatcherTimer();

            _dispatcherTimer.Interval = TimeSpan.FromSeconds(Convert.ToInt32(_appSettings.TimeReloadData));

            _dispatcherTimer.Tick += DispatcherTimer_Tick;
        }
        private void DispatcherTimer_Tick(object sender, EventArgs e)
        {

            ReloadData(FactoryCode, AddressCurrent);

        }


        private async void ReloadData(string factory, int address)
        {
            if (!string.IsNullOrEmpty(Port))
            {
                List<BusDataWithDevice> data = await  _toolService.GetListDataWithDevicesAsync(Port, factory, address, Baudrate,CurrentLanguage);

                if (data != null)
                {
                    var find = data;

                    // Clear the existing items and add new ones
                    Temperatures.Clear();
                    foreach (var item in find)
                    {
                        Temperatures.Add(item);
                    }
                }
            }
        }

        public void StartTimer()
        {
            ReloadData(FactoryCode, AddressCurrent);
            _dispatcherTimer.Start();
        }

        public void StopTimer()
        {
            _dispatcherTimer.Stop();
        }


        public void Start()
        {
            _mySerialPort = new MySerialPortService();
            _mySerialPort.Port = Port;
            _mySerialPort.Baudrate = Baudrate;
            _mySerialPort.Sdre += SerialPort_DataReceived;

            _mySerialPort.Conn();
        }
        public void Close()
        {
            try
            {
                _mySerialPort.Stop();

            }
            catch (Exception ex)
            {

            }
        }
        #region Language
        private string _currentLanguage;
        public string CurrentLanguage
        {
            get => _currentLanguage;
            set
            {
                _currentLanguage = value;
                OnPropertyChanged(nameof(CurrentLanguage));
            }
        }
        #endregion
        #region Read Temperature and save database

        private async void SerialPort_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                int BytesCount = 0;
                SerialPort sp = (SerialPort)sender;
                BytesCount = sp.BytesToRead;

                byte[] bufferb = new byte[BytesCount];
                sp.Read(bufferb, 0, BytesCount);
                if (Tool.CRC_PD(bufferb))
                {
                    //03 is function read data
                    if (bufferb[1] == 3)
                    {
                        int address = bufferb[0];
                        string factory = FactoryCode;
                        var modBusDTO = new BusDataTemp();
                        byte[] bytes = new byte[] { bufferb[4], bufferb[3], bufferb[6], bufferb[5] };
                        float temp = BitConverter.ToSingle(bytes, 0);
                        if (temp >= 999)
                        {
                            return;
                        }
                        modBusDTO.Temp = (double)temp;
                        modBusDTO.Channel = _mySerialPort.MapByteResponeToChannel(bufferb[2], _appSettings);
                        modBusDTO.Port = _appSettings.Port;
                        modBusDTO.Factory = factory;

                        modBusDTO.Line = await _toolService.GetLineByAddressAndFactoryAsync(address, factory);
                        modBusDTO.Baudrate = Baudrate;
                        modBusDTO.AddressMachine = bufferb[0];
                        //var data = _appSettings.ConfigCommand.FirstOrDefault(x => x.Name.Equals(modBusDTO.Channel));
                        modBusDTO.Max = Max;
                        modBusDTO.Min = Min;
                        modBusDTO.Sensor_Typeid = 7;
                        modBusDTO.UploadDate = DateTime.Now;
                        modBusDTO.IsWarning = temp > Max;

                        // Save data asynchronously
                        await Task.Run(async () =>
                        {
                            try
                            {
                                await _toolService.InsertData(modBusDTO);
                            }
                            catch (Exception insertEx)
                            {
                                Tool.Log($"Error saving data: {insertEx.Message}");
                                if (insertEx.StackTrace != null)
                                    Tool.Log(insertEx.StackTrace.ToString());
                            }
                        });

                        
                    }
                }
                else
                {
                    //AddMessage($"Invalid data: {BitConverter.ToString(bufferb)}");
                }
            }
            catch (Exception ex)
            {
                Tool.Log(ex.Message);
                if (ex.StackTrace != null)
                    Tool.Log(ex.StackTrace.ToString());
            }
        }

        #endregion

        #region Handle Modbus
        // Write to get Temperature
        
        public async Task GetTempFromMachine(int address)
        {
            while (true)
            {
                var listChannel = _appSettings.ConfigCommand;
                foreach (var item in listChannel)
                {
                    //if (_settingViewModel.IsEnabledBtnConnect)
                    //{
                    //    return;
                    //}
                    ModbusSendFunction("0" + address + item.AddressWrite);
                    await Task.Delay(TimeSpan.FromSeconds(Convert.ToInt32(_appSettings.TimeBusTemp)));
                }
            }
        }

        private void ModbusSendFunction(string decimalString)
        {
            try
            {
                var hexWithCRC = Helper.CalculateCRC(decimalString);
                var message = hexWithCRC.Replace(" ", "");
                _mySerialPort.Write(message);
            }
            catch (Exception ex)
            {

                MessageBox.Show($"Errorrs: {ex.Message}");
                return;
            }

        }
        #endregion

    }
}
