CREATE TABLE [dbo].[Sat_Izdavachi]
(
	Izdavachi_HK CHAR(32) NOT NULL,
	Grad NVARCHAR(100) NOT NULL,
	Drzava NVARCHAR(100) NOT NULL,
	HashDiff NVARCHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	EndDate DATETIME2 NULL,
	RecordSource NVARCHAR(50) NOT NULL,	

	CONSTRAINT pk_Sat_Izdavachi PRIMARY KEY (Izdavachi_HK, LoadDate),
	CONSTRAINT fk_Sat_Izdavachi_Hub_Izdavachi FOREIGN KEY (Izdavachi_HK) REFERENCES Hub_Izdavachi(Izdavachi_HK)
)
