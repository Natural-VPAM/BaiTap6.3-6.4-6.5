CREATE DATABASE ProjectTaskDB;

USE ProjectTaskDB;

CREATE TABLE DuAn
(
    MaDuAn INT IDENTITY(1,1) PRIMARY KEY,
    TenDuAn NVARCHAR(150) NOT NULL,
    NgayBatDau DATETIME,
    NgayKetThuc DATETIME,
    TrangThai NVARCHAR(50)
);

CREATE TABLE ThanhVien
(
    MaThanhVien INT IDENTITY(1,1) PRIMARY KEY,
    HoTen NVARCHAR(100) NOT NULL,
    Email VARCHAR(100) UNIQUE,
    ChuyenMon NVARCHAR(100)
);

CREATE TABLE CongViec
(
    MaTask INT IDENTITY(1,1) PRIMARY KEY,
    MaDuAn INT NOT NULL,
    TenTask NVARCHAR(200) NOT NULL,
    MoTa NVARCHAR(MAX),
    HanNop DATETIME,
    DoUuTien NVARCHAR(20),
    TrangThai NVARCHAR(50),

    CONSTRAINT FK_CongViec_DuAn
    FOREIGN KEY(MaDuAn)
    REFERENCES DuAn(MaDuAn)
);

CREATE TABLE PhanCong
(
    MaTask INT,
    MaThanhVien INT,
    NgayGiao DATETIME DEFAULT GETDATE(),

    PRIMARY KEY(MaTask, MaThanhVien),

    FOREIGN KEY(MaTask)
    REFERENCES CongViec(MaTask),

    FOREIGN KEY(MaThanhVien)
    REFERENCES ThanhVien(MaThanhVien)
);

INSERT INTO DuAn VALUES
('Website Bán Hàng','2024-01-01','2024-08-30','Đang chạy'),
('Hệ thống CRM','2024-03-01','2024-10-01','Đang chạy');

INSERT INTO ThanhVien VALUES
('Nguyễn Văn A','a@gmail.com','Backend'),
('Trần Thị B','b@gmail.com','Frontend'),
('Lê Văn C','c@gmail.com','Tester');

INSERT INTO CongViec VALUES
(1,'Thiết kế CSDL','Tạo database SQL Server','2024-07-10','Cao','In Progress'),

(1,'Thiết kế giao diện','Xây dựng giao diện WinForms','2024-07-15','Trung bình','To Do'),

(2,'Xây dựng API','Viết API ASP.NET','2024-07-20','Cao','Done');

INSERT INTO PhanCong(MaTask,MaThanhVien) VALUES
(1,1),
(2,2),
(3,1),
(3,3);