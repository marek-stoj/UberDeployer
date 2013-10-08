# Przygotowywanie paczek do rêcznego wdra¿ania

W webowym kliencie ÜberDeployera znajduje siê przycisk `Create Package` umo¿liwiaj¹cy przygotowanie paczki gotowej do rêcznego wdro¿enia aplikacji.
Podobnie jak przy zwyk³ym wdra¿aniu nale¿y wybraæ projekt, który chcemy wdro¿yæ, konfiguracjê, build i œrodowisko.
Po klikniêciu w przycisk `Create Package` otwiera siê okno dialogowe z domyœln¹ œcie¿k¹ do katalogu do którego ma trafiæ paczka.

Domyœlna œcie¿ka jest ustawiana per œrodowisko (`EvironmentInfos.xml` - parametr `ManualDeploymentPackageDirPath`, wiêcej info w [Configuration Guide](Configuration Guide PL.md)).

Proces przygotowania paczki sk³ada siê z:
- pobrania artefaktów z TeamCity
- rozpakowania artefaktów
- przygotowania konfiguracji na wybrane œrodowisko
- przekopiowania plików do wybranej lokalizacji