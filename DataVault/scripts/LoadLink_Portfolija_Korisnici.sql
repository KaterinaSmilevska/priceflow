INSERT INTO Link_Portfolija_Korisnici
	(Portfolija_Korisnici_HK, Portfolija_HK, Korisnici_HK, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(hp.Portfolija_HK, '|', hk.Korisnici_HK)), 2) AS Portfolija_Korisnici_HK,
	hp.Portfolija_HK,
	hk.Korisnici_HK,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].Portfolija AS p
INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
	ON k.Id = p.KorisnikId
INNER JOIN Hub_Portfolija AS hp
	ON hp.Username = k.Username
		AND hp.Ime = p.Ime
INNER JOIN Hub_Korisnici AS hk
	ON hk.Username = k.Username
LEFT OUTER JOIN Link_Portfolija_Korisnici AS lpk
	ON lpk.Portfolija_HK = hp.Portfolija_HK
		AND lpk.Korisnici_HK = hk.Korisnici_HK
WHERE lpk.Portfolija_HK IS NULL
		AND lpk.Korisnici_HK IS NULL