CREATE TABLE [dbo].[DnevenPromet]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [HVId] INT NOT NULL, 
    [Datum] DATETIME NOT NULL, 
    [CenaPoslednaTransakcija] DECIMAL(18, 2) NULL, 
    [MaxCena] DECIMAL(18, 2) NULL, 
    [MinCena] DECIMAL(18, 2) NULL, 
    [ProsecnaCena] DECIMAL(18, 2) NULL, 
    [ProcentPromena] DECIMAL(18, 2) NULL, 
    [KolicinaIstrguvaniAkcii] INT NULL, 
    [PrometBESTDenari] INT NULL, 
    [VkupenPrometDenari] INT NULL,

    CONSTRAINT pk_DnevenPromet PRIMARY KEY (Id),
    CONSTRAINT un_DnevenPromet_Datum_HVId UNIQUE (Datum, HVId),
    CONSTRAINT fk_DnevenPromet_HartiiOdVrednost FOREIGN KEY (HVId) REFERENCES HartiiOdVrednost(Id)
)
