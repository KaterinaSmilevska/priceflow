CREATE PROCEDURE [dbo].Load_Sat_HartiiOdVrednost @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'HartiiOdVrednost'
			)

			DECLARE @HV_Result TABLE
			(
				Kod NVARCHAR(50) NOT NULL,
				ISIN NVARCHAR(12) NOT NULL,
				VkupenBrojAkcii INT NOT NULL,
				HashDiff AS
				(
					CONVERT(CHAR(32), HASHBYTES(
						'MD5',
						UPPER(
							TRIM(
								CONCAT(
									ISNULL(CONVERT(NVARCHAR(50), ISIN),  ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), VkupenBrojAkcii), '')
								)
							)
						)
					), 2)
				),
				HartiiOdVrednost_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Kod))), 2)
				),
				DateModified DATETIME NOT NULL
			);

			INSERT INTO @HV_Result
			(
				Kod,
				ISIN,
				VkupenBrojAkcii,
				DateModified
			)
			SELECT
				hv.Kod,
				hv.ISIN,
				hv.VkupenBrojAkcii,
				hv.DateModified
			FROM [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
			WHERE hv.DateModified > @LastLoadDate AND hv.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				UPDATE shv
				SET EndDate = r.DateModified
				FROM [dbo].Sat_HartiiOdVrednost AS shv
				INNER JOIN @HV_Result AS r
					ON r.HartiiOdVrednost_HK = shv.HartiiOdVrednost_HK
				WHERE shv.HashDiff <> r.HashDiff
					AND shv.EndDate IS NULL

				INSERT INTO [dbo].Sat_HartiiOdVrednost
				(
					HartiiOdVrednost_HK,
					ISIN,
					VkupenBrojAkcii,
					LoadDate,
					HashDiff,
					RecordSource
				)
				SELECT
					r.HartiiOdVrednost_HK,
					r.ISIN,
					r.VkupenBrojAkcii,
					SYSUTCDATETIME(),
					r.HashDiff,
					'PriceFlowDb'
				FROM @HV_Result AS r
				LEFT OUTER JOIN [dbo].Sat_HartiiOdVrednost AS shv
					ON shv.HartiiOdVrednost_HK = r.HartiiOdVrednost_HK
						AND shv.HashDiff = r.HashDiff
						AND shv.EndDate IS NULL
				WHERE shv.HartiiOdVrednost_HK IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @HV_Result
				)
				WHERE SourceTableName = 'HartiiOdVrednost'

			END
			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO
