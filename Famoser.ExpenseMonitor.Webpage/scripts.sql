DROP TABLE Expense;
DROP TABLE ExpenseCollection;
DROP TABLE ExpenseTakerListRelation;
DROP TABLE ExpenseTaker;

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
	Content text,
	CreateTime datetime,
	IsCompleted bool
);