;WITH SourceData AS (
		SELECT
			hb.Brokeri_HK,
			b.ProcentProvizija,
			CONVERT(CHAR(32), HASHBYTES(
				'MD5',
				UPPER(
					TRIM(
						CONCAT(
							ISNULL(CONVERT(NVARCHAR(50), b.ProcentProvizija), '')
						)
					)
				)
			), 2) AS HashDiff,
			SYSUTCDATETIME() AS LoadDate,
			'PriceFlowDb' AS RecordSource
			FROM [$(PriceFlowDb)].[dbo].Brokeri AS b
			INNER JOIN Hub_Brokeri AS hb
				ON hb.Kompanija = b.Kompanija
)

MERGE Sat_Brokeri AS target
USING SourceData AS source
	ON target.Brokeri_HK = source.Brokeri_HK
WHEN MATCHED AND target.HashDiff <> source.HashDiff AND target.EndDate IS NULL THEN
	UPDATE SET EndDate = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
	INSERT (Brokeri_HK, ProcentProvizija, HashDiff, LoadDate,EndDate, RecordSource)
	VALUES (source.Brokeri_HK, source.ProcentProvizija, source.HashDiff, source.LoadDate, NULL, source.RecordSource)
;