CREATE TABLE [dbo].[Link_Transakcii]
(
	Transakcii_HK CHAR(32) NOT NULL,
	HartiiOdVrednost_HK CHAR(32) NOT NULL,
	Portfolija_HK CHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Link_Transakcii PRIMARY KEY (Transakcii_HK),
	CONSTRAINT fk_Link_Transakcii_Hub_HartiiOdVrednost FOREIGN KEY (HartiiOdVrednost_HK) REFERENCES Hub_HartiiOdVrednost(HartiiOdVrednost_HK),
	CONSTRAINT fk_Link_Transakcii_Hub_Portfolija FOREIGN KEY (Portfolija_HK) REFERENCES Hub_Portfolija(Portfolija_HK)
)
