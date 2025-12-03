;WITH SourceData AS (
		SELECT
			lt.Transakcii_HK,
			t.KolicinaAkcii,
			t.EdinecnaCenaAkcija,
			t.Datum,
			t.Iznos,
			t.BerzanskaProvizija,
			t.BrokerskaProvizija,
			t.CDHVProvizija,
			t.TipTransakcija,
			t.Realna,
			CONVERT(CHAR(32), HASHBYTES(
				'MD5',
				UPPER(
					TRIM(
						CONCAT(
							ISNULL(CONVERT(NVARCHAR(50), t.KolicinaAkcii), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), t.EdinecnaCenaAkcija), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), t.Datum, 126), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), t.Iznos), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), t.BerzanskaProvizija), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), t.BrokerskaProvizija), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), t.CDHVProvizija), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), t.TipTransakcija, 126), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), t.Realna, 126), '')
						)
					)
				)
			), 2) AS HashDiff,
			SYSUTCDATETIME() AS LoadDate,
			'PriceFlowDb' AS RecordSource
			FROM [$(PriceFlowDb)].[dbo].Transakcii AS t
			INNER JOIN [$(PriceFlowDb)].[dbo].Portfolija AS p
				ON p.Id = t.PortfolioId
			INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
				ON k.Id = p.KorisnikId
			INNER JOIN Hub_Portfolija AS hp
				ON hp.Username = k.Username
					AND hp.Ime = p.Ime
			INNER JOIN [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
				ON hv.Id = pp.HVId
			INNER JOIN Hub_HartiiOdVrednost AS hhv
				ON hhv.Kod = hv.Kod
			INNER JOIN Link_Transakcii AS lt
				ON lt.Portfolija_HK = hp.Portfolija_HK
					AND lt.HartiiOdVrednost_HK = hhv.HartiiOdVrednost_HK
)

MERGE Sat_Transakcii AS target
USING SourceData AS source
	ON target.Transakcii_HK = source.Transakcii_HK
WHEN MATCHED AND target.HashDiff <> source.HashDiff AND target.EndDate IS NULL THEN
	UPDATE SET EndDate = SYSUTCDATETIME()
WHEN NOT MATCHED THEN
	INSERT (Transakcii_HK, 
			t.KolicinaAkcii, 
			t.EdinecnaCenaAkcija, 
			t.Datum, 
			t.Iznos, 
			t.BerzanskaProvizija, 
			t.BrokerskaProvizija, 
			t.CDHVProvizija, 
			t.TipTransakcija, 
			t.Realna, 
			HashDiff, 
			LoadDate, 
			EndDate, 
			RecordSource)
	VALUES (source.Transakcii_HK,
			source.KolicinaAkcii,
			source.EdinecnaCenaAkcija,
			source.Datum,
			source.Iznos,
			source.BerzanskaProvizija,
			source.BrokerskaProvizija,
			source.CDHVProvizija,
			source.TipTransakcija,
			source.Realna,
			source.HashDiff, 
			source.LoadDate, 
			NULL, 
			source.RecordSource)
;