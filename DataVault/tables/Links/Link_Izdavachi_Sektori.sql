CREATE TABLE [dbo].[Link_Izdavachi_Sektori]
(
	Izdavachi_Sektori_HK CHAR(32) NOT NULL,
	Izdavachi_HK CHAR(32) NOT NULL,
	Sektori_HK CHAR(32) NOT NULL,
	LoadDate DATETIME2 NOT NULL,
	RecordSource NVARCHAR(50) NOT NULL,

	CONSTRAINT pk_Link_Izdavachi_Sektori PRIMARY KEY (Izdavachi_Sektori_HK),
	CONSTRAINT fk_Link_Izdavachi_Sektori_Hub_Izdavachi FOREIGN KEY (Izdavachi_HK) REFERENCES Hub_Izdavachi(Izdavachi_HK),
	CONSTRAINT fk_Link_Izdavachi_Sektori_Hub_Sektori FOREIGN KEY (Sektori_HK) REFERENCES Hub_Sektori(Sektori_HK)
)
