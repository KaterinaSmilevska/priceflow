CREATE PROCEDURE Load_Link_Izdavachi_Sektori @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY;

			DECLARE @Izdavachi_Sektori_Result TABLE
			(
				Izdavachi_HK CHAR(32) NOT NULL,
				Sektori_HK CHAR(32) NOT NULL,
				Izdavachi_Sektori_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', 
						CONCAT(
							Izdavachi_HK, 
							'|', 
							Sektori_HK
						)), 2)
				)
			);

			INSERT INTO @Izdavachi_Sektori_Result
			(
				Izdavachi_HK,
				Sektori_HK
			)
			SELECT
				hi.Izdavachi_HK,
				hs.Sektori_HK
			FROM [$(PriceFlowDb)].[dbo].Izdavachi AS i
			INNER JOIN [$(PriceFlowDb)].[dbo].Sektori AS s
				ON s.Id = i.SektorId
			INNER JOIN [dbo].Hub_Izdavachi AS hi
				ON hi.Izdavachi_HK = CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(i.Ime))), 2)
			INNER JOIN [dbo].Hub_Sektori AS hs
				ON hs.Sektori_HK = CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(s.Ime))), 2)

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Link_Izdavachi_Sektori
				(
					Izdavachi_Sektori_HK, 
					Izdavachi_HK,
					Sektori_HK,
					LoadDate, 
					RecordSource
				)
				SELECT
					r.Izdavachi_Sektori_HK,
					r.Izdavachi_HK,
					r.Sektori_HK,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @Izdavachi_Sektori_Result AS r
				LEFT OUTER JOIN [dbo].Link_Izdavachi_Sektori AS lis
					ON lis.Izdavachi_HK = r.Izdavachi_HK
						AND lis.Sektori_HK = r.Sektori_HK
				WHERE lis.Izdavachi_HK IS NULL
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO