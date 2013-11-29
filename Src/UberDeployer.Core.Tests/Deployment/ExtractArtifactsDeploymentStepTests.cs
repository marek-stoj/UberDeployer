using System;
using System.Collections.Specialized;
using Moq;
using NUnit.Framework;
using UberDeployer.Common.IO;
using UberDeployer.Core.Deployment;
using UberDeployer.Core.Domain;
using UberDeployer.Core.Tests.Generators;
using UberDeployer.Core.Tests.TestUtils;

namespace UberDeployer.Core.Tests.Deployment
{
  [TestFixture]
  public class ExtractArtifactsDeploymentStepTests
  {
    private const string _ArtifactsFilePath = @"TestData\TestArtifacts\artifacts.zip";
    private const string _TargetArtifactsDirPath = "target_artifacts_dir_path";

    private EnvironmentInfo _environmentInfo;
    private DeploymentInfo _deploymentInfo;
    private ProjectInfo _projectInfo;
    private Mock<IFileAdapter> _fileAdapterFake;
    private Mock<IZipFileAdapter> _zipFileAdapterFake;

    private ExtractArtifactsDeploymentStep _deploymentStep;

    #region SetUp and TearDown

    [SetUp]
    public void SetUp()
    {
      _environmentInfo = DeploymentDataGenerator.GetEnvironmentInfo();
      _deploymentInfo = DeploymentInfoGenerator.GetDbDeploymentInfo();
      _projectInfo = ProjectInfoGenerator.GetTerminalAppProjectInfo();
      _fileAdapterFake = new Mock<IFileAdapter>(MockBehavior.Loose);
      _zipFileAdapterFake = new Mock<IZipFileAdapter>(MockBehavior.Loose);

      _deploymentStep =
        new ExtractArtifactsDeploymentStep(
          _projectInfo,
          _environmentInfo,
          _deploymentInfo,
          _ArtifactsFilePath,
          _TargetArtifactsDirPath,
          _fileAdapterFake.Object,
          _zipFileAdapterFake.Object);
    }

    #endregion

    #region Tests

    [Test]
    [TestCase("environmentInfo", typeof(ArgumentNullException))]
    [TestCase("artifactsFilePath", typeof(ArgumentException))]
    [TestCase("targetArtifactsDirPath", typeof(ArgumentException))]
    public void Constructor_fails_when_parameter_is_null(string nullArgumentName, Type exceptionType)
    {
      // act, assert
      Assert.Throws(
        exceptionType,
        () => ReflectionTestTools.CreateInstance<ExtractArtifactsDeploymentStep>(GetConstructorDefaultParams(), nullArgumentName));
    }

    [Test]
    public void Description_is_not_empty()
    {
      // arrange
      _deploymentStep.Prepare();

      // act, assert
      Assert.IsNotNullOrEmpty(_deploymentStep.Description);
    }

    [Test]
    public void BinariesDirPath_returns_path_with_config_template_name_when_artifacts_are_env_specific()
    {
      // arrange
      var environmentInfo = DeploymentDataGenerator.GetEnvironmentInfo();

      NtServiceProjectInfo projectInfo =
        ProjectInfoGenerator.GetNtServiceProjectInfo(areEnvironmentSpecific: true);

      _deploymentStep =
        new ExtractArtifactsDeploymentStep(
          projectInfo,
          environmentInfo,
          _deploymentInfo,
          _ArtifactsFilePath,
          _TargetArtifactsDirPath,
          _fileAdapterFake.Object,
          _zipFileAdapterFake.Object);

      _deploymentStep.Prepare();

      // act, assert
      Assert.IsTrue(_deploymentStep.BinariesDirPath.Contains(environmentInfo.ConfigurationTemplateName));
    }

    [Test]
    public void BinariesDirPath_returns_path_without_config_template_name_when_artifacts_are_not_env_specific()
    {
      // arrange
      NtServiceProjectInfo projectInfo =
        ProjectInfoGenerator.GetNtServiceProjectInfo(areEnvironmentSpecific: false);

      _deploymentStep =
        new ExtractArtifactsDeploymentStep(
          projectInfo,
          _environmentInfo,
          _deploymentInfo,
          _ArtifactsFilePath,
          _TargetArtifactsDirPath,
          _fileAdapterFake.Object,
          _zipFileAdapterFake.Object);

      _deploymentStep.Prepare();

      // act, assert
      Assert.IsFalse(_deploymentStep.BinariesDirPath.Contains(_environmentInfo.ConfigurationTemplateName));
    }

    [Test]
    public void DoExecute_extracts_artifacts()
    {
      // arrange
      _fileAdapterFake.Setup(x => x.Exists(It.IsAny<string>()))
        .Returns(true);

      // act
      _deploymentStep.PrepareAndExecute();

      // assert
      _zipFileAdapterFake.Verify(
        x => x.ExtractAll(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()),
        Times.Exactly(2));
    }

    #endregion

    #region Private helper methods.

    private OrderedDictionary GetConstructorDefaultParams()
    {
      OrderedDictionary defaultParams =
        new OrderedDictionary
        {
          { "projectInfo", ProjectInfoGenerator.GetTerminalAppProjectInfo() },
          { "environmentInfo", DeploymentDataGenerator.GetEnvironmentInfo() },
          { "deploymentInfo", DeploymentInfoGenerator.GetTerminalAppDeploymentInfo() },
          { "artifactsFilePath", _ArtifactsFilePath },
          { "targetArtifactsDirPath", _TargetArtifactsDirPath },
          { "fileAdapter", _fileAdapterFake.Object },
          { "zipFileAdapter", _zipFileAdapterFake.Object },
        };

      return defaultParams;
    }

    #endregion
  }
}
