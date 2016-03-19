DROP TABLE Expenses;
DROP TABLE ExpenseCollections;
DROP TABLE ExpenseTakerExpenseCollectionRelations;
DROP TABLE ExpenseTakers;

CREATE TABLE ExpenseTakers
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Guid varchar(255)
);

CREATE TABLE ExpenseTakerExpenseCollectionRelations
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	ExpenseTakerId int,
	ExpenseCollectionId int
);

CREATE TABLE ExpenseCollections
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	Guid varchar(255),
	Name text,
	CreateTime datetime
);

CREATE TABLE Expenses
(
	Id INTEGER PRIMARY KEY AUTOINCREMENT,
	ExpenseCollectionId int,
	Guid varchar(255),
	Description text,
	CreateTime datetime,
	Amount double
);