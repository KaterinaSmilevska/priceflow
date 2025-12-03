; WITH SourceData AS (
	SELECT
		hk.Korisnici_HK,
		k.Ime,
		k.Email,
		CONVERT(CHAR(32), HASHBYTES(
				'MD5',
				UPPER(
					TRIM(
						CONCAT(
							ISNULL(CONVERT(NVARCHAR(50), k.Ime),  ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), k.Email), '')
						)
					)
				)
			), 2) AS HashDiff,
			SYSUTCDATETIME() AS LoadDate,
			'PriceFlowDb' AS RecordSource
	FROM [$(PriceFlowDb)].[dbo].Korisnici AS k
	INNER JOIN Hub_Korisnici AS hk
		ON hk.Username = k.Username
)
MERGE Sat_Korisnici AS target
USING SourceData AS source
	ON target.Korisnici_HK = source.Korisnici_HK
WHEN MATCHED AND target.HashDiff <> source.HashDiff AND target.EndDate IS NULL THEN
	UPDATE SET EndDate = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
	INSERT (Korisnici_HK, Ime, Email, HashDiff, LoadDate, EndDate, RecordSource)
	VALUES (source.Korisnici_HK, source.Ime, source.Email, source.HashDiff, source.LoadDate, NULL, source.RecordSource)
;