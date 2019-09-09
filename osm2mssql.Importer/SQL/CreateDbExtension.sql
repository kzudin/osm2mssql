﻿exec sp_configure 'show advanced options', 1;
RECONFIGURE;
exec sp_configure 'clr enabled', 1;
RECONFIGURE;
GO
use [OSM];
GO
if exists (select * from sys.objects where name = 'CreateLineString')
	drop aggregate dbo.CreateLineString;
GO
if exists (select * from sys.objects where name = 'GeographyUnion')
	drop aggregate dbo.GeographyUnion;
GO
if exists (select * from sys.objects where name = 'ConvertToPolygon')
	drop function dbo.ConvertToPolygon;
GO
if exists (select * from sys.assemblies where name = 'osm2mssqlSqlExtension')
	drop assembly osm2mssqlSqlExtension;
GO
create assembly osm2mssqlSqlExtension FROM [DllExtension] WITH PERMISSION_SET = UNSAFE;
GO
create aggregate dbo.CreateLineString(@lat float,@lon float,@sort int) returns geography
	external name osm2mssqlSqlExtension.[osm2mssql.DbExtensions.LineStringBuilder];
GO
create aggregate dbo.GeographyUnion(@geo geography) returns geography
	external name osm2mssqlSqlExtension.[osm2mssql.DbExtensions.GeographyUnion];
GO
create function dbo.ConvertToPolygon(@geo geography) returns geography 
	external name [osm2mssqlSqlExtension].[osm2mssql.DbExtensions.Functions].ConvertToPolygon;
GO