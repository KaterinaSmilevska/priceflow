CREATE PROCEDURE Load_Link_HartiiOdVrednost_TipHV @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY;

			DECLARE @HartiiOdVrednost_TipHV_Result TABLE
			(
				HartiiOdVrednost_HK CHAR(32) NOT NULL,
				TipHV_HK CHAR(32) NOT NULL,
				HartiiOdVrednost_TipHV_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', 
						CONCAT(
							HartiiOdVrednost_HK,
							'|',
							TipHV_HK
						)), 2) 
				)
			);

			INSERT INTO @HartiiOdVrednost_TipHV_Result
			(
				HartiiOdVrednost_HK,
				TipHV_HK
			)
			SELECT
				hhv.HartiiOdVrednost_HK,
				hthv.TipHV_HK
			FROM [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
			INNER JOIN [$(PriceFlowDb)].[dbo].TipHV AS thv
				ON thv.Id = hv.TipHVId
			INNER JOIN [dbo].Hub_HartiiOdVrednost AS hhv
				ON hhv.HartiiOdVrednost_HK = CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(hv.Kod))), 2)
			INNER JOIN [dbo].Hub_TipHV AS hthv
				ON hthv.TipHV_HK = CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(thv.Ime))), 2)

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Link_HartiiOdVrednost_TipHV
				(
					HartiiOdVrednost_TipHV_HK,
					HartiiOdVrednost_HK, 
					TipHV_HK,
					LoadDate, 
					RecordSource
				)
				SELECT
					r.HartiiOdVrednost_TipHV_HK,
					r.HartiiOdVrednost_HK,
					r.TipHV_HK,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @HartiiOdVrednost_TipHV_Result AS r
				LEFT OUTER JOIN [dbo].Link_HartiiOdVrednost_TipHV AS lhvthv
					ON lhvthv.HartiiOdVrednost_HK = r.HartiiOdVrednost_HK
						AND lhvthv.TipHV_HK = r.TipHV_HK
				WHERE lhvthv.HartiiOdVrednost_HK IS NULL
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO