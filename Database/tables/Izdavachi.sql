CREATE TABLE [dbo].[Izdavachi]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [Ime] NVARCHAR(100) NOT NULL, 
    [Grad] NVARCHAR(100) NOT NULL, 
    [Drzava] NVARCHAR(100) NOT NULL,
    [SektorId] INT NOT NULL,
    [DateModified] DATETIME NOT NULL CONSTRAINT df_Izdavachi_DateModified DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT pk_Izdavachi PRIMARY KEY (Id),
    CONSTRAINT un_Izdavachi_Ime UNIQUE (Ime),
    CONSTRAINT fk_Izdavachi_Sektori FOREIGN KEY (SektorId) references Sektori(Id) ON DELETE CASCADE
)
