CREATE PROCEDURE Load_Link_HartiiOdVrednost_Izdavachi @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY;

			DECLARE @HartiiOdVrednost_Izdavachi_Result TABLE
			(
				HartiiOdVrednost_HK CHAR(32) NOT NULL,
				Izdavachi_HK CHAR(32) NOT NULL,
				HartiiOdVrednost_Izdavachi_HK AS 
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', 
						CONCAT(
							HartiiOdVrednost_HK,
							'|',
							Izdavachi_HK
						)), 2)
				)
			);

			INSERT INTO @HartiiOdVrednost_Izdavachi_Result
			(
				HartiiOdVrednost_HK,
				Izdavachi_HK
			)
			SELECT
				hhv.HartiiOdVrednost_HK,
				hi.Izdavachi_HK
			FROM [$(PriceFlowDb)].[dbo].HartiiOdVrednost AS hv
			INNER JOIN [$(PriceFlowDb)].[dbo].Izdavachi AS i
				ON i.Id = hv.IzdavachId
			INNER JOIN [dbo].Hub_HartiiOdVrednost AS hhv
				ON hhv.HartiiOdVrednost_HK = CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(hv.Kod))), 2)
			INNER JOIN [dbo].Hub_Izdavachi AS hi
				ON hi.Izdavachi_HK = CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(i.Ime))), 2)

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Link_HartiiOdVrednost_Izdavachi
				(
					HartiiOdVrednost_Izdavachi_HK,
					HartiiOdVrednost_HK, 
					Izdavachi_HK,
					LoadDate, 
					RecordSource
				)
				SELECT
					r.HartiiOdVrednost_Izdavachi_HK,
					r.HartiiOdVrednost_HK,
					r.Izdavachi_HK,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @HartiiOdVrednost_Izdavachi_Result AS r
				LEFT OUTER JOIN [dbo].Link_HartiiOdVrednost_Izdavachi AS lhvi
					ON lhvi.HartiiOdVrednost_HK = r.HartiiOdVrednost_HK
						AND lhvi.Izdavachi_HK = r.Izdavachi_HK
				WHERE lhvi.HartiiOdVrednost_HK IS NULL
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO