CREATE TABLE [Task] (
	"ID" INTEGER PRIMARY KEY,
	"Name" TEXT NULL,
	"Duration" INTEGER NULL,
	"SortOrder" INTEGER NULL,
	"Note" TEXT NULL,
	
	"StartDate" DATETIME NULL,
	"EndDate" DATETIME NULL,
	"Fixed" BOOLEAN NULL
);