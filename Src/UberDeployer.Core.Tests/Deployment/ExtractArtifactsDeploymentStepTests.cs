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

    private readonly OrderedDictionary _ConstructorDefaultParams = GetConstructorDefaultParams();

    private ExtractArtifactsDeploymentStep _deploymentStep;

    #region SetUp and TearDown

    [SetUp]
    public void SetUp()
    {
      _environmentInfo = DeploymentDataGenerator.GetEnvironmentInfo();      
      _deploymentInfo = DeploymentInfoGenerator.GetDbDeploymentInfo();

      _deploymentStep = new ExtractArtifactsDeploymentStep(
        _environmentInfo,        
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
      _deploymentStep.Prepare(_deploymentInfo);

      // act, assert
      Assert.IsNotNullOrEmpty(_deploymentStep.Description);
    }

    [Test]
    public void BinariesDirPath_returns_path_with_config_template_name_when_artifacts_are_env_specific()
    {
      // arrange
      var environmentInfo = DeploymentDataGenerator.GetEnvironmentInfo();

      _deploymentStep =
        new ExtractArtifactsDeploymentStep(
          environmentInfo,                    
          _ArtifactsFilePath,
          _TargetArtifactsDirPath);

      var envSpecificDeploymentInfo = DeploymentInfoGenerator.GetNtServiceDeploymentInfo(areEnvironmentSpecific: true);
      _deploymentStep.Prepare(envSpecificDeploymentInfo);

      // act, assert
      Assert.IsTrue(_deploymentStep.BinariesDirPath.Contains(environmentInfo.ConfigurationTemplateName));
    }

    [Test]
    public void BinariesDirPath_returns_path_without_config_template_name_when_artifacts_are_not_env_specific()
    {
      // arrange
      var environmentInfo = DeploymentDataGenerator.GetEnvironmentInfo();

      _deploymentStep = 
        new ExtractArtifactsDeploymentStep(
        environmentInfo,
        _ArtifactsFilePath,
        _TargetArtifactsDirPath);

      var envNotSpecificDeploymentInfo = DeploymentInfoGenerator.GetNtServiceDeploymentInfo(areEnvironmentSpecific: false);
      _deploymentStep.Prepare(envNotSpecificDeploymentInfo);

      // act, assert
      Assert.IsFalse(_deploymentStep.BinariesDirPath.Contains(environmentInfo.ConfigurationTemplateName));
    }

    [Test]
    public void DoExecute_extracts_artifacts()
    {
      // arrange
      string expectedPath = Path.Combine(_TargetArtifactsDirPath, _deploymentInfo.ProjectInfo.ArtifactsRepositoryDirName);
      const int filesCountInArtifacts = 2;

      if (Directory.Exists(expectedPath))
      {
        Directory.Delete(_TargetArtifactsDirPath, true);
      }
      
      // act
      _deploymentStep.PrepareAndExecute(_deploymentInfo);

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
                                   { "environmentInfo", DeploymentDataGenerator.GetEnvironmentInfo() },                                   
                                   { "artifactsFilePath", _ArtifactsFilePath },
                                   { "targetArtifactsDirPath", _TargetArtifactsDirPath }
                                 };

      return defaultParams;
    }

    #endregion
  }
}
