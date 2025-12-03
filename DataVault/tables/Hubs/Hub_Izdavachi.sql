CREATE TABLE [dbo].[Hub_Izdavachi]
(
	Izdavachi_HK CHAR(32) NOT NULL,
	Ime NVARCHAR(100) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Hub_Izdavachi PRIMARY KEY (Izdavachi_HK),
	CONSTRAINT un_Hub_Izdavachi_Ime UNIQUE (Ime)
)
