CREATE PROCEDURE [dbo].Load_Hub_Portfolija @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'Portfolija'
			)

			DECLARE @Portfolija_Result TABLE
			(
				Username NVARCHAR(100) NOT NULL,
				Ime NVARCHAR(50) NOT NULL,
				Portfolija_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(UPPER(TRIM(Username)), '|', UPPER(TRIM(Ime)))), 2) 
				),
				DateModified DATETIME NOT NULL
			);

			INSERT INTO @Portfolija_Result
			(
				Username,
				Ime,
				DateModified
			)
			SELECT
				k.Username,
				p.Ime,
				p.DateModified
			FROM [$(PriceFlowDb)].[dbo].Portfolija AS p
			INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
				ON k.Id = p.KorisnikId
			WHERE p.DateModified > @LastLoadDate AND p.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Hub_Portfolija
				(
					Portfolija_HK, 
					Username, 
					Ime, 
					LoadDate,
					RecordSource
				)
				SELECT
					r.Portfolija_HK,
					r.Username,
					r.Ime,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @Portfolija_Result AS r
				LEFT OUTER JOIN [dbo].Hub_Portfolija AS hp
					ON hp.Username = r.Username
						AND hp.Ime = r.Ime
				WHERE hp.Username IS NULL
						AND hp.Ime IS NULL
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO