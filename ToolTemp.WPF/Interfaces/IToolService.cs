using ToolTemp.WPF.Models;

namespace ToolTemp.WPF.Interfaces
{
    public interface IToolService
    {
        Task<int> InsertData(BusDataTemp model);
        Task<bool> InsertToStyle(Style model);
        Task<List<BusDataWithDevice>> GetListDataWithDevicesAsync(string port, string factory, int address, int baudRate, string language);
        List<Factory> GetListFactory(string factoryCode);
        List<Style> GetListStyle();
        public bool DeleteStyleByName(string Name);
        Task<string> GetLineByAddressAndFactoryAsync(int address,string factory);
        public List<Style> GetAllStyles();



    }
}
