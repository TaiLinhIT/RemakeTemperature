CREATE TABLE dv_BusDataTemp (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Channel NVARCHAR(50) NOT NULL,
    Factory NVARCHAR(100) NOT NULL,
    Line NVARCHAR(100) NOT NULL,
    AddressMachine INT NOT NULL,

    Port NVARCHAR(50) NOT NULL,
    Temp FLOAT NOT NULL,
    Baudrate INT NOT NULL,
    Min FLOAT NOT NULL,
    Max FLOAT NOT NULL,
    UploadDate DATETIME NOT NULL,
    IsWarning BIT NOT NULL
);

CREATE table dv_style(
    Id INT PRIMARY KEY IDENTITY(1,1),
    NameStyle NVARCHAR(50) NOT NULL,
    Max INT NOT NULL,
    Min INT NOT NULL
);

create table dv_MapFactoryAddress(
    Id int primary key identity(1,1),
    Factory nvarchar(50) not null,
    Address int not null,
    Status bit not null
);


CREATE TABLE dv_Factory_Configs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    FactoryCode NVARCHAR(50) NOT NULL,
    Line NVARCHAR(100) NOT NULL,
    Address INT NOT NULL
);


CREATE TABLE dv_BusDataTemp_Configs (
    Id INT PRIMARY KEY IDENTITY(1,1),
    Channel NVARCHAR(50) NOT NULL,
    Company NVARCHAR(100) NOT NULL,
    Port NVARCHAR(50) NOT NULL,
    CommandWrite NVARCHAR(100) NOT NULL,
	Min FLOAT NOT NULL,
	Max FLOAT NOT NULL,
	DisplayName NVARCHAR(100) NOT NULL
);
CREATE TABLE dv_Device_Configs(
    Id INT PRIMARY KEY IDENTITY(1,1),
    Company NVARCHAR(50) NOT NULL,
    AddressMachine FLOAT NOT NULL
);


INSERT INTO dv_BusDataTemp_Configs (Channel, Company, Port, CommandWrite, Min, Max, DisplayName) VALUES
('CH1', 'VA1', 'COM4', '01 03 00 18 00 02', -60, 60, 'A1'),
('CH2', 'VA1', 'COM4', '01 03 00 38 00 03', -60, 60, 'A2'),
('CH3', 'VA1', 'COM4', '01 03 00 58 00 04', -60, 60, 'A3'),
('CH4', 'VA1', 'COM4', '01 03 00 78 00 05', -60, 60, 'A4'),
('CH5', 'VA1', 'COM4', '01 03 00 98 00 06', -60, 60, 'A5'),
('CH6', 'VA1', 'COM4', '01 03 00 118 00 07', -60, 60, 'A6'),
('CH7', 'VA1', 'COM4', '01 03 00 138 00 08', -60, 60,'A7')
