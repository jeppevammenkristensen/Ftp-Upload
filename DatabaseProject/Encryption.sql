﻿CREATE TABLE [dbo].[Encryption]
(
	[Id] INT NOT NULL PRIMARY KEY IDENTITY, 
    [Key] VARBINARY(50) NOT NULL, 
    [IV] VARBINARY(50) NOT NULL
)