INSERT INTO [dbo].[PortfolioPrinosi] (Datum, NetoIznos, Danok, PortfolioId, HVId)
SELECT Datum, NetoIznos, Danok, p.Id, hv.Id
FROM (
	VALUES
	('2025-08-12', 15000.00, 500, N'Технологија', 'ALK')
) as pp (Datum, NetoIznos, Danok, PortfolioIme, HVKod)
JOIN [dbo].[Portfolija] as p
	ON p.Ime = pp.PortfolioIme
JOIN [dbo].[HartiiOdVrednost] as hv
	ON hv.Kod = pp.HVKod