 var g_AppPrefix = '/';

var g_lastSeenMessageId = -1;
var g_DiagnosticMessagesLoaderInterval = 500;

var g_TargetEnvironmentCookieName = 'target-environment-name';
var g_TargetEnvironmentCookieExpirationInDays = 365;

var g_ProjectList = [];

var KICKASSVERSION = '2.0';

function setAppPrefix(appPrefix) {
  g_AppPrefix = appPrefix;
}

var APP_TYPES = {
  NotSet: 0,
  Db: 1,
  NtService: 2,
  SchedulerApp: 3,
  TerminalApp: 4,
  WebApp: 5,
  WebService: 6
};

function Project(name, type) {
  var self = this;  

  self.name = '';
  self.type = '';

  self.update = function(projectName, projectType) {
    self.name = projectName;
    self.type = projectType;
  };

  self.update(name, type);
}

function initializeDeploymentPage() {
  $.ajaxSetup({
    'error': function (error) {
      domHelper.showError(error);
    }
  });
  
  $('#btn-deploy').click(function () {
    deploy();
  });

  domHelper.getProjectsElement().change(function () {
    var projectName = getSelectedProjectName();

    if (!projectName) {
      return;
    }

    clearProjectConfigurationBuilds();

    loadProjectConfigurations(projectName);

    domHelper.getEnvironmentsElement().trigger('change');
  });
  
  domHelper.getProjectConfigsElement().change(function () {
    var projectName = getSelectedProjectName();
    var projectConfigurationName = getSelectedProjectConfigurationName();

    if (!projectName || !projectConfigurationName) {
      return;
    }

    clearProjectConfigurationBuilds();

    loadProjectConfigurationBuilds(
      projectName,
      projectConfigurationName,
      function () {
        // select first element
        $('#lst-project-config-builds')
          .val($('#lst-project-config-builds option').eq(0).val());

        $('#lst-project-config-builds').trigger('change');
      });
  });

  loadProjects(function () {
    //// select first element
    domHelper.getProjectsElement()
      .val($('#lst-projects option').eq(0).val());

    domHelper.getProjectsElement().trigger('change');
  });

  loadEnvironments(function () {
    restoreRememberedTargetEnvironmentName();
  });

  domHelper.getEnvironmentsElement().change(function () {
    rememberTargetEnvironmentName();
    loadWebMachinesList();
  });
  
  startDiagnosticMessagesLoader();
}

function deploy() {
  var projectName = getSelectedProjectName();
  var projectConfigurationName = getSelectedProjectConfigurationName();
  var projectConfigurationBuildId = getSelectedProjectConfigurationBuildId();
  var targetEnvironmentName = getSelectedTargetEnvironmentName();
  var targetMachines = getSelectedTargetMachines();    

  if (!projectName || !projectConfigurationName || !projectConfigurationBuildId || !targetEnvironmentName) {
    alert('Selected project, configuration, build and environment!');
    return;
  }

  if (g_ProjectList[projectName].type == APP_TYPES.WebApp && (!targetMachines || targetMachines.length == 0)) {
    alert('Select web machine for selected environment!');
    return;
  }

  $.ajax({
    url: g_AppPrefix + 'Api/Deploy',
    type: "POST",
    data: {
      projectName: projectName,
      projectConfigurationName: projectConfigurationName,
      projectConfigurationBuildId: projectConfigurationBuildId,
      targetEnvironmentName: targetEnvironmentName,
      projectType: g_ProjectList[projectName].type,
      targetMachines: targetMachines
    },
    traditional: true,
    success: function(data) {
      if (!data.status || data.status.toLowerCase() === 'fail') {
        var message;

        if (data.errorMessage) {
          message = data.errorMessage;
        } else {
          message = '(no error message)';
        }

        logMessage('Error: ' + message, 'error');
      }
    }
  });
}

function loadEnvironments(onFinishedCallback) {
  clearEnvironments();

  $.getJSON(
    g_AppPrefix + 'Api/GetEnvironments',
    function(data) {
      clearEnvironments();

      $.each(data.environments, function(i, val) {
        domHelper.getEnvironmentsElement()
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

function loadWebMachinesList() {
  var $lstEnvironments = domHelper.getEnvironmentsElement();
  var $lstMachines = domHelper.getMachinesElement();
  var selectedProject = domHelper.getProjectsElement().val();

  clearTargetMachines();

  if (g_ProjectList[selectedProject].type != APP_TYPES.WebApp) {
    $lstMachines.attr('disabled', 'disabled');
    return;
  }

  $.getJSON(
    g_AppPrefix + 'Api/GetWebMachineNames',
    { envName: $lstEnvironments.val() },
    function(machines) {
      clearTargetMachines();

      $lstMachines.removeAttr('disabled');

      $.each(machines, function(i, val) {
        $lstMachines.append(
          $('<option></option>')
            .attr('value', val)
            .attr('selected', 'selected')
            .text(val));
      });
    });
}

function loadProjects(onFinishedCallback) {
  clearProjects();

  $.getJSON(
    g_AppPrefix + 'Api/GetProjects',
    function(data) {
      g_ProjectList = [];
      clearProjects();

      $.each(data.projects, function(i, val) {
        g_ProjectList[val.Name] = new Project(val.Name, val.Type);

        domHelper.getProjectsElement()
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

function loadProjectConfigurations(projectName, onFinishedCallback) {
  clearProjectConfigurations();

  $.getJSON(
      g_AppPrefix + 'Api/GetProjectConfigurations?projectName=' + encodeURIComponent(projectName),
      function (data) {
        clearProjectConfigurations();

        $.each(data.projectConfigurations, function(i, val) {
          var $lstProjectConfigs = $('#lst-project-configs');

          $lstProjectConfigs
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

function loadProjectConfigurationBuilds(projectName, projectConfigurationName, onFinishedCallback) {
  clearProjectConfigurationBuilds();

  $.getJSON(
    g_AppPrefix + 'Api/GetProjectConfigurationBuilds?projectName=' + encodeURIComponent(projectName) + '&projectConfigurationName=' + encodeURIComponent(projectConfigurationName),
    function (data) {
      clearProjectConfigurationBuilds();

      $.each(data.projectConfigurationBuilds, function(i, val) {
        var $lstProjectConfigBuilds = $('#lst-project-config-builds');

        $lstProjectConfigBuilds
          .append(
            $('<option></option>')
              .attr('value', val.Id)
              .text(val.StartDate + ' | ' + val.StartTime + ' | ' + val.Number + ' | ' + val.Status));
      });

      if (onFinishedCallback) {
        onFinishedCallback();
      }
    });
}

function startDiagnosticMessagesLoader() {
  loadNewDiagnosticMessages();

  setTimeout(
    function() {
      startDiagnosticMessagesLoader();
    },
    g_DiagnosticMessagesLoaderInterval);
}

function loadNewDiagnosticMessages() {
  $.getJSON(
    g_AppPrefix + 'Api/GetDiagnosticMessages?lastSeenMaxMessageId=' + g_lastSeenMessageId,
    function(data) {
      $.each(data.messages, function(i, val) {
        if (val.MessageId > g_lastSeenMessageId) {
          logMessage(val.Message, val.Type);
          g_lastSeenMessageId = val.MessageId;
        }
      });
    });
  }

function domHelper() {}

domHelper.getProjectsElement = function() {
  return $('#lst-projects');
};

domHelper.getProjectConfigsElement = function () {
  return $('#lst-project-configs');
};

domHelper.getEnvironmentsElement = function() {
  return $('#lst-environments');
};

domHelper.getMachinesElement = function() {
  return $('#lst-machines');
};

domHelper.getSelectedMachines = function() {
  return $.map($('#lst-machines option:selected'), function (item) { return $(item).val(); });
};

domHelper.showError = function(error) {
  $('#errorMsg').html('<strong>Error</strong>, Status Code:' + error.status + ' ' + error.statusText + ' ' + error.responseText);
  $('#errorMsg').show();
  $('#main-container').hide();
};

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

function getSelectedTargetMachines() {
  return domHelper.getSelectedMachines();
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

function clearTargetMachines() {
  domHelper.getMachinesElement().empty();
}

function rememberTargetEnvironmentName() {
  var targetEnvironmentName = getSelectedTargetEnvironmentName();
  
  if (!targetEnvironmentName) {
    return;
  }

  setCookie(
    g_TargetEnvironmentCookieName,
    targetEnvironmentName,
    g_TargetEnvironmentCookieExpirationInDays);
}

function restoreRememberedTargetEnvironmentName() {
  var cookie = getCookie(g_TargetEnvironmentCookieName);

  if (cookie === null) {
    var firstVal = domHelper.getEnvironmentsElement().find('option').eq(0).val();

    if (firstVal !== undefined) {
      domHelper.getEnvironmentsElement().val(firstVal);
      domHelper.getEnvironmentsElement().trigger('change');
    }

    return;
  }

  domHelper.getEnvironmentsElement().val(cookie);
  domHelper.getEnvironmentsElement().trigger('change');
}

function setCookie(c_name, value, exdays) {
  var exdate = new Date();

  exdate.setDate(exdate.getDate() + exdays);

  var c_value = escape(value) + ((exdays == null) ? "" : "; expires=" + exdate.toUTCString());

  document.cookie = c_name + "=" + c_value;
}

function getCookie(c_name) {
  var i, x, y, ARRcookies = document.cookie.split(";");

  for (i = 0; i < ARRcookies.length; i++) {
    x = ARRcookies[i].substr(0, ARRcookies[i].indexOf("="));
    y = ARRcookies[i].substr(ARRcookies[i].indexOf("=") + 1);
    x = x.replace(/^\s+|\s+$/g, "");

    if (x == c_name) {
      return unescape(y);
    }
  }

  return null;
}

function alternateTableRows(tableId) {
  $('#' + tableId + ' tr:even')
    .each(function() {
      $(this).addClass('even');
    });
}

function logMessage(message, type) {
  var $txtLogs = $('#txt-logs');
  var $logMsg = $('<div></div>');

  $logMsg.text('>> ' + message);
  $logMsg.addClass('log-msg');

  var typeLower = type.toLowerCase();

  if (typeLower === 'trace') {
    $logMsg.addClass('log-msg-trace');
  }
  else if (typeLower == 'info') {
    $logMsg.addClass('log-msg-info');
  }
  else if (typeLower == 'warn') {
    $logMsg.addClass('log-msg-warn');
  }
  else if (typeLower == 'error') {
    $logMsg.addClass('log-msg-error');
  }

  $txtLogs.append($logMsg);
  $txtLogs.scrollTop($txtLogs[0].scrollHeight - $txtLogs.height());
}

function kickAss() {
  var s = document.createElement('script');
  
  s.type = 'text/javascript';
  document.body.appendChild(s);
  s.src = '//hi.kickassapp.com/kickass.js';
}

$(document).ready(function() {
  // do nothing
});
