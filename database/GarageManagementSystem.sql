CREATE DATABASE GarageDB

USE GarageDB

CREATE TABLE KhachHang
(
    MaKH INT IDENTITY(1,1) PRIMARY KEY,
    TenKH NVARCHAR(100) NOT NULL,
    DienThoai VARCHAR(15) UNIQUE,
    DiaChi NVARCHAR(200)
)

CREATE TABLE Xe
(
    BienSo VARCHAR(20) PRIMARY KEY,
    MaKH INT,
    HieuXe NVARCHAR(50),
    NamSanXuat INT,

    FOREIGN KEY(MaKH) REFERENCES KhachHang(MaKH)
)

CREATE TABLE PhuTung
(
    MaPhuTung INT IDENTITY(1,1) PRIMARY KEY,
    TenPhuTung NVARCHAR(150) NOT NULL,
    DonGia DECIMAL(18,2),
    SoLuongTon INT CHECK(SoLuongTon >=0)
)

CREATE TABLE PhieuSuaChua
(
    MaPhieu INT IDENTITY(1,1) PRIMARY KEY,
    BienSo VARCHAR(20),
    NgayNhan DATETIME DEFAULT GETDATE(),
    NoiDungSua NVARCHAR(MAX),
    TongTien DECIMAL(18,2),
    TrangThai NVARCHAR(30),

    FOREIGN KEY(BienSo) REFERENCES Xe(BienSo)
)

CREATE TABLE ChiTietSuaChua
(
    MaPhieu INT,
    MaPhuTung INT,
    SoLuong INT CHECK(SoLuong>0),
    TienCong DECIMAL(18,2),

    PRIMARY KEY(MaPhieu, MaPhuTung),

    FOREIGN KEY(MaPhieu) REFERENCES PhieuSuaChua(MaPhieu),
    FOREIGN KEY(MaPhuTung) REFERENCES PhuTung(MaPhuTung)
)