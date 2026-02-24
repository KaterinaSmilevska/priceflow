CREATE PROCEDURE [dbo].Load_Sat_Portfolija @SyncDate DATETIME
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
				Opis NVARCHAR(100) NULL,
				HashDiff AS
				(
					CONVERT(CHAR(32), HASHBYTES(
						'MD5',
						UPPER(
							TRIM(
								ISNULL(CONVERT(NVARCHAR(50), Opis), '')
							)
						)
					), 2)
				),
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
				Opis,
				DateModified
			)
			SELECT
				k.Username,
				p.Ime,
				p.Opis,
				p.DateModified
			FROM [$(PriceFlowDb)].[dbo].Portfolija AS p
			INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
				ON k.Id = p.KorisnikId
			WHERE p.DateModified > @LastLoadDate AND p.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				UPDATE sp
				SET EndDate = r.DateModified
				FROM [dbo].Sat_Portfolija AS sp
				INNER JOIN @Portfolija_Result AS r
					ON r.Portfolija_HK = sp.Portfolija_HK
				WHERE sp.HashDiff <> r.HashDiff
					AND sp.EndDate IS NULL
				
				INSERT INTO [dbo].Sat_Portfolija
				(
					Portfolija_HK,
					Opis,
					LoadDate,
					HashDiff,
					RecordSource
				)
				SELECT
					r.Portfolija_HK,
					r.Opis,
					SYSUTCDATETIME(),
					r.HashDiff,
					'PriceFlowDb'
				FROM @Portfolija_Result AS r
				LEFT OUTER JOIN [dbo].Sat_Portfolija AS sp
					ON sp.Portfolija_HK = r.Portfolija_HK
						AND sp.HashDiff = r.HashDiff
						AND sp.EndDate IS NULL
				WHERE sp.Portfolija_HK IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @Portfolija_Result
				)
				WHERE SourceTableName = 'Portfolija'
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO