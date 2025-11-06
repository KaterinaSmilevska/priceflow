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
JOIN [dbo].[Portfolija] as p
	ON p.Ime = t.PortfolioIme
JOIN [dbo].[HartiiOdVrednost] as hv
	ON hv.Kod = t.HVKod