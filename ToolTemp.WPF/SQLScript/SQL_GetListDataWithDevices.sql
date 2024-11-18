CREATE PROCEDURE GetListDataWithDevices
    @Port NVARCHAR(50),
    @Factory NVARCHAR(100),
    @Address INT,
    @Baudrate INT,
    @Language NVARCHAR(10) -- Thêm tham số để xác định ngôn ngữ
AS
BEGIN
    -- Xác định typeid dựa trên ngôn ngữ
    DECLARE @TypeId INT;
    IF @Language = 'vi'  -- Tiếng Việt
        SET @TypeId = 2;
    ELSE IF @Language = 'en'  -- Tiếng Anh
        SET @TypeId = 3;
    ELSE IF @Language = 'cn'  -- Tiếng Trung
        SET @TypeId = 1;
    ELSE
        SET @TypeId = NULL; -- Giá trị mặc định nếu ngôn ngữ không hợp lệ

    -- Dùng CTE để lọc dữ liệu như trước và thêm join với bảng devices
    WITH tempt AS (
        SELECT *,
               ROW_NUMBER() OVER (PARTITION BY Channel, Factory, AddressMachine, Port, Baudrate ORDER BY UploadDate DESC) AS rn
        FROM dv_BusDataTemp
        WHERE Factory = @Factory 
          AND AddressMachine = @Address 
          AND Port = @Port 
          AND Baudrate = @Baudrate
    )
    SELECT t.Id,
           t.Channel,
           t.Temp,
           t.Factory,
           t.Line,
           t.AddressMachine,
           t.Port,
           t.Baudrate,
           t.IsWarning,
           ROUND(CAST(t.Min AS FLOAT), 2) AS Min,
           ROUND(CAST(t.Max AS FLOAT), 2) AS Max,
           t.UploadDate,
           d.name -- Lấy tên từ bảng devices
    FROM tempt t
    LEFT JOIN devices d ON t.Channel = d.devid  -- Join với bảng devices theo điều kiện Channel = devid
    WHERE t.rn = 1
      AND d.typeid = @TypeId;  -- Lọc theo typeid dựa trên ngôn ngữ
END;

