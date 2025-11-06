CREATE TABLE [dbo].[FinansiskiPokazateli]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [IzdavachId] INT NOT NULL,
    [Godina] INT NULL, 
    [OperativnaDobivka] DECIMAL(18, 2) NULL, 
    [NetoDobivkaPoAkcija] DECIMAL(18, 2) NULL, 
    [KoefCenaDobivkaPoAkcija] DECIMAL(18, 2) NULL, 
    [KnigovodstvenaVrednostPoAkcija] DECIMAL(18, 2) NULL, 
    [KoefCenaKnigovodstvenaVrednostPoAkcija] DECIMAL(18, 2) NULL, 
    [DividendaPoAkcija] DECIMAL(18, 2) NULL, 
    [DividendenPrinos] DECIMAL(18, 2) NULL, 

    CONSTRAINT pk_FinansiskiPokazateli PRIMARY KEY (Id),
    CONSTRAINT fk_FinansiskiPokazateli_Izdavachi FOREIGN KEY (IzdavachId) REFERENCES Izdavachi(Id)
)
