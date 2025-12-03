CREATE TABLE [dbo].[Ref_AplikativniParametri]
(
    AplikativniParametri_HK CHAR(32) NOT NULL,
    APId INT NOT NULL,
	[PersonalenDanok] DECIMAL(18, 2) NOT NULL, 
    [BerzanskaProvizija] DECIMAL(18, 2) NOT NULL, 
    [CDHVProvizija] DECIMAL(18, 2) NOT NULL,
    HashDiff NVARCHAR(32) NOT NULL,
    LoadDate DATETIME2 NOT NULL,
    EndDate DATETIME2 NULL,
    RecordSource NVARCHAR(50) NOT NULL,

    CONSTRAINT pk_Ref_AplikativniParametri PRIMARY KEY (AplikativniParametri_HK, LoadDate)
)
