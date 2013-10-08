# Przygotowywanie paczek do r�cznego wdra�ania

W webowym kliencie UberDeployera znajduje si� przycisk 'Create Package' umo�liwiaj�cy przygotowanie paczki gotowej do r�cznego wdro�enia aplikacji.
Podobnie jak przy zwyk�ym wdra�aniu nale�y wybra� projekt, kt�ry chcemy wdro�y�, konfiguracj�, build i �rodowisko.
Po klikni�ciu w przycisk 'Create Package' otwiera si� okno dialogowe z domy�ln� �cie�k� do katalogu do kt�rego maja trafi� paczka.

Domy�lna �cie�ka jest ustawiana per �rodowisko (EvironmentInfo.xml - parametr ManualDeploymentPackageDirPath, wi�cej info w Configuration Guide).

Proces przygotowania paczki sk�ada si� z:
- pobrania artefakt�w z TeamCity
- rozpakowania artefakt�w
- przygotowania konfiguracji na wybrane �rodowisko
- przekopiowania plik�w do wybranej lokalizacji