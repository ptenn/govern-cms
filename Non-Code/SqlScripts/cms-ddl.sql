USE [munidb]
GO
CREATE SCHEMA cms
GO
/****** Object:  Table [cms].[Artifact]    Script Date: 8/28/2017 12:36:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cms].[Artifact](
	[ArtifactId] [int] NOT NULL,
	[Version] [int] NOT NULL,
	[Name] [varchar](500) NOT NULL,
	[Description] [text] NULL,
	[OwnerId] [int] NOT NULL,
	[OrganizationId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[UpdateDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Artifact] PRIMARY KEY CLUSTERED 
(
	[ArtifactId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
ALTER TABLE [cms].[Artifact]  WITH CHECK ADD  CONSTRAINT [FK_Artifact_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [cms].[Organization] ([OrganizationId])
GO
ALTER TABLE [cms].[Artifact] CHECK CONSTRAINT [FK_Artifact_Organization]
GO
ALTER TABLE [cms].[Artifact]  WITH CHECK ADD  CONSTRAINT [FK_Artifact_Owner] FOREIGN KEY([OwnerId])
REFERENCES [cms].[User] ([UserId])
GO
ALTER TABLE [cms].[Artifact] CHECK CONSTRAINT [FK_Artifact_Owner]
GO
/****** Object:  Table [cms].[ArtifactGroupEditor]    Script Date: 8/28/2017 12:36:55 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cms].[ArtifactGroupEditor](
	[ArtifactId] [int] NOT NULL,
	[GroupId] [int] NOT NULL
)
GO
/****** Object:  Table [cms].[ArtifactUserEditor]    Script Date: 8/28/2017 12:36:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cms].[ArtifactUserEditor](
	[ArtifactId] [int] NOT NULL,
	[UserId] [int] NOT NULL
)
GO
/****** Object:  Table [cms].[Content]    Script Date: 8/28/2017 12:36:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cms].[Content](
	[ContentId] [int] NOT NULL,
	[Version] [int] NOT NULL,
	[ContentHtml] [ntext] NULL,
	[ContentUrl] [varchar](255) NULL,
	[ArtifactId] [int] NOT NULL,
	[CreatorId] [int] NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[UpdateDate] [datetime2](7) NULL,
 CONSTRAINT [PK_Content] PRIMARY KEY CLUSTERED 
(
	[ContentId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
/****** Object:  Table [cms].[Group]    Script Date: 8/28/2017 12:36:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cms].[Group](
	[Id] [int] NOT NULL,
	[GroupName] [varchar](500) NOT NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[OrganizationId] [int] NOT NULL,
 CONSTRAINT [PK_Group] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
/****** Object:  Table [cms].[LoginAttempt]    Script Date: 8/28/2017 12:36:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cms].[LoginAttempt](
	[LoginAttemptId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[ServerVariables] [text] NOT NULL,
 CONSTRAINT [PK_LoginAttempt] PRIMARY KEY CLUSTERED 
(
	[LoginAttemptId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
/****** Object:  Table [cms].[Organization]    Script Date: 8/28/2017 12:36:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cms].[Organization](
	[OrganizationId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [varchar](255) NULL,
	[Url] [varchar](255) NULL,
	[EmailHost] [varchar](255) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[Slug] [varchar](255) NULL,
 CONSTRAINT [PK_Organization] PRIMARY KEY NONCLUSTERED 
(
	[OrganizationId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
/****** Object:  Table [cms].[User]    Script Date: 8/28/2017 12:36:56 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [cms].[User](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[EmailAddr] [varchar](255) NOT NULL,
	[Passwd] [varchar](255) NOT NULL,
	[Salt] [int] NOT NULL,
	[FirstName] [varchar](100) NOT NULL,
	[LastName] [varchar](100) NOT NULL,
	[GroupId] [int] NOT NULL,
	[OrganizationId] [int] NOT NULL,
	[Admin] [bit] NOT NULL,
	[Type] [varchar](25) NULL,
	[CreateDate] [datetime2](7) NOT NULL,
	[UpdateDate] [datetime2](7) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON),
 CONSTRAINT [AK_EmailAddr] UNIQUE NONCLUSTERED 
(
	[EmailAddr] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON)
)
GO
ALTER TABLE [cms].[ArtifactGroupEditor]  WITH CHECK ADD  CONSTRAINT [FK_ArtifactGroupEditor_Artifact] FOREIGN KEY([ArtifactId])
REFERENCES [cms].[Artifact] ([ArtifactId])
GO
ALTER TABLE [cms].[ArtifactGroupEditor] CHECK CONSTRAINT [FK_ArtifactGroupEditor_Artifact]
GO
ALTER TABLE [cms].[ArtifactGroupEditor]  WITH CHECK ADD  CONSTRAINT [FK_ArtifactGroupEditor_Group] FOREIGN KEY([GroupId])
REFERENCES [cms].[Group] ([Id])
GO
ALTER TABLE [cms].[ArtifactGroupEditor] CHECK CONSTRAINT [FK_ArtifactGroupEditor_Group]
GO
ALTER TABLE [cms].[ArtifactUserEditor]  WITH CHECK ADD  CONSTRAINT [FK_ArtifactUserEditor_Artifact] FOREIGN KEY([ArtifactId])
REFERENCES [cms].[Artifact] ([ArtifactId])
GO
ALTER TABLE [cms].[ArtifactUserEditor] CHECK CONSTRAINT [FK_ArtifactUserEditor_Artifact]
GO
ALTER TABLE [cms].[ArtifactUserEditor]  WITH CHECK ADD  CONSTRAINT [FK_ArtifactUserEditor_User] FOREIGN KEY([UserId])
REFERENCES [cms].[User] ([UserId])
GO
ALTER TABLE [cms].[ArtifactUserEditor] CHECK CONSTRAINT [FK_ArtifactUserEditor_User]
GO
ALTER TABLE [cms].[Content]  WITH CHECK ADD  CONSTRAINT [FK_Content_Artifact] FOREIGN KEY([ArtifactId])
REFERENCES [cms].[Artifact] ([ArtifactId])
GO
ALTER TABLE [cms].[Content] CHECK CONSTRAINT [FK_Content_Artifact]
GO
ALTER TABLE [cms].[Content]  WITH CHECK ADD  CONSTRAINT [FK_Content_User] FOREIGN KEY([CreatorId])
REFERENCES [cms].[User] ([UserId])
GO
ALTER TABLE [cms].[Content] CHECK CONSTRAINT [FK_Content_User]
GO
ALTER TABLE [cms].[Group]  WITH CHECK ADD  CONSTRAINT [FK_Group_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [cms].[Organization] ([OrganizationId])
GO
ALTER TABLE [cms].[Group] CHECK CONSTRAINT [FK_Group_Organization]
GO
ALTER TABLE [cms].[LoginAttempt]  WITH CHECK ADD  CONSTRAINT [FK_LoginAttempt_User] FOREIGN KEY([UserId])
REFERENCES [cms].[User] ([UserId])
GO
ALTER TABLE [cms].[LoginAttempt] CHECK CONSTRAINT [FK_LoginAttempt_User]
GO
ALTER TABLE [cms].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Group] FOREIGN KEY([GroupId])
REFERENCES [cms].[Group] ([Id])
GO
ALTER TABLE [cms].[User] CHECK CONSTRAINT [FK_User_Group]
GO
ALTER TABLE [cms].[User]  WITH CHECK ADD  CONSTRAINT [FK_User_Organization] FOREIGN KEY([OrganizationId])
REFERENCES [cms].[Organization] ([OrganizationId])
GO
ALTER TABLE [cms].[User] CHECK CONSTRAINT [FK_User_Organization]
GO
