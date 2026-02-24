CREATE PROCEDURE [dbo].Load_Sat_DnevenPromet @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME =
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'DnevenPromet'
			)

			DECLARE @DnevenPromet_Result TABLE
			(
				Kod NVARCHAR(50) NOT NULL,
				Datum DATETIME NOT NULL,
				CenaPoslednaTransakcija DECIMAL(18, 2) NULL, 
				MaxCena DECIMAL(18, 2) NULL, 
				MinCena DECIMAL(18, 2) NULL, 
				ProsecnaCena DECIMAL(18, 2) NULL, 
				ProcentPromena DECIMAL(18, 2) NULL, 
				KolicinaIstrguvaniAkcii INT NULL, 
				PrometBESTDenari INT NULL, 
				VkupenPrometDenari INT NULL,
				HashDiff AS
				(
					CONVERT(CHAR(32), HASHBYTES(
						'MD5',
						UPPER(
							TRIM(
								CONCAT(
									ISNULL(CONVERT(NVARCHAR(50), Datum, 126), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), CenaPoslednaTransakcija), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), MaxCena), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), MinCena), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), ProsecnaCena), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), ProcentPromena), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), KolicinaIstrguvaniAkcii), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), PrometBESTDenari), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), VkupenPrometDenari), '')
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
			)

			INSERT INTO @DnevenPromet_Result
			(
				Kod,
				Datum,
				CenaPoslednaTransakcija,
				MaxCena,
				MinCena,
				ProsecnaCena,
				ProcentPromena,
				KolicinaIstrguvaniAkcii,
				PrometBESTDenari,
				VkupenPrometDenari,
				DateModified
			)
			SELECT
				hv.Kod,
				dp.Datum,
				dp.CenaPoslednaTransakcija,
				dp.MaxCena,
				dp.MinCena,
				dp.ProsecnaCena,
				dp.ProcentPromena,
				dp.KolicinaIstrguvaniAkcii,
				dp.PrometBESTDenari,
				dp.VkupenPrometDenari,
				dp.DateModified
			FROM [$(PriceFlowDb)].[dbo].DnevenPromet AS dp
			INNER JOIN [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
				ON hv.Id = dp.HVId
			WHERE dp.DateModified > @LastLoadDate AND dp.DateModified <= @SyncDate
				
			IF @@ROWCOUNT > 0

			BEGIN
				UPDATE sdp
				SET EndDate = r.DateModified
				FROM [dbo].Sat_DnevenPromet AS sdp
				INNER JOIN @DnevenPromet_Result AS r
					ON r.HartiiOdVrednost_HK = sdp.HartiiOdVrednost_HK
						AND r.Datum = sdp.Datum
				WHERE sdp.HashDiff <> r.HashDiff
					AND sdp.EndDate IS NULL

				INSERT INTO [dbo].Sat_DnevenPromet
				(
					HartiiOdVrednost_HK,
					Datum,
					CenaPoslednaTransakcija,
					MaxCena,
					MinCena,
					ProsecnaCena,
					ProcentPromena,
					KolicinaIstrguvaniAkcii,
					PrometBESTDenari,
					VkupenPrometDenari,
					LoadDate,
					HashDiff,
					RecordSource
				)
				SELECT
					r.HartiiOdVrednost_HK,
					r.Datum,
					r.CenaPoslednaTransakcija,
					r.MaxCena,
					r.MinCena,
					r.ProsecnaCena,
					r.ProcentPromena,
					r.KolicinaIstrguvaniAkcii,
					r.PrometBESTDenari,
					r.VkupenPrometDenari,
					SYSUTCDATETIME(),
					r.HashDiff,
					'PriceFlowDb'
				FROM @DnevenPromet_Result AS r
				LEFT OUTER JOIN [dbo].Sat_DnevenPromet AS sdp
					ON sdp.HartiiOdVrednost_HK = r.HartiiOdVrednost_HK
						AND sdp.Datum = r.Datum
						AND sdp.HashDiff = r.HashDiff
						AND sdp.EndDate IS NULL
				WHERE sdp.HartiiOdVrednost_HK IS NULL

				UPDATE [dbo].ETL_Load
					SET LastLoadDate = (
						SELECT max(DateModified)
						FROM @DnevenPromet_Result
					)
					WHERE SourceTableName = 'DnevenPromet'
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO
