CREATE TABLE [dbo].[DnevenPromet]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [HVId] INT NOT NULL, 
    [Datum] DATE NOT NULL, 
    [CenaPoslednaTransakcija] DECIMAL NULL, 
    [MaxCena] DECIMAL NULL, 
    [MinCena] DECIMAL NULL, 
    [ProsecnaCena] DECIMAL(18, 2) NULL, 
    [ProcentPromena] DECIMAL(18, 2) NULL, 
    [KolicinaIstrguvaniAkcii] INT NULL, 
    [PrometBESTDenari] INT NULL, 
    [VkupenPrometDenari] INT NULL,

    CONSTRAINT pk_DnevenPromet PRIMARY KEY (Id),
    CONSTRAINT un_Datum UNIQUE (Datum),
    CONSTRAINT fk_DnevenPromet_HartiiOdVrednost FOREIGN KEY (HVId) REFERENCES HartiiOdVrednost(Id)
)
