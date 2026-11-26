CREATE DATABASE BankAtmDB;
GO
USE BankAtmDB;
GO

CREATE TABLE AtmStations (
    StationID NVARCHAR(20) PRIMARY KEY,
    Address NVARCHAR(200) NOT NULL,
    CashBalance DECIMAL(18, 0) NOT NULL,
    Status NVARCHAR(50) NOT NULL,
    ImagePath NVARCHAR(500) NULL
);

CREATE TABLE ReconciliationLogs (
    LogID INT IDENTITY(1,1) PRIMARY KEY,
    ReconcileDate DATETIME DEFAULT GETDATE(),
    TotalTransactions INT,
    MatchedCount INT,
    DiscrepancyCount INT,
    Note NVARCHAR(255)
);
GO

INSERT INTO AtmStations (StationID, Address, CashBalance, Status, ImagePath) VALUES
('ATM-HN-001', N'124 Trần Duy Hưng, Cầu Giấy, Hà Nội', 850000000, N'Hoạt động', NULL),
('ATM-HN-002', N'54 Liễu Giai, Ba Đình, Hà Nội', 120000000, N'Sắp hết tiền', NULL),
('ATM-HN-003', N'01 Đại Cồ Việt, Hai Bà Trưng, Hà Nội', 0, N'Bảo trì', NULL);
GO