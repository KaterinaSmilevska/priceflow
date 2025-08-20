CREATE TABLE [dbo].[AplikativniParametri]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [PersonalenDanok] DECIMAL(18, 2) NOT NULL, 
    [BerzanskaProvizija] DECIMAL(18, 2) NOT NULL, 
    [CDHVProvizija] DECIMAL(18, 2) NOT NULL,

    CONSTRAINT pk_AplikativniParametri PRIMARY KEY (Id),
    CONSTRAINT ck_AplikativniParametri_Id CHECK (Id=1)
)
