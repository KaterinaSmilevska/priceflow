INSERT INTO Hub_Izdavachi
(Izdavachi_HK, Ime, LoadDate, RecordSource)
SELECT
	CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(i.Ime))), 2) AS Izdavachi_HK,
	i.Ime,
	SYSUTCDATETIME(),
	'PriceFlowDb'
FROM [$(PriceFlowDb)].[dbo].Izdavachi AS i
LEFT OUTER JOIN Hub_Izdavachi AS hi
	ON hi.Ime = i.Ime
WHERE hi.Ime IS NULL