/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
INSERT INTO [dbo].[TipHV] (Ime)
VALUES
(N'Акции'),
(N'Обврзници');
INSERT INTO [dbo].[Sektor] (Ime)
VALUES
(N'Банкарство'),
(N'Услуги'),
(N'Трговија'),
(N'Индустрија'),
(N'Градежништво'),
(N'Угостителство');
INSERT INTO [dbo].[Broker] (Kompanija, ProcentProvizija)
VALUES
(N'Комерцијална банка АД Скопје', 0.75),
(N'Стопанска банка АД Скопје', 1.015),
(N'Илирика Инвестментс АД Скопје', 3),
(N'Фершпед брокер АД Скопје', 0.9),
(N'Еурохаус АД Скопје', 2.95);
INSERT INTO [dbo].[AplikativniParametri] (PersonalenDanok, BerzanskaProvizija, CDHVProvizija)
VALUES
(10, 0.20, 0.10)
INSERT INTO [dbo].[Korisnik] (Ime, Username, PasswordHash, Email, Uloga)
VALUES
('Katerina Smilevska', 'Katerina', CONVERT(VARBINARY(48), 'admin'), 'smilevskakaterina5@gmail.com', N'Админ');
INSERT INTO [dbo].[Izdavach] (Ime, Grad, Drzava, SektorId)
SELECT i.Ime, i.Grad, i.Drzava, s.Id
FROM (
	VALUES
	(N'Алкалоид АД Скопје', N'Скопје', N'Р.Македонија', N'Индустрија'),
	(N'Комерцијална банка АД Скопје', N'Скопје', N'Р.Македонија', N'Банкарство'),
	(N'Стопанска банка АД Скопје', N'Скопје', N'Р.Македонија', N'Банкарство'),
	(N'Жито Лукс АД Скопје', N'Скопје', N'Р.Македонија', N'Индустрија'),
	(N'ТТК банка АД Скопје', N'Скопје', N'Р.Македонија', N'Банкарство'),
	(N'Гранит АД Скопје', N'Скопје', N'Р.Македонија', N'Градежништво'),
	(N'НЛБ банка АД Скопје', N'Скопје', N'Р.Македонија', N'Банкарство'),
	(N'Макпетрол АД Скопје', N'Скопје', N'Р.Македонија', N'Трговија'),
	(N'Реплек АД Скопје', N'Скопје', N'Р.Македонија', N'Трговија'),
	(N'Македонски Телеком АД Скопје', N'Скопје', N'Р.Македонија', N'Услуги'),
	(N'Пелистерка АД Скопје', N'Скопје', N'Р.Македонија', N'Индустрија'),
	(N'Македонијатурист АД Скопје', N'Скопје', N'Р.Македонија', N'Угостителство'),
	(N'Стопанска банка АД Битола', N'Битола', N'Р.Македонија', N'Банкарство'),
	(N'Витаминка АД Прилеп', N'Прилеп', N'Р.Македонија', N'Индустрија'),
	(N'Фершпед АД Скопје', N'Скопје', N'Р.Македонија', N'Услуги')
) AS i (Ime, Grad, Drzava, SektorIme)
JOIN [dbo].[Sektor] AS s
	ON s.Ime = i.SektorIme;

INSERT INTO [dbo].[HartiiOdVrednost] (ISIN, Kod, VkupenBrojAkcii, TipHVId, IzdavachId)
SELECT hv.ISIN, hv.Kod, hv.VkupenBrojAkcii, thv.Id, i.Id
FROM (
	VALUES
	('MKALKA101011', 'ALK', 1431353, N'Акции', N'Алкалоид АД Скопје'),
	('MKKMBS101019', 'KMB', 2279067, N'Акции', N'Комерцијална банка АД Скопје'),
	('MKSTBS101014', 'STB', 17460180, N'Акции', N'Стопанска банка АД Скопје'),
	('MKSTBS120014', 'STBP', 227444, N'Акции', N'Стопанска банка АД Скопје'),
	('MKZILU101012', 'ZILU', 819238, N'Акции', N'Жито Лукс АД Скопје'),
	('MKTTKS101012', 'TTK', 1033173, N'Акции', N'ТТК банка АД Скопје'),
	('MKGRNT101015', 'GRNT', 3071377, N'Акции', N'Гранит АД Скопје'),
	('MKTNBA101019', 'TNB', 854061, N'Акции', N'НЛБ банка АД Скопје'),
	('MKMPTS101014', 'MPT', 112382, N'Акции', N'Макпетрол АД Скопје'),
	('MKREPL101013', 'REPL', 259200, N'Акции', N'Реплек АД Скопје'),
	('MKMTSK101019', 'TEL', 95838780, N'Акции', N'Македонски Телеком АД Скопје'),
	('MKLOZP101011', 'LOZP', 50.180, N'Акции', N'Пелистерка АД Скопје'),
	('MKMTUR101018', 'MTUR', 452247, N'Акции', N'Македонијатурист АД Скопје'),
	('MKSBTB101013', 'SBT', 390.977, N'Акции', N'Стопанска банка АД Битола'),
	('MKVITA101012', 'VITA', 76720, N'Акции', N'Витаминка АД Прилеп'),
	('MKFERS101018', 'FERS', 18113, N'Акции', N'Фершпед АД Скопје')
) as hv (ISIN, Kod, VkupenBrojAkcii, TipHVIme, IzdavachIme)
JOIN [dbo].[TipHV] as thv
	ON thv.Ime = hv.TipHVIme
JOIN [dbo].[Izdavach] as i
	ON i.Ime = hv.IzdavachIme

INSERT INTO [dbo].[Portfolio] (Ime, Opis, KorisnikId)
SELECT p.Ime, Opis, k.Id
FROM (
	VALUES
	(N'Технологија', N'Портфолио за трошоци за компјутерски делови', 'Katerina')
) as p (Ime, Opis, KorisnikUsername)
JOIN [dbo].[Korisnik] as k
	ON k.Username = p.KorisnikUsername
INSERT INTO [dbo].[FinansiskiPokazateli] (IzdavachId, Godina, OperativnaDobivka, NetoDobivkaPoAkcija, KoefCenaDobivkaPoAkcija,
KnigovodstvenaVrednostPoAkcija, KoefCenaKnigovodstvenaVrednostPoAkcija, DividendaPoAkcija, DividendenPrinos)
SELECT i.Id, Godina, OperativnaDobivka, NetoDobivkaPoAkcija, KoefCenaDobivkaPoAkcija,
KnigovodstvenaVrednostPoAkcija, KoefCenaKnigovodstvenaVrednostPoAkcija, DividendaPoAkcija, DividendenPrinos
FROM (
VALUES
	(N'Алкалоид АД Скопје', 2024, 1991702, 1176.54, 24.01, 10266.10, 2.75, 630.00, 2.23),
	(N'Алкалоид АД Скопје', 2023, 1822830, 1101.69, 16.48, 9634.96, 1.88, 540.00, 2.97),
	(N'Алкалоид АД Скопје', 2022, 1669971, 1022.41, 16.78, 8738.53, 1.96, 490.00, 2.86),
	(N'Комерцијална банка АД Скопје', 2024, NULL, 2176.38, 13.07, 9207.79, 3.09, 1250.00, 4.40),
	(N'Комерцијална банка АД Скопје', 2023, NULL, 1580.64, 9.00, 7942.88, 1.79, 910.00, 6.40),
	(N'Комерцијална банка АД Скопје', 2022, NULL, 952.89, 12.26, 6876.74, 1.70, 500.00, 4.28),
	(N'Стопанска банка АД Скопје', 2024, NULL, 203.55, 14.61, 1674.59, 1.78, 229.00, 7.70),
	(N'Стопанска банка АД Скопје', 2023, NULL, 198.78, 6.63, 1473.40, 0.89, NULL, 0.00),
	(N'Стопанска банка АД Скопје', 2022, NULL, 136.02, 9.90, 1271.55, 1.06, NULL, 0.00),
	(N'Жито Лукс АД Скопје', 2024, 55678, 39.72, 2.59, 767.09, 0.13, NULL, 0.00),
	(N'Жито Лукс АД Скопје', 2023, 40634, 30.31, 2.64, 730.18, 0.11, NULL, 0.00),
	(N'Жито Лукс АД Скопје', 2022, 6581, -10.04, NULL, 551.00, 0.21, NULL, 0.00),
	(N'ТТК банка АД Скопје', 2024, NULL, 104.78, 18.61, 1247.74, 1.56, 61.00, 3.13),
	(N'ТТК банка АД Скопје', 2023, NULL, 108.95, 13.31, 1260.82, 1.15, 108.00, 7.45),
	(N'ТТК банка АД Скопје', 2022, NULL, 67.03, 20.22, 1228.40, 1.10, 67.00, 4.94),
	(N'Гранит АД Скопје', 2024, -65108, 11.97, 155.27, 2567.36, 0.72, 38.00, 2.04),
	(N'Гранит АД Скопје', 2023, 69366, 41.39, 28.49, 2072.47, 0.57, 42.00, 3.56),
	(N'Гранит АД Скопје', 2022, -32506, 14.97, 85.16, 1971.00, 0.65, 36.00, 2.82),
	(N'НЛБ банка АД Скопје', 2024, NULL, 3858.03, 14.56, 21692.73, 2.59, 2596.00, 4.62),
	(N'НЛБ банка АД Скопје', 2023, NULL, 3766.55, 7.84, 19626.73, 1.51, 2636.00, 8.92),
	(N'НЛБ банка АД Скопје', 2022, NULL, 2849.81, 8.32, 18231.74, 1.30, 2431.00, 10.26),
	(N'Макпетрол АД Скопје', 2024, 1033075, 9829.83, 9.44, 83071.68, 1.12, 4200.00, 4.53),
	(N'Макпетрол АД Скопје', 2023, 726247, 6987.26, 9.21, 69725.61, 0.92, 3700.00, 5.75),
	(N'Макпетрол АД Скопје', 2022, 1317489, 11863.24, 5.68, 65577.29, 1.03, 3300.00, 4.90),
	(N'Реплек АД Скопје', 2024, 276949, 864.95, 19.19, 9135.37, 1.82, 400.00, 2.41),
	(N'Реплек АД Скопје', 2023, 307565, 1082.67, 8.59, 8865.88, 1.05, 300.00, 3.23),
	(N'Реплек АД Скопје', 2022, 104776, 316.11, 27.70, 5917.57, 1.48, 200.00, 2.28),
	(N'Македонски Телеком АД Скопје', 2024, 2458390, 23.36, 17.12, 167.77, 2.38, 27.62, 6.91),
	(N'Македонски Телеком АД Скопје', 2023, 2340749, 21.71, 17.68, 167.49, 2.29, 25.65, 6.68),
	(N'Македонски Телеком АД Скопје', 2022, 1978781, 17.40, 20.97, 155.72, 2.34, 19.34, 5.30),
	(N'Пелистерка АД Скопје', 2024, 39966, 369.65, 7.09, 11358.33, 0.23, NULL, 0.00),
	(N'Пелистерка АД Скопје', 2023, 22785, 64.87, 40.38, 10845.24, 0.24, NULL, 0.00),
	(N'Пелистерка АД Скопје', 2022, 24255, 269.89, 9.70, 9843.18, 0.27, NULL, 0.00),
	(N'Македонијатурист АД Скопје', 2024, 96364, 379.15, 22.29, 8157.90, 1.04, NULL, 0.00),
	(N'Македонијатурист АД Скопје', 2023, 43906, 213.15, 22.40, 6361.39, 0.75, 200.00, 4.19),
	(N'Македонијатурист АД Скопје', 2022, -26285, 68.28, 64.45, 5974.11, 0.74, 129.89, 2.95),
	(N'Стопанска банка АД Битола', 2024, NULL, 417.34, 7.19, 4909.91, 0.61, NULL, 0.00),
	(N'Стопанска банка АД Битола', 2023, NULL, -485.50, NULL, 4326.76, 0.55, NULL, 0.00),
	(N'Стопанска банка АД Битола', 2022, NULL, 76.59, 31.73, 4789.01, 0.51, NULL, 0.00),
	(N'Витаминка АД Прилеп', 2024, 166154, 1173.12, 9.38, 16417.30, 0.67, 400.00, 3.64),
	(N'Витаминка АД Прилеп', 2023, 174606, 1294.28, 8.27, 14244.93, 0.75, 400.00, 3.74),
	(N'Витаминка АД Прилеп', 2022, 112284, 910.16, 13.40, 12717.99, 0.96, NULL, 0.00),
	(N'Фершпед АД Скопје', 2024, 157304, 8349.36, 6.41, 139990.78, 0.38, 720.00, 1.35),
	(N'Фершпед АД Скопје', 2023, 161613, 8481.70, 6.06, 132289.41, 0.39, 720.00, 1.40),
	(N'Фершпед АД Скопје', 2022, 104544, 5221.39, 10.15, 124302.82, 0.43, 550.00, 1.04)
) AS fp (IzdavachIme, Godina, OperativnaDobivka, NetoDobivkaPoAkcija, KoefCenaDobivkaPoAkcija,
KnigovodstvenaVrednostPoAkcija, KoefCenaKnigovodstvenaVrednostPoAkcija, DividendaPoAkcija, DividendenPrinos)
JOIN [dbo].[Izdavach] as i
	ON i.Ime = fp.IzdavachIme
INSERT INTO [dbo].[PortfolioPrinosi] (Datum, NetoIznos, Danok, PortfolioId, HVId)
SELECT Datum, NetoIznos, Danok, p.Id, hv.Id
FROM (
	VALUES
	('2025-08-12', 15000.00, 500, N'Технологија', 'ALK')
) as pp (Datum, NetoIznos, Danok, PortfolioIme, HVKod)
JOIN [dbo].[Portfolio] as p
	ON p.Ime = pp.PortfolioIme
JOIN [dbo].[HartiiOdVrednost] as hv
	ON hv.Kod = pp.HVKod
INSERT INTO [dbo].[Transakcii] (PortfolioId, KolicinaAkcii, EdinecnaCenaAkcija, Datum, Iznos, BerzanskaProvizija, BrokerskaProvizija,
CDHVProvizija, TipTransakcija, Realna, HVId)
SELECT p.Id, KolicinaAkcii, EdinecnaCenaAkcija, Datum, Iznos, BerzanskaProvizija, BrokerskaProvizija,
CDHVProvizija, TipTransakcija, Realna, hv.Id
FROM (
	VALUES
	(N'Технологија', 50, 1200, '2025-08-10', 60000, 0.20, 0.75, 0.10, N'Купување', 1, 'KMB'),
	(N'Технологија', 75, 3100, '2025-08-15', 232500, 0.20, 2.95, 0.10, N'Продавање', 2, 'ALK')
) AS t (PortfolioIme, KolicinaAkcii, EdinecnaCenaAkcija, Datum, Iznos, BerzanskaProvizija, BrokerskaProvizija,
CDHVProvizija, TipTransakcija, Realna, HVKod)
JOIN [dbo].[Portfolio] as p
	ON p.Ime = t.PortfolioIme
JOIN [dbo].[HartiiOdVrednost] as hv
	ON hv.Kod = t.HVKod
GO
