CREATE TABLE [dbo].[Sat_Korisnici_Sensitive]
(
	Korisnici_HK CHAR(32) NOT NULL,
	PasswordHash VARBINARY(48) NOT NULL,
	IsEmailVerified BIT NOT NULL DEFAULT 0,
    EmailVerificationToken UNIQUEIDENTIFIER NULL,
    ResetPasswordToken UNIQUEIDENTIFIER NULL,
    ResetPasswordTokenExpiry DATETIME NULL,
	HashDiff NVARCHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	EndDate DATETIME2 NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Sat_Korisnici_Sensitive PRIMARY KEY (Korisnici_HK, LoadDate),
	CONSTRAINT fk_Sat_Korisnici_Sensitive_Hub_Korisnici FOREIGN KEY (Korisnici_HK) REFERENCES Hub_Korisnici(Korisnici_HK)
)
