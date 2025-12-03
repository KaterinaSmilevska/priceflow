CREATE TABLE [dbo].[Sat_PortfolioPrinosi]
(
	PortfolioPrinosi_HK CHAR(32) NOT NULL,
	Datum DATE NOT NULL,
	NetoIznos DECIMAL NOT NULL,
	Danok DECIMAL(18, 2) NOT NULL,
	HashDiff NVARCHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	EndDate DATETIME2 NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Sat_PortfolioPrinosi PRIMARY KEY (PortfolioPrinosi_HK, LoadDate),
	CONSTRAINT fk_Sat_PortfolioPrinosi_Link_PortfolioPrinosi FOREIGN KEY (PortfolioPrinosi_HK) REFERENCES Link_PortfolioPrinosi(PortfolioPrinosi_HK)
)
