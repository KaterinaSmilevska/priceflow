CREATE PROCEDURE ExecuteAll
AS
BEGIN
	DECLARE @SyncDate DATETIME = SYSUTCDATETIME()

	EXECUTE [dbo].Load_Hub_Brokeri @SyncDate
	EXECUTE [dbo].Load_Hub_TipHV @SyncDate
	EXECUTE [dbo].Load_Hub_Sektori @SyncDate
	EXECUTE [dbo].Load_Hub_Izdavachi @SyncDate
	EXECUTE [dbo].Load_Hub_Ulogi @SyncDate
	EXECUTE [dbo].Load_Hub_Korisnici @SyncDate
	EXECUTE [dbo].Load_Hub_HartiiOdVrednost @SyncDate
	EXECUTE [dbo].Load_Hub_Portfolija @SyncDate

	EXECUTE [dbo].Load_Link_HartiiOdVrednost_Izdavachi @SyncDate
	EXECUTE [dbo].Load_Link_HartiiOdVrednost_TipHV @SyncDate
	EXECUTE [dbo].Load_Link_Izdavachi_Sektori @SyncDate
	EXECUTE [dbo].Load_Link_Portfolija_Korisnici @SyncDate
	EXECUTE [dbo].Load_Link_KorisniciUlogi @SyncDate
	EXECUTE [dbo].Load_Link_PortfolioPrinosi @SyncDate
	EXECUTE [dbo].Load_Link_Transakcii @SyncDate

	EXECUTE [dbo].Load_Sat_Brokeri @SyncDate
	EXECUTE [dbo].Load_Sat_Izdavachi @SyncDate
	EXECUTE [dbo].Load_Sat_FinansiskiPokazateli @SyncDate
	EXECUTE [dbo].Load_Sat_Korisnici @SyncDate
	EXECUTE [dbo].Load_Sat_HartiiOdVrednost @SyncDate
	EXECUTE [dbo].Load_Sat_DnevenPromet @SyncDate
	EXECUTE [dbo].Load_Sat_Portfolija @SyncDate
	EXECUTE [dbo].Load_Sat_PortfolioPrinosi @SyncDate
	EXECUTE [dbo].Load_Sat_Transakcii @SyncDate

	EXECUTE [dbo].Load_Ref_AplikativniParametri @SyncDate
END
GO