function initializeDashboard() {
  $('#lst-projects').change(function() {
    var projectName = getSelectedProjectName();

    if (!projectName) {
      return;
    }

    clearProjectConfigurationBuilds();

    loadProjectConfigurations(projectName);
  });

  $('#lst-project-configs').change(function() {
    var projectName = getSelectedProjectName();
    var projectConfigurationName = getSelectedProjectConfigurationName();

    if (!projectName || !projectConfigurationName) {
      return;
    }

    clearProjectConfigurationBuilds();

    loadProjectConfigurationBuilds(projectName, projectConfigurationName);
  });

  $('#btn-deploy').click(function() {
    deploy();
  });

  loadEnvironments();

  loadProjects(function() {
    // select first element
    $('#lst-projects')
      .val($('#lst-projects option').eq(0).val());

    $('#lst-projects').trigger('change');
  });
}

function deploy() {
  var projectName = getSelectedProjectName();
  var projectConfigurationName = getSelectedProjectConfigurationName();
  var projectConfigurationBuildId = getSelectedProjectConfigurationBuildId();
  var targetEnvironmentName = getSelectedTargetEnvironmentName();

  if (!projectName || !projectConfigurationName || !projectConfigurationBuildId || !targetEnvironmentName) {
    alert('Selected project, configuration, build and environment!');
    return;
  }

  $.post(
    '/Api/Deploy',
    {
      projectName: projectName,
      projectConfigurationName: projectConfigurationName,
      projectConfigurationBuildId: projectConfigurationBuildId,
      targetEnvironmentName: targetEnvironmentName
    });
}

function loadEnvironments() {
  clearEnvironments();

  $.getJSON(
    '/Api/GetEnvironments',
    function(data) {
      $.each(data.environments, function(i, val) {
        var $lstEnvironments = $('#lst-environments');

        $lstEnvironments
          .append(
            $('<option></option>')
              .attr('value', val.Name)
              .text(val.Name));
      });
    });
}

function loadProjects(onFinishedCallback) {
  clearProjects();

  $.getJSON(
    '/Api/GetProjects',
    function(data) {
      var $lstProjects = $('#lst-projects');

      $.each(data.projects, function(i, val) {
        $lstProjects
          .append(
            $('<option></option>')
              .attr('value', val.Name)
              .text(val.Name));
      });

      if (onFinishedCallback) {
        onFinishedCallback();
      }
    });
}

function loadProjectConfigurations(projectName) {
  clearProjectConfigurations();

  $.getJSON(
      '/Api/GetProjectConfigurations?projectName=' + encodeURIComponent(projectName),
      function(data) {
        $.each(data.projectConfigurations, function(i, val) {
          var $lstProjectConfigs = $('#lst-project-configs');
          
          $lstProjectConfigs
            .append(
              $('<option></option>')
                .attr('value', val.Name)
                .text(val.Name));
        });
      });
}

function loadProjectConfigurationBuilds(projectName, projectConfigurationName) {
  clearProjectConfigurationBuilds();

  $.getJSON(
      '/Api/GetProjectConfigurationBuilds?projectName=' + encodeURIComponent(projectName) + '&projectConfigurationName=' + encodeURIComponent(projectConfigurationName),
      function(data) {
        $.each(data.projectConfigurationBuilds, function(i, val) {
          var $lstProjectConfigBuilds = $('#lst-project-config-builds');

          $lstProjectConfigBuilds
            .append(
              $('<option></option>')
                .attr('value', val.Id)
                .text(val.StartDate + ' | ' + val.StartTime + ' | ' + val.Number + ' | ' + val.Status));
        });
      });
}

function getSelectedTargetEnvironmentName() {
  return $('#lst-environments').val();
}

function getSelectedProjectName() {
  return $('#lst-projects').val();
}

function getSelectedProjectConfigurationName() {
  return $('#lst-project-configs').val();
}

function getSelectedProjectConfigurationBuildId() {
  return $('#lst-project-config-builds').val();
}

function clearEnvironments() {
  $('#lst-environments').empty();
}

function clearProjects() {
  $('#lst-projects').empty();
}

function clearProjectConfigurations() {
  $('#lst-project-configs').empty();
}

function clearProjectConfigurationBuilds() {
  $('#lst-project-config-builds').empty();
}

$(document).ready(function() {
  // do nothing
});
