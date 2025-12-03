INSERT INTO Hub_Sektori
(Sektori_HK, Ime, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(s.Ime))), 2) AS Sektori_HK,
	s.Ime,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].Sektori AS s
LEFT OUTER JOIN Hub_Sektori AS hs
	ON hs.Ime = s.Ime
WHERE hs.Ime IS NULL