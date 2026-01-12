CREATE PROCEDURE Load_Link_Portfolija_Korisnici @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY;

			DECLARE @Portfolija_Korisnici_Result TABLE
			(
				Portfolija_HK CHAR(32) NOT NULL,
				Korisnici_HK CHAR(32) NOT NULL,
				Portfolija_Korisnici_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', 
						CONCAT(
							Portfolija_HK,
							'|', 
							Korisnici_HK
						)), 2)
				)
			);

			INSERT INTO @Portfolija_Korisnici_Result
			(
				Portfolija_HK,
				Korisnici_HK
			)
			SELECT
				hp.Portfolija_HK,
				hk.Korisnici_HK
			FROM [$(PriceFlowDb)].[dbo].Portfolija AS p
			INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
				ON k.Id = p.KorisnikId
			INNER JOIN [dbo].Hub_Portfolija AS hp
				ON hp.Portfolija_HK = CONVERT(CHAR(32), HASHBYTES('MD5', CONCAT(UPPER(TRIM(k.Username)), '|', UPPER(TRIM(p.Ime)))), 2)
			INNER JOIN [dbo].Hub_Korisnici AS hk
				ON hk.Korisnici_HK = CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(k.Username))), 2)

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Link_Portfolija_Korisnici
				(
					Portfolija_Korisnici_HK, 
					Portfolija_HK,
					Korisnici_HK,
					LoadDate, 
					RecordSource
				)
				SELECT
					r.Portfolija_Korisnici_HK,
					r.Portfolija_HK,
					r.Korisnici_HK,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @Portfolija_Korisnici_Result AS r
				LEFT OUTER JOIN [dbo].Link_Portfolija_Korisnici AS lpk
					ON lpk.Portfolija_HK = r.Portfolija_HK
						AND lpk.Korisnici_HK = r.Korisnici_HK
				WHERE lpk.Portfolija_HK IS NULL
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO