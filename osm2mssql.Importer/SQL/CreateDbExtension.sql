EXEC sp_configure 'show advanced options', 1;
RECONFIGURE;
EXEC sp_configure 'clr enabled', 1;
RECONFIGURE;
GO
USE [OSM];
GO
IF EXISTS (SELECT * FROM sys.objects WHERE [name] = 'CreateLineString')
	DROP AGGREGATE [dbo].[CreateLineString];
GO
IF EXISTS (SELECT * FROM sys.objects WHERE [name] = 'GeographyUnion')
	DROP AGGREGATE [dbo].[GeographyUnion];
GO
IF EXISTS (SELECT * FROM sys.objects WHERE [name] = 'ConvertToPolygon')
	DROP FUNCTION [dbo].[ConvertToPolygon];
GO
IF EXISTS (SELECT * FROM sys.assemblies WHERE [name] = 'osm2mssqlSqlExtension')
	DROP ASSEMBLY osm2mssqlSqlExtension;
GO
CREATE ASSEMBLY osm2mssqlSqlExtension FROM [DllExtension] WITH PERMISSION_SET = UNSAFE;
GO
CREATE AGGREGATE dbo.CreateLineString(@lat float,@lon float,@sort int) RETURNS geography
	EXTERNAL NAME osm2mssqlSqlExtension.[osm2mssql.DbExtensions.LineStringBuilder];
GO
CREATE AGGREGATE dbo.GeographyUnion(@geo geography) RETURNS geography
	EXTERNAL NAME osm2mssqlSqlExtension.[osm2mssql.DbExtensions.GeographyUnion];
GO
CREATE FUNCTION dbo.ConvertToPolygon(@geo geography) RETURNS geography
	EXTERNAL NAME [osm2mssqlSqlExtension].[osm2mssql.DbExtensions.Functions].ConvertToPolygon;
GO