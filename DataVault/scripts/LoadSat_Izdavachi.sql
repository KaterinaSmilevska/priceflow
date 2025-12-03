; WITH SourceData AS (
	SELECT
		hi.Izdavachi_HK,
		i.Grad,
		i.Drzava,
		CONVERT(CHAR(32), HASHBYTES(
				'MD5',
				UPPER(
					TRIM(
						CONCAT(
							ISNULL(CONVERT(NVARCHAR(50), i.Grad),  ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), i.Drzava), '')
						)
					)
				)
			), 2) AS HashDiff,
			SYSUTCDATETIME() AS LoadDate,
			'PriceFlowDb' AS RecordSource
	FROM [$(PriceFlowDb)].[dbo].Izdavachi AS i
	INNER JOIN Hub_Izdavachi AS hi
		ON hi.Ime = i.Ime
)
MERGE Sat_Izdavachi AS target
USING SourceData AS source
	ON target.Izdavachi_HK = source.Izdavachi_HK
WHEN MATCHED AND target.HashDiff <> source.HashDiff AND target.EndDate IS NULL THEN
	UPDATE SET EndDate = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
	INSERT (Izdavachi_HK, Grad, Drzava, HashDiff, LoadDate, EndDate, RecordSource)
	VALUES (source.Izdavachi_HK, source.Grad, source.Drzava, source.HashDiff, source.LoadDate, NULL, source.RecordSource)
;
