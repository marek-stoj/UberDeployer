# Przygotowywanie paczek do r�cznego wdra�ania

W webowym kliencie �berDeployera znajduje si� przycisk `Create Package` umo�liwiaj�cy przygotowanie paczki gotowej do r�cznego wdro�enia aplikacji.
Podobnie jak przy zwyk�ym wdra�aniu nale�y wybra� projekt, kt�ry chcemy wdro�y�, konfiguracj�, build i �rodowisko.
Po klikni�ciu w przycisk `Create Package` otwiera si� okno dialogowe z domy�ln� �cie�k� do katalogu do kt�rego ma trafi� paczka.

Domy�lna �cie�ka jest ustawiana per �rodowisko (`EvironmentInfos.xml` - parametr `ManualDeploymentPackageDirPath`, wi�cej info w [Configuration Guide](Configuration Guide PL.md)).

Proces przygotowania paczki sk�ada si� z:
- pobrania artefakt�w z TeamCity
- rozpakowania artefakt�w
- przygotowania konfiguracji na wybrane �rodowisko
- przekopiowania plik�w do wybranej lokalizacji