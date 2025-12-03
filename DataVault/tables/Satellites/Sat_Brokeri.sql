CREATE TABLE [dbo].[Sat_Brokeri]
(
	Brokeri_HK CHAR(32) NOT NULL,
	ProcentProvizija DECIMAL(18, 3) NOT NULL,
	HashDiff NVARCHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	EndDate DATETIME2 NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Sat_Brokeri PRIMARY KEY (Brokeri_HK, LoadDate),
	CONSTRAINT fk_Sat_Brokeri_Hub_Brokeri FOREIGN KEY (Brokeri_HK) REFERENCES Hub_Brokeri(Brokeri_HK)
)
