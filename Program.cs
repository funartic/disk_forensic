
using disk_forensic;
using static disk_forensic.Fat16BootCode;
using static disk_forensic.FatTable;
using static disk_forensic.RootDirectory;


namespace MyApplication
{
    // The Program class is the entry point of the application
    public class Program
    {
        // Main method - Entry point of the application
        public static void Main(string[] args)
        {

          
            string diskFilePath = args[0];

            // Instanziere das BinaryUtils Objekt
            BinaryUtils utils = new BinaryUtils(diskFilePath);

            int currentPositionCounter = 0;

            // Beispiel: Hex-Dump ausgeben
            Console.WriteLine("Master Boot Record (0 - 512 Bytes):");
            utils.PrintHexDump(offset: currentPositionCounter, length: 512, bytesPerLine: 32);
            byte[] byteArray = utils.GetByteArray(offset: currentPositionCounter, length: 512);

            Mbr.MbrInBytes mbrInBytes = Mbr.MbrParser.parseMbrInBytes(byteArray);
            // we skip bootsector here
            Mbr.MbrPartitionTableInBytes mbrPartitionTableInBytes1 = Mbr.MbrParser.parseMbrPartitionTableInBytes(mbrInBytes.PartitionTable1);
            Mbr.MbrPartitionTableInBytes mbrPartitionTableInBytes2 = Mbr.MbrParser.parseMbrPartitionTableInBytes(mbrInBytes.PartitionTable2);
            Mbr.MbrPartitionTableInBytes mbrPartitionTableInBytes3 = Mbr.MbrParser.parseMbrPartitionTableInBytes(mbrInBytes.PartitionTable3);
            Mbr.MbrPartitionTableInBytes mbrPartitionTableInBytes4 = Mbr.MbrParser.parseMbrPartitionTableInBytes(mbrInBytes.PartitionTable4);
            // we skip signature here

            Console.WriteLine("");
            Console.WriteLine("Bootsektor ist alles 0x00");
            Console.WriteLine("");
            Console.WriteLine("Erste Partition aus dem MBR gelesen:");
            Console.WriteLine(mbrPartitionTableInBytes1);
            Console.WriteLine("");
            Console.WriteLine("Zweite Partition aus dem MBR gelesen:");
            Console.WriteLine(mbrPartitionTableInBytes2);
            Console.WriteLine("");
            Console.WriteLine("Dritte Partition aus dem MBR gelesen:");
            Console.WriteLine(mbrPartitionTableInBytes3);
            Console.WriteLine("");
            Console.WriteLine("Vierte Partition aus dem MBR gelesen:");
            Console.WriteLine(mbrPartitionTableInBytes4);
            Console.WriteLine("");
            Console.WriteLine($"MBR signature: {BitConverter.ToString(mbrInBytes.Signature)}");

            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine($"Volume Boot Record (FAT16) (512 - {2* 512} Bytes):");

            currentPositionCounter += 512;
            byte[] sector2 = utils.GetByteArray(offset: currentPositionCounter, length: 512);
            utils.PrintHexDump(offset: currentPositionCounter, length: 512, bytesPerLine: 32);
            Mbr.MbrInBytes volumeBootRecordInBytes = Mbr.MbrParser.parseMbrInBytes(sector2);
           
            // Parsen des Bootsektors
            Fat16BootCodeDto fat16BootCodeDto = Fat16BootCodeParser.Parse(sector2);
            // Ausgabe der Informationen
            Console.WriteLine(fat16BootCodeDto.ToString());

            int fat1StartPosition = currentPositionCounter + fat16BootCodeDto.ReservedSectors * fat16BootCodeDto.BytesPerSector;
            int fatSize = fat16BootCodeDto.SectorsPerFat * fat16BootCodeDto.BytesPerSector;

            currentPositionCounter = fat1StartPosition;
            Console.WriteLine();
            Console.WriteLine($"Erste FAT Tabelle (Position: {currentPositionCounter} - {currentPositionCounter + fatSize}):");
            //utils.PrintHexDump(offset: fat1StartPosition, length: fatSize, bytesPerLine: 32);
            byte[] fatTabelleInBytes = utils.GetByteArray(offset: currentPositionCounter, length: fatSize);
            FatTable.FatChain fatChain = FatTable.FatTableParser.Parse(fatTabelleInBytes);

            // TODO: add fat table 2
            currentPositionCounter += fatSize;
            currentPositionCounter += fatSize;



            List<FatCluster> chain = fatChain.GetChain(4);
            FatTable.FatChain.printClusterChain(chain);
            byte[] originalFile = utils.GetByteArray(offset: 0x00, -1);
            byte[] bytes = FatTable.ConcatAndGetChainToByteArray(fat16BootCodeDto ,chain, originalFile);

            BinaryUtils.PrintHexDump(bytes, 32);

            Console.WriteLine();
            Console.WriteLine($"Root Directory ({currentPositionCounter} bis {currentPositionCounter + fat16BootCodeDto.RootDirEntries * 32})");

            byte[] rootDirectoryBytes = utils.GetByteArray(offset: currentPositionCounter, length: fat16BootCodeDto.RootDirEntries*32); 
            var entries = RootDirectoryParser.Parse(rootDirectoryBytes, false);

            foreach (RootDirectoryEntry entry in entries)
            {
                if(entry.Status != EntryStatus.Unused)
                Console.WriteLine(entry);
            }

        }
    }
}
