USE [OSM];

CREATE TABLE [dbo].[tWayCreation]
(
	[WayId]  bigint NOT NULL,
	[NodeId] bigint NOT NULL,
	[Sort]   int    NOT NULL
);
CREATE TABLE [dbo].[tWay]
(
	[Id]   bigint    NOT NULL,
	[line] geography     NULL
);
CREATE TABLE [dbo].[tWayTag]
(
	[WayId] bigint        NOT NULL,
	[Typ]   int           NOT NULL,
	[Info]  nvarchar(max) NOT NULL
);
CREATE TABLE [dbo].[tNode]
(
	[Id]        bigint    NOT NULL,
	[Location]  geography NOT NULL,
	[Latitude]  float     NOT NULL,
	[Longitude] float     NOT NULL
);
CREATE TABLE [dbo].[tNodeTag]
(
	[NodeId] bigint         NOT NULL,
	[Typ]    int            NOT NULL,
	[Info]   nvarchar(1000) NOT NULL
);
CREATE TABLE [dbo].[tTagType]
(
	[Typ]  int           NOT NULL CONSTRAINT [PK_tTagType] PRIMARY KEY CLUSTERED,
	[Name] nvarchar(255)     NULL
);
CREATE TABLE [dbo].[tRelationCreation]
(
	[RelationId] bigint NOT NULL,
	[Ref]        bigint NOT NULL,
	[Type]       int    NOT NULL,
	[Role]       int    NOT NULL,
	[Sort]       int    NOT NULL
);
CREATE TABLE [dbo].[tRelation]
(
	[Id]   bigint    NOT NULL,
	[Geo]  geography     NULL,
	[Role] int       NOT NULL
);
CREATE TABLE [dbo].[tRelationTag]
(
	[RelationId] bigint        NOT NULL,
	[Typ]        int           NOT NULL,
	[Info]       nvarchar(max) NOT NULL
);
CREATE TABLE [dbo].[tMemberType]
(
	[Id]   int           NOT NULL CONSTRAINT [PK_tMemberType_Id] PRIMARY KEY CLUSTERED,
	[Name] nvarchar(255)     NULL
);
CREATE TABLE [dbo].[tMemberRole]
(
	[Id]   int           NOT NULL CONSTRAINT [PK_tMemberRole_Id] PRIMARY KEY CLUSTERED,
	[Name] nvarchar(255)     NULL
);