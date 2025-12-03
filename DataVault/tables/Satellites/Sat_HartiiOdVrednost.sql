CREATE TABLE [dbo].[Sat_HartiiOdVrednost]
(
	HartiiOdVrednost_HK CHAR(32) NOT NULL,
	ISIN NVARCHAR(12) NOT NULL,
	VkupenBrojAkcii INT NOT NULL,
	HashDiff NVARCHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	EndDate DATETIME2 NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Sat_HartiiOdVrednost PRIMARY KEY (HartiiOdVrednost_HK, LoadDate),
	CONSTRAINT fk_Sat_HartiiOdVrednost_Hub_HartiiOdVrednost FOREIGN KEY (HartiiOdVrednost_HK) REFERENCES Hub_HartiiOdVrednost(HartiiOdVrednost_HK)
)
