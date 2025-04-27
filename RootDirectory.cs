using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disk_forensic
{
    public class RootDirectory
    {

        public class RootDirectoryEntry
        {
            public string FileName { get; set; }
            public byte Attributes { get; set; }
            public EntryStatus Status { get; set; }  // Status der Datei: aktiv, gelöscht, unbenutzt
            public ushort CreateTime { get; set; }
            public ushort CreateDate { get; set; }
            public ushort LastAccessDate { get; set; }
            public ushort FirstCluster { get; set; }
            public uint FileSize { get; set; }

            public override string ToString()
            {
                // Hier formatieren wir die Ausgabe als lesbaren String
                return $"FileName: {FileName}, " +
                       $"Attributes: {Attributes:X2}, " +
                       $"Status: {Status}, " +
                       $"CreateTime: {CreateTime}, " +
                       $"CreateDate: {CreateDate}, " +
                       $"LastAccessDate: {LastAccessDate}, " +
                       $"FirstCluster: {FirstCluster}, " +
                       $"FileSize: {FileSize} bytes";
            }
        }

        public enum EntryStatus
        {
            Active,
            Deleted,
            Unused
        }


        public class RootDirectoryParser
        {
            public static List<RootDirectoryEntry> Parse(byte[] directoryData, bool includeSystemFiles = false)
            {
                var entries = new List<RootDirectoryEntry>();

                for (int i = 0; i < directoryData.Length; i += 32)
                {
                    // Verhindern, dass wir an einem ungültigen Speicherbereich arbeiten
                    if (i + 31 >= directoryData.Length)
                        break;

                    var entryData = directoryData.Skip(i).Take(32).ToArray();

                    // Hier wird der erste Byte überprüft, um festzustellen, ob der Eintrag unbenutzt oder gelöscht ist.
                    byte firstByte = entryData[0];
                    byte[] bytes = entryData.Take(8).ToArray();
                    // Auch unbenutzte und gelöschte Einträge werden berücksichtigt
                    RootDirectoryEntry entry = new RootDirectoryEntry
                    {
                        FileName = GetCleanAscii(bytes, 0,bytes.Length),
                        Attributes = entryData[11],
                        Status = firstByte == 0xE5 ? EntryStatus.Deleted : (firstByte == 0x00 ? EntryStatus.Unused : EntryStatus.Active), // Status je nach Byte
                        CreateTime = BitConverter.ToUInt16(entryData.Skip(14).Take(2).ToArray(), 0),
                        CreateDate = BitConverter.ToUInt16(entryData.Skip(16).Take(2).ToArray(), 0),
                        LastAccessDate = BitConverter.ToUInt16(entryData.Skip(18).Take(2).ToArray(), 0),
                        FirstCluster = BitConverter.ToUInt16(entryData.Skip(20).Take(2).ToArray(), 0),
                        FileSize = BitConverter.ToUInt32(entryData.Skip(28).Take(4).ToArray(), 0)
                    };

                    // Optional: Filter für Systemdateien
                    if (includeSystemFiles || !IsSystemFile(entry.Attributes))
                    {
                        entries.Add(entry);
                    }
                }

                return entries;
            }

            // Hilfsmethode, um zu prüfen, ob eine Datei eine Systemdatei ist
            private static bool IsSystemFile(byte attributes)
            {
                // Systemdateien haben das Attribut 0x04
                return (attributes & 0x04) != 0;
            }

            private static string GetCleanAscii(byte[] data, int offset, int length)
            {
                var sb = new StringBuilder();

                for (int i = 0; i < length; i++)
                {
                    byte b = data[offset + i];

                    // Gültige ASCII Zeichen (nur sichtbare Zeichen 0x20 - 0x7E)
                    if (b >= 0x20 && b <= 0x7E)
                        sb.Append((char)b);
                }

                return sb.ToString().Trim();
            }

            private static string GetCleanAscii(byte[] data)
            {
                // Extrahiert den Dateinamen und entfernt ungültige Zeichen (wie Nullbytes).
                var result = Encoding.ASCII.GetString(data).Trim();
                return result.Replace("\0", string.Empty);  // Entfernt Nullbytes
            }
        }
    
    }
}