CREATE TABLE [Project] (
        [ID] INTEGER PRIMARY KEY ,
        [CurrentTaskNoteID] integer NULL ,
        [ProjectNoteID] integer NULL ,
        [Title] varchar(255) NULL
);

CREATE TABLE [Registration] (
        [ID] INTEGER PRIMARY KEY ,
        [ActiveAcknowledge] boolean NULL ,
        [NoteID] integer NULL ,
        [ProjectID] integer NULL ,
        [Time] datetime NULL
);

CREATE TABLE [SpecificEvent] (
        [ID] INTEGER PRIMARY KEY ,
        [Data] varchar(255) NULL ,
        [RegistrationID] integer NULL ,
        [Time] datetime NULL ,
        [EventType] integer NULL
);

CREATE TABLE [Note] (
        [ID] INTEGER PRIMARY KEY ,
        [NoteText] text NULL
);

CREATE TABLE [LeftSide] (
      [ID] INTEGER PRIMARY KEY ,
		[Text] varchar(255) NULL
);

CREATE TABLE [RightSide] (
      [ID] INTEGER PRIMARY KEY ,
		[Text] varchar(255) NULL
);

CREATE TABLE [ManyToMany] (
      [ID] INTEGER PRIMARY KEY ,
		[LeftID] INTEGER NULL,
		[RightID] INTEGER NULL
);

CREATE TABLE [TableWithNoAutoincremetion] (
      [ID] text PRIMARY KEY ,
		[Meh] INTEGER NULL
);