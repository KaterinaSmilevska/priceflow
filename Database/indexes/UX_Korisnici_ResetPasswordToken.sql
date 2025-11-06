CREATE UNIQUE NONCLUSTERED INDEX [UX_Korisnici_ResetPasswordToken]
	ON [dbo].[Korisnici]
	(ResetPasswordToken)
WHERE ResetPasswordToken IS NOT NULL
