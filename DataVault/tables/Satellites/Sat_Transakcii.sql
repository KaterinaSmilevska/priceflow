CREATE TABLE [dbo].[Sat_Transakcii]
(
	Transakcii_HK CHAR(32) NOT NULL,
	KolicinaAkcii INT NOT NULL, 
    EdinecnaCenaAkcija INT NOT NULL, 
    Datum DATE NOT NULL, 
    Iznos DECIMAL(18,2) NOT NULL, 
    BerzanskaProvizija DECIMAL(18, 2) NOT NULL, 
    BrokerskaProvizija DECIMAL(18, 2) NOT NULL, 
    CDHVProvizija DECIMAL(18, 2) NOT NULL, 
    TipTransakcija NVARCHAR(20) NOT NULL, 
    Realna NVARCHAR(2) NOT NULL,
    HashDiff NVARCHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	EndDate DATETIME2 NULL,
	RecordSource NVARCHAR(50) NOT NULL,

    CONSTRAINT pk_Sat_Transakcii PRIMARY KEY (Transakcii_HK, LoadDate),
    CONSTRAINT fk_Sat_Transakcii_Link_Transakcii FOREIGN KEY (Transakcii_HK) REFERENCES Link_Transakcii(Transakcii_HK)

)
