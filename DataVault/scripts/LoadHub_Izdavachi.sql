CREATE PROCEDURE [dbo].Load_Hub_Izdavachi @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate_Izdavachi DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'Izdavachi'
			)

			DECLARE @Izdavachi_Result TABLE
			(
				Ime NVARCHAR(100) NOT NULL,
				Izdavachi_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Ime))), 2)
				),
				DateModified DATETIME NOT NULL
			)

			INSERT INTO @Izdavachi_Result 
			(
				Ime,
				DateModified
			)
			SELECT
				i.Ime,
				i.DateModified
			FROM [$(PriceFlowDb)].[dbo].Izdavachi AS i
			WHERE i.DateModified > @LastLoadDate_Izdavachi AND i.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Hub_Izdavachi
				(
					Izdavachi_HK,
					Ime,
					LoadDate,
					RecordSource
				)
				SELECT
					r.Izdavachi_HK,
					r.Ime,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @Izdavachi_Result AS r
				LEFT OUTER JOIN [dbo].Hub_Izdavachi AS hi
					ON hi.Ime = r.Ime
				WHERE hi.Ime IS NULL
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO
