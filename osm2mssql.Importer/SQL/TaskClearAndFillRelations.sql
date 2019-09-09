TRUNCATE TABLE [dbo].[tRelation]
GO
INSERT INTO [dbo].[tRelation] ([Id], [Geo], [Role])
SELECT R.[RelationId], [dbo].[GeographyUnion](X.[Geo]) AS [Geo], R.[Role]
FROM [dbo].[tRelationCreation] R
	LEFT JOIN
	(
		SELECT [Id], [Location] AS [Geo], (SELECT [Id] FROM [dbo].[tMemberType] WHERE [Name] = 'node') AS [Type] FROM [dbo].[tNode]
		UNION ALL
		SELECT [Id], [Line]     AS [Geo], (SELECT [id] FROM [dbo].[tMemberType] WHERE [Name] = 'way')  AS [Type] FROM [dbo].[tWay]
	) X
		ON  R.[Ref]  = x.[Id]
		AND R.[Type] = x.[Type]
GROUP BY R.[RelationId], R.[Role];
GO
UPDATE [dbo].[tRelation]
	SET [Geo] = [dbo].[ConvertToPolygon]([Geo])
WHERE [Id] IN
(
	SELECT [RelationId] FROM [dbo].[tRelationTag] WHERE [Info] = 'multipolygon'
);
GO