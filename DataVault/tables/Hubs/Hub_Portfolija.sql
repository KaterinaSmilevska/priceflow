CREATE TABLE [dbo].[Hub_Portfolija]
(
	Portfolija_HK CHAR(32) NOT NULL,
	Username NVARCHAR(100) NOT NULL,
	Ime NVARCHAR(50) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Hub_Portfolija PRIMARY KEY (Portfolija_HK),
	CONSTRAINT un_Hub_Portfolija_Username_Ime UNIQUE (Username, Ime)
)
