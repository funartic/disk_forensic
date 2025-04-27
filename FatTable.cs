using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static disk_forensic.Fat16BootCode;
using static disk_forensic.FatTable;

namespace disk_forensic
{
    public class FatTable
    {
        public static byte[] ConcatAndGetChainToByteArray(
       Fat16BootCodeDto fat16BootCodeDto,
       List<FatCluster> chain,
       byte[] diskImage)
        {
            int bytesPerCluster = fat16BootCodeDto.BytesPerSector * fat16BootCodeDto.SectorsPerCluster;
            List<byte> result = new List<byte>();

            foreach (var cluster in chain)
            {
                // Rechne Offset des Clusters aus:
                int firstDataSector = fat16BootCodeDto.ReservedSectors + (fat16BootCodeDto.NumberOfFats * fat16BootCodeDto.SectorsPerFat) + (fat16BootCodeDto.RootDirEntries * 32 / fat16BootCodeDto.BytesPerSector);
                int sectorOfCluster = firstDataSector + (cluster.ClusterNumber - 2) * fat16BootCodeDto.SectorsPerCluster;
                int offset = sectorOfCluster * fat16BootCodeDto.BytesPerSector;

                if (offset + bytesPerCluster > diskImage.Length)
                    break; // Schutz: nicht über das Ende hinaus lesen

                byte[] clusterData = new byte[bytesPerCluster];
                Array.Copy(diskImage, offset, clusterData, 0, bytesPerCluster);
                result.AddRange(clusterData);
            }

            return result.ToArray();
        }


        public enum FatEntryType
        {
            Free,
            Reserved,
            BadCluster,
            EndOfChain,
            ValidCluster
        }

        public class FatCluster
        {
            public ushort ClusterNumber { get; set; }        // Aktueller Cluster
            public ushort? NextClusterNumber { get; set; }    // Nächster Cluster (null wenn Ende)
            public FatEntryType EntryType { get; set; }       // Typ des Eintrags

            public FatCluster(ushort clusterNumber, ushort? nextClusterNumber, FatEntryType entryType)
            {
                ClusterNumber = clusterNumber;
                NextClusterNumber = nextClusterNumber;
                EntryType = entryType;
            }
        }

        public class FatChain
        {
            private Dictionary<ushort, FatCluster> _clusters = new();

            public static void printClusterChain(List<FatCluster> chain)
            {
                foreach (FatCluster cluster in chain)
                {
                    Console.WriteLine($"Cluster {cluster.ClusterNumber:X4}: Typ {cluster.EntryType}, Next {cluster.NextClusterNumber?.ToString("X4") ?? "END"}");
                }
            }

            public void AddEntry(ushort clusterNumber, ushort nextCluster)
            {
                FatEntryType type;
                ushort? nextClusterNumber = null;

                if (nextCluster == 0x0000)
                {
                    type = FatEntryType.Free;
                }
                else if (nextCluster >= 0xFFF0 && nextCluster <= 0xFFF6)
                {
                    type = FatEntryType.Reserved;
                }
                else if (nextCluster == 0xFFF7)
                {
                    type = FatEntryType.BadCluster;
                }
                else if (nextCluster >= 0xFFF8 && nextCluster <= 0xFFFF)
                {
                    type = FatEntryType.EndOfChain;
                }
                else
                {
                    type = FatEntryType.ValidCluster;
                    nextClusterNumber = nextCluster;
                }

                _clusters[clusterNumber] = new FatCluster(clusterNumber, nextClusterNumber, type);
            }

            public List<FatCluster> GetChain(ushort startCluster)
            {
                Console.WriteLine($"Gibt cluster chain für StartCluster {startCluster}");
                var chain = new List<FatCluster>();
                var current = startCluster;

                while (current != 0 && _clusters.ContainsKey(current))
                {
                    var cluster = _clusters[current];
                    chain.Add(cluster);

                    if (cluster.EntryType == FatEntryType.EndOfChain ||
                        cluster.EntryType == FatEntryType.Reserved ||
                        cluster.EntryType == FatEntryType.BadCluster ||
                        cluster.NextClusterNumber == null)
                    {
                        // Wenn Ende erreicht, abbrechen
                        break;
                    }

                    current = cluster.NextClusterNumber.Value;
                }

                return chain;
            }


            public FatCluster GetCluster(ushort clusterNumber)
            {
                _clusters.TryGetValue(clusterNumber, out var cluster);
                return cluster;
            }
        }

        public class FatTableParser
        {
            public static FatChain Parse(byte[] fatTableInByte)
            {
                var fatChain = new FatChain();

                for (int i = 0; i < fatTableInByte.Length; i += 2)
                {
                    if (i + 1 >= fatTableInByte.Length)
                        break; // Ende erreicht

                    ushort value = (ushort)(fatTableInByte[i] | (fatTableInByte[i + 1] << 8));
                    ushort clusterNumber = (ushort)(i / 2);

                    fatChain.AddEntry(clusterNumber, value);
                }

                return fatChain;
            }
        }
    }
}
