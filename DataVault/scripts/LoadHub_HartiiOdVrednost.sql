INSERT INTO Hub_HartiiOdVrednost
(HartiiOdVrednost_HK, Kod, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(hv.Kod))), 2) AS HartiiOdVrednost_HK,
	hv.Kod,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].HartiiOdVrednost as hv
LEFT OUTER JOIN Hub_HartiiOdVrednost AS hhv
	ON hhv.Kod = hv.Kod
WHERE hhv.Kod IS NULL