using System;
using System.Collections.Specialized;
using System.IO;
using NUnit.Framework;
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

    private readonly OrderedDictionary _ConstructorDefaultParams = GetConstructorDefaultParams();

    private ExtractArtifactsDeploymentStep _deploymentStep;

    #region SetUp and TearDown

    [SetUp]
    public void SetUp()
    {
      _environmentInfo = DeploymentDataGenerator.GetEnvironmentInfo();
      _deploymentInfo = DeploymentInfoGenerator.GetDbDeploymentInfo();
      _projectInfo = ProjectInfoGenerator.GetTerminalAppProjectInfo();

      _deploymentStep =
        new ExtractArtifactsDeploymentStep(
          _projectInfo,
          _environmentInfo,
          _deploymentInfo,
          _ArtifactsFilePath,
          _TargetArtifactsDirPath);
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
        () => ReflectionTestTools.CreateInstance<ExtractArtifactsDeploymentStep>(_ConstructorDefaultParams, nullArgumentName));
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
          _TargetArtifactsDirPath);

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
          _TargetArtifactsDirPath);

      _deploymentStep.Prepare();

      // act, assert
      Assert.IsFalse(_deploymentStep.BinariesDirPath.Contains(_environmentInfo.ConfigurationTemplateName));
    }

    [Test]
    public void DoExecute_extracts_artifacts()
    {
      // arrange
      string expectedPath = Path.Combine(_TargetArtifactsDirPath, _projectInfo.ArtifactsRepositoryDirName);
      const int filesCountInArtifacts = 2;

      if (Directory.Exists(expectedPath))
      {
        Directory.Delete(_TargetArtifactsDirPath, true);
      }

      // act
      _deploymentStep.PrepareAndExecute();

      // assert
      Assert.True(Directory.Exists(expectedPath));
      Assert.AreEqual(filesCountInArtifacts, Directory.GetFiles(expectedPath).Length);
    }

    #endregion

    #region Private helper methods.

    private static OrderedDictionary GetConstructorDefaultParams()
    {
      OrderedDictionary defaultParams =
        new OrderedDictionary
        {
          { "projectInfo", ProjectInfoGenerator.GetTerminalAppProjectInfo() },
          { "environmentInfo", DeploymentDataGenerator.GetEnvironmentInfo() },
          { "deploymentInfo", DeploymentInfoGenerator.GetTerminalAppDeploymentInfo() },
          { "artifactsFilePath", _ArtifactsFilePath },
          { "targetArtifactsDirPath", _TargetArtifactsDirPath }
        };

      return defaultParams;
    }

    #endregion
  }
}
