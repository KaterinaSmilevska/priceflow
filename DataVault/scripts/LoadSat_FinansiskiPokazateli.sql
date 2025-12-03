;WITH SourceData AS (
		SELECT
			hi.Izdavachi_HK,
			fp.Godina,
			fp.OperativnaDobivka,
			fp.NetoDobivkaPoAkcija,
			fp.KoefCenaDobivkaPoAkcija,
			fp.KnigovodstvenaVrednostPoAkcija,
			fp.KoefCenaKnigovodstvenaVrednostPoAkcija,
			fp.DividendaPoAkcija,
			fp.DividendenPrinos,
			CONVERT(CHAR(32), HASHBYTES(
				'MD5',
				UPPER(
					TRIM(
						CONCAT(
							ISNULL(CONVERT(NVARCHAR(50), fp.Godina), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), fp.OperativnaDobivka), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), fp.NetoDobivkaPoAkcija), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), fp.KoefCenaDobivkaPoAkcija), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), fp.KnigovodstvenaVrednostPoAkcija), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), fp.KoefCenaKnigovodstvenaVrednostPoAkcija), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), fp.DividendaPoAkcija), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), fp.DividendenPrinos), '')
						)
					)
				)
			), 2) AS HashDiff,
			SYSUTCDATETIME() AS LoadDate,
			'PriceFlowDb' AS RecordSource
			FROM [$(PriceFlowDb)].[dbo].FinansiskiPokazateli AS fp
			INNER JOIN [$(PriceFlowDb)].[dbo].Izdavachi AS i
				ON i.Id = fp.IzdavachId
			INNER join Hub_Izdavachi AS hi
				ON hi.Ime = i.Ime
)

MERGE Sat_FinansiskiPokazateli AS target
USING SourceData AS source
	ON target.Izdavachi_HK = source.Izdavachi_HK
		AND target.Godina = source.Godina
WHEN MATCHED AND target.HashDiff <> source.HashDiff AND target.EndDate IS NULL THEN
	UPDATE SET EndDate = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
	INSERT (
			Izdavachi_HK,
			Godina,
			OperativnaDobivka,
			NetoDobivkaPoAkcija,
			KoefCenaDobivkaPoAkcija,
			KnigovodstvenaVrednostPoAkcija,
			KoefCenaKnigovodstvenaVrednostPoAkcija,
			DividendaPoAkcija,
			DividendenPrinos,
			HashDiff,
			LoadDate,
			EndDate, 
			RecordSource
	)
	VALUES (
			source.Izdavachi_HK, 
			source.Godina,
			source.OperativnaDobivka,
			source.NetoDobivkaPoAkcija,
			source.KoefCenaDobivkaPoAkcija,
			source.KnigovodstvenaVrednostPoAkcija,
			source.KoefCenaKnigovodstvenaVrednostPoAkcija,
			source.DividendaPoAkcija,
			source.DividendenPrinos,
			source.HashDiff, 
			source.LoadDate, 
			NULL, 
			source.RecordSource
	)
;