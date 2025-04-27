using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace disk_forensic
{
    public static class Fat16BootCode
    {


        // DTO für den Bootcode in Bytes
        public class Fat16BootCodeInBytes
        {
            public byte[] JumpInstruction { get; set; }
            public string OemName { get; set; }
            public ushort BytesPerSector { get; set; }
            public byte SectorsPerCluster { get; set; }
            public ushort ReservedSectors { get; set; }
            public byte NumberOfFats { get; set; }
            public ushort RootDirEntries { get; set; }
            public ushort TotalSectorsOld { get; set; }
            public byte MediaDescriptor { get; set; }
            public ushort SectorsPerFat { get; set; }
            public ushort SectorsPerTrack { get; set; }
            public ushort NumberOfHeads { get; set; }
            public uint HiddenSectors { get; set; }
            public uint TotalSectorsNew { get; set; }
            public uint Unused { get; set; }
            public uint VolumeId { get; set; }
            public string VolumeLabel { get; set; }
            public string FileSystemType { get; set; }
            public byte[] BootCodeStart { get; set; }
            public byte[] BootCodeContinue { get; set; }
            public ushort BootSectorSignature { get; set; }
        }


        // DTO für die analysierten Daten des FAT16 Bootsektors
        public class Fat16BootCodeDto
        {
            public string OemName { get; set; }
            public ushort BytesPerSector { get; set; }
            public byte SectorsPerCluster { get; set; }
            public ushort ReservedSectors { get; set; }
            public byte NumberOfFats { get; set; }
            public ushort RootDirEntries { get; set; }
            public ushort TotalSectorsOld { get; set; }
            public byte MediaDescriptor { get; set; }
            public ushort SectorsPerFat { get; set; }
            public ushort SectorsPerTrack { get; set; }
            public ushort NumberOfHeads { get; set; }
            public uint HiddenSectors { get; set; }
            public uint TotalSectorsNew { get; set; }
            public uint VolumeId { get; set; }
            public string VolumeLabel { get; set; }
            public string FileSystemType { get; set; }
            public ushort BootSectorSignature { get; set; }
            public string JumpCommand { get; internal set; }
            public uint Unused { get; internal set; }
            public string BootCodeStart { get; internal set; }
            public string BootCodeContinue { get; internal set; }

            public override string ToString()
            {
                return $"OEM Name: {OemName}{Environment.NewLine}" +
                       $"Bytes per Sector: {BytesPerSector}{Environment.NewLine}" +
                       $"Sectors per Cluster: {SectorsPerCluster}{Environment.NewLine}" +
                       $"Reserved Sectors: {ReservedSectors}{Environment.NewLine}" +
                       $"FATs: {NumberOfFats}{Environment.NewLine}" +
                       $"Root Directory Entries: {RootDirEntries}{Environment.NewLine}" +
                       $"Media Descriptor: 0x{MediaDescriptor:X2}{Environment.NewLine}" +
                       $"Sectors per FAT: {SectorsPerFat}{Environment.NewLine}" +
                       $"Sectors per Track: {SectorsPerTrack}{Environment.NewLine}" +
                       $"Heads: {NumberOfHeads}{Environment.NewLine}" +
                       $"Hidden Sectors: {HiddenSectors}{Environment.NewLine}" +
                       $"Total Sectors (new): {TotalSectorsNew}{Environment.NewLine}" +
                       $"Volume ID: 0x{VolumeId:X8}{Environment.NewLine}" +
                       $"Volume Label: {VolumeLabel}{Environment.NewLine}" +
                       $"FileSystem Type: {FileSystemType}{Environment.NewLine}" +
                       $"Boot Sector Signature: 0x{BootSectorSignature:X4}";
            }

        }




        public class Fat16BootCodeParser {

            public static Fat16BootCodeDto Parse(byte[] bootSectorData)
            {
               
                // Parsing der Daten
                Fat16BootCodeDto dto = new Fat16BootCodeDto();
                dto.JumpCommand = BitConverter.ToString(bootSectorData, 0x00, 3);
                dto.OemName = System.Text.Encoding.ASCII.GetString(bootSectorData, 0x03, 8).Trim();
                dto.BytesPerSector = BitConverter.ToUInt16(bootSectorData, 0x0B);
                dto.SectorsPerCluster = bootSectorData[0x0D];
                dto.ReservedSectors = BitConverter.ToUInt16(bootSectorData, 0x0E);
                dto.NumberOfFats = bootSectorData[0x10];
                dto.RootDirEntries = BitConverter.ToUInt16(bootSectorData, 0x11);
                dto.TotalSectorsOld = BitConverter.ToUInt16(bootSectorData, 0x13);
                dto.MediaDescriptor = bootSectorData[0x15];
                dto.SectorsPerFat = BitConverter.ToUInt16(bootSectorData, 0x16);
                dto.SectorsPerTrack = BitConverter.ToUInt16(bootSectorData, 0x18);
                dto.NumberOfHeads = BitConverter.ToUInt16(bootSectorData, 0x1A);
                dto.HiddenSectors = BitConverter.ToUInt32(bootSectorData, 0x1C);
                dto.TotalSectorsNew = BitConverter.ToUInt32(bootSectorData, 0x20);
                dto.VolumeId = BitConverter.ToUInt32(bootSectorData, 0x28);
                dto.VolumeLabel = System.Text.Encoding.ASCII.GetString(bootSectorData, 0x2B, 11).Trim();
                dto.FileSystemType = System.Text.Encoding.ASCII.GetString(bootSectorData, 0x36, 8).Trim();
                dto.BootSectorSignature = BitConverter.ToUInt16(bootSectorData, 0x1FE);

                return dto;
            }

        }
    }
}
