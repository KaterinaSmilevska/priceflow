INSERT INTO Hub_TipHV
(TipHV_HK, Ime, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(thv.Ime))), 2) AS TipHV_HK,
	thv.Ime,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].TipHV AS thv
LEFT OUTER JOIN Hub_TipHV AS hthv
	ON hthv.Ime = thv.Ime
WHERE hthv.Ime IS NULL