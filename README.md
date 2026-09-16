# Astro Orbit

Astro Orbit ist ein astronomischer Bildfeldrotator auf Basis eines ESP32 und
eines Feetech ST3215. Er lässt sich über USB, WLAN und einen normalen Browser
steuern und unterstützt sowohl **ASCOM** als auch **ASCOM Alpaca**.

## Funktionen

- Positionsbereich von 0 bis 360°
- Feetech ST3215 im Motor-Modus mit 2:1-Riemenübersetzung
- Native ASCOM-USB-Verbindung für Windows und N.I.N.A.
- ASCOM Alpaca über WLAN
- Steuerung über eine integrierte Weboberfläche
- Position anfahren, stoppen, Richtung umkehren und Geschwindigkeit einstellen
- Mechanische und astronomische Position auf 0° setzen
- Persistente mechanische Position über Controller-Neustarts hinweg
- OLED-Anzeige für Position, Motorstatus und IP-Adresse
- Firmware-Installation über USB im Browser
- Weitere Firmware-Updates über WLAN und Browser

## Hardware

- ESP32 Dev Module
- Feetech ST3215
- Servo-Bus: 1 Mbit/s, RX GPIO 18, TX GPIO 19
- SSD1306 OLED 128 × 32, I²C-Adresse `0x3C`
- I²C: SDA GPIO 21, SCL GPIO 22
- Riemenscheiben: 70 zu 140 Zähne, Übersetzung 2:1

## Firmware installieren

### USB mit Chrome oder Edge

Der Web-Flasher installiert ein vollständiges Firmware-Image. VS Code,
PlatformIO und zusätzliche Flash-Programme werden nicht benötigt.

**[Astro Orbit Web-Flasher öffnen](https://moma13570.github.io/Astro-Orbit/)**

1. Astro Orbit per USB mit dem Computer verbinden.
2. N.I.N.A., serielle Monitore und andere Programme schließen, die den COM-Port verwenden.
3. Den Web-Flasher in Chrome oder Edge öffnen.
4. **USB Firmware installieren** wählen.
5. Den COM-Port des ESP32 auswählen und die Installation starten.
6. Nach dem Neustart die WLAN-Einrichtung durchführen.

Bei **Failed to initialize** zuerst N.I.N.A. und `AstroOrbit.LocalServer.exe`
vollständig schließen. Falls es erneut fehlschlägt, BOOT gedrückt halten, die
Installation starten, kurz EN/RESET drücken und BOOT nach erfolgreicher
Verbindung loslassen.

Für USB wird aus dem neuesten GitHub-Release automatisch
`astro-orbit-factory.bin` verwendet.

### OTA über WLAN

OTA steht zur Verfügung, sobald Firmware 1.3.0 oder neuer installiert ist.

1. `astro-orbit-firmware.bin` aus dem
   **[neuesten Release](https://github.com/MoMa13570/Astro-Orbit/releases/latest)** herunterladen.
2. Mit demselben Netzwerk wie Astro Orbit verbinden.
3. `http://astro-orbit.local/update` öffnen.
4. Die heruntergeladene Datei auswählen und hochladen.
5. Strom und WLAN bis zum automatischen Neustart nicht trennen.

Falls der Hostname nicht erreichbar ist, kann die Update-Seite über
`http://<IP-Adresse>/update` geöffnet werden.

## WLAN und Websteuerung

Beim ersten Start öffnet Astro Orbit einen WLAN-Zugangspunkt mit dem Namen
**Astro Orbit**. Damit verbinden und im Captive Portal das eigene WLAN auswählen.
Nach dem Neustart verbindet sich der Controller mit diesem Netzwerk.

Die wichtigsten Seiten sind:

| Funktion | Adresse |
| --- | --- |
| Rotator steuern | `http://astro-orbit.local/setup/v1/rotator/0/configdevices` |
| WLAN einrichten | `http://astro-orbit.local/setup/v1/rotator/0/wifi` |
| Firmware aktualisieren | `http://astro-orbit.local/update` |

Alternativ kann überall die im OLED angezeigte IP-Adresse verwendet werden.

## ASCOM unter Windows

Der native ASCOM-Treiber verbindet N.I.N.A. und andere Windows-Astroprogramme
direkt per USB mit Astro Orbit. Ein zusätzlicher ASCOM-Hub wird nicht benötigt.

Voraussetzungen:

- Windows 10 oder 11
- ASCOM Platform 7.1 oder neuer
- .NET Framework 4.8 oder neuer

Installation:

1. Sobald der Windows-Installer in einem
   **[Release](https://github.com/MoMa13570/Astro-Orbit/releases)** bereitsteht,
   die Datei `Astro-Orbit-ASCOM-Setup-<Version>.exe` herunterladen.
2. N.I.N.A. und andere Astroprogramme schließen.
3. Das Setup als Administrator ausführen.
4. In N.I.N.A. als Rotator **Astro Orbit** auswählen.
5. Über das Zahnrad den COM-Port festlegen.
6. Zum Nullen im selben Fenster **Set current position to 0°** verwenden. Der
   Treiber verbindet den ausgewählten COM-Port dafür kurz selbstständig.
7. Astro Orbit verbinden und zunächst eine kleine Bewegung testen.

Der ASCOM-Treiber stellt Position, mechanische Position, Zielposition,
`IsMoving`, `Reverse`, `Move`, `MoveAbsolute`, `MoveMechanical`, `Sync` und
`Halt` bereit.

Eine ausführliche Anleitung für Entwickler und den Windows-Build steht in
[USB-ASCOM.md](USB-ASCOM.md).

## ASCOM Alpaca über WLAN

Astro Orbit stellt einen Alpaca-Rotator der Interface-Version 3 über HTTP-Port
80 bereit. In N.I.N.A. oder einem anderen Alpaca-Client wird die IP-Adresse des
Controllers und die Gerätenummer `0` verwendet.

1. Computer und Astro Orbit mit demselben Netzwerk verbinden.
2. In der Astrosoftware einen ASCOM-Alpaca-Rotator hinzufügen.
3. `astro-orbit.local` oder die angezeigte IP-Adresse eintragen.
4. Port `80`, Gerätetyp `Rotator` und Gerätenummer `0` verwenden.
5. Verbinden und eine kleine Testbewegung ausführen.

USB-ASCOM und Alpaca sollten nicht gleichzeitig Bewegungsbefehle senden. Während
einer aktiven USB-Sitzung sperrt die Firmware andere schreibende Zugriffe.

## Position und Nullen

- **Mechanical Position** ist die virtuelle mechanische Stellung des Rotators.
- **Position** enthält zusätzlich den über ASCOM `Sync` gesetzten astronomischen Offset.
- Die mechanische Position bleibt über Neustarts hinweg gespeichert.
- Der Nullknopf setzt mechanische Position, Position und Zielposition gemeinsam auf 0°.
- `Sync` ändert nur die astronomische Positionszuordnung und bewegt den Motor nicht.

## Downloads

- **[Firmware und Releases](https://github.com/MoMa13570/Astro-Orbit/releases)**
- **[Web-Flasher](https://moma13570.github.io/Astro-Orbit/)**
- [USB-/ASCOM-Details](USB-ASCOM.md)
- [USB-Protokoll](USB-PROTOCOL.md)

## Aktuelle Versionen

- Firmware: **1.3.0-usb**
- ASCOM-Treiber: **1.4.2**
- Alpaca Interface: **3**
