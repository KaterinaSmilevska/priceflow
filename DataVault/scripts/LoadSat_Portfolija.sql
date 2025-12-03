;WITH SourceData AS (
		SELECT
			hp.Portfolija_HK,
			p.Opis,
			CONVERT(CHAR(32), HASHBYTES(
				'MD5',
				UPPER(
					TRIM(
						CONCAT(
							ISNULL(CONVERT(NVARCHAR(50), p.Opis), '')
						)
					)
				)
			), 2) AS HashDiff,
			SYSUTCDATETIME() AS LoadDate,
			'PriceFlowDb' AS RecordSource
			FROM [$(PriceFlowDb)].[dbo].Portfolija AS p
			INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
				ON k.Id = p.KorisnikId
			INNER JOIN Hub_Portfolija AS hp
				ON (hp.Username = k.Username AND hp.Ime = p.Ime)
)

MERGE Sat_Portfolija AS target
USING SourceData AS source
	ON target.Portfolija_HK = source.Portfolija_HK
WHEN MATCHED AND target.HashDiff <> source.HashDiff AND target.EndDate IS NULL THEN
	UPDATE SET EndDate = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
	INSERT (Portfolija_HK, Opis, HashDiff, LoadDate,EndDate, RecordSource)
	VALUES (source.Portfolija_HK, source.Opis, source.HashDiff, source.LoadDate, NULL, source.RecordSource)
;