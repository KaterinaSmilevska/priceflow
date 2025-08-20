CREATE TABLE [dbo].[Izdavach]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [Ime] NVARCHAR(100) NOT NULL, 
    [Grad] NVARCHAR(100) NOT NULL, 
    [Drzava] NVARCHAR(100) NOT NULL,
    [SektorId] INT NOT NULL,

    CONSTRAINT pk_Izdavach PRIMARY KEY (Id),
    CONSTRAINT un_Izdavach_Ime UNIQUE (Ime),
    CONSTRAINT fk_Izdavach_Sektor FOREIGN KEY (SektorId) references Sektor(Id)
)
