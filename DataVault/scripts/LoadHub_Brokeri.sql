CREATE PROCEDURE [dbo].Load_Hub_Brokeri @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'Brokeri'
			)
			
			DECLARE @Brokeri_Result TABLE
			(
				Kompanija NVARCHAR(100) NOT NULL,
				Brokeri_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Kompanija))), 2)
				),
				DateModified DATETIME NOT NULL
			); 

			INSERT INTO @Brokeri_Result
			(
				Kompanija,
				DateModified
			)
			SELECT
				b.Kompanija,
				b.DateModified
			FROM [$(PriceFlowDb)].[dbo].Brokeri AS b
			WHERE b.DateModified > @LastLoadDate AND b.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Hub_Brokeri
				(
					Brokeri_HK,
					Kompanija,
					LoadDate,
					RecordSource
				)
				SELECT
					r.Brokeri_HK,
					r.Kompanija,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @Brokeri_Result AS r
				LEFT OUTER JOIN [dbo].Hub_Brokeri AS hb
					ON hb.Kompanija = r.Kompanija
				WHERE hb.Kompanija IS NULL
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO