CREATE UNIQUE NONCLUSTERED INDEX [UX_EmailVerificationToken]
	ON [dbo].[Korisnici]
	(EmailVerificationToken)
	WHERE EmailVerificationToken IS NOT NULL;
