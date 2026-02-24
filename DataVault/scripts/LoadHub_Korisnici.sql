CREATE PROCEDURE [dbo].Load_Hub_Korisnici @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'Korisnici'
			)

			DECLARE @Korisnici_Result TABLE
			(
				Username NVARCHAR(100) NOT NULL,
				Korisnici_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Username))), 2)
				),
				DateModified DATETIME NOT NULL
			);

			INSERT INTO @Korisnici_Result
			(
				Username,
				DateModified
			)
			SELECT
				k.Username,
				k.DateModified
			FROM [$(PriceFlowDb)].[dbo].Korisnici AS k
			WHERE k.DateModified > @LastLoadDate AND k.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Hub_Korisnici
				(
					Korisnici_HK, 
					Username, 
					LoadDate,
					RecordSource
				)
				SELECT
					r.Korisnici_HK,
					r.Username,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @Korisnici_Result AS r
				LEFT OUTER JOIN [dbo].Hub_Korisnici AS hk
					ON hk.Username = r.Username
				WHERE hk.Username IS NULL
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO