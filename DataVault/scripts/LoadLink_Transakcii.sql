INSERT INTO Link_Transakcii
	(Transakcii_HK, Portfolija_HK, HartiiOdVrednost_HK, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(hp.Portfolija_HK, '|', hhv.HartiiOdVrednost_HK)), 2) AS Transakcii_Hk,
	hp.Portfolija_HK,
	hhv.HartiiOdVrednost_HK,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].Transakcii AS t
INNER JOIN [$(PriceFlowDb)].[dbo].Portfolija AS p
	ON p.Id = t.PortfolioId
INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
	ON k.Id = p.KorisnikId
INNER JOIN Hub_Portfolija as hp
	ON hp.Username = k.Username
		AND hp.Ime = p.Ime
INNER JOIN [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
	ON hv.Id = t.HVId
INNER JOIN Hub_HartiiOdVrednost AS hhv
	ON hhv.Kod = hv.Kod
LEFT OUTER JOIN Link_Transakcii AS lt
	ON lt.Portfolija_HK = hp.Portfolija_HK
		AND lt.HartiiOdVrednost_HK = hhv.HartiiOdVrednost_HK
WHERE lt.Portfolija_HK IS NULL
		AND lt.HartiiOdVrednost_HK IS NULL