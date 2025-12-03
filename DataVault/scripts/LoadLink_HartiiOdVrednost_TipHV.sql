INSERT INTO Link_HartiiOdVrednost_TipHV
	(HartiiOdVrednost_TipHV_HK, HartiiOdVrednost_HK, TipHV_HK, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(hhv.HartiiOdVrednost_HK, '|', hthv.TipHV_HK)), 2) AS HartiiOdVrednost_TipHV_HK,
	hhv.HartiiOdVrednost_HK,
	hthv.TipHV_HK,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
INNER JOIN Hub_HartiiOdVrednost AS hhv
	ON hhv.Kod = hv.Kod
INNER JOIN [$(PriceFlowDb)].[dbo].TipHV AS thv
	ON thv.Id = hv.TipHVId
INNER JOIN Hub_TipHV AS hthv
	ON hthv.Ime = thv.Ime
LEFT OUTER JOIN Link_HartiiOdVrednost_TipHV AS lhvthv
	ON lhvthv.HartiiOdVrednost_HK = hhv.HartiiOdVrednost_HK
		AND lhvthv.TipHV_HK = hthv.TipHV_HK
WHERE lhvthv.HartiiOdVrednost_HK IS NULL
		AND lhvthv.TipHV_HK IS NULL