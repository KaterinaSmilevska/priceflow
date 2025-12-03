CREATE TABLE [dbo].[Hub_Brokeri]
(
	Brokeri_HK CHAR(32) NOT NULL,
	Kompanija NVARCHAR(100) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Hub_Brokeri PRIMARY KEY (Brokeri_HK),
	CONSTRAINT un_Hub_Brokeri_Kompanija UNIQUE (Kompanija)
)
