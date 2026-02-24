CREATE PROCEDURE [dbo].Load_Hub_Sektori @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'Sektori'
			)

			DECLARE @Sektori_Result TABLE
			(
				Ime NVARCHAR(100) NOT NULL,
				Sektori_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Ime))), 2)
				),
				DateModified DATETIME NOT NULL
			);

			INSERT INTO @Sektori_Result
			(
				Ime,
				DateModified
			)
			SELECT
				s.Ime,
				s.DateModified
			FROM [$(PriceFlowDb)].[dbo].Sektori AS s
			WHERE s.DateModified > @LastLoadDate AND s.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Hub_Sektori
				(
					Sektori_HK,
					Ime,
					LoadDate, 
					RecordSource
				)
				SELECT
					r.Sektori_HK,
					r.Ime,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @Sektori_Result AS r
				LEFT OUTER JOIN [dbo].Hub_Sektori AS hs
					ON hs.Ime = r.Ime
				WHERE hs.Ime IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @Sektori_Result
				)
				WHERE SourceTableName = 'Sektori'

			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO
