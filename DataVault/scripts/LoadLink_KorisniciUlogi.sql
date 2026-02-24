CREATE PROCEDURE Load_Link_KorisniciUlogi @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY;

			DECLARE @KorisniciUlogi_Result TABLE
			(
				Korisnici_HK CHAR(32) NOT NULL,
				Ulogi_HK CHAR(32) NOT NULL,
				KorisniciUlogi_HK AS 
				(
					CONVERT(CHAR(32), HASHBYTES('MD5',
						CONCAT(
							Korisnici_HK,
							'|',
							Ulogi_HK
					)),2)
				)
			);

			INSERT INTO @KorisniciUlogi_Result
			(
				Korisnici_HK,
				Ulogi_HK
			)
			SELECT
				hk.Korisnici_HK,
				hu.Ulogi_HK
			FROM [$(PriceFlowDb)].[dbo].KorisniciUlogi AS ku
			INNER JOIN [$(PriceFlowDb)].[dbo].Korisnici AS k
				ON k.Id = ku.KorisnikId
			INNER JOIN [$(PriceFlowDb)].[dbo].Ulogi AS u
				ON u.Id = ku.UlogaId
			INNER JOIN [dbo].Hub_Korisnici AS hk
				ON hk.Korisnici_HK = CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(k.Username))), 2)
			INNER JOIN [dbo].Hub_Ulogi AS hu
				ON hu.Ulogi_HK = CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(u.Ime))), 2)

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Link_KorisniciUlogi
				(
					KorisniciUlogi_HK, 
					Korisnici_HK,
					Ulogi_HK,
					LoadDate, 
					RecordSource
				)
				SELECT
					r.KorisniciUlogi_HK,
					r.Korisnici_HK,
					r.Ulogi_HK,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @KorisniciUlogi_Result AS r
				LEFT OUTER JOIN [dbo].Link_KorisniciUlogi AS lku
					ON lku.KorisniciUlogi_HK = r.KorisniciUlogi_HK
				WHERE lku.KorisniciUlogi_HK IS NULL
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO