; WITH SourceData AS (
	SELECT
		hhv.HartiiOdVrednost_HK,
		hv.ISIN,
		hv.VkupenBrojAkcii,
		CONVERT(CHAR(32), HASHBYTES(
				'MD5',
				UPPER(
					TRIM(
						CONCAT(
							ISNULL(CONVERT(NVARCHAR(50), hv.ISIN),  ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), hv.VkupenBrojAkcii), '')
						)
					)
				)
			), 2) AS HashDiff,
			SYSUTCDATETIME() AS LoadDate,
			'PriceFlowDb' AS RecordSource
	FROM [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
	INNER JOIN Hub_HartiiOdVrednost AS hhv
		ON hhv.Kod = hv.Kod
)
MERGE Sat_HartiiOdVrednost AS target
USING SourceData AS source
	ON target.HartiiOdVrednost_HK = source.HartiiOdVrednost_HK
WHEN MATCHED AND target.HashDiff <> source.HashDiff AND target.EndDate IS NULL THEN
	UPDATE SET EndDate = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
	INSERT (HartiiOdVrednost_HK, ISIN, VkupenBrojAkcii, HashDiff, LoadDate, EndDate, RecordSource)
	VALUES (source.HartiiodVrednost_HK, source.ISIN, source.VkupenBrojAkcii, source.HashDiff, source.LoadDate, NULL, source.RecordSource)
;
