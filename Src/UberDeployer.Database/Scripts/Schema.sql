USE [UberDeployer]
GO

CREATE TABLE [DeploymentRequest] (
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[DateStarted] [datetime] NOT NULL,
	[DateFinished] [datetime] NOT NULL,
	[RequesterIdentity] [nvarchar](256) NOT NULL,
	[ProjectName] [nvarchar](256) NOT NULL,
	[ProjectConfigurationName] [nvarchar](256) NOT NULL,
	[ProjectConfigurationBuildId] [nvarchar](256) NOT NULL,
	[TargetEnvironmentName] [nvarchar](64) NOT NULL,
	[FinishedSuccessfully] [bit] NOT NULL,
  CONSTRAINT [PK_DeploymentRequest] PRIMARY KEY clustered (
	  [Id] ASC
  )
)
GO

CREATE NONCLUSTERED INDEX [IX_DeploymentRequest_DateFinished] ON [DeploymentRequest] (
	[DateFinished] DESC
)
GO
