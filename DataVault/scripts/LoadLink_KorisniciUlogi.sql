INSERT INTO Link_KorisniciUlogi
	(KorisniciUlogi_HK, Korisnici_HK, Ulogi_HK, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(hk.Korisnici_HK, '|', hu.Ulogi_HK)), 2) AS KorisniciUlogi_HK,
	hk.Korisnici_HK,
	hu.Ulogi_HK,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].KorisniciUlogi AS ku
INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
	ON k.Id = ku.KorisnikId
INNER JOIN Hub_Korisnici AS hk
	ON hk.Username = k.Username
INNER JOIN [$(PriceFlowDb)].[dbo].Ulogi as u
	ON u.Id = ku.UlogaId
INNER JOIN Hub_Ulogi AS hu
	ON hu.Ime = u.Ime
LEFT OUTER JOIN Link_KorisniciUlogi as lku
	ON lku.Korisnici_HK = hk.Korisnici_HK
		AND lku.Ulogi_HK = hu.Ulogi_HK
WHERE lku.Korisnici_HK IS NULL
		AND lku.Ulogi_HK IS NULL