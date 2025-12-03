CREATE TABLE [dbo].[Hub_HartiiOdVrednost]
(
	HartiiOdVrednost_HK CHAR(32) NOT NULL,
	Kod NVARCHAR(50) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Hub_HartiiOdVrednost PRIMARY KEY (HartiiOdVrednost_HK),
	CONSTRAINT un_Hub_HartiiOdVrednost_Kod UNIQUE (Kod)
)
