CREATE PROCEDURE [dbo].Load_Hub_TipHV @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			
			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'TipHV'
			)

			DECLARE @TipHV_Result TABLE
			(
				Ime NVARCHAR(10) NOT NULL,
				TipHV_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Ime))), 2)
				),
				DateModified DATETIME NOT NULL
			);

			INSERT INTO @TipHV_Result
			(
				Ime,
				DateModified
			)
			SELECT
				thv.Ime,
				thv.DateModified
			FROM [$(PriceFlowDb)].[dbo].TipHV AS thv
			WHERE thv.DateModified > @LastLoadDate AND thv.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Hub_TipHV
				(
					TipHV_HK, 
					Ime,
					LoadDate,
					RecordSource
				)
				SELECT
					r.TipHV_HK,
					r.Ime,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @TipHV_Result AS r
				LEFT OUTER JOIN [dbo].Hub_TipHV AS hthv
					ON hthv.Ime = r.Ime
				WHERE hthv.Ime IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @TipHV_Result
				)
				WHERE SourceTableName = 'TipHV'
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO