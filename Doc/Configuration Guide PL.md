# Konfiguracja ÜberDeployera

## Konfiguracja serwisu WCF UberDeployer.Agent.NtService

Dane konfiguracyjne części usługowej ÜberDeployera trzymane są w plikach XML: `ApplicationConfiguration.xml`, `EnvironmentInfo_XYZ.xml` oraz `ProjectInfos.xml`.
Po zmianie konfiguracji konieczne jest wdrożenie usługi agenta ÜberDeployera. Niestety na tę chwilę nie da się wdrażać agenta ÜberDeployera ÜberDeployerem,
więc trzeba to robić ręcznie. W zasadzie wystarczy tylko podmieniać zmodyfikowane pliki konfiguracyjne (z podkatalogu Data), natomiast trzeba pamiętać
o zrestartowaniu usługi NT o nazwie `UberDeployer.Agent.NtService`.

W kontekście ÜberDeployera rozróźniamy 5 rodzajów aplikacji/projektów: usługi NT, aplikacje webowe, aplikacje harmonogramu zadań, aplikacje terminalowe,
projekty bazodanowe. Za chwilę po kolei omówimy sposób ich konfiguracji, wpierw jednak przyjrzyjmy się kilku elementom wspólnym w konfiguracji projektów.

### Konfiguracja projektów &mdash; elementy wspólne

Każdy projekt opisany jest przez element XML o nazwie `ProjectInfoXml`, który wymaga podania dwóch atrybutów:

- `xsi:Type` &mdash; typ projektu; dopuszczalne wartości: `NtServiceProjectInfoXml`, `WebAppProjectInfoXml`, `SchedulerAppProjectInfoXml`,
`TerminalAppProjectInfoXml`, `DbProjectInfoXml`.
- `allowedEnvironments` &mdash; lista nazw środowisk, na których dopuszczalne jest wdrożenie danego projektu, np.: `"dev1,dev2,test,prod"`.

W konfiguracji każdego projektu, niezależnie od typu, występują 3 elementy XML:

- `Name` &mdash; nazwa projektu; arbitralna &mdash; pojawia się na liście w aplikacji webowej ÜberDeployera.
- `ArtifactsRepositoryName` &mdash; nazwa projektu, jaka używana jest w repozytorium artefaktów (czyli w TeamCity).
- `ArtifactsRepositoryDirName` &mdash; nazwa katalogu w archiwum ZIP (pobieranym z repozytorium artefaktów), w którym znajdują się binarki danego projektu
(używane w sytuacji, gdy w artefaktach jednego projektu jest więcej, niezależnie wdrażanych, podprojektów).

### Konfiguracja usług NT

Przykładowa konfiguracja:

```xml
<ProjectInfoXml xsi:type="NtServiceProjectInfoXml" allowedEnvironments="dev1,dev2,test,prod">
  <Name>UberDeployer.SampleNtService</Name>
  <ArtifactsRepositoryName>UberDeployerSamples</ArtifactsRepositoryName>
  <ArtifactsRepositoryDirName>SampleNtService</ArtifactsRepositoryDirName>
  <NtServiceName>UberDeployer.SampleNtService</NtServiceName>
  <NtServiceDirName>UberDeployer.SampleNtService</NtServiceDirName>
  <NtServiceDisplayName>UberDeployer.SampleNtService</NtServiceDisplayName>
  <NtServiceExeName>UberDeployerSamples.SampleNtService.exe</NtServiceExeName>
  <NtServiceUserId>Sample.User</NtServiceUserId>
</ProjectInfoXml>
```

Omówienie elementów specyficznych dla usług NT:

- `NtServiceName` &mdash; nazwa usługi, jaka będzie zarejestrowana w systemie Windows.
- `NtServiceDirName` &mdash; nazwa podkatalogu na docelowym serwerze, w którym zostaną umieszczone binarki.
- `NtServiceDisplayName` &mdash; nazwa usługi NT na potrzeby wyświetlania na liście usług w systemie Windows.
- `NtServiceExeName` &mdash; nazwa pliku wykonywalnego usługi NT.
- `NtServiceUserId` &mdash; identyfikator użytkownika, na którym ma działać usługa NT. Jest to identyfikator wewnętrzny &mdash; używany tylko
w obrębie ÜberDeployera.

Uwagi:

- Dokładna ścieżka do binarek na docelowym serwerze zależy od środowiska, na które projekt jest wdrażany &mdash; patrz sekcja
"Konfiguracja środowisk".
- Jeśli środowisko, na które chcemy wdrożyć usługę NT, jest sklastrowane, w konfiguracji tego środowiska musi znajdować się element
określający, jak nazywa się grupa klastrowa danej usługi &mdash; patrz sekcja "Konfiguracja środowisk", element
`ProjectToFailoverClusterGroupMappings`.
- Mapowanie identyfikatorów użytkowników na nazwy użytkowników domenowych definiuje się per środowisko &mdash; patrz sekcja
"Konfiguracja środowisk".

### Konfiguracja aplikacji webowych

Przykładowa konfiguracja:

```xml
<ProjectInfoXml xsi:type="WebAppProjectInfoXml" allowedEnvironments="dev1,dev2,test,prod">
  <Name>UberDeployer.SampleWebApp</Name>
  <ArtifactsRepositoryName>UberDeployerSamples</ArtifactsRepositoryName>
  <ArtifactsRepositoryDirName>SampleWebApp</ArtifactsRepositoryDirName>
  <AppPoolId>ASP.NET v4.0</AppPoolId>
  <WebSiteName>Default Web Site</WebSiteName>
  <WebAppDirName>UberDeployer.SampleWebApp</WebAppDirName>
  <WebAppName>UberDeployer.SampleWebApp</WebAppName>
</ProjectInfoXml>
```

Omówienie elementów specyficznych dla aplikacji webowych:

- `AppPoolId` &mdash; wewnętrzny identyfikator puli aplikacji IIS, z której ma korzystać dana aplikacja. Patrz również element `AppPoolInfos`.
- `WebSiteName` &mdash; nazwa witryny IIS, pod którą ma być podpięta dana aplikacja.
- `WebAppDirName` &mdash; nazwa katalogu na docelowym serwerze, w którym umieszczona będzie aplikacja.
Uwaga: w tej chwili nieużywane &mdash; docelowy katalog dla aplikacji webowych zależy od konfiguracji witryny w IIS, pod którą dana aplikacja
jest podpięta.
-`WebAppName` &mdash; nazwa, pod którą aplikacja będzie widniała w IIS.

Każdy z tych elementów można nadpisać dla konkretnego środowiska &mdash; patrz element `WebAppProjectConfigurationOverride` w sekcji "Konfiguracja środowisk".

### Konfiguracja aplikacji harmonogramu zadań

Przykładowa konfiguracja:

```xml
<ProjectInfoXml xsi:type="SchedulerAppProjectInfoXml" allowedEnvironments="dev1,dev2,test,prod">
  <Name>UberDeployer.SampleSchedulerApp</Name>
  <ArtifactsRepositoryName>UberDeployerSamples</ArtifactsRepositoryName>
  <ArtifactsRepositoryDirName>SampleSchedulerApp</ArtifactsRepositoryDirName>
  <SchedulerAppDirName>SampleSchedulerApp</SchedulerAppDirName>
  <SchedulerAppExeName>UberDeployerSamples.SampleSchedulerApp.exe</SchedulerAppExeName>
  <SchedulerAppTasks>
    <SchedulerAppTaskXml>
      <Name>SampleSchedulerApp</Name>
      <ExecutableName>SampleSchedulerApp</ExecutableName>
      <UserId>Sample.User</UserId>
      <ScheduledHour>12</ScheduledHour>
      <ScheduledMinute>0</ScheduledMinute>
      <ExecutionTimeLimitInMinutes>1</ExecutionTimeLimitInMinutes>
      <Repetition>
        <Enabled>true</Enabled>
        <Interval>00:15:00</Interval>
        <Duration>1.00:00:00</Duration>
        <StopAtDurationEnd>true</StopAtDurationEnd>
      </Repetition>
    </SchedulerAppTaskXml>
  </SchedulerAppTasks>
</ProjectInfoXml>
```

Omówienie elementów specyficznych dla aplikacji harmonogramu zadań:

- `SchedulerAppDirName` &mdash; nazwa podkatalogu na docelowym serwerze, w którym zostaną umieszczone binarki.

- `SchedulerAppExeName` &mdash; nazwa pliku wykonywalnego aplikacji &mdash; używana tylko na potrzeby sprawdzania przez UberDeployera
wersji wdrożonego komponentu; konfiguracja pliku wykonywalnego, który rzeczywiście będzie uruchamiany z harmonogramu zadań, znajduje się
w elemencie konkretnego zadania.

- `SchedulerAppTasks` - lista zadań harmonogramu zadań.

  Opis elementów:

  * `Name` &mdash; nazwa zadania na potrzeby wyświetlania na liście w harmonogramie zadań w systemie Windows.
  * `UserId` &mdash; identyfikator użytkownika, na którym ma działać zadanie. Jest to identyfikator wewnętrzny &mdash; używany tylko
  w obrębie ÜberDeployera.
  * `ExecutableName` &mdash; nazwa pliku wykonywalnego.
  * `ScheduledHour` &mdash; o której godzinie zadanie ma się uruchamiać.
  * `ScheduledMinute` &mdash; w której minucie (podanej wyżej godziny) zadanie ma się uruchamiać.
  * `ExecutionTimeLimitInMinutes` &mdash; limit na czas jednorazowego działania zadania (0 oznacza brak limitu).
  * `Repetition` &mdash; konfiguracja powtarzania danego zadania.
    - `Enabled` &mdash; czy powtarzanie ma być włączone.
    - `Interval` &mdash; częstotliwość powtarzania.
    - `Duration` &mdash; przez jaki okres powtarzać zadanie.
    - `StopAtDurationEnd` &mdash; czy zadanie ma być zatrzymane, gdy upłynie okres powtarzania.

Uwagi:

- Dokładna ścieżka do binarek na docelowym serwerze zależy od środowiska, na które projekt jest wdrażany &mdash; patrz sekcja
"Konfiguracja środowisk".
- Mapowanie identyfikatorów użytkowników na nazwy użytkowników domenowych definiuje się per środowisko &mdash; patrz sekcja
"Konfiguracja środowisk".

### Konfiguracja aplikacji terminalowych

Przykładowa konfiguracja:

```xml
<ProjectInfoXml xsi:type="TerminalAppProjectInfoXml" allowedEnvironments="dev1,dev2,test,prod">
  <Name>UberDeployer.SampleTerminalApp</Name>
  <ArtifactsRepositoryName>UberDeployerSamples</ArtifactsRepositoryName>
  <ArtifactsRepositoryDirName>SampleTerminalApp</ArtifactsRepositoryDirName>
  <TerminalAppName>UberDeployer.SampleTerminalApp</TerminalAppName>
  <TerminalAppDirName>UberDeployer.SampleTerminalApp</TerminalAppDirName>
  <TerminalAppExeName>UberDeployer.SampleTerminalApp.exe</TerminalAppExeName>
</ProjectInfoXml>
```

Omówienie elementów specyficznych dla aplikacji terminalowych:

- `TerminalAppName` &mdash; nazwa aplikacji terminalowej (nieużywane).
- `TerminalAppDirName` &mdash; nazwa podkatalogu na docelowym serwerze, w którym zostaną umieszczone binarki.
- `TerminalAppExeName` &mdash; nazwa pliku wykonywalnego aplikacji.

Uwagi:

- Dokładna ścieżka do binarek na docelowym serwerze zależy od środowiska, na które projekt jest wdrażany &mdash; patrz sekcja
"Konfiguracja środowisk".
- Mapowanie identyfikatorów użytkowników na nazwy użytkowników domenowych definiuje się per środowisko &mdash; patrz sekcja
"Konfiguracja środowisk".

### Konfiguracja projektów bazodanowych

Przykładowa konfiguracja:

```xml
<ProjectInfoXml xsi:type="DbProjectInfoXml" allowedEnvironments="dev1,dev2,test,prod">
  <Name>UberDeployer.SampleDb</Name>
  <ArtifactsRepositoryName>UberDeployerSamples</ArtifactsRepositoryName>
  <ArtifactsRepositoryDirName>DbScripts</ArtifactsRepositoryDirName>
  <ArtifactsAreNotEnvironmentSpecific>true</ArtifactsAreNotEnvironmentSpecific>
  <DbName>UberDeployerSample</DbName>
  <DatabaseServerId>Default</DatabaseServerId>
</ProjectInfoXml>
```

Omówienie elementów specyficznych dla projektów bazodanowych:

- `ArtifactsAreNotEnvironmentSpecific` &mdash; informacja dla ÜberDeployera, że artefakty tego projektu nie są zależne od środowiska,
na które projekt ma być wdrożony.
- `DbName` &mdash; nazwa bazy danych na docelowym serwerze.
- `DatabaseServerId` &mdash; identyfikator wewnętrzny serwera bazodanowego. Patrz również element `DatabaseServers`.

Każdy z tych elementów można nadpisać dla konkretnego środowiska &mdash; patrz element `DbProjectConfigurationOverride` w sekcji "Konfiguracja środowisk".

## Konfiguracja środowisk

Każde środowisko opisane jest przez element XML o nazwie `EnvironmentInfoXml`. Poniżej znajduje się omówienie wszystkich elementów
konfiguracyjnych:

- `Name` &mdash; nazwa środowiska; arbitralna &mdash; pojawia się na liście w aplikacji webowej ÜberDeployera.

- `ConfigurationTemplateName` &mdash; nazwa podkatalogu (w archiwum ZIP z artefaktami), w którym znajdują się binarki specyficzne dla
danego środowiska.

- `AppServerMachineName` &mdash; nazwa serwera aplikacji; na ten serwer wdrażane są aplikacje harmonogramu zadań. Dodatkowo, jeśli
klastrowanie na danym środowisku jest wyłączone (patrz element `EnableFailoverClusteringForNtServices`), to na ten serwer wdrażane
są również usługi NT.

- `FailoverClusterMachineName` &mdash; nazwa serwera zarządzającego klastrem. Używane tylko, gdy klastrowanie na danym środowisku
jest włączone (patrz element `EnableFailoverClusteringForNtServices`).

- `WebServerMachineNames` &mdash; lista nazw serwerów webowych &mdash; na te serwery wdrażane są aplikacje webowe. Przykład:

    ```xml
    <WebServerMachineNames>
      <string>REMOTE</string>
    </WebServerMachineNames>
    ```

- `TerminalServerMachineName` &mdash; nazwa serwera terminalowego &mdash; na ten serwer wdrażane są aplikacje terminalowe.

- `SchedulerServerTasksMachineNames` &mdash; lista nazw serwerów aplikacji harmonogramu zadań &mdash; na tych serwerach konfigurowane są aplikacje harmonogramu zadań. Przykład:

    ```xml
    <SchedulerServerTasksMachineNames>
      <string>REMOTE</string>
    </SchedulerServerTasksMachineNames>
    ```

- `SchedulerServerBinariesMachineNames` &mdash; lista nazw serwerów aplikacji harmonogramu zadań &mdash; na te serwery wdrażane są aplikacje harmonogramu zadań. Przykład:

    ```xml
    <SchedulerServerBinariesMachineNames>
      <string>REMOTE</string>
    </SchedulerServerBinariesMachineNames>
    ```

- `NtServicesBaseDirPath` &mdash; bezwględna ścieżka do katalogu na docelowym serwerze, w którym będą umieszczane binarki usług NT.

- `WebAppsBaseDirPath` &mdash; bezwględna ścieżka do katalogu na docelowym serwerze, w którym będą umieszczane binarki aplikacji webowych. Uwaga:
w tej chwili nieużywane &mdash; docelowy katalog dla aplikacji webowych zależy od konfiguracji witryny w IIS.

- `SchedulerAppsBaseDirPath` &mdash; bezwględna ścieżka do katalogu na docelowym serwerze, w którym będą umieszczane binarki aplikacji harmonogramu
zadań.

- `TerminalAppsBaseDirPath` &mdash; bezwględna ścieżka do katalogu na docelowym serwerze, w którym będą umieszczane binarki aplikacji terminalowych.

- `TerminalAppsShortcutFolder` &mdash; bezwględna ścieżka do katalogu na docelowym serwerze, w którym będą umieszczane skróty do aplikacji terminalowych
(zazwyczaj będzie to ścieżka katalogu reprezentującego pulpit wszystkich użytkowników serwera terminalowego).

- `EnableFailoverClusteringForNtServices` &mdash; informuje ÜberDeployera o tym, czy dane środowisko jest sklastrowane &mdash; od tego będzie zależeć
sposób wdrażania usług NT.

- `ManualDeploymentPackageDirPath` &mdash; domyślna ścieżka do katalogu, do którego zapisywane będą paczki wdrożeniowe w przypadku ręcznego wdrożenia.
Obsługiwane są następujące zmienne:

  * `${current.date}` &mdash; data ,
  * `${order.number}` &mdash; numer porządkowy (zwiększany automatycznie w sytuacji, gdy paczka wdrożeniowa już istnieje we wskazanym katalogu),
  * `${project.name}` &mdash; nazwa projektu (wewnętrzna, czyli ta używana w obrębie ÜberDeployera).

- `EnvironmentUsers` &mdash; definiuje mapowanie wewnętrznych identyfikatorów użytkowników na nazwy użytkowników domenowych. Używane dla projektów,
które wymagają podania użytkownika, w kontekście którego dana aplikacja ma działać (usługi NT, aplikacje harmonogramu zadań). Przykład:

    ```xml
    <EnvironmentUsers>
      <EnvironmentUserXml>
        <Id>Sample.User</Id>
        <UserName>UberDeployer</UserName>
      </EnvironmentUserXml>
    </EnvironmentUsers>
    ```

  Opis elementów:
  
  * `Id` &mdash; wewnętrzny identyfikator użytkownika.
  * `UserName` &mdash; nazwa domenowa użytkownika.

- `AppPoolInfos` &mdash; lista pul aplikacji IIS, z którymi mogą być skojarzone aplikacje webowe. Przykład:

    ```xml
    <AppPoolInfos>
      <AppPoolInfoXml>
        <Id>ASP.NET v4.0</Id>
        <Name>ASP.NET v4.0</Name>
        <Version>V4_0</Version>
        <Mode>Integrated</Mode>
      </AppPoolInfoXml>
    </AppPoolInfos>
    ```

  Opis elementów:
  
  * `Id` &mdash; wewnętrzny identyfikator puli aplikacji IIS. Patrz również element `WebAppProjectConfigurations`.
  * `Name` &mdash; nazwa puli aplikacji w IIS.
  * `Version` &mdash; wersja frameworka .NET używanego przez pulę aplikacji IIS.
  * `Mode` &mdash; Tryb działania puli aplikacji IIS.

- `DatabaseServers` &mdash; definiuje mapowanie wewnętrznych identyfikatorów serwerów bazodanowych na właściwe nazwy tychże serwerów. Przykład:

    ```xml
    <DatabaseServers>
      <DatabaseServerXml>
        <Id>Default</Id>
        <MachineName>REMOTE</MachineName>
      </DatabaseServerXml>
    </DatabaseServers>
    ```

  Opis elementów:
  
  * `Id` &mdash; wewnętrzny identyfikator serwera bazodanowego.
  * `MachineName` &mdash; nazwa serwera bazodanowego.

- `ProjectToFailoverClusterGroupMappings` &mdash; definiuje mapowanie projektów typu usługa NT na nazwy grup klastrowych. Używane, gdy na danym
środowisku włączone jest klastrowanie (patrz element `EnableFailoverClusteringForNtServices`). Przykład:

    ```xml
    <ProjectToFailoverClusterGroupMappingXml>
      <ProjectName>UberDeployer.SampleNtService</ProjectName>
      <ClusterGroupName>UD_SAMPLE</ClusterGroupName>
    </ProjectToFailoverClusterGroupMappingXml>
    ```

  Opis elementów:
  
  * `ProjectName` &mdash; nazwa projektu (wewnętrzna, czyli ta używana w obrębie ÜberDeployera).
  * `ClusterGroupName` &mdash; nazwa grupy klastrowej.

- `WebAppProjectConfigurationOverrides` &mdash; lista opcjonalnych elementów konfiguracyjnych poszczególnych aplikacji webowych specyficznych dla danego środowiska. Przykład:

    ```xml
    <WebAppProjectConfigurationOverrides>
      <WebAppProjectConfigurationOverrideXml projectName="UberDeployer.SampleWebApp">
        <AppPoolId>ASP.NET v4.0</AppPoolId>
        <WebSiteName>Default Web Site</WebSiteName>
        <WebAppDirName>UberDeployer.SampleWebApp</WebAppDirName>
        <WebAppName>UberDeployer.SampleWebApp</WebAppName>
      </WebAppProjectConfigurationOverrideXml>
    </WebAppProjectConfigurationOverrides>
    ```
        
  Opis elementów:
  
  * `AppPoolId` &mdash; wewnętrzny identyfikator puli aplikacji IIS, z której ma korzystać dana aplikacja. Patrz również element `AppPoolInfos`.
  * `WebSiteName` &mdash; nazwa witryny IIS, pod którą ma być podpięta dana aplikacja.
  * `WebAppDirName` &mdash; nazwa katalogu na docelowym serwerze, w którym umieszczona będzie aplikacja.
  Uwaga: w tej chwili nieużywane &mdash; docelowy katalog dla aplikacji webowych zależy od konfiguracji witryny w IIS, pod którą dana aplikacja
  jest podpięta.
  * `WebAppName` &mdash; nazwa, pod którą aplikacja będzie widniała w IIS.

- `DbProjectConfigurationOverrides` &mdash; lista opcjonalnych elementów konfiguracyjnych poszczególnych projektów bazodanowych specyficznych dla danego środowiska. Przykład:

    ```xml
    <DbProjectConfigurationOverrideXml projectName="UberDeployer.SampleDb">
      <DatabaseServerId>Default</DatabaseServerId>
    </DbProjectConfigurationOverrideXml>
    ```

  Opis atrybutów i elementów:
  
  * `projectName` &mdash; nazwa projektu (wewnętrzna, czyli ta używana w obrębie ÜberDeployera).
  * `DatabaseServerId` &mdash; identyfikator wewnętrzny serwera bazodanowego. Patrz również element `DatabaseServers`.

## Konfiguracja aplikacji webowej UberDeployer.WebApp

Przykładowa konfiguracja (w pliku `Web.config`):

```xml
<appSettings>
  <add key="VisibleEnvironments" value="^dev.*;^test$" />
  <add key="DeployableEnvironments" value="^test$" />
  <add key="AllowedProjectConfigurations" value="Test;Production" />
  <add key="CanDeployRole" value="UberDeployer_Deploy" />
  <add key="OnlyDeployableCheckedByDefault" value="true" />
</appSettings>
```

Opis:

- `VisibleEnvironments` &mdash; lista nazw środowisk (oddzielonych średnikiem), które będą widoczne na liście w
aplikacji UberDeployer.WebApp. Każda nazwa może być wyrażeniem regularnym.
- `DeployableEnvironments` &mdash; lista nazw środowisk (oddzielonych średnikiem), na które możliwe będzie wdrażanie. Każda nazwa
może być wyrażeniem regularnym.
- `AllowedProjectConfigurations` &mdash; lista nazw konfiguracji projektów, które będzę można wdrażać. Każda nazwa może być
wyrażeniem regularnym.
- `CanDeployRole` &mdash; nazwa grupy domenowej, do której musi należeć użytkownik, aby mógł on inicjować wdrożenia.
- `OnlyDeployableCheckedByDefault` &mdash; domyślna wartość checkboxa, który decyduje o tym, czy aplikacja ma wyświetlać
tylko projekty wdrażalne na aktualnie wybrane środowisko.
