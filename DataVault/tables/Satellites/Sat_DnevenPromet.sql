CREATE TABLE [dbo].[Sat_DnevenPromet]
(
	HartiiOdVrednost_HK CHAR(32) NOT NULL,
	Datum DATETIME NOT NULL,
	CenaPoslednaTransakcija DECIMAL(18, 2) NULL, 
    MaxCena DECIMAL(18, 2) NULL, 
    MinCena DECIMAL(18, 2) NULL, 
    ProsecnaCena DECIMAL(18, 2) NULL, 
    ProcentPromena DECIMAL(18, 2) NULL, 
    KolicinaIstrguvaniAkcii INT NULL, 
    PrometBESTDenari INT NULL, 
    VkupenPrometDenari INT NULL,
	HashDiff NVARCHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	EndDate DATETIME2 NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Sat_DnevenPromet PRIMARY KEY (HartiiOdVrednost_HK, Datum, LoadDate),
	CONSTRAINT fk_Sat_DnevenPromet_Hub_HartiiOdVrednost FOREIGN KEY (HartiiOdVrednost_HK) REFERENCES Hub_HartiiOdVrednost(HartiiOdVrednost_HK)
)
