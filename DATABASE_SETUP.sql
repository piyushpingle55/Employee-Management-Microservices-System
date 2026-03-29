-- ============================================================
-- CREATE DATABASE: EmployeeDataBase
-- ============================================================

-- Create Database
CREATE DATABASE EmployeeDataBase;
GO

-- Use the database
USE EmployeeDataBase;
GO

-- ============================================================
-- CREATE TABLE: Employees
-- ============================================================

CREATE TABLE Employees (
    Id UNIQUEIDENTIFIER PRIMARY KEY DEFAULT NEWID(),
    Name NVARCHAR(100) NOT NULL,
    Email NVARCHAR(100) NOT NULL,
    Salary DECIMAL(10, 2) NOT NULL,
    DepartmentId UNIQUEIDENTIFIER NOT NULL,
    CreatedAt DATETIME2 DEFAULT GETUTCDATE(),
    UpdatedAt DATETIME2 DEFAULT GETUTCDATE()
);

GO

-- ============================================================
-- CREATE INDEXES
-- ============================================================

CREATE INDEX IX_Employees_Email ON Employees(Email);
CREATE INDEX IX_Employees_DepartmentId ON Employees(DepartmentId);

GO

-- ============================================================
-- INSERT SAMPLE DATA (Optional)
-- ============================================================

INSERT INTO Employees (Name, Email, Salary, DepartmentId)
VALUES 
    ('John Smith', 'john.smith@company.com', 50000.00, '550e8400-e29b-41d4-a716-446655440000'),
    ('Jane Doe', 'jane.doe@company.com', 60000.00, '550e8400-e29b-41d4-a716-446655440001'),
    ('Bob Johnson', 'bob.johnson@company.com', 55000.00, '550e8400-e29b-41d4-a716-446655440000');

GO

-- ============================================================
-- VERIFY TABLES CREATED
-- ============================================================

SELECT * FROM Employees;
