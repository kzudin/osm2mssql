SET NOCOUNT ON;
DECLARE @IDX TABLE([WayId] bigint);
DECLARE @AllWayId bigint = 0;
DECLARE @CntWayId bigint = 0;
DECLARE @MaxWayId bigint = 0;
DECLARE @TopWayId bigint = 50000;
DELETE FROM @IDX;
SELECT
	@AllWayId = (SELECT COUNT(*) FROM [dbo].[tWay]),
	@CntWayId = 0,
	@MaxWayId = 0;
WHILE (1 = 1)
BEGIN
	RAISERROR('Deleted %I64d of %I64d', 0, 1, @CntWayId, @AllWayId) WITH NOWAIT;
	IF (0 < @MaxWayId OR 0 < @CntWayId)
		WAITFOR DELAY '00:00:01';
	DELETE TOP(@TopWayId) FROM [dbo].[tWay] OUTPUT DELETED.[Id] INTO @IDX
	IF (0 = @@ROWCOUNT)
		BREAK;
	SELECT
		@CntWayId = @CntWayId + COALESCE(COUNT(*),     0),
		@MaxWayId =             COALESCE(MAX([WayId]), 0)
	FROM @IDX;
	DELETE FROM @IDX;
END;

DELETE FROM @IDX;
SELECT
	@AllWayId = (SELECT COUNT(DISTINCT [WayId]) FROM [dbo].[tWayCreation] W INNER JOIN [dbo].[tNode] N ON N.[Id] = W.[NodeId]),
	@CntWayId = 0,
	@MaxWayId = 0;
WHILE (1 = 1)
BEGIN
	RAISERROR('Inserted %I64d of %I64d', 0, 1, @CntWayId, @AllWayId) WITH NOWAIT;
	INSERT TOP(@TopWayId) INTO [dbo].[tWay] ([Id], [Line])
	OUTPUT INSERTED.[Id] INTO @IDX
	SELECT
		W.[WayId], [dbo].[CreateLineString]([Latitude], [Longitude], [Sort])
	FROM [dbo].[tWayCreation] W
		INNER JOIN [dbo].[tNode] N
			ON N.[Id] = W.[NodeId]
	WHERE W.[WayId] > @MaxWayId
	GROUP BY W.[WayId];
	IF (0 = @@ROWCOUNT)
		BREAK;
	SELECT
		@CntWayId = @CntWayId + COALESCE(COUNT(*),     0),
		@MaxWayId =             COALESCE(MAX([WayId]), 0)
	FROM @IDX;
	DELETE FROM @IDX;
	IF (0 < @MaxWayId OR 0 < @CntWayId)
		WAITFOR DELAY '00:00:01';
END;
GO