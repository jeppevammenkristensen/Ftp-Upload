CREATE TABLE [dbo].[Encryption] (
   [Key] VARBINARY (50) NOT NULL,
    [IV]  VARBINARY (50) NOT NULL,
      [Id]  INT            IDENTITY (1, 1) NOT NULL,
   PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[FtpInformation] (
    [Id]       INT            IDENTITY (1, 1) NOT NULL,
    [Password] NVARCHAR (255) NULL,
    [Server]   NVARCHAR (255) NULL,
    [UserName] NVARCHAR (255) NULL,
    [Name]     NVARCHAR (255) NULL,
    [Path]     NVARCHAR (255) NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);

CREATE TABLE [dbo].[Version] (
    [Id]      INT IDENTITY (1, 1) NOT NULL,
    [Version] INT NULL,
    PRIMARY KEY CLUSTERED ([Id] ASC)
);