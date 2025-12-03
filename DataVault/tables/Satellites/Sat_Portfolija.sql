CREATE TABLE [dbo].[Sat_Portfolija]
(
	Portfolija_HK CHAR(32) NOT NULL,
	Opis NVARCHAR(100) NULL,
	HashDiff NVARCHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	EndDate DATETIME2 NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Sat_Portfolija PRIMARY KEY (Portfolija_HK, LoadDate),
	CONSTRAINT fk_Sat_Portfolija_Hub_Portfolija FOREIGN KEY (Portfolija_HK) REFERENCES Hub_Portfolija(Portfolija_HK)
)
