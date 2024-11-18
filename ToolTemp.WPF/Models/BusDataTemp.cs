﻿using System.ComponentModel.DataAnnotations.Schema;

namespace ToolTemp.WPF.Models
{
    [Table("dv_BusDataTemp")]
    public class BusDataTemp
    {
        public int Id { get; set; } 
        public string? Channel { get; set; }
        public string? Factory { get; set; }
        public string? Line { get; set; }
        public int AddressMachine { get; set; }//thêm mới
        public string? Port { get; set; }
        public double Temp { get; set; } = 0;
        public int Baudrate { get; set; }
        public double Min { get; set; } = 0;
        public double Max { get; set; } = 0;
        public DateTime UploadDate { get; set; }
        public bool IsWarning { get; set; } = false;
        public int? Sensor_Typeid { get; set; } // INT

        public string? Sensor_kind { get; set; } // NVARCHAR(63)

        public string? Sensor_ant { get; set; } // NVARCHAR(63)
    }
}