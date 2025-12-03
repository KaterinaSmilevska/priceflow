CREATE TABLE [dbo].[Link_HartiiOdVrednost_TipHV]
(
	HartiiOdVrednost_TipHV_HK CHAR(32) NOT NULL,
	HartiiOdVrednost_HK CHAR(32) NOT NULL,
	TipHV_HK CHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Link_HartiiOdVrednost_TipHV PRIMARY KEY (HartiiOdVrednost_TipHV_HK),
	CONSTRAINT fk_Link_HartiiOdVrednost_TipHV_Hub_HartiiOdVrednost FOREIGN KEY (HartiiOdVrednost_HK) REFERENCES Hub_HartiiOdVrednost(HartiiOdVrednost_HK),
	CONSTRAINT fk_Link_HartiiOdVrednost_TipHV_Hub_TipHV FOREIGN KEY (TipHV_HK) REFERENCES Hub_TipHV(TipHV_HK)
)
