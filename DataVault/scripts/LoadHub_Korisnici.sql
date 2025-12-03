INSERT INTO Hub_Korisnici
(Korisnici_HK, Username, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(k.Username))), 2) AS Korisnici_HK,
	k.Username,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].Korisnici as k
LEFT OUTER JOIN Hub_Korisnici AS hk
	ON hk.Username = k.Username
WHERE hk.Username IS NULL