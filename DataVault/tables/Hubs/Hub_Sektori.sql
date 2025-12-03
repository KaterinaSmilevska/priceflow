CREATE TABLE [dbo].[Hub_Sektori]
(
	Sektori_HK CHAR(32) NOT NULL,
	Ime NVARCHAR(100) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Hub_Sektori PRIMARY KEY (Sektori_HK),
	CONSTRAINT un_Hub_Sektori_Ime UNIQUE (Ime)
)
