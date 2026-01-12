CREATE PROCEDURE [dbo].Load_Hub_HartiiOdVrednost @SyncDate DATETIME
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
				HartiiOdVrednost_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Kod))), 2)
				),
				DateModified DATETIME NOT NULL
			);


			INSERT INTO @HV_Result
			(
				Kod,
				DateModified
			)
			SELECT
				hv.Kod,
				hv.DateModified
			FROM [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
			WHERE hv.DateModified > @LastLoadDate AND hv.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Hub_HartiiOdVrednost
				(
					HartiiOdVrednost_HK,
					Kod, 
					LoadDate,
					RecordSource
				)
				SELECT
					r.HartiiOdVrednost_HK,
					r.Kod,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @HV_Result AS r	
				LEFT OUTER JOIN [dbo].Hub_HartiiOdVrednost AS hhv
					ON hhv.Kod = r.Kod
				WHERE hhv.Kod IS NULL
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO
