CREATE TABLE [dbo].[Brokeri]
(
	[Id] INT IDENTITY(1, 1) NOT NULL, 
    [Kompanija] NVARCHAR(100) NOT NULL, 
    [ProcentProvizija] DECIMAL(18, 3) NOT NULL,
    [DateModified] DATETIME NOT NULL CONSTRAINT df_Brokeri_DateModified DEFAULT (SYSUTCDATETIME()),

    CONSTRAINT pk_Brokeri PRIMARY KEY (Id),
    CONSTRAINT un_Brokeri_Kompanija UNIQUE (Kompanija)
)
