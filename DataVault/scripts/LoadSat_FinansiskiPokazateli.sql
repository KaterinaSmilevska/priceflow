CREATE PROCEDURE [dbo].Load_Sat_FinansiskiPokazateli @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'FinansiskiPokazateli'
			)

			DECLARE @FP_Result TABLE
			(
				Ime NVARCHAR(100) NOT NULL,
				Godina INT NOT NULL DEFAULT 2025,
				OperativnaDobivka DECIMAL(18, 2) NULL, 
				NetoDobivkaPoAkcija DECIMAL(18, 2) NULL, 
				KoefCenaDobivkaPoAkcija DECIMAL(18, 2) NULL, 
				KnigovodstvenaVrednostPoAkcija DECIMAL(18, 2) NULL, 
				KoefCenaKnigovodstvenaVrednostPoAkcija DECIMAL(18, 2) NULL, 
				DividendaPoAkcija DECIMAL(18, 2) NULL, 
				DividendenPrinos DECIMAL(18, 2) NULL,
				HashDiff AS
				(
					CONVERT(CHAR(32), HASHBYTES(
						'MD5',
						UPPER(
							TRIM(
								CONCAT(
									ISNULL(CONVERT(NVARCHAR(50), OperativnaDobivka), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), NetoDobivkaPoAkcija), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), KoefCenaDobivkaPoAkcija), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), KnigovodstvenaVrednostPoAkcija), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), KoefCenaKnigovodstvenaVrednostPoAkcija), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), DividendaPoAkcija), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), DividendenPrinos), '')
								)
							)
						)
					), 2)
				),
				Izdavachi_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Ime))), 2)
				),
				DateModified DATETIME NOT NULL
			);

			INSERT INTO @FP_Result 
			(
				Ime,
				Godina,
				OperativnaDobivka, 
				NetoDobivkaPoAkcija, 
				KoefCenaDobivkaPoAkcija, 
				KnigovodstvenaVrednostPoAkcija, 
				KoefCenaKnigovodstvenaVrednostPoAkcija, 
				DividendaPoAkcija, 
				DividendenPrinos,
				DateModified
			)
			SELECT
				i.Ime,
				fp.Godina,
				fp.OperativnaDobivka, 
				fp.NetoDobivkaPoAkcija, 
				fp.KoefCenaDobivkaPoAkcija, 
				fp.KnigovodstvenaVrednostPoAkcija, 
				fp.KoefCenaKnigovodstvenaVrednostPoAkcija, 
				fp.DividendaPoAkcija, 
				fp.DividendenPrinos,
				fp.DateModified
			FROM [$(PriceFlowDb)].[dbo].FinansiskiPokazateli AS fp
			INNER JOIN [$(PriceFlowDb)].[dbo].Izdavachi AS i
				ON i.Id = fp.IzdavachId
			WHERE fp.DateModified > @LastLoadDate AND fp.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				UPDATE sfp
				SET EndDate = r.DateModified
				FROM [dbo].Sat_FinansiskiPokazateli AS sfp
				INNER JOIN @FP_Result AS r
					ON r.Izdavachi_HK = sfp.Izdavachi_HK
						AND r.Godina = sfp.Godina 
				WHERE sfp.HashDiff <> r.HashDiff
						AND sfp.EndDate IS NULL

				INSERT INTO [dbo].Sat_FinansiskiPokazateli
				(
					Izdavachi_HK,
					Godina,
					OperativnaDobivka, 
					NetoDobivkaPoAkcija, 
					KoefCenaDobivkaPoAkcija, 
					KnigovodstvenaVrednostPoAkcija, 
					KoefCenaKnigovodstvenaVrednostPoAkcija, 
					DividendaPoAkcija, 
					DividendenPrinos,
					LoadDate,
					HashDiff,
					RecordSource
				)
				SELECT
					r.Izdavachi_HK,
					r.Godina,
					r.OperativnaDobivka, 
					r.NetoDobivkaPoAkcija, 
					r.KoefCenaDobivkaPoAkcija, 
					r.KnigovodstvenaVrednostPoAkcija, 
					r.KoefCenaKnigovodstvenaVrednostPoAkcija, 
					r.DividendaPoAkcija, 
					r.DividendenPrinos,
					SYSUTCDATETIME(),
					r.HashDiff,
					'PriceFlowDb'
				FROM @FP_Result AS r
				LEFT OUTER JOIN [dbo].Sat_FinansiskiPokazateli AS sfp
					ON sfp.Izdavachi_HK = r.Izdavachi_HK
						AND sfp.Godina = r.Godina
						AND sfp.HashDiff = r.HashDiff
						AND sfp.EndDate IS NULL
				WHERE sfp.Izdavachi_HK IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @FP_Result
				)
				WHERE SourceTableName = 'FinansiskiPokazateli'
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO
