# Przygotowywanie paczek do ręcznego wdrażania

W webowym kliencie ÜberDeployera znajduje się przycisk `Create Package` umożliwiający przygotowanie paczki gotowej do ręcznego wdrożenia aplikacji.
Podobnie jak przy zwykłym wdrażaniu należy wybrać projekt, który chcemy wdrożyć, konfigurację, build i środowisko.
Po kliknięciu w przycisk `Create Package` otwiera się okno dialogowe z domyślną ścieżką do katalogu do którego ma trafić paczka.

Domyślna ścieżka jest ustawiana per środowisko (`EvironmentInfos.xml` - parametr `ManualDeploymentPackageDirPath`, więcej info w [Configuration Guide](Configuration Guide PL.md)).

Proces przygotowania paczki składa się z:
- pobrania artefaktów z TeamCity
- rozpakowania artefaktów
- przygotowania konfiguracji na wybrane środowisko
- przekopiowania plików do wybranej lokalizacji