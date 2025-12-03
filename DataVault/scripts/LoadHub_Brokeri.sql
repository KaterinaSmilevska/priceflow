INSERT INTO Hub_Brokeri
(Brokeri_HK, Kompanija, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(b.Kompanija))), 2) AS Brokeri_HK,
	b.Kompanija,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].Brokeri as b
LEFT OUTER JOIN Hub_Brokeri AS hb
	ON hb.Kompanija = b.Kompanija
WHERE hb.Kompanija IS NULL