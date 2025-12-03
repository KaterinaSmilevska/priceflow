INSERT INTO Link_PortfolioPrinosi
	(PortfolioPrinosi_HK, Portfolija_HK, HartiiOdVrednost_HK, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(hp.Portfolija_HK, '|', hhv.HartiiOdVrednost_HK)), 2) AS PortfolioPrinosi_HK,
	hp.Portfolija_HK,
	hhv.HartiiOdVrednost_HK,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].PortfolioPrinosi AS pp
INNER JOIN [$(PriceFlowDb)].[dbo].Portfolija AS p
	ON p.Id = pp.PortfolioId
INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
	ON k.Id = p.KorisnikId
INNER JOIN Hub_Portfolija AS hp
	ON hp.Username = k.Username
		AND hp.Ime = p.Ime
INNER JOIN [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
	ON hv.Id = pp.HVId
INNER JOIN Hub_HartiiOdVrednost AS hhv
	ON hhv.Kod = hv.Kod
LEFT OUTER JOIN Link_PortfolioPrinosi AS lpp
	ON lpp.Portfolija_HK = hp.Portfolija_HK
		AND lpp.HartiiOdVrednost_HK = hhv.HartiiOdVrednost_HK
WHERE lpp.Portfolija_HK IS NULL
		AND lpp.HartiiOdVrednost_HK IS NULL