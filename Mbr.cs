using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static disk_forensic.Mbr;

namespace disk_forensic
{
    public static class Mbr
    {

        public class MbrInBytes
        {
            public MbrInBytes(byte[] bootCode, byte[] partitionTable1, byte[] partitionTable2, byte[] partitionTable3, byte[] partitionTable4, byte[] signature)
            {
                BootCode = bootCode;
                PartitionTable1 = partitionTable1;
                PartitionTable2 = partitionTable2;
                PartitionTable3 = partitionTable3;
                PartitionTable4 = partitionTable4;
                Signature = signature;
            }

            public byte[] BootCode { get; }
            public byte[] PartitionTable1 { get; }
            public byte[] PartitionTable2 { get; }
            public byte[] PartitionTable3 { get; }
            public byte[] PartitionTable4 { get; }
            public byte[] Signature { get; }
        }

        public class MbrPartitionTableInBytes
        {
            public byte BootFlag { get; set; }
            public byte[] StartChs { get; set; }
            public byte PartitionType { get; set; }
            public byte[] EndChs { get; set; }
            public uint LbaStart { get; set; }
            public uint NumberOfSectors { get; set; }

            public MbrPartitionTableInBytes(byte bootFlag, byte[] startChs, byte partitionType, byte[] endChs, uint lbaStart, uint numberOfSectors)
            {
                BootFlag = bootFlag;
                StartChs = startChs;
                PartitionType = partitionType;
                EndChs = endChs;
                LbaStart = lbaStart;
                NumberOfSectors = numberOfSectors;
            }

            public override string ToString()
            {
                // Umwandlung der CHS-Werte in lesbare Form (hexadezimal)
                string startChsHex = BitConverter.ToString(StartChs).Replace("-", " ");
                string endChsHex = BitConverter.ToString(EndChs).Replace("-", " ");
                return $"BootFlag: 0x{BootFlag:X2}\n" +
                       $"Start-CHS: {startChsHex}\n" +
                       $"PartitionType: 0x{PartitionType:X2} ({GetPartitionTypeName()})\n" +
                       $"End-CHS: {endChsHex}\n" +
                       $"LBA-Start: 0x{LbaStart:X8} (Sektor {LbaStart})\n" +
                       $"Anzahl Sektoren: 0x{NumberOfSectors:X8} ({NumberOfSectors})\n" +
                       $"Partition Größe: {GetPartitionSizeInMB()} MB";
            }

            public  MbrPartitionTable mapToDto( )
            {
                return new MbrPartitionTable
                {

                };
            }

            // Methode zur Ermittlung des Partitionstyps anhand des Hex-Werts
            private string GetPartitionTypeName()
            {
                return PartitionType switch
                {
                    0x0B => "FAT32",
                    0x0C => "FAT32 (LBA)",
                    0x07 => "NTFS",
                    0x83 => "Linux ext4",
                    _ => "Unbekannt"
                };
            }

            // Berechnung der Partitiongröße in MB
            private double GetPartitionSizeInMB()
            {
                const int sectorSize = 512; // Sektorgröße in Bytes
                return (NumberOfSectors * sectorSize) / (1024.0 * 1024.0); // MB
            }
        }

        public class MbrPartitionTable
        {
            public byte BootFlag { get; set; }
            public byte[] StartChs { get; set; }
            public byte PartitionType { get; set; }
            public byte[] EndChs { get; set; }
            public uint LbaStart { get; set; }
            public uint NumberOfSectors { get; set; }
        }

        public static class MbrParser
        {
            public const int FIXED_BOOT_CODE_SIZE = 446;
            public const int FIXED_PARTITION_TABLE_COUNT = 4;
            public const int FIXED_SINGLE_PARTITION_TABLE_SIZE = 16;
            //FIXED_MBR_SIGNATURE_POSTION = 510
            public const int FIXED_MBR_SIGNATURE_POSTION = FIXED_BOOT_CODE_SIZE + FIXED_PARTITION_TABLE_COUNT * FIXED_SINGLE_PARTITION_TABLE_SIZE;
            public const int FIXED_MBR_SIGNATURE_SIZE = 2;

            public static MbrInBytes parseMbrInBytes(byte[] mbrData)
            {
                if (mbrData.Length != 512)
                {
                    throw new ArgumentException("MBR-Daten müssen 512 Bytes lang sein.");
                }

                byte[] bootCode = new byte[FIXED_BOOT_CODE_SIZE];
                byte[] partitionTable1 = new byte[FIXED_SINGLE_PARTITION_TABLE_SIZE];
                byte[] partitionTable2 = new byte[FIXED_SINGLE_PARTITION_TABLE_SIZE];
                byte[] partitionTable3 = new byte[FIXED_SINGLE_PARTITION_TABLE_SIZE];
                byte[] partitionTable4 = new byte[FIXED_SINGLE_PARTITION_TABLE_SIZE];
                byte[] signature = new byte[FIXED_MBR_SIGNATURE_SIZE];

                Array.Copy(mbrData, 0, bootCode, 0, FIXED_BOOT_CODE_SIZE);
                Array.Copy(mbrData,  FIXED_BOOT_CODE_SIZE, partitionTable1, 0, FIXED_SINGLE_PARTITION_TABLE_SIZE);
                Array.Copy(mbrData, FIXED_BOOT_CODE_SIZE + 1 * FIXED_SINGLE_PARTITION_TABLE_SIZE, partitionTable2, 0, FIXED_SINGLE_PARTITION_TABLE_SIZE);
                Array.Copy(mbrData, FIXED_BOOT_CODE_SIZE + 2 * FIXED_SINGLE_PARTITION_TABLE_SIZE, partitionTable3, 0, FIXED_SINGLE_PARTITION_TABLE_SIZE);
                Array.Copy(mbrData, FIXED_BOOT_CODE_SIZE + 3 * FIXED_SINGLE_PARTITION_TABLE_SIZE, partitionTable4, 0, FIXED_SINGLE_PARTITION_TABLE_SIZE);
                Array.Copy(mbrData, FIXED_MBR_SIGNATURE_POSTION, signature, 0, FIXED_MBR_SIGNATURE_SIZE);


                return new MbrInBytes(bootCode, partitionTable1, partitionTable2, partitionTable3, partitionTable4, signature);

            }

            public static MbrPartitionTableInBytes parseMbrPartitionTableInBytes(byte[] singlePartitionTableInBytes)
            {

                // Überprüfen, ob das Eingabebyte-Array die erwartete Größe hat (16 Bytes für einen Partitionseintrag)
                if (singlePartitionTableInBytes.Length != 16)
                {
                    throw new ArgumentException("Der Partitionseintrag muss genau 16 Bytes lang sein.");
                }

                // Bootflag extrahieren (1 Byte)
                byte bootFlag = singlePartitionTableInBytes[0];

                // Start-CHS extrahieren (3 Bytes)
                byte[] startChs = new byte[3];
                Array.Copy(singlePartitionTableInBytes, 1, startChs, 0, 3);

                // Partitionstyp extrahieren (1 Byte)
                byte partitionType = singlePartitionTableInBytes[4];

                // End-CHS extrahieren (3 Bytes)
                byte[] endChs = new byte[3];
                Array.Copy(singlePartitionTableInBytes, 5, endChs, 0, 3);

                // LBA-Start (4 Bytes)
                uint lbaStart = BitConverter.ToUInt32(singlePartitionTableInBytes, 8);

                // Anzahl der Sektoren (4 Bytes)
                uint numberOfSectors = BitConverter.ToUInt32(singlePartitionTableInBytes, 12);

                // Erstelle das MbrPartitionTableInBytes DTO
                return new MbrPartitionTableInBytes(bootFlag, startChs, partitionType, endChs, lbaStart, numberOfSectors);
            }
        }
    }
}
