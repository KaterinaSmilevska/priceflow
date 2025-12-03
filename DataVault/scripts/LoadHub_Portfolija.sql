INSERT INTO Hub_Portfolija
(Portfolija_HK, Username, Ime, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(UPPER(TRIM(k.Username)), '|', UPPER(TRIM(p.Ime)))), 2) AS Portfolija_HK,
	k.Username,
	p.Ime,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].Portfolija AS p
INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
	ON k.Id = p.KorisnikId
LEFT OUTER JOIN Hub_Portfolija AS hp
	ON hp.Username = k.Username
		AND hp.Ime = p.Ime
WHERE hp.Username IS NULL
		AND hp.Ime IS NULL