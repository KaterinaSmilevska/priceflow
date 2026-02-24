CREATE PROCEDURE [dbo].Load_Hub_Ulogi @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'Ulogi'
			)

			DECLARE @Ulogi_Result TABLE
			(
				Ime NVARCHAR(50) NOT NULL,
				ulogi_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Ime))), 2)
				),
				DateModified DATETIME NOT NULL
			);

			INSERT INTO @Ulogi_Result
			(
				Ime,
				DateModified
			)
			SELECT
				u.Ime,
				u.DateModified
			FROM [$(PriceFlowDb)].[dbo].Ulogi AS u
			WHERE u.DateModified > @LastLoadDate AND u.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				INSERT INTO [dbo].Hub_Ulogi
				(
					Ulogi_HK, 
					Ime,
					LoadDate, 
					RecordSource
				)
				SELECT
					r.Ulogi_HK,
					r.Ime,
					SYSUTCDATETIME(),
					'PriceFlowDb'
				FROM @Ulogi_Result AS r
				LEFT OUTER JOIN [dbo].Hub_Ulogi AS hu
					ON hu.Ime = r.Ime
				WHERE hu.Ime IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @Ulogi_Result
				)
				WHERE SourceTableName = 'Ulogi'
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO
