INSERT INTO Hub_Ulogi
(Ulogi_HK, Ime, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(u.Ime))), 2) AS Ulogi_HK,
	u.Ime,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].Ulogi as u
LEFT OUTER JOIN Hub_Ulogi AS hu
	ON hu.Ime = u.Ime
WHERE hu.Ime IS NULL