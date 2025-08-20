INSERT INTO [dbo].[Portfolio] (Ime, Opis, KorisnikId)
SELECT p.Ime, Opis, k.Id
FROM (
	VALUES
	(N'Технологија', N'Портфолио за трошоци за компјутерски делови', 'Katerina')
) as p (Ime, Opis, KorisnikUsername)
JOIN [dbo].[Korisnik] as k
	ON k.Username = p.KorisnikUsername