CREATE TABLE [dbo].[FtpInformation]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Password] NVARCHAR(255) NULL, 
    [Server] NVARCHAR(255) NULL, 
    [UserName] NVARCHAR(255) NULL, 
    [Name] NVARCHAR(255) NULL, 
    [Path] NVARCHAR(255) NULL
)
