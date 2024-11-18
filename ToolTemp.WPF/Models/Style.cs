using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ToolTemp.WPF.Models
{
    [Table("dv_style")]
    public class Style
    {
        public int Id { get; set; }
        public string? NameStyle { get; set; }
        public decimal Max { get; set; }
        public decimal Min { get; set; }
        public string? Devid { get; set; } // Cột này nếu có trong DB

        [Column("Standard_temp")]
        public string? StandardTemp { get; set; }
        [Column("Compensate_Vaild")]
        public decimal? CompensateVaild { get; set; }
    }


}
