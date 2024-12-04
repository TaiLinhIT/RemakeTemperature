using GalaSoft.MvvmLight.Messaging;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Windows.Forms;
using ToolTemp.WPF.Interfaces;
using ToolTemp.WPF.Models;
using ToolTemp.WPF.Utils;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace ToolTemp.WPF.Services
{

    public class ToolService: IToolService
    {
        private readonly MyDbContext _context;
        public ToolService(MyDbContext myDbContext)
        {
            _context = myDbContext;
        }

        public async Task<bool> InsertToStyle(Style model)
        {
            try
            {
                await _context.Style.AddAsync(model);
                await _context.SaveChangesAsync();
                

                return true; // Trả về 1 nếu thêm dữ liệu thành công
            }
            catch (Exception ex)
            {
                Tool.Log(ex.Message);
                return false;
            }
        }

        public async Task<int> InsertData(BusDataTemp model)
        {
            try
            {
                await _context.BusDataTemp.AddAsync(model);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                Tool.Log(ex.Message);
                return 0;
            }
        }




        public async Task<List<BusDataWithDevice>> GetListDataWithDevicesAsync(string port, string factory, int address, int baudRate, string language)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();

                // Ensure the connection is open
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                // Using 'using' for command ensures proper disposal
                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GetListDataWithDevices"; // Stored procedure name
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Create parameters and add them
                    command.Parameters.AddRange(new[]
                    {
                new SqlParameter("@Language", System.Data.DbType.String) { Value = language },
                new SqlParameter("@Port", System.Data.DbType.String) { Value = port },
                new SqlParameter("@Factory", System.Data.DbType.String) { Value = factory },
                new SqlParameter("@Address", System.Data.DbType.Int32) { Value = address },
                new SqlParameter("@Baudrate", System.Data.DbType.Int32) { Value = baudRate }
            });

                    var result = new List<BusDataWithDevice>();

                    // Execute the command and process results
                    using (var reader = await command.ExecuteReaderAsync()) // Sử dụng ExecuteReaderAsync
                    {
                        while (await reader.ReadAsync()) // Sử dụng ReadAsync
                        {
                            var busDataTempWithDevice = new BusDataWithDevice
                            {
                                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                                Channel = reader.GetString(reader.GetOrdinal("Channel")),
                                Factory = reader.GetString(reader.GetOrdinal("Factory")),
                                Line = reader.GetString(reader.GetOrdinal("Line")),
                                AddressMachine = reader.GetInt32(reader.GetOrdinal("AddressMachine")),
                                Port = reader.GetString(reader.GetOrdinal("Port")),
                                Temp = reader.GetDouble(reader.GetOrdinal("Temp")),
                                Baudrate = reader.GetInt32(reader.GetOrdinal("Baudrate")),
                                Min = reader.GetDouble(reader.GetOrdinal("Min")),
                                Max = reader.GetDouble(reader.GetOrdinal("Max")),
                                UploadDate = reader.GetDateTime(reader.GetOrdinal("UploadDate")),
                                IsWarning = reader.GetBoolean(reader.GetOrdinal("IsWarning")),
                                DeviceName = reader.GetString(reader.GetOrdinal("name")) // Kiểm tra tên cột cho chính xác
                            };

                            result.Add(busDataTempWithDevice);
                        }
                    }


                    return result;
                }
            }
            catch (Exception ex)
            {
                Tool.Log($"Error in GetListDataWithDevicesAsync: {ex.Message}");
                return new List<BusDataWithDevice>(); // Return an empty list instead of null to avoid potential null references.
            }
        }


        public List<Style> GetListStyle()
        {
            try
            {
                var result = _context.Style.FromSqlRaw("EXEC GetListStyle").ToList();
               
                return result;
            }
            catch (Exception ex)
            {
                Tool.Log($"Errors GetListStyle: {ex.Message}");
                return null;
            }
        }
        public bool DeleteStyleByName(string name)
        {
            try
            {
                Style find =  _context.Style.Where(x => x.NameStyle == name).FirstOrDefault();
                _context.Style.Remove(find);
                _context.SaveChanges();
                return true;
            }
            catch (Exception ex)
            {

                Tool.Log($"Errors Delete style: {ex.Message}");
                return false;
            }
        }

        public List<Factory> GetListAssembling(string factoryCode)
        {
            try
            {
                var factoryCodeParam = new SqlParameter("@FactoryCode", factoryCode);
                var result = _context.FactoryConfigs.FromSqlRaw("EXEC GetListFactory @FactoryCode", factoryCodeParam).ToList();
                return result;
            }
            catch (Exception ex)
            {
                Tool.Log($"Errors GetListFactory: {ex.Message}");
                return null;
            }
        }



        public async Task<string> GetLineByAddressAndFactoryAsync(int address, string factory)
        {
            try
            {
                var connection = _context.Database.GetDbConnection();
                if (connection.State != System.Data.ConnectionState.Open)
                {
                    await connection.OpenAsync();
                }

                using (var command = connection.CreateCommand())
                {
                    command.CommandText = "GetLineByAddressAndFactory";
                    command.CommandType = System.Data.CommandType.StoredProcedure;

                    // Thêm tham số @Address
                    var addressParameter = command.CreateParameter();
                    addressParameter.ParameterName = "@Address";
                    addressParameter.Value = address;
                    addressParameter.DbType = System.Data.DbType.Int32;
                    command.Parameters.Add(addressParameter);

                    // Thêm tham số @Factory
                    var factoryParameter = command.CreateParameter();
                    factoryParameter.ParameterName = "@Factory";
                    factoryParameter.Value = factory;
                    factoryParameter.DbType = System.Data.DbType.String;
                    command.Parameters.Add(factoryParameter);

                    var result = await command.ExecuteScalarAsync();
                    return result as string;
                }
            }
            catch (Exception ex)
            {
                Tool.Log($"Error in GetLineByAddressAndFactoryAsync: {ex.Message}");
                return null;
            }
        }


        public List<Style> GetAllStyles()
        {
            try
            {
                return _context.Style.ToList();

            }
            catch (Exception ex)
            {
                Tool.Log($"Error retrieving data: {ex.Message}");
                if (ex.StackTrace != null)
                    Tool.Log(ex.StackTrace.ToString());
                return new List<Style>();
            }
        }

        public async Task<int> InsertToMachine(Machine machine)
        {
            try
            {
                await _context.machines.AddAsync(machine);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public async Task<int> DeleteToMachine(Machine machine)
        {
            try
            {
                _context.machines.Remove(machine);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return 0;
            }
        }

        public async Task<int> EditToMachine(Machine machine)
        {
            try
            {
                _context.machines.Update(machine);
                await _context.SaveChangesAsync();
                return 1;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return 0;
            }


        }
    }
}
