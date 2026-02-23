CREATE PROCEDURE Load_Link_PortfolioPrinosi @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY;

			DECLARE @PortfolioPrinosi_Result TABLE
			(
				Portfolija_HK CHAR(32) NOT NULL,
				HartiiOdVrednost_HK CHAR(32) NOT NULL,
				PortfolioPrinosi_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5',
					CONCAT(
						Portfolija_HK, 
						'|',
						HartiiOdVrednost_HK
					)), 2)
				)
			);

			INSERT INTO @PortfolioPrinosi_Result
			(
				Portfolija_HK,
				HartiiOdVrednost_HK
			)
			SELECT
				hp.Portfolija_HK,
				hhv.HartiiOdVrednost_HK
			FROM [$(PriceFlowDb)].[dbo].PortfolioPrinosi AS pp
			INNER JOIN [$(PriceFlowDb)].[dbo].Portfolija AS p
				ON p.Id = pp.PortfolioId
			INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
				ON k.Id = p.KorisnikId
			INNER JOIN [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
				ON hv.Id = pp.HVId
			INNER JOIN [dbo].Hub_Portfolija AS hp
				ON hp.Portfolija_HK = CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(UPPER(TRIM(k.Username)), '|', UPPER(TRIM(p.Ime)))), 2)
			INNER JOIN [dbo].Hub_HartiiOdVrednost AS hhv
				ON hhv.HartiiOdVrednost_HK = CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(hv.Kod))), 2)

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Link_PortfolioPrinosi
				(
					PortfolioPrinosi_HK, 
					Portfolija_HK,
					HartiiOdVrednost_HK,
					LoadDate, 
					RecordSource
				)
				SELECT
					r.PortfolioPrinosi_HK,
					r.Portfolija_HK,
					r.HartiiOdVrednost_HK,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @PortfolioPrinosi_Result AS r
				LEFT OUTER JOIN [dbo].Link_PortfolioPrinosi AS lpp
					ON lpp.PortfolioPrinosi_HK = r.PortfolioPrinosi_HK
				WHERE lpp.PortfolioPrinosi_HK IS NULL
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO