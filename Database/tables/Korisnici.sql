CREATE TABLE [dbo].[Korisnici]
(
	[Id] INT IDENTITY(1,1) NOT NULL, 
    [Ime] NVARCHAR(100) NOT NULL, 
    [Username] NVARCHAR(100) NOT NULL, 
    [PasswordHash] VARBINARY(48) NOT NULL, 
    [Email] NVARCHAR(100) NOT NULL,
    [IsEmailVerified] BIT NOT NULL DEFAULT 0,
    [EmailVerificationToken] UNIQUEIDENTIFIER NULL,
    [ResetPasswordToken] UNIQUEIDENTIFIER NULL,
    [ResetPasswordTokenExpiry] DATETIME NULL,

    CONSTRAINT pk_Korisnici PRIMARY KEY (Id),
    CONSTRAINT un_Korisnici_Username UNIQUE (Username)
)
