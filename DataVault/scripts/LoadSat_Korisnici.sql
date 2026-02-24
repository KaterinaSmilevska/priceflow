CREATE PROCEDURE [dbo].Load_Sat_Korisnici @SyncDate DATETIME
AS
	BEGIN TRANSACTION;
		BEGIN TRY

			DECLARE @LastLoadDate DATETIME = 
			(
				SELECT LastLoadDate
				FROM [dbo].ETL_Load
				WHERE SourceTableName = 'Korisnici'
			)

			DECLARE @Korisnici_Result TABLE
			(
				Username NVARCHAR(100) NOT NULL,
				Ime NVARCHAR(100) NOT NULL,
				Email NVARCHAR(100) NOT NULL,
				PasswordHash VARBINARY(48) NOT NULL,
				IsEmailVerified BIT NOT NULL DEFAULT 0,
				EmailVerificationToken UNIQUEIDENTIFIER NULL,
				ResetPasswordToken UNIQUEIDENTIFIER NULL,
				ResetPasswordTokenExpiry DATETIME NULL,
				HashDiff_Korisnici AS
				(
					CONVERT(CHAR(32), HASHBYTES(
					'MD5',
					UPPER(
						TRIM(
							CONCAT(
								ISNULL(CONVERT(NVARCHAR(50), Ime),  ''), '|',
								ISNULL(CONVERT(NVARCHAR(50), Email), '')
							)
						)
					)
					), 2)
				),
				HashDiff_Korisnici_Sensitive AS
				(
					CONVERT(CHAR(32), HASHBYTES(
						'MD5',
						UPPER(
							TRIM(
								CONCAT(
									ISNULL(CONVERT(NVARCHAR(50), PasswordHash),  ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), IsEmailVerified), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), EmailVerificationToken), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), ResetPasswordToken), ''), '|',
									ISNULL(CONVERT(NVARCHAR(50), ResetPasswordTokenExpiry), '')
								)
							)
						)
					), 2)
				),
				Korisnici_HK AS
				(
					CONVERT(CHAR(32), HASHBYTES('MD5', UPPER(TRIM(Username))), 2)
				),
				DateModified DATETIME NOT NULL
			);

			INSERT INTO @Korisnici_Result
			(
				Username,
				Ime,
				Email,
				PasswordHash,
				IsEmailVerified,
				EmailVerificationToken,
				ResetPasswordToken,
				ResetPasswordTokenExpiry,
				DateModified
			)
			SELECT
				k.Username,
				k.Ime,
				k.Email,
				k.PasswordHash,
				k.IsEmailVerified,
				k.EmailVerificationToken,
				k.ResetPasswordToken,
				k.ResetPasswordTokenExpiry,
				k.DateModified
			FROM [$(PriceFlowDb)].[dbo].Korisnici AS k
			WHERE k.DateModified > @LastLoadDate AND k.DateModified <= @SyncDate

			IF @@ROWCOUNT > 0

			BEGIN
				UPDATE sk
				SET EndDate = r.DateModified
				FROM [dbo].Sat_Korisnici AS sk
				INNER JOIN @Korisnici_Result AS r
					ON r.Korisnici_HK = sk.Korisnici_HK
				WHERE sk.HashDiff <> r.HashDiff_Korisnici
						AND sk.EndDate IS NULL

				INSERT INTO [dbo].Sat_Korisnici
				(
					Korisnici_HK,
					Ime,
					Email,
					LoadDate,
					HashDiff,
					RecordSource
				)
				SELECT
					r.Korisnici_HK,
					r.Ime,
					r.Email,
					SYSUTCDATETIME(),
					r.HashDiff_Korisnici,
					'PriceFlowDb'
				FROM @Korisnici_Result AS r
				LEFT OUTER JOIN [dbo].Sat_Korisnici AS sk
					ON sk.Korisnici_HK = r.Korisnici_HK
						AND sk.HashDiff = r.HashDiff_Korisnici
						AND sk.EndDate IS NULL
				WHERE sk.Korisnici_HK IS NULL

				UPDATE sks
				SET EndDate = r.DateModified
				FROM [dbo].Sat_Korisnici_Sensitive AS sks
				INNER JOIN @Korisnici_Result AS r
					ON r.Korisnici_HK = sks.Korisnici_HK
				WHERE sks.HashDiff <> r.HashDiff_Korisnici_Sensitive
						AND sks.EndDate IS NULL

				INSERT INTO [dbo].Sat_Korisnici_Sensitive
				(
					Korisnici_HK,
					PasswordHash,
					IsEmailVerified,
					EmailVerificationToken,
					ResetPasswordToken,
					ResetPasswordTokenExpiry,
					LoadDate,
					HashDiff,
					RecordSource
				)
				SELECT
					r.Korisnici_HK,
					r.PasswordHash,
					r.IsEmailVerified,
					r.EmailVerificationToken,
					r.ResetPasswordToken,
					r.ResetPasswordTokenExpiry,
					SYSUTCDATETIME(),
					r.HashDiff_Korisnici_Sensitive,
					'PriceFlowDb'
				FROM @Korisnici_Result AS r
				LEFT OUTER JOIN [dbo].Sat_Korisnici_Sensitive AS sks
					ON sks.Korisnici_HK = r.Korisnici_HK
						AND sks.HashDiff = r.HashDiff_Korisnici_Sensitive
						AND sks.EndDate IS NULL
				WHERE sks.Korisnici_HK IS NULL

				UPDATE [dbo].ETL_Load
				SET LastLoadDate = (
					SELECT max(DateModified)
					FROM @Korisnici_Result
				)
				WHERE SourceTableName = 'Korisnici'
			END

			COMMIT TRANSACTION;
		END TRY

		BEGIN CATCH

		IF @@TRANCOUNT > 0
			ROLLBACK TRANSACTION;

		THROW;
		END CATCH;
GO