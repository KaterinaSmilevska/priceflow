CREATE PROCEDURE [dbo].Load_Sat_Transakcii @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'Transakcii'
			)

			DECLARE @Transakcii_Result TABLE
			(
				Username NVARCHAR(100) NOT NULL,
				Ime NVARCHAR(50) NOT NULL,
				Kod NVARCHAR(50) NULL,
				KolicinaAkcii INT NOT NULL, 
				EdinecnaCenaAkcija INT NOT NULL, 
				Datum DATE NOT NULL, 
				Iznos DECIMAL(18,2) NOT NULL, 
				BerzanskaProvizija DECIMAL(18, 2) NOT NULL, 
				BrokerskaProvizija DECIMAL(18, 2) NOT NULL, 
				CDHVProvizija DECIMAL(18, 2) NOT NULL, 
				TipTransakcija NVARCHAR(20) NOT NULL, 
				Realna NVARCHAR(2) NOT NULL,
				HashDiff AS
				(
					CONVERT(CHAR(32), HASHBYTES(
						'MD5',
						UPPER(
							TRIM(
								CONCAT(
									ISNULL(CONVERT(NVARCHAR(50), KolicinaAkcii), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), EdinecnaCenaAkcija), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), Datum, 126), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), Iznos), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), BerzanskaProvizija), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), BrokerskaProvizija), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), CDHVProvizija), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), TipTransakcija, 126), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), Realna, 126), '')
								)
							)
						)
					), 2)
				),
				Transakcii_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', 
						CONCAT(
							CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(UPPER(TRIM(Username)), '|', UPPER(TRIM(Ime)))), 2),
							'|',
							CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Kod))), 2)
					)), 2)
				),
				DateModified DATETIME NOT NULL
			);

			INSERT INTO @Transakcii_Result
			(
				Username,
				Ime,
				Kod,
				KolicinaAkcii, 
				EdinecnaCenaAkcija, 
				Datum, 
				Iznos, 
				BerzanskaProvizija, 
				BrokerskaProvizija, 
				CDHVProvizija, 
				TipTransakcija, 
				Realna,
				DateModified
			)
			SELECT
				k.Username,
				p.Ime,
				hv.Kod,
				t.KolicinaAkcii, 
				t.EdinecnaCenaAkcija, 
				t.Datum, 
				t.Iznos, 
				t.BerzanskaProvizija, 
				t.BrokerskaProvizija, 
				t.CDHVProvizija, 
				t.TipTransakcija, 
				t.Realna,
				t.DateModified
			FROM [$(PriceFlowDb)].[dbo].Transakcii AS t
			INNER JOIN [$(PriceFlowDb)].[dbo].Portfolija AS p
				ON p.Id = t.PortfolioId
			INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
				ON k.Id = p.KorisnikId
			INNER JOIN [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
				ON hv.Id = t.HVId
			WHERE t.DateModified > @LastLoadDate AND t.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				UPDATE st
				SET EndDate = r.DateModified
				FROM [dbo].Sat_Transakcii AS st
				INNER JOIN @Transakcii_Result AS r
					ON r.Transakcii_HK = st.Transakcii_HK
				WHERE st.HashDiff <> r.HashDiff
					AND st.EndDate IS NULL
				
				INSERT INTO [dbo].Sat_Transakcii
				(
					Transakcii_HK,
					KolicinaAkcii, 
					EdinecnaCenaAkcija, 
					Datum, 
					Iznos, 
					BerzanskaProvizija, 
					BrokerskaProvizija, 
					CDHVProvizija, 
					TipTransakcija, 
					Realna,
					LoadDate,
					HashDiff,
					RecordSource
				)
				SELECT
					r.Transakcii_HK,
					r.KolicinaAkcii, 
					r.EdinecnaCenaAkcija, 
					r.Datum, 
					r.Iznos, 
					r.BerzanskaProvizija, 
					r.BrokerskaProvizija, 
					r.CDHVProvizija, 
					r.TipTransakcija, 
					r.Realna,
					SYSUTCDATETIME(),
					r.HashDiff,
					'PriceFlowDb'
				FROM @Transakcii_Result AS r
				LEFT OUTER JOIN [dbo].Sat_Transakcii AS st
					ON st.Transakcii_HK = r.Transakcii_HK
						AND st.HashDiff = r.HashDiff
						AND st.EndDate IS NULL
				WHERE st.Transakcii_HK IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @Transakcii_Result
				)
				WHERE SourceTableName = 'Transakcii'
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO