CREATE NONCLUSTERED INDEX [UX_HartiiOdVrednost_IzdavachId]
	ON [dbo].[HartiiOdVrednost]
	(IzdavachId)
WHERE IzdavachId IS NOT NULL
