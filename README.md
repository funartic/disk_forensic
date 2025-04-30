# Was ist der MBR?

Der MBR ist der erste Sektor einer Festplatte (Sektor 0, 512 Bytes groß).

Er enthält:

- Bootloader-Code (normalerweise die ersten 446 Bytes),
- eine Partitionstabelle (vier Einträge à 16 Bytes = 64 Bytes),
- und eine Signatur (2 Bytes: 0x55AA am Ende).

### Wie visualisiere ich die Daten? (Tools)

- HxD
- xxd

Mit Hxd kann man sich den kompletten Datenträgerabbild laden: Extras -> Datenträgerabbild öffnen

### Datenträgerabbild klonen

Das Erstellen eines forensik-sicheren Klons eines Datenträgerabbildes mit dd ist eine wichtige Methode, wenn du die Integrität der Daten auf dem Originalmedium wahren und sicherstellen möchtest, dass keine Änderungen an den Daten vorgenommen werden. Anschließendes prüfen des SHA-Wertes:

```bash
$ dd if=disk1.hdd of=cloned_disk.hdd bs=64K conv=noerror,sync status=progress
73007104 bytes (73 MB, 70 MiB) copied, 1 s, 73.0 MB/s
1526+1 records in
1527+0 records out
100073472 bytes (100 MB, 95 MiB) copied, 1.36277 s, 73.4 MB/s
```

Original-Datenträger nur im Lese-Modus mounten.

### Hexdump

```bash
$ xxd -s 0 -l 512 -c 32 disk1.hdd
00000000: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000020: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000040: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000060: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000080: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000000a0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000000c0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000000e0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000100: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000120: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000140: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000160: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000180: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000001a0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 00fe  ................................
000001c0: ffff 0bfe ffff 0100 0000 2ffb 0200 0000 0000 0000 0000 0000 0000 0000 0000 0000  ........../.....................
000001e0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 55aa  ..............................U.
```

### Analyse vom Bootloader-Code (die ersten 446 Bytes):

```bash
$ xxd -s 0 -l 446 -c 32 disk1.hdd
00000000: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000020: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000040: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000060: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000080: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000000a0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000000c0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000000e0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000100: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000120: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000140: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000160: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000180: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000001a0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000       ..............................
```

### Analyse vom der der Partitionstabellen (die nächsten 4mal 16 Bytes):

| Offset (Bereich) | Größe | Bedeutung                        | Erklärungen                                                                                                                                                                                                                                                                                                                                                                                                  |
| ---------------- | ----- | -------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------ |
| 0-0              | 1     | Bootflag                         | **Bootfähig**: Ein Wert von `0x80` bedeutet, dass diese Partition bootfähig ist. Wenn `0x00`, dann ist sie **nicht bootfähig**. In deinem Fall ist der Wert `0x00`, was bedeutet, dass diese Partition nicht bootfähig ist.                                                                                                                                                                                  |
| 1-3              | 3     | Start-CHS (Cylinder-Head-Sector) | **Start-Position (CHS)**: Diese Werte (Cylinder, Head, Sector) beschreiben die Position des ersten Sektors der Partition in der alten CHS-Darstellung (Cylinder-Head-Sector). Heute wird jedoch meistens das LBA (Logical Block Addressing) verwendet, daher wird dieser Wert oft ignoriert. Die Werte `0xfe`, `0xff` und `0xff` sind einfach Teil dieser alten Darstellungsform und werden nicht verwendet. |
| 4-4              | 1     | Partitionstyp                    | **Partitionstyp**: Der Typ der Partition wird durch eine einstellige Zahl im Hex-Format angegeben. In deinem Fall `0x0B`, was für **FAT32** steht. Dies bedeutet, dass die Partition das **FAT32-Dateisystem** verwendet.                                                                                                                                                                                    |
| 5-7              | 3     | End-CHS (Cylinder-Head-Sector)   | **End-Position (CHS)**: Diese Werte geben an, wo die Partition endet, ebenfalls im CHS-Format. Wie beim Start-CHS-Wert wird dieser Bereich in modernen Systemen häufig ignoriert, da LBA verwendet wird. Die Werte `0xfe`, `0xff` und `0xff` sind Teil dieser alten CHS-Darstellung.                                                                                                                         |
| 8-11             | 4     | LBA-Startadresse                 | **LBA-Start**: Der Startsektor der Partition im LBA-Format. Der Wert `0x01000000` bedeutet, dass die Partition **bei Sektor 1** beginnt. (LBA-0 ist der Master Boot Record, daher ist Sektor 1 der erste Nutzsektor der Festplatte.)                                                                                                                                                                         |
| 12-15            | 4     | Anzahl Sektoren                  | **Größe der Partition**: Die Anzahl der Sektoren, die die Partition umfasst. Der Wert `0x2ffb0200` entspricht 194991 Sektoren. Da jeder Sektor 512 Byte groß ist, ergibt sich eine Partitiongröße von etwa **99,87 MiB**.                                                                                                                                                                                    |

---

```bash
$ xxd -s 446 -l 16 -c 32 disk1.hdd
000001be: 00fe ffff 0bfe ffff 0100 0000 2ffb 0200                                          ............/...
```

Aufschlüsselung:

- `0x00` → Bootflag: nicht bootfähig (kein 0x80)
- `0xfeffff` → Start-CHS (alte CHS-Werte, meistens ignoriert)
- `0x0b` → Partitionstyp: 0x0B → FAT32 (mit CHS)!
- `0xfeffff` → End-CHS (auch meistens ignoriert)
- `0x01000000` → LBA-Start: 0x00000001 → Sektor 1
- `0x2ffb0200` → Anzahl Sektoren: 0x0002FB2F → 194991 Sektoren

```bash
$ xxd -s $((446+1*16)) -l 16 -c 32 disk1.hdd
000001ce: 0000 0000 0000 0000 0000 0000 0000 0000                                          ................
$ xxd -s $((446+2*16)) -l 16 -c 32 disk1.hdd
000001de: 0000 0000 0000 0000 0000 0000 0000 0000                                          ................
$ xxd -s $((446+3*16)) -l 16 -c 32 disk1.hdd
000001ee: 0000 0000 0000 0000 0000 0000 0000 0000                                          ................
```

→ Es gibt keine weiteren Partitionen

### MBR der ersten Partition

Es wird nun der zweite Sektor (offset 512) des Datenträgerabbilds disk1.hdd ausgelesen mit einer Länge von 512 Bytes.

```bash
$ xxd -s 512 -l 512 -c 32 disk1.hdd
00000200: eb3c 9042 5344 2020 342e 3400 0204 0100 0200 0200 00f8 bf00 2000 1000 0100 0000  .<.BSD  4.4............. .......
00000220: 2ffb 0200 8000 29fb 0be4 c344 4953 4b31 2020 2020 2020 4641 5431 3620 2020 fa31  /.....)....DISK1      FAT16   .1
00000240: c08e d0bc 007c fb8e d8e8 0000 5e83 c619 bb07 00fc ac84 c074 06b4 0ecd 10eb f530  .....|......^..........t.......0
00000260: e4cd 16cd 190d 0a4e 6f6e 2d73 7973 7465 6d20 6469 736b 0d0a 5072 6573 7320 616e  .......Non-system disk..Press an
00000280: 7920 6b65 7920 746f 2072 6562 6f6f 740d 0a00 0000 0000 0000 0000 0000 0000 0000  y key to reboot.................
000002a0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000002c0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000002e0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000300: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000320: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000340: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000360: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00000380: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000003a0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000003c0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000003e0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 55aa  ..............................U.
```

### Analyse: Volume Boot Record (FAT16) – Sektor 2

| Offset (von–bis) | Größe | Bedeutung                      | Inhalt                                                            | Erklärung                                                            |
| :--------------- | :---- | :----------------------------- | :---------------------------------------------------------------- | :------------------------------------------------------------------- |
| 0x200–0x202      | 3     | Jump Befehl                    | `eb3c90`                                                          | Kurzer Maschinenbefehl, um den Bootcode zu überspringen (`jmp 0x3c`) |
| 0x203–0x205      | 8     | OEM Name                       | `4253442020342e34` → `BSD  4.4`                                   | Hersteller- und Versionsangabe                                       |
| 0x20B–0x20C      | 2     | Bytes pro Sektor               | `0200` → 512                                                      | Standardgröße für einen Sektor                                       |
| 0x20D            | 1     | Sektoren pro Cluster           | `02`                                                              | Clustergröße: 2 Sektoren pro Cluster                                 |
| 0x20E–0x20F      | 2     | Reservierte Sektoren           | `0002`                                                            | 2 reservierte Sektoren (inkl. Bootsektor)                            |
| 0x210            | 1     | Anzahl FATs                    | `02`                                                              | Es gibt 2 FAT-Tabellen                                               |
| 0x211–0x212      | 2     | Anzahl Root-Directory-Einträge | `00f8` → 248 (ACHTUNG: Wert ungewöhnlich, normalerweise `0x0200`) |
| 0x213–0x214      | 2     | Gesamtsektoren (alt)           | `bf00`                                                            | Wird nicht benutzt, da später größerer Wert                          |
| 0x215            | 1     | Medienkennung                  | `20`                                                              | Medienbezeichner, normalerweise `f8` für Festplatten                 |
| 0x216–0x217      | 2     | Sektoren pro FAT               | `0010` → 16                                                       | Jede FAT-Tabelle ist 16 Sektoren groß                                |
| 0x218–0x219      | 2     | Sektoren pro Track             | `0010`                                                            | (CHS-Adressierung, unwichtig bei LBA)                                |
| 0x21A–0x21B      | 2     | Anzahl Köpfe                   | `0100`                                                            | (ebenfalls CHS-Altlast)                                              |
| 0x21C–0x21F      | 4     | Versteckte Sektoren            | `00000000`                                                        | Keine versteckten Sektoren                                           |
| 0x220–0x223      | 4     | Gesamtsektoren (neu)           | `2ffb0200`                                                        | Großer Wert für die gesamte Partition                                |
| 0x224–0x227      | 4     | Nicht belegt (bei FAT16)       | `800029fb`                                                        | Enthält u.a. Volume-ID                                               |
| 0x228–0x22B      | 4     | Volume-ID (Seriennummer)       | `0be4c344`                                                        | Eindeutige Festplatten-ID                                            |
| 0x22C–0x236      | 11    | Volume Label                   | `DISK1      `                                                     | Name des Volumes, hier: "DISK1"                                      |
| 0x237–0x23F      | 8     | Dateisystemtyp                 | `FAT16   `                                                        | Typ der Dateisystemspezifikation                                     |
| 0x240–0x25F      | 32    | Bootcode (Start)               | `fa31c08ed0bc...`                                                 | Start des Bootcodes für das Laden des Betriebssystems                |
| 0x260–0x27F      | 32    | Bootcode (weiter)              | `e4cd16cd190d...`                                                 | Weitere Teile des Bootcodes, zeigt "Non-system disk" Fehler          |
| ...              | ...   | ...                            | ...                                                               | Der Rest ist entweder Bootcode oder Padding                          |
| 0x3FE–0x3FF      | 2     | Bootsektor-Signatur            | `55aa`                                                            | Klassische Boot-Signatur, wichtig für BIOS                           |

### FAT Tabellen

🧹 Typische kleine Fallen, auf die du achten solltest:

- Blockgrößenfehler: immer mit 512 Bytes rechnen, es sei denn, du hast Hinweise auf 4K-Sektoren.
- Root-Directory ist feste Größe bei FAT16 (nicht dynamisch wie bei FAT32).
- FAT-Tabellen können beschädigt sein → doppeltes Absichern.

Dateizuordnungen und Clusterketten untersuchen:
FAT-Tabellen zeigen, wie die Cluster miteinander verkettet sind. Du kannst die FATs einzeln dumpen und anschauen.

📚 Kurze Wiederholung: Wichtige Infos aus deinem Bootsektor

- Bytes pro Sektor: 512
- Reservierte Sektoren: 2
- Anzahl FATs: 2
- Sektoren pro FAT: 16

Formel zum Beginn der ersten FAT-Tabelle:

```
FAT1-Start = (Anzahl reservierter Sektoren) × (Sektorgröße)
FAT1-Start = 2 × 512 = 1024 Bytes (0x400)
```

Formel zur Berechnung wie groß die erste FAT ist:

```
Größe (in Bytes) = (Sektoren pro FAT) × (Bytes pro Sektor)
Größe (in Bytes) =                 16 × 512 = 8192 Bytes
```

- Jede Cluster-Nummer wird in der FAT als 16-Bit Wert (2 Bytes) gespeichert (bei FAT16)
- Typische Werte:
  - 0x0000: Freier Cluster
  - 0xFFF7: Defekter Cluster
  - 0xFFF8 – 0xFFFF: Dateiende-Marker (EOC – End of Clusterchain)
  - Ein anderer Wert (z.B. 0x0034) → zeigt auf den nächsten Cluster in einer Dateikette
- Wir lesen ab 0x400 (Anzahl reservierter Sektoren), immer 2 Bytes = 1 Eintrag

```bash
$ xxd -s 1024 -l 512 -c 32 disk1.hdd
00000400: f8ff ffff 0000 0000 da2c 0000 0000 0800 0900 0a00 0b00 0c00 0d00 0e00 0f00 1000  .........,......................
00000420: 1100 1200 1300 1400 1500 1600 1700 1800 1900 1a00 1b00 1c00 1d00 1e00 1f00 2000  .............................. .
00000440: 2100 2200 2300 2400 2500 2600 2700 2800 2900 2a00 2b00 2c00 2d00 2e00 2f00 3000  !.".#.$.%.&.'.(.).*.+.,.-.../.0.
00000460: 3100 3200 3300 3400 3500 3600 3700 3800 3900 3a00 3b00 3c00 3d00 3e00 3f00 4000  1.2.3.4.5.6.7.8.9.:.;.<.=.>.?.@.
00000480: 4100 4200 4300 4400 4500 4600 4700 4800 4900 4a00 4b00 4c00 4d00 4e00 4f00 5000  A.B.C.D.E.F.G.H.I.J.K.L.M.N.O.P.
000004a0: 5100 5200 5300 5400 5500 5600 5700 5800 5900 5a00 5b00 5c00 5d00 5e00 5f00 6000  Q.R.S.T.U.V.W.X.Y.Z.[.\.].^._.`.
000004c0: 6100 6200 6300 6400 6500 6600 6700 6800 6900 6a00 6b00 6c00 6d00 6e00 6f00 7000  a.b.c.d.e.f.g.h.i.j.k.l.m.n.o.p.
000004e0: 7100 7200 7300 7400 7500 7600 ffff 7800 7900 7a00 7b00 7c00 7d00 7e00 7f00 8000  q.r.s.t.u.v...x.y.z.{.|.}.~.....
00000500: 8100 8200 8300 8400 8500 8600 8700 8800 8900 8a00 8b00 8c00 8d00 8e00 8f00 9000  ................................
00000520: 9100 9200 9300 9400 9500 9600 9700 9800 9900 9a00 9b00 9c00 9d00 9e00 9f00 a000  ................................
00000540: a100 a200 a300 a400 a500 a600 a700 a800 a900 aa00 ab00 ac00 ad00 ae00 af00 b000  ................................
00000560: b100 b200 b300 b400 b500 b600 b700 b800 b900 ba00 bb00 bc00 bd00 be00 bf00 c000  ................................
00000580: c100 c200 c300 c400 c500 c600 c700 c800 c900 ca00 cb00 cc00 cd00 ce00 cf00 d000  ................................
000005a0: d100 d200 d300 d400 d500 d600 d700 d800 d900 da00 db00 dc00 dd00 de00 df00 e000  ................................
000005c0: e100 e200 e300 e400 e500 e600 e700 e800 e900 ea00 eb00 ec00 ed00 ee00 ef00 f000  ................................
000005e0: f100 f200 f300 f400 f500 f600 f700 f800 f900 fa00 fb00 fc00 fd00 fe00 ff00 0001  ................................
```

| Offset (hex) | Cluster-Nr | Wert (hex) | Bedeutung                  |
| :----------- | :--------- | :--------- | :------------------------- |
| 0x400        | 0          | F8FF       | Medienkennung + reserviert |
| 0x402        | 1          | FFFF       | reserviert                 |
| 0x404        | 2          | 0000       | frei                       |
| 0x406        | 3          | 0000       | frei                       |
| 0x408        | 4          | 2CDA       | zeigt auf Cluster 11354    |
| 0x40A        | 5          | 0000       | frei                       |
| 0x40C        | 6          | 0800       | zeigt auf Cluster 8        |
| 0x40E        | 7          | 0900       | zeigt auf Cluster 9        |
| 0x410        | 8          | 0A00       | zeigt auf Cluster 10       |
| 0x412        | 9          | 0B00       | zeigt auf Cluster 11       |
| 0x414        | 10         | 0C00       | zeigt auf Cluster 12       |
| 0x416        | 11         | 0D00       | zeigt auf Cluster 13       |
| 0x418        | 12         | 0E00       | zeigt auf Cluster 14       |
| 0x41A        | 13         | 0F00       | zeigt auf Cluster 15       |
| 0x41C        | 14         | 1000       | zeigt auf Cluster 16       |
| 0x41E        | 15         | 1100       | zeigt auf Cluster 17       |
| ...          | ...        | ...        | ...                        |

✍️ Was bedeutet das?

- Cluster 0 und Cluster 1 sind speziell (reserviert für das Dateisystem selbst, Medienkennung, etc.).
- Cluster 2 und 3 sind frei (keine Datei dort gespeichert).
- Cluster 4 zeigt auf Cluster 0x2CDA → sehr ungewöhnlich, normalerweise sollten kleine Werte folgen (evtl. Datei-Fragmentierung oder Fehler).
- Danach siehst du eine saubere Kette: 8 → 9 → 10 → 11 → 12 → ...
- (jede Cluster zeigt auf den nächsten, typisch für eine Datei ohne Fragmentierung!)

→ Eine Datei läuft sequentiell durch viele Cluster! 📂📂📂

### Dateizuordnungen und Clusterketten untersuchen

Um die Verzeichnisstruktur zu analysieren muss folgendes gemacht werden:

- Lies den Root-Directory-Bereich der Partition aus.
- Der Root-Directory liegt direkt nach den FAT-Tabellen.
  Dazu musst du wissen:
- Anzahl reservierter Sektoren (aus Bootsektor: hier 2 Sektoren)
- Anzahl FATs (hier 2) × Sektoren pro FAT (hier 16) = 32 Sektoren
- Root Directory beginnt also nach 2 + 32 = 34 Sektoren.

### Eine bestimmte Datei rekonstruieren (anhand der Clusterkette)

📍 Was brauchen wir konkret?

| Parameter                   | Wert                                  | Erklärung                          |
| :-------------------------- | :------------------------------------ | :--------------------------------- |
| Bytes pro Sektor            | 512                                   | Standardgröße eines Sektors        |
| Sektoren pro Cluster        | 2                                     | Clustergröße laut Bootsektor       |
| Bytes pro Cluster           | 1024 (512 × 2)                        | Größe eines Clusters in Bytes      |
| Reservierte Sektoren        | 2                                     | Enthält Bootsektor + evtl. weitere |
| Anzahl der FATs             | 2                                     | Redundante Kopien der FAT          |
| Sektoren pro FAT            | 16                                    | Größe einer FAT                    |
| Größe Root Directory        | 32 Sektoren (512 Einträge à 32 Bytes) | Platz für Dateieinträge            |
| Startsektor der Datenregion | 66 (2 + 32 + 32)                      | Nach Reserviert + FATs + Rootdir   |
| Bytes bis zur Datenregion   | 33792 Bytes (66 × 512)                | Ab hier liegen die Dateidaten      |

📐 Formel zur Cluster-Offset-Berechnung

```
Offset (Byte) = (reservierte Sektoren + FATs × Sektoren pro FAT + RootDirGröße in Sektoren + (Cluster-Nummer - 2) × Sektoren pro Cluster) × Bytes pro Sektor
Root-Directory-Größe:
Bei FAT16 meist 512 Einträge × 32 Byte = 16384 Bytes
→ 16384 Bytes ÷ 512 Bytes/Sektor = 32 Sektoren.
Bspl Cluster 8 Offset: (66 + (8-2) × 2) × 512 = 39936 Bytes
```

### Gefundene Cluster clonen

```bash
dd if=disk1.hdd of=cluster8.bin bs=512 skip=78 count=2
```

- skip=78 → ab Sektor 78
- count=2 → weil 2 Sektoren pro Cluster
- bs=512 → Blocksize 512 Bytes

### Datei aus mehreren Clustern zusammensetzen/clonen

```bash
# Cluster 8
dd if=disk1.hdd of=file_recovery.bin bs=512 skip=78 count=2
# Cluster 9
dd if=disk1.hdd of=tmp.bin bs=512 skip=80 count=2
cat tmp.bin >> file_recovery.bin
# Cluster 10
dd if=disk1.hdd of=tmp.bin bs=512 skip=82 count=2
cat tmp.bin >> file_recovery.bin
```

### Das Root-Directory auslesen und schauen, was da alles verzeichnet ist

📚 Was wir wissen (Bootsektor-Werte):

- 0x0B: 0200 → 512 Bytes/Sektor ✅
- 0x0D: 04 → 4 Sektoren pro Cluster ✅
- 0x0E: 0100 → 1 reservierter Sektor ✅
- 0x10: 02 → 2 FATs ✅
- 0x11: 0002 → 2 Rootdir-Einträge ❗ (das ist extrem wenig! Normal sind 512 oder 224)
- 0x16: bf00 → 0x00BF = 191 Sektoren pro FAT ✅

1. Bootsektor:
   - Offset: 0
   - Länge: 1 Sektor → 512 Bytes
2. FAT-Tabellen:
   - 2 FATs
   - Jeweils 191 Sektoren → 191 × 512 Bytes = 97.792 Bytes pro FAT
   - Insgesamt: 2 × 191 = 382 Sektoren
3. Root Directory:
   - Jeder Rootdir-Eintrag braucht 32 Bytes
   - 2 Einträge → 2 × 32 = 64 Bytes
   - Anzahl Sektoren für Root Directory:
   - 64 Bytes / 512 Bytes/Sektor = 0,125 → aufrunden → 1 Sektor
4. Datenbereich Start:

   - Nach:
     - 1 reservierter Sektor
       - 382 Sektoren (FATs)
       - 1 Sektor (Rootdir)

   ➔ Start bei Sektor 1 + 382 + 1 = 384 relativ zum Partitionstart

| Bereich        | Relativer Sektor innerhalb der Partition | Absoluter Offset |
| -------------- | ---------------------------------------- | ---------------- |
| Bootsektor     | 0                                        | 0x200            |
| FAT1           | 1                                        | 0x200 + 512      |
| FAT2           | 192                                      | 0x200 + 192×512  |
| Root Directory | 383                                      | 0x200 + 383×512  |
| Datenbereich   | 384                                      | 0x200 + 384×512  |

```bash
$ xxd -s $((384*512)) -l 512 -c 32 disk1.hdd
00030000: 4449 534b 3120 2020 2020 2028 0000 0000 0000 0000 0000 2f26 894c 0000 0000 0000  DISK1      (........../&.L......
00030020: e500 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 ffff ffff  ................................
00030040: e520 2020 2020 2020 2020 2012 00ab 2126 894c 894c 0000 2126 894c 0200 0000 0000  .          ...!&.L.L..!&.L......
00030060: 4443 494d 2020 2020 2020 2010 004c 5d23 894c 894c 0000 5d23 894c 0400 0000 0000  DCIM       ..L]#.L.L..]#.L......
00030080: e564 0066 0000 00ff ffff ff0f 0093 ffff ffff ffff ffff ffff ffff 0000 ffff ffff  .d.f............................
000300a0: e570 006f 0063 006f 0072 000f 0093 6700 7400 6600 6f00 3000 3300 0000 2e00 7000  .p.o.c.o.r....g.t.f.o.0.3.....p.
000300c0: e54f 434f 5247 7e31 5044 4620 0082 8699 0545 894c 0000 8699 0545 eb59 902f 9801  .OCORG~1PDF .....E.L.....E.Y./..
000300e0: e52e 0070 0064 0066 0000 000f 00ee ffff ffff ffff ffff ffff ffff 0000 ffff ffff  ...p.d.f........................
00030100: e52e 005f 0070 006f 0063 000f 00ee 6f00 7200 6700 7400 6600 6f00 0000 3000 3300  ..._.p.o.c....o.r.g.t.f.o...0.3.
00030120: e550 4f43 4f52 7e31 5044 4622 0082 ab25 894c 894c 0000 ab25 894c 0200 0010 0000  .POCOR~1PDF"...%.L.L...%.L......
00030140: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00030160: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
00030180: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000301a0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000301c0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
000301e0: 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000 0000  ................................
```

| Offset  | Dateiname  | Dateityp    | Größe | Weitere Details                       |
| ------- | ---------- | ----------- | ----- | ------------------------------------- |
| 0x30000 | DISK1      | Verzeichnis | -     | Sieht aus wie ein Verzeichnis         |
| 0x30020 | DCIM       | Verzeichnis | -     | Verzeichnis, könnte Kamera-Daten sein |
| 0x30040 | OCORG~1PDF | Datei       | -     | Sieht aus wie eine Datei (PDF?)       |
| 0x30060 | \_poc      | Verzeichnis | -     | Möglicherweise ein Verzeichnis        |
| 0x30080 | \_pdf      | Verzeichnis | -     | Möglicherweise ein Verzeichnis        |

Wir schauen uns den ersten Eintrag an (32 bytes):

```bash
$ xxd -s $((384*512)) -l 32 -c 32 disk1.hdd
00030000: 4449 534b 3120 2020 2020 2028 0000 0000 0000 0000 0000 2f26 894c 0000 0000 0000  DISK1      (........../&.L......
```

| **Byte-Offset**   | **Hexdump**                   | **Bedeutung**                           | **Kommentar**                            |
| ----------------- | ----------------------------- | --------------------------------------- | ---------------------------------------- |
| 0x30000 - 0x30003 | 4449 534b 31                  | **DISK1** (Dateiname)                   | Name der Datei/Verzeichnis               |
| 0x30003 - 0x30007 | 2020 2020 2028                | Leerzeichen (Padding)                   | Auffüllen auf 11 Zeichen                 |
| 0x30007 - 0x3000F | 0000 0000 0000                | Keine speziellen Attribute              | Keine speziellen Dateitypen oder Flags   |
| 0x3000F - 0x30013 | 2f26 894c                     | Verweis auf Verzeichnis oder Datei-Info | Möglicherweise ein Verweis auf Metadaten |
| 0x30013 - 0x3001F | 0000 0000 0000 0000 0000 0000 | Füllbytes                               | Keine Daten, gefüllt mit `0x00`          |

```bash
$ xxd -s $((384*512+(3*32))) -l 32 -c 32 disk1.hdd
00030060: 4443 494d 2020 2020 2020 2010 004c 5d23 894c 894c 0000 5d23 894c 0400 0000 0000  DCIM       ..L]#.L.L..]#.L......
```

| **Byte-Offset**   | **Hexdump**    | **Bedeutung**                              | **Kommentar**                                   |
| ----------------- | -------------- | ------------------------------------------ | ----------------------------------------------- |
| 0x30060 - 0x3006B | 4443 494d      | **DCIM** (Dateiname)                       | Der Dateiname im 8.3-Format (4 Zeichen)         |
| 0x3006B - 0x3006F | 2020 2020 2020 | Leerzeichen (Padding)                      | Auffüllen des Dateinamens auf 11 Zeichen        |
| 0x3006F - 0x30071 | 2010           | Möglicherweise Dateieigenschaften          | Könnte den Dateityp oder eine Flagge darstellen |
| 0x30071 - 0x30076 | 004c 5d23      | Verweis auf Metadaten oder Verzeichnisinfo | Verweis auf zusätzliche Metadaten               |
| 0x30076 - 0x3007B | 894c 894c      | Weitere Verweise oder Metadaten            | Weitere Daten zur Dateistruktur oder -referenz  |
| 0x3007B - 0x3007F | 0000 5d23 894c | Weitere Metadaten / Verzeichnisinfo        | Möglicherweise Verweise auf Datenstrukturen     |
| 0x3007F - 0x30083 | 0400 0000 0000 | Füllbytes oder nicht verwendete Daten      | Wahrscheinlich Füllbytes                        |

### Unterverzeichnis von DCIM lesen

- Bestimme den Cluster des DCIM Verzeichnisses (Bereits aus der Analyse des Verzeichniseintrags bekannt: Clusterwert 0x004c 5d23).
- Berechne den Speicherort des Clusters auf der Festplatte.
- Lese die Verzeichniseinträge im Cluster, um herauszufinden, ob es weitere Unterverzeichnisse gibt.
- Wiederhole den Prozess, um in Unterverzeichnisse zu gelangen.

Nächster Schritt: Cluster von DCIM aufrufen

Nun müssen wir den Clusterwert 0x004c und seine Sektor-Referenzen verwenden, um den Speicherort des Verzeichnisses DCIM zu bestimmen und die 32-Byte-Einträge zu lesen, um festzustellen, ob es Unterverzeichnisse gibt. Wenn du diese Schritte durchführen möchtest, können wir gemeinsam die Sektor-Adresse und Clusterauflösungen berechnen.

### Skizzierung des Datenträgerabbild-Aufbaus

## Datenträgeraufbau

| Bereich                        | Offset-Bereich (von-bis) | Größe (Bytes)                                        | Beschreibung                                                                 |
| :----------------------------- | :----------------------- | :--------------------------------------------------- | :--------------------------------------------------------------------------- |
| **MBR (Master Boot Record)**   | 0 - 511                  | 512 Bytes                                            | Enthält den Bootloader und die Partitionstabelle (4 Einträge à 16 Bytes)     |
| **Reserved Area (Bootsektor)** | 0 - (je nach Partition)  | Je nach Partition                                    | Erster Sektor (meist 512 Bytes) und reservierte Sektoren für den Bootbereich |
| **FAT1**                       | Nach Reserved Area       | SectorsPerFAT × BytesPerSector                       | Erste FAT-Tabelle, enthält Informationen zu den Clustern                     |
| **FAT2**                       | Nach FAT1                | SectorsPerFAT × BytesPerSector                       | Zweite FAT-Tabelle (Backup)                                                  |
| **Root Directory**             | Nach FAT2                | MaxRootEntries × 32                                  | Verzeichnisstruktur der ersten Ebene, Einträge je 32 Byte                    |
| **Clusterbereich (Dateien)**   | Nach Root Directory      | (TotalSectors - StartClusterSektor) × BytesPerSector | Enthält die Cluster, in denen die Dateiinhalte gespeichert sind              |
