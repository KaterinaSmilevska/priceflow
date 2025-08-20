INSERT INTO [dbo].[Korisnik] (Ime, Username, PasswordHash, Email, Uloga)
VALUES
('Katerina Smilevska', 'Katerina', CONVERT(VARBINARY(48), 'admin'), 'smilevskakaterina5@gmail.com', N'Админ');