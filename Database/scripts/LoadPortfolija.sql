INSERT INTO [dbo].[Portfolija] (Ime, Opis, KorisnikId)
SELECT p.Ime, Opis, k.Id
FROM (
	VALUES
	(N'Портфолио 1', N'Портфолио 1 опис', 'katerinasmilevska')
) as p (Ime, Opis, KorisnikUsername)
JOIN [dbo].[Korisnici] as k
	ON k.Username = p.KorisnikUsername