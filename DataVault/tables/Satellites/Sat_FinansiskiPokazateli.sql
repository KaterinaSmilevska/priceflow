CREATE TABLE [dbo].[Sat_FinansiskiPokazateli]
(
	Izdavachi_HK CHAR(32) NOT NULL,
	Godina INT NOT NULL DEFAULT 2025,
	OperativnaDobivka DECIMAL(18, 2) NULL, 
    NetoDobivkaPoAkcija DECIMAL(18, 2) NULL, 
    KoefCenaDobivkaPoAkcija DECIMAL(18, 2) NULL, 
    KnigovodstvenaVrednostPoAkcija DECIMAL(18, 2) NULL, 
    KoefCenaKnigovodstvenaVrednostPoAkcija DECIMAL(18, 2) NULL, 
    DividendaPoAkcija DECIMAL(18, 2) NULL, 
    DividendenPrinos DECIMAL(18, 2) NULL,
	HashDiff NVARCHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	EndDate DATETIME2 NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Sat_FinansiskiPokazateli PRIMARY KEY (Izdavachi_HK, Godina, LoadDate),
	CONSTRAINT fk_Sat_FinansiskiPokazateli_Hub_Izdavachi FOREIGN KEY (Izdavachi_HK) REFERENCES Hub_Izdavachi(Izdavachi_HK)
)
