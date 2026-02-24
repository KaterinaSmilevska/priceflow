CREATE PROCEDURE [dbo].Load_Sat_PortfolioPrinosi @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'PortfolioPrinosi'
			)

			DECLARE @PortfolioPrinosi_Result TABLE
			(
				Username NVARCHAR(100) NOT NULL,
				Ime NVARCHAR(50) NOT NULL,
				Kod NVARCHAR(50) NULL,
				Datum DATE NOT NULL,
				NetoIznos DECIMAL NOT NULL,
				Danok DECIMAL(18, 2) NOT NULL,
				HashDiff AS
				(
					CONVERT(CHAR(32), HASHBYTES(
						'MD5',
						UPPER(
							TRIM(
								CONCAT(
									ISNULL(CONVERT(NVARCHAR(50), Datum, 126), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), NetoIznos), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), Danok), '')
								)
							)
						)
					), 2)
				),
				PortfolioPrinosi_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5',
					CONCAT(
						CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(UPPER(TRIM(Username)), '|', UPPER(TRIM(Ime)))), 2), 
						'|',
						CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Kod))), 2)
					)), 2)
				),
				DateModified DATETIME NOT NULL
			);

			INSERT INTO @PortfolioPrinosi_Result
			(
				Username,
				Ime,
				Kod,
				Datum,
				NetoIznos,
				Danok,
				DateModified
			)
			SELECT
				k.Username,
				p.Ime,
				hv.Kod,
				pp.Datum,
				pp.NetoIznos,
				pp.Danok,
				pp.DateModified
			FROM [$(PriceFlowDb)].[dbo].PortfolioPrinosi AS pp
			INNER JOIN [$(PriceFlowDb)].[dbo].Portfolija AS p
				ON p.Id = pp.PortfolioId
			INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
				ON k.Id = p.KorisnikId
			INNER JOIN [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
				ON hv.Id = pp.HVId
			WHERE pp.DateModified > @LastLoadDate AND pp.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				UPDATE spp
				SET EndDate = r.DateModified
				FROM [dbo].Sat_PortfolioPrinosi AS spp
				INNER JOIN @PortfolioPrinosi_Result AS r
					ON r.PortfolioPrinosi_HK = spp.PortfolioPrinosi_HK
				WHERE spp.HashDiff <> r.HashDiff
					AND spp.EndDate IS NULL
				
				INSERT INTO [dbo].Sat_PortfolioPrinosi
				(
					PortfolioPrinosi_HK,
					Datum,
					NetoIznos,
					Danok,
					LoadDate,
					HashDiff,
					RecordSource
				)
				SELECT
					r.PortfolioPrinosi_HK,
					r.Datum,
					r.NetoIznos,
					r.Danok,
					SYSUTCDATETIME(),
					r.HashDiff,
					'PriceFlowDb'
				FROM @PortfolioPrinosi_Result AS r
				LEFT OUTER JOIN [dbo].Sat_PortfolioPrinosi AS spp
					ON spp.PortfolioPrinosi_HK = r.PortfolioPrinosi_HK
						AND spp.HashDiff = r.HashDiff
						AND spp.EndDate IS NULL
				WHERE spp.PortfolioPrinosi_HK IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @PortfolioPrinosi_Result
				)
				WHERE SourceTableName = 'PortfolioPrinosi'
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO