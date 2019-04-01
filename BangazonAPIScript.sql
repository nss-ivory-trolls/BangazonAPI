--DELETE FROM OrderProduct;
--DELETE FROM ComputerEmployee;
--DELETE FROM EmployeeTraining;
--DELETE FROM Employee;
--DELETE FROM TrainingProgram;
--DELETE FROM Computer;
--DELETE FROM Department;
--DELETE FROM [Order];
--DELETE FROM PaymentType;
--DELETE FROM Product;
--DELETE FROM ProductType;
--DELETE FROM Customer;


--ALTER TABLE Employee DROP CONSTRAINT [FK_EmployeeDepartment];
--ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Employee];
--ALTER TABLE ComputerEmployee DROP CONSTRAINT [FK_ComputerEmployee_Computer];
--ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Employee];
--ALTER TABLE EmployeeTraining DROP CONSTRAINT [FK_EmployeeTraining_Training];
--ALTER TABLE Product DROP CONSTRAINT [FK_Product_ProductType];
--ALTER TABLE Product DROP CONSTRAINT [FK_Product_Customer];
--ALTER TABLE PaymentType DROP CONSTRAINT [FK_PaymentType_Customer];
--ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Customer];
--ALTER TABLE [Order] DROP CONSTRAINT [FK_Order_Payment];
--ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Product];
--ALTER TABLE OrderProduct DROP CONSTRAINT [FK_OrderProduct_Order];


DROP TABLE IF EXISTS OrderProduct;
DROP TABLE IF EXISTS ComputerEmployee;
DROP TABLE IF EXISTS EmployeeTraining;
DROP TABLE IF EXISTS Employee;
DROP TABLE IF EXISTS TrainingProgram;
DROP TABLE IF EXISTS Computer;
DROP TABLE IF EXISTS Department;
DROP TABLE IF EXISTS [Order];
DROP TABLE IF EXISTS PaymentType;
DROP TABLE IF EXISTS Product;
DROP TABLE IF EXISTS ProductType;
DROP TABLE IF EXISTS Customer;


CREATE TABLE Department (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL,
	Budget 	INTEGER NOT NULL
);

CREATE TABLE Employee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL,
	DepartmentId INTEGER NOT NULL,
	IsSuperVisor BIT NOT NULL DEFAULT(0),
    CONSTRAINT FK_EmployeeDepartment FOREIGN KEY(DepartmentId) REFERENCES Department(Id)
);

CREATE TABLE Computer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	PurchaseDate DATETIME NOT NULL,
	DecomissionDate DATETIME,
	Make VARCHAR(55) NOT NULL,
	Manufacturer VARCHAR(55) NOT NULL
);

CREATE TABLE ComputerEmployee (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	ComputerId INTEGER NOT NULL,
	AssignDate DATETIME NOT NULL,
	UnassignDate DATETIME,
    CONSTRAINT FK_ComputerEmployee_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_ComputerEmployee_Computer FOREIGN KEY(ComputerId) REFERENCES Computer(Id)
);


CREATE TABLE TrainingProgram (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(255) NOT NULL,
	StartDate DATETIME NOT NULL,
	EndDate DATETIME NOT NULL,
	MaxAttendees INTEGER NOT NULL
);

CREATE TABLE EmployeeTraining (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	EmployeeId INTEGER NOT NULL,
	TrainingProgramId INTEGER NOT NULL,
    CONSTRAINT FK_EmployeeTraining_Employee FOREIGN KEY(EmployeeId) REFERENCES Employee(Id),
    CONSTRAINT FK_EmployeeTraining_Training FOREIGN KEY(TrainingProgramId) REFERENCES TrainingProgram(Id)
);

CREATE TABLE ProductType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	[Name] VARCHAR(55) NOT NULL
);

CREATE TABLE Customer (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	FirstName VARCHAR(55) NOT NULL,
	LastName VARCHAR(55) NOT NULL
);

CREATE TABLE Product (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	ProductTypeId INTEGER NOT NULL,
	CustomerId INTEGER NOT NULL,
	Price INTEGER NOT NULL,
	Title VARCHAR(255) NOT NULL,
	[Description] VARCHAR(255) NOT NULL,
	Quantity INTEGER NOT NULL,
    CONSTRAINT FK_Product_ProductType FOREIGN KEY(ProductTypeId) REFERENCES ProductType(Id),
    CONSTRAINT FK_Product_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);


CREATE TABLE PaymentType (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	AcctNumber INTEGER NOT NULL,
	[Name] VARCHAR(55) NOT NULL,
	CustomerId INTEGER NOT NULL,
    CONSTRAINT FK_PaymentType_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id)
);

CREATE TABLE [Order] (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	CustomerId INTEGER NOT NULL,
	PaymentTypeId INTEGER,
    CONSTRAINT FK_Order_Customer FOREIGN KEY(CustomerId) REFERENCES Customer(Id),
    CONSTRAINT FK_Order_Payment FOREIGN KEY(PaymentTypeId) REFERENCES PaymentType(Id)
);

CREATE TABLE OrderProduct (
	Id INTEGER NOT NULL PRIMARY KEY IDENTITY,
	OrderId INTEGER NOT NULL,
	ProductId INTEGER NOT NULL,
    CONSTRAINT FK_OrderProduct_Product FOREIGN KEY(ProductId) REFERENCES Product(Id),
    CONSTRAINT FK_OrderProduct_Order FOREIGN KEY(OrderId) REFERENCES [Order](Id)
);

insert into Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) values ('01 Jan 2019', '01 Jan 2024', 'MacBook Pro', 'Apple')
insert into Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) values ('01 Jan 2018', '01 Jan 2023', 'Inspiron', 'Dell')
insert into Computer (PurchaseDate, DecomissionDate, Make, Manufacturer) values ('01 Jan 2017', '01 Jan 2022', 'MacBook Air', 'Apple')

insert into Department ([Name], Budget) values ('Accounting', 400000)
insert into Department ([Name], Budget) values ('IT', 40000)
insert into Department ([Name], Budget) values ('Sales', 450000)

insert into Employee (FirstName, LastName, DepartmentId, IsSuperVisor)  values ('Hernando', 'Rivera', 1, 0)
insert into Employee (FirstName, LastName, DepartmentId, IsSuperVisor)  values ('Mary', 'Phillips', 2, 0)
insert into Employee (FirstName, LastName, DepartmentId, IsSuperVisor)  values ('Lorenzo', 'Lopez', 1, 1)

insert into Customer (FirstName, LastName) values ('Fred', 'Flinstone')
insert into Customer (FirstName, LastName) values ('Barney', 'Rubble')
insert into Customer (FirstName, LastName) values ('George', 'Jetson')

insert into PaymentType (AcctNumber, [Name], CustomerId) values (1000, 'Visa', 1)
insert into PaymentType (AcctNumber, [Name], CustomerId) values (2000, 'MasterCard', 2)
insert into PaymentType (AcctNumber, [Name], CustomerId) values (3000, 'AmEx', 3)

insert into [Order] (CustomerId, PaymentTypeId) values (1, 1)
insert into [Order] (CustomerId, PaymentTypeId) values (2, 2)
insert into [Order] (CustomerId, PaymentTypeId) values (3, 3)

insert into ProductType ([Name]) values ('Electronics')
insert into ProductType ([Name]) values ('Sports Equipment')
insert into ProductType ([Name]) values ('Furniture')

insert into Product (ProductTypeId, CustomerId, Title, [Description], Quantity, Price) values (1, 1, 'Television', 'Classic 1970s antique TV', 1, 10)
insert into Product (ProductTypeId, CustomerId, Title, [Description], Quantity, Price) values (2, 2, 'Baseball Bat', 'Wooden', 1, 5)
insert into Product (ProductTypeId, CustomerId, Title, [Description], Quantity, Price) values (3, 3, 'Chair', 'Folding', 1, 3)

insert into  ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) values (1, 1, '01 Jan 2019', NULL)
insert into  ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) values (2, 2, '01 Jan 2018', NULL)
insert into  ComputerEmployee (EmployeeId, ComputerId, AssignDate, UnassignDate) values (3, 3, '01 Jan 2017', NULL)

insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (1, 1)
insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (2, 2)
insert into EmployeeTraining (EmployeeId, TrainingProgramId) values (3, 3)

insert into TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) values ('How to Count Beans', '14 Feb 2019', '15 Feb 2019', 10)
insert into TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) values ('How to Spell "IT"', '14 Feb 2019', '15 Feb 2019', 10)
insert into TrainingProgram ([Name], StartDate, EndDate, MaxAttendees) values ('How to Sell Beans', '14 Feb 2019', '15 Feb 2019', 10)

insert into OrderProduct (OrderId, ProductId) values (1, 1)
insert into OrderProduct (OrderId, ProductId) values (2, 2)
insert into OrderProduct (OrderId, ProductId) values (2, 2)
insert into OrderProduct (OrderId, ProductId) values (3, 3)