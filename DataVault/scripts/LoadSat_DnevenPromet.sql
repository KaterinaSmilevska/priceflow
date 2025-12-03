;WITH SourceData AS (
		SELECT
			hhv.HartiiOdVrednost_HK,
			dp.Datum,
			dp.CenaPoslednaTransakcija,
			dp.MaxCena,
			dp.MinCena,
			dp.ProsecnaCena,
			dp.ProcentPromena,
			dp.KolicinaIstrguvaniAkcii,
			dp.PrometBESTDenari,
			dp.VkupenPrometDenari,
			CONVERT(CHAR(32), HASHBYTES(
				'MD5',
				UPPER(
					TRIM(
						CONCAT(
							ISNULL(CONVERT(NVARCHAR(50), dp.Datum, 126), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), dp.CenaPoslednaTransakcija), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), dp.MaxCena), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), dp.MinCena), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), dp.ProsecnaCena), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), dp.ProcentPromena), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), dp.KolicinaIstrguvaniAkcii), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), dp.PrometBESTDenari), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), dp.VkupenPrometDenari), '')
						)
					)
				)
			), 2) AS HashDiff,
			SYSUTCDATETIME() AS LoadDate,
			'PriceFlowDb' AS RecordSource
			FROM [$(PriceFlowDb)].[dbo].DnevenPromet AS dp
			INNER JOIN [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
				ON hv.Id = dp.HVId
			INNER join Hub_HartiiOdVrednost AS hhv
				ON hhv.Kod = hv.Kod
)

MERGE Sat_DnevenPromet AS target
USING SourceData AS source
	ON target.HartiiOdVrednost_HK = source.HartiiOdVrednost_HK
		AND target.Datum = source.Datum
WHEN MATCHED AND target.HashDiff <> source.HashDiff AND target.EndDate IS NULL THEN
	UPDATE SET EndDate = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
	INSERT (
			HartiiOdVrednost_HK,
			Datum,
			CenaPoslednaTransakcija,
			MaxCena,
			MinCena,
			ProsecnaCena,
			ProcentPromena,
			KolicinaIstrguvaniAkcii,
			PrometBESTDenari,
			VkupenPrometDenari,
			HashDiff,
			LoadDate,
			EndDate, 
			RecordSource
	)
	VALUES (
			source.HartiiOdVrednost_HK, 
			source.Datum,
			source.CenaPoslednaTransakcija,
			source.MaxCena,
			source.MinCena,
			source.ProsecnaCena,
			source.ProcentPromena,
			source.KolicinaIstrguvaniAkcii,
			source.PrometBESTDenari,
			source.VkupenPrometDenari,
			source.HashDiff, 
			source.LoadDate, 
			NULL, 
			source.RecordSource
	)
;