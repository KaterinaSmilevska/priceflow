INSERT INTO Link_Izdavachi_Sektori
	(Izdavachi_Sektori_HK, Izdavachi_HK, Sektori_HK, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(hi.Izdavachi_HK, '|', hs.Sektori_HK)), 2) AS Izdavachi_Sektori_HK,
	hi.Izdavachi_HK,
	hs.Sektori_HK,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].Izdavachi AS i
INNER JOIN Hub_Izdavachi AS hi
	ON hi.Ime = i.Ime
INNER JOIN [$(PriceFlowDb)].[dbo].Sektori AS s
	ON s.Id = i.SektorId
INNER JOIN Hub_Sektori AS hs
	ON hs.Ime = s.Ime
LEFT OUTER JOIN Link_Izdavachi_Sektori AS lis
	ON lis.Izdavachi_HK = hi.Izdavachi_HK
		AND lis.Sektori_HK = hs.Sektori_HK
WHERE lis.Izdavachi_HK IS NULL
		AND lis.Sektori_HK IS NULL
