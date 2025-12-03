; WITH SourceData AS (
	SELECT
		hk.Korisnici_HK,
		k.PasswordHash,
		k.IsEmailVerified,
		k.EmailVerificationToken,
		k.ResetPasswordToken,
		k.ResetPasswordTokenExpiry,
		CONVERT(CHAR(32), HASHBYTES(
				'MD5',
				UPPER(
					TRIM(
						CONCAT(
							ISNULL(CONVERT(NVARCHAR(50), k.PasswordHash),  ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), k.IsEmailVerified), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), k.EmailVerificationToken), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), k.ResetPasswordToken), ''), '|',
							ISNULL(CONVERT(NVARCHAR(50), k.ResetPasswordTokenExpiry), '')
						)
					)
				)
			), 2) AS HashDiff,
			SYSDATETIME() AS LoadDate,
			'PriceFlowDb' AS RecordSource
	FROM [$(PriceFlowDb)].[dbo].Korisnici AS k
	INNER JOIN Hub_Korisnici AS hk
		ON hk.Username = k.Username
)
MERGE Sat_Korisnici_Sensitive AS target
USING SourceData AS source
	ON target.Korisnici_HK = source.Korisnici_HK
WHEN MATCHED AND target.HashDiff <> source.HashDiff AND target.EndDate IS NULL THEN
	UPDATE SET EndDate = SYSUTCDATETIME()
WHEN NOT MATCHED BY TARGET THEN
	INSERT (Korisnici_HK, PasswordHash, IsEmailVerified, EmailVerificationToken, 
			ResetPasswordToken, ResetPasswordTokenExpiry, HashDiff, LoadDate, EndDate, RecordSource)
	VALUES (source.Korisnici_HK, source.PasswordHash, source.IsEmailVerified, source.EmailVerificationToken, 
			source.ResetPasswordToken, source.ResetPasswordTokenExpiry, source.HashDiff, source.LoadDate, NULL, source.RecordSource)
;

