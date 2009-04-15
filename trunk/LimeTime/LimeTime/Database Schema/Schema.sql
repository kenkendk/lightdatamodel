CREATE TABLE [Project] (
	"ID" INTEGER PRIMARY KEY,
	"Title" TEXT NULL,
	"Type" TEXT NULL,
	"UseAnnoyClock" BOOLEAN NULL
);

CREATE TABLE [Registration] (
	"ID" INTEGER PRIMARY KEY,
	"Time" DATETIME NULL,
	"ProjectID" INTEGER NULL,
	"Note" TEXT NULL
);

CREATE TABLE [Version] (
	"ID" INTEGER PRIMARY KEY,
	"Version" INTEGER NULL
);

CREATE TABLE [RecentEntry] (
	"ID" INTEGER PRIMARY KEY,
	"Time" DATETIME NULL,
	"ProjectID" INTEGER NULL,
	"TypedText" TEXT NULL
);


INSERT INTO VERSION (Version) VALUES (2);