CREATE TABLE [dbo].[Korisnik]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
    [Ime] NVARCHAR(100) NOT NULL, 
    [Username] NVARCHAR(100) NOT NULL, 
    [PasswordHash] VARBINARY(48) NOT NULL, 
    [Email] NVARCHAR(100) NOT NULL, 

    CONSTRAINT pk_Korisnik PRIMARY KEY (Id),
    CONSTRAINT un_Username UNIQUE (Username)
)
