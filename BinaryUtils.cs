using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disk_forensic
{
 
    public class BinaryUtils
    {
        private string filePath;

        public BinaryUtils(string filePath)
        {
            this.filePath = filePath;
        }

        // Liest die Binärdatei ab einem bestimmten Offset und für eine bestimmte Länge.
        public byte[] ReadBinary(long offset = 0, int length = -1)
        {
            try
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read))
                {
                    // Stellen Sie sicher, dass der Offset innerhalb des Dateibereichs liegt
                    if (offset < 0 || offset >= fs.Length)
                    {
                        throw new ArgumentOutOfRangeException(nameof(offset), "Offset liegt außerhalb des gültigen Bereichs.");
                    }

                    fs.Seek(offset, SeekOrigin.Begin);

                    // Wenn keine Länge angegeben ist, dann bis zum Ende der Datei
                    if (length == -1)
                    {
                        length = (int)(fs.Length - offset);
                    }

                    // Sicherstellen, dass die Länge nicht über das Ende der Datei hinausgeht
                    if (offset + length > fs.Length)
                    {
                        length = (int)(fs.Length - offset);
                    }

                    byte[] buffer = new byte[length];
                    fs.Read(buffer, 0, length);
                    return buffer;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Fehler beim Lesen der Datei: {ex.Message}");
                return null;
            }
        }


        // Gibt einen Hex-Dump der Binärdatei aus
        public void PrintHexDump(long offset = 0, int length = 512, int bytesPerLine = 32)
        {
            Console.WriteLine($"xxd -s {offset} -l {length} -c {bytesPerLine} disk1.hdd");
            byte[] data = ReadBinary(offset, length);
            StringBuilder hexBuilder = new StringBuilder();

            for (int i = 0; i < data.Length; i += bytesPerLine)
            {
                int bytesToPrint = Math.Min(bytesPerLine, data.Length - i);
                byte[] line = new byte[bytesToPrint];
                Array.Copy(data, i, line, 0, bytesToPrint);

                hexBuilder.AppendFormat("{0:X8}  ", offset + i);
                foreach (byte b in line)
                {
                    hexBuilder.AppendFormat("{0:X2} ", b);
                }

                // ASCII Darstellung
                hexBuilder.Append("  ");
                foreach (byte b in line)
                {
                    hexBuilder.Append(b >= 32 && b <= 126 ? (char)b : '.');
                }

                hexBuilder.AppendLine();
            }

            Console.WriteLine(hexBuilder.ToString());
        }

        // Gibt die Binärdaten als Byte-Array zurück
        public byte[] GetByteArray(long offset = 0, int length = 512)
        {
            return ReadBinary(offset, length);
        }

        // Gibt die rohen Bytes der Binärdatei aus
        public void PrintBytes(long offset = 0, int length = 512)
        {
            byte[] data = ReadBinary(offset, length);
            Console.WriteLine(BitConverter.ToString(data).Replace("-", " "));
        }

        // Gibt die ASCII-Darstellung der Binärdaten aus
        public void PrintAscii(long offset = 0, int length = 512)
        {
            byte[] data = ReadBinary(offset, length);
            StringBuilder asciiBuilder = new StringBuilder();

            foreach (byte b in data)
            {
                asciiBuilder.Append(b >= 32 && b <= 126 ? (char)b : '.');
            }

            Console.WriteLine(asciiBuilder.ToString());
        }


        public static void PrintHexDump(byte[] data, int bytesPerLine = 16)
        {
            if (data == null)
            {
                Console.WriteLine("<null>");
                return;
            }

            int length = data.Length;
            for (int i = 0; i < length; i += bytesPerLine)
            {
                // Offset
                Console.Write($"{i:X8}  ");

                // Hex-Teil
                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (i + j < length)
                        Console.Write($"{data[i + j]:X2} ");
                    else
                        Console.Write("   "); // Padding wenn letzte Zeile kürzer
                }

                Console.Write(" ");

                // ASCII-Teil
                for (int j = 0; j < bytesPerLine; j++)
                {
                    if (i + j < length)
                    {
                        var b = data[i + j];
                        char c = (b >= 32 && b <= 126) ? (char)b : '.';
                        Console.Write(c);
                    }
                }

                Console.WriteLine();
            }
        }
    }

  

}
