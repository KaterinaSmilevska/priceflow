CREATE NONCLUSTERED INDEX [UX_FinansiskiPokazateli_IzdavachId]
	ON [dbo].[FinansiskiPokazateli]
	(IzdavachId)
WHERE IzdavachId IS NOT NULL
