TRUNCATE TABLE [dbo].[tWay]
GO
INSERT INTO [dbo].[tWay] ([Id], [Line])
SELECT W.[WayId], [dbo].[CreateLineString]([Latitude], [Longitude], [Sort])
FROM [dbo].[tWayCreation] W
	INNER JOIN [dbo].[tNode] N
		ON N.[Id] = W.[NodeId]
GROUP BY W.[WayId];
GO