using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolTemp.WPF.Models
{
    public class BusDataWithDevice
    {
        public int Id { get; set; }
        public string Channel { get; set; }
        public string Factory { get; set; }
        public string Line { get; set; }
        public int AddressMachine { get; set; }
        public string Port { get; set; }
        public double Temp { get; set; }
        public int Baudrate { get; set; }
        public double Min { get; set; }
        public double Max { get; set; }
        public DateTime UploadDate { get; set; }
        public bool IsWarning { get; set; }
        public string DeviceName { get; set; } // Thông tin từ bảng devices
    }
}
