USE [UberDeployer]
GO

CREATE TABLE [dbo].[DeploymentRequest] (
  [Id] [int] IDENTITY(1,1) NOT NULL,
  [DateRequested] [datetime] NOT NULL,
  [RequesterIdentity] [nvarchar](256) NOT NULL,
  [ProjectName] [nvarchar](256) NOT NULL,
  [TargetEnvironmentName] [nvarchar](64) NOT NULL,
  [FinishedSuccessfully] [bit] NOT NULL,
  CONSTRAINT [PK_DeploymentRequest] PRIMARY KEY CLUSTERED (
    [Id] ASC
  )
)
GO
