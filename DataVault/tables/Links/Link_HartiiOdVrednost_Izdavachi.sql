CREATE TABLE [dbo].[Link_HartiiOdVrednost_Izdavachi]
(
	HartiiOdVrednost_Izdavachi_HK CHAR(32) NOT NULL,
	HartiiOdVrednost_HK CHAR(32) NOT NULL,
	Izdavachi_HK CHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Link_HartiiOdVrednost_Izdavachi PRIMARY KEY (HartiiOdVrednost_Izdavachi_HK),
	CONSTRAINT fk_Link_HartiiOdVrednost_Izdavachi_Hub_HartiiOdVrednost FOREIGN KEY (HartiiOdVrednost_HK) REFERENCES Hub_HartiiOdVrednost(HartiiOdVrednost_HK),
	CONSTRAINT fk_Link_HartiiOdVrednost_Izdavachi_Hub_Izdavachi FOREIGN KEY (Izdavachi_HK) REFERENCES Hub_Izdavachi(Izdavachi_HK)
)
