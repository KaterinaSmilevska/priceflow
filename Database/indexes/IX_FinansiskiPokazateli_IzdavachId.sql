CREATE NONCLUSTERED INDEX [IX_FinansiskiPokazateli_IzdavachId]
	ON [dbo].[FinansiskiPokazateli]
	(Godina, IzdavachId)
WHERE IzdavachId IS NOT NULL
