CREATE PROCEDURE [dbo].Load_Sat_Brokeri @SyncDate DATETIME
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
				ProcentProvizija DECIMAL(18, 3) NOT NULL,
				HashDiff AS
				(
					CONVERT(CHAR(32), HASHBYTES(
							'MD5',
							UPPER(
								TRIM(
									ISNULL(CONVERT(NVARCHAR(50), ProcentProvizija),  '')
								)
							)
						), 2)	
				),
				Brokeri_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Kompanija))), 2)
				),
				DateModified DATETIME NOT NULL
			); 

			INSERT INTO @Brokeri_Result
			(
				Kompanija,
				ProcentProvizija,
				DateModified
			)
			SELECT
				b.Kompanija,
				b.ProcentProvizija,
				b.DateModified
			FROM [$(PriceFlowDb)].[dbo].Brokeri AS b
			WHERE b.DateModified > @LastLoadDate AND b.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				UPDATE sb
				SET EndDate = r.DateModified
				FROM [dbo].Sat_Brokeri AS sb
				INNER JOIN @Brokeri_Result AS r
					ON r.Brokeri_HK = sb.Brokeri_HK
				WHERE sb.HashDiff <> r.HashDiff
						AND sb.EndDate IS NULL

				INSERT INTO [dbo].Sat_Brokeri
				(
					Brokeri_HK,
					ProcentProvizija,
					LoadDate,
					HashDiff,
					RecordSource
				)
				SELECT
					r.Brokeri_HK,
					r.ProcentProvizija,
					SYSUTCDATETIME(),
					r.HashDiff,
					'PriceFlowDb'
				FROM @Brokeri_Result AS r
				LEFT OUTER JOIN [dbo].Sat_Brokeri AS sb
					ON sb.Brokeri_HK = r.Brokeri_HK
						AND sb.HashDiff = r.HashDiff
						AND sb.EndDate IS NULL
				WHERE sb.Brokeri_HK IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @Brokeri_Result
				)
				WHERE SourceTableName = 'Brokeri'
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO