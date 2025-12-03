;WITH SourceData AS (
		SELECT
			CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT('AP-', CONVERT(NVARCHAR(10), ap.Id))), 2) AS AplikativniParametri_HK,
			ap.Id AS APId,
			ap.PersonalenDanok,
			ap.BerzanskaProvizija,
			ap.CDHVProvizija,
			CONVERT(CHAR(32), HASHBYTES(
				'MD5',
				UPPER(
					TRIM(
						CONCAT(
							ISNULL(CONVERT(NVARCHAR(50), ap.PersonalenDanok), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), ap.BerzanskaProvizija), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), ap.CDHVProvizija), '')
						)
					)
				)
			), 2) AS HashDiff,
			SYSUTCDATETIME() AS LoadDate,
			'PriceFlowDb' AS RecordSource
			FROM [$(PriceFlowDb)].[dbo].AplikativniParametri AS ap
)

MERGE Ref_AplikativniParametri AS target
USING SourceData AS source
	ON target.AplikativniParametri_HK = source.AplikativniParametri_HK
WHEN MATCHED AND target.HashDiff <> source.HashDiff AND target.EndDate IS NULL THEN
	UPDATE SET target.EndDate = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
	INSERT (
		AplikativniParametri_HK,
		APId,
		PersonalenDanok,
		BerzanskaProvizija, 
		CDHVProvizija, 
		HashDiff, 
		LoadDate,
		EndDate,
		RecordSource
	)
	VALUES (
		source.AplikativniParametri_HK,
		source.APId,
		source.PersonalenDanok,
		source.BerzanskaProvizija, 
		source.CDHVProvizija, 
		source.HashDiff, 
		source.LoadDate,
		NULL,
		source.RecordSource
	)
;