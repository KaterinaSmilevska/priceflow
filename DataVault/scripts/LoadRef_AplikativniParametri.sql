CREATE PROCEDURE [dbo].Load_Ref_AplikativniParametri @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY
		
			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'AplikativniParametri'
			)

			DECLARE @AP_Result TABLE
			(
				Id INT NOT NULL,
				PersonalenDanok DECIMAL(18, 2) NOT NULL, 
				BerzanskaProvizija DECIMAL(18, 2) NOT NULL, 
				CDHVProvizija DECIMAL(18, 2) NOT NULL,
				HashDiff AS
				(
					CONVERT(CHAR(32), HASHBYTES(
						'MD5',
						UPPER(
							TRIM(
								CONCAT(
									ISNULL(CONVERT(NVARCHAR(50), PersonalenDanok), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), BerzanskaProvizija), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), CDHVProvizija), '')
								)
							)
						)
					), 2) 
				),
				AP_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT('AP-', CONVERT(NVARCHAR(10), Id))), 2)
				),
				DateModified DATETIME NOT NULL
			);

			INSERT INTO @AP_Result
			(
				Id,
				PersonalenDanok,
				BerzanskaProvizija,
				CDHVProvizija,
				DateModified
			)
			SELECT
				ap.Id,
				ap.PersonalenDanok,
				ap.BerzanskaProvizija, 
				ap.CDHVProvizija,
				ap.DateModified
			FROM [$(PriceFlowDb)].[dbo].AplikativniParametri AS ap
			WHERE ap.DateModified > @LastLoadDate AND ap.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				UPDATE rap
				SET EndDate = r.DateModified
				FROM [dbo].Ref_AplikativniParametri AS rap
				INNER JOIN @AP_Result AS r
					ON r.Id = rap.APId
				WHERE rap.HashDiff <> r.HashDiff
						AND rap.EndDate IS NULL

				INSERT INTO [dbo].Ref_AplikativniParametri
				(
					AplikativniParametri_HK,
					APId,
					PersonalenDanok,
					BerzanskaProvizija,
					CDHVProvizija,
					LoadDate,
					HashDiff,
					RecordSource
				)
				SELECT
					r.AP_HK,
					r.Id,
					r.PersonalenDanok,
					r.BerzanskaProvizija,
					r.CDHVProvizija,
					SYSUTCDATETIME(),
					r.HashDiff,
					'PriceFlowDb'
				FROM @AP_Result AS r
				LEFT OUTER JOIN [dbo].Ref_AplikativniParametri AS rap
					ON r.Id = rap.APId
						AND rap.HashDiff = r.HashDiff
						AND rap.EndDate IS NULL
				WHERE rap.AplikativniParametri_HK IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @AP_Result
				)
				WHERE SourceTableName = 'AplikativniParametri'
			END
			
			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO