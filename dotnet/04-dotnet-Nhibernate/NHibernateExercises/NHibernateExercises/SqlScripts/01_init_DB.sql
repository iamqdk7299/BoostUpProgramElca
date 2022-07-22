SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[Project](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[Number] int NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[StartDate] [datetime] NOT NULL,
	[EndDate] [datetime] NULL,
	[RowVersion] int not NULL,
 CONSTRAINT [PK_dbo.Project] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO

SET IDENTITY_INSERT [dbo].[Project] ON 
GO

INSERT [dbo].[Project] ([Id], [Number], [Name], [StartDate], [EndDate], [RowVersion]) VALUES (1, 10001, N'WWS', CAST(N'2019-01-01 00:00:00.000' AS DateTime), NULL, 1)
INSERT [dbo].[Project] ([Id], [Number], [Name], [StartDate], [EndDate], [RowVersion]) VALUES (2, 10002, N'AAA', CAST(N'2019-01-01 00:00:00.000' AS DateTime), NULL, 1)
INSERT [dbo].[Project] ([Id], [Number], [Name], [StartDate], [EndDate], [RowVersion]) VALUES (3, 10003, N'AAB', CAST(N'2019-01-01 00:00:00.000' AS DateTime), NULL, 1)
INSERT [dbo].[Project] ([Id], [Number], [Name], [StartDate], [EndDate], [RowVersion]) VALUES (4, 10004, N'AAC', CAST(N'2019-01-01 00:00:00.000' AS DateTime), NULL, 1)
INSERT [dbo].[Project] ([Id], [Number], [Name], [StartDate], [EndDate], [RowVersion]) VALUES (5, 10005, N'AAD', CAST(N'2019-01-01 00:00:00.000' AS DateTime), NULL, 1)
INSERT [dbo].[Project] ([Id], [Number], [Name], [StartDate], [EndDate], [RowVersion]) VALUES (6, 10006, N'AAE', CAST(N'2019-01-01 00:00:00.000' AS DateTime), NULL, 1)
INSERT [dbo].[Project] ([Id], [Number], [Name], [StartDate], [EndDate], [RowVersion]) VALUES (7, 10007, N'AAF', CAST(N'2019-01-01 00:00:00.000' AS DateTime), NULL, 1)
INSERT [dbo].[Project] ([Id], [Number], [Name], [StartDate], [EndDate], [RowVersion]) VALUES (8, 10008, N'AAG', CAST(N'2019-01-01 00:00:00.000' AS DateTime), NULL, 1)
INSERT [dbo].[Project] ([Id], [Number], [Name], [StartDate], [EndDate], [RowVersion]) VALUES (9, 10009, N'AAH', CAST(N'2019-01-01 00:00:00.000' AS DateTime), NULL, 1)
INSERT [dbo].[Project] ([Id], [Number], [Name], [StartDate], [EndDate], [RowVersion]) VALUES (10, 10010, N'AAI', CAST(N'2019-01-01 00:00:00.000' AS DateTime), NULL, 1)

GO

SET IDENTITY_INSERT [dbo].[Project] OFF
GO

CREATE TABLE [dbo].[ParameterDefinition](
	[Id] int NOT NULL,
	[Name] [nvarchar](100) NOT NULL,
	[Type] [nvarchar](100) NOT NULL,
	[RowVersion] int not NULL,
 CONSTRAINT [PK_dbo.ParameterDefinition] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO

CREATE TABLE [dbo].[ParameterValue](
	[Id] int NOT NULL,
	[Value] [nvarchar](100),
	[Year] int,
	[RowVersion] int not NULL,
	[ParameterDefinitionId] int not NULL,
 CONSTRAINT [PK_dbo.ParameterValue] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] 

GO

ALTER TABLE [dbo].[ParameterValue]  WITH CHECK ADD  CONSTRAINT [FK_ParameterValue_ParameterDefinition] FOREIGN KEY([ParameterDefinitionId])
REFERENCES [dbo].[ParameterDefinition] ([Id])
GO

ALTER TABLE [dbo].[ParameterValue] CHECK CONSTRAINT [FK_ParameterValue_ParameterDefinition]
GO
