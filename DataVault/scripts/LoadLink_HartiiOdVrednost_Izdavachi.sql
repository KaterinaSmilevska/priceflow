INSERT INTO Link_HartiiOdVrednost_Izdavachi
	(HartiiOdVrednost_Izdavachi_HK, HartiiOdVrednost_HK, Izdavachi_HK, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(hhv.HartiiOdVrednost_HK, '|', hi.Izdavachi_HK)), 2) AS HartiiOdVrednost_Izdavachi_HK,
	hhv.HartiiOdVrednost_HK,
	hi.Izdavachi_HK,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
INNER JOIN Hub_HartiiOdVrednost AS hhv
	ON hhv.Kod = hv.Kod
INNER JOIN [$(PriceFlowDb)].[dbo].Izdavachi AS i
	ON i.Id = hv.IzdavachId
INNER JOIN Hub_Izdavachi AS hi
	ON hi.Ime = i.Ime
LEFT OUTER JOIN Link_HartiiOdVrednost_Izdavachi AS lhvi
	ON lhvi.HartiiOdVrednost_HK = hhv.HartiiOdVrednost_HK
		AND lhvi.Izdavachi_HK = hi.Izdavachi_HK
WHERE lhvi.HartiiOdVrednost_HK IS NULL
		AND lhvi.Izdavachi_HK IS NULL