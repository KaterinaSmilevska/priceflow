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
