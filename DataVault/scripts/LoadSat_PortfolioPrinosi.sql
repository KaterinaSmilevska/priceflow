;WITH SourceData AS (
		SELECT
			lpp.PortfolioPrinosi_HK,
			pp.Datum,
			pp.NetoIznos,
			pp.Danok,
			CONVERT(CHAR(32), HASHBYTES(
				'MD5',
				UPPER(
					TRIM(
						CONCAT(
							ISNULL(CONVERT(NVARCHAR(50), pp.Datum, 126), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), pp.NetoIznos), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), pp.Danok), '')
						)
					)
				)
			), 2) AS HashDiff,
			SYSUTCDATETIME() AS LoadDate,
			'PriceFlowDb' AS RecordSource
			FROM [$(PriceFlowDb)].[dbo].PortfolioPrinosi AS pp
			INNER JOIN [$(PriceFlowDb)].[dbo].Portfolija AS p
				ON p.Id = pp.PortfolioId
			INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
				ON k.Id = p.KorisnikId
			INNER JOIN Hub_Portfolija AS hp
				ON hp.Username = k.Username
					AND hp.Ime = p.Ime
			INNER JOIN [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
				ON hv.Id = pp.HVId
			INNER JOIN Hub_HartiiOdVrednost AS hhv
				ON hhv.Kod = hv.Kod
			INNER JOIN Link_PortfolioPrinosi AS lpp
				ON lpp.Portfolija_HK = hp.Portfolija_HK
					AND lpp.HartiiOdVrednost_HK = hhv.HartiiOdVrednost_HK
)

MERGE Sat_PortfolioPrinosi AS target
USING SourceData AS source
	ON target.PortfolioPrinosi_HK = source.PortfolioPrinosi_HK
WHEN MATCHED AND target.HashDiff <> source.HashDiff AND target.EndDate IS NULL THEN
	UPDATE SET EndDate = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
	INSERT (PortfolioPrinosi_HK, Datum, NetoIznos, Danok, HashDiff, LoadDate,EndDate, RecordSource)
	VALUES (source.PortfolioPrinosi_HK, source.Datum, source.NetoIznos, source.Danok,
	source.HashDiff, source.LoadDate, NULL, source.RecordSource)
;