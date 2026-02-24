CREATE PROCEDURE [dbo].Load_Sat_Izdavachi @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'Izdavachi'
			)

			DECLARE @Izdavachi_Result TABLE
			(
				Ime NVARCHAR(100) NOT NULL,
				Grad NVARCHAR(100) NOT NULL,
				Drzava NVARCHAR(100) NOT NULL,
				HashDiff AS
				(
					CONVERT(CHAR(32), HASHBYTES(
						'MD5',
						UPPER(
							TRIM(
								CONCAT(
									ISNULL(CONVERT(NVARCHAR(50), Grad),  ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), Drzava), '')
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

			INSERT INTO @Izdavachi_Result 
			(
				Ime,
				Grad,
				Drzava,
				DateModified
			)
			SELECT
				i.Ime,
				i.Grad,
				i.Drzava,
				i.DateModified
			FROM [$(PriceFlowDb)].[dbo].Izdavachi AS i
			WHERE i.DateModified > @LastLoadDate AND i.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				UPDATE si
				SET EndDate = r.DateModified
				FROM [dbo].Sat_Izdavachi AS si
				INNER JOIN @Izdavachi_Result AS r
					ON r.Izdavachi_HK = si.Izdavachi_HK
				WHERE si.HashDiff <> r.HashDiff
						AND si.EndDate IS NULL

				INSERT INTO [dbo].Sat_Izdavachi
				(
					Izdavachi_HK,
					Grad,
					Drzava,
					LoadDate,
					HashDiff,
					RecordSource
				)
				SELECT
					r.Izdavachi_HK,
					r.Grad,
					r.Drzava,
					SYSUTCDATETIME(),
					r.HashDiff,
					'PriceFlowDb'
				FROM @Izdavachi_Result AS r
				LEFT OUTER JOIN [dbo].Sat_Izdavachi AS si
					ON si.Izdavachi_HK = r.Izdavachi_HK
						AND si.HashDiff = r.HashDiff
						AND si.EndDate IS NULL
				WHERE si.Izdavachi_HK IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @Izdavachi_Result
				)
				WHERE SourceTableName = 'Izdavachi'
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO
