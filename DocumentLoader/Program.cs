using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DocumentLoader
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.Win32.SafeHandles;

    public partial class Program
    {
        static void Main(string[] args)
        {
            Console.Title = "Edi Document Importer";
            Console.ForegroundColor = ConsoleColor.Green;
            Stopwatch s = new Stopwatch(), s1 = new Stopwatch();
            List<Document> db_docs = new List<Document>();

            uint dateIncrementerFrom = args.Length < 1 ? 0 : uint.Parse(args[0]),
                 dateIncrementerTo = args.Length < 2 ? 0 : uint.Parse(args[1]);

            DateTime dateFrom = args.Length < 1 ? new DateTime(DateTime.UtcNow.AddDays(0).Ticks - DateTime.UtcNow.AddDays(0).TimeOfDay.Ticks) : ,
                     dateTo = new DateTime(DateTime.UtcNow.AddDays(0).Ticks + (TimeSpan.FromHours(24).Ticks - DateTime.UtcNow.AddDays(0).TimeOfDay.Ticks)),
                     w32fileCreateTime;

            string dir = args.Length < 3 ? $"\\\\192.168.1.20\\exchange\\exports\\orders\\" : args[2];
            int fCount = 0, tfCount = 0, aCount = 0, taCount = 0;
            long tsCount = 0;
            byte[] aBuffer;
            WIN32_FIND_DATA w32file = new WIN32_FIND_DATA();
            SafeFileHandle hFile = null;

            Console.WriteLine($"Operation\t\t| Result");
            Console.WriteLine("========================================================");

            s1.Start();
            string[] clients = Directory.GetDirectories(dir);
            s1.Stop(); Console.WriteLine($"GetDirectories\t\t| dirs read: {clients.Count()} for {s1.ElapsedMilliseconds}ms"); s1.Reset(); s1.Start();
            foreach (var cli in clients)
            {
                db_docs = GetDocuments(dateFrom, dateTo, cli.Substring(dir.Length));
            }
            s1.Stop(); Console.WriteLine($"GetDocuments in db\t| db_docs count: {db_docs.Count()} for {s1.ElapsedMilliseconds}ms"); s1.Reset();

            foreach (var cli in clients)
            {
                s.Start();
                fCount = 0;
                aCount = 0;
                tfCount = 0;

                Console.WriteLine("--------------------------------------------------------");

                IntPtr h = FindFirstFile((cli + @"\*.*"), out w32file);
                FindNextFile(h, out w32file);

                while (FindNextFile(h, out w32file))
                {
                    tfCount++;
                    w32fileCreateTime = DateTime.FromFileTime((((long)w32file.ftCreationTime.dwHighDateTime) << 32) | ((uint)w32file.ftCreationTime.dwLowDateTime));
                    if (!(w32fileCreateTime > dateFrom && w32fileCreateTime < dateTo)) continue;
                    hFile = CreateFile(cli + "\\" + w32file.cFileName, DesiredAccess.GENERIC_READ, ShareMode.FILE_SHARE_READ, IntPtr.Zero, CreationDisposition.OPEN_EXISTING, 0, IntPtr.Zero);
                    if (hFile.IsInvalid)
                    {
                        int Err = Marshal.GetLastWin32Error();
                        throw new System.ComponentModel.Win32Exception(Err);
                        continue;
                    }
                    aBuffer = new byte[w32file.nFileSizeLow];
                    ReadFile(hFile, aBuffer, w32file.nFileSizeLow, 0, IntPtr.Zero);
                    hFile.Close();

                    var doc = (DocumentOrder)new XmlSerializer(typeof(DocumentOrder))
                        .Deserialize(new StringReader(((XmlNode[])((Envelope)new XmlSerializer(typeof(Envelope))
                        .Deserialize(new StringReader(Encoding.UTF8.GetString(aBuffer)))).Body.receiveResponse.@return.cnt)
                        .First().OuterXml));


                    if (!db_docs.Any(x => doc.OrderHeader.OrderNumber == x.ORDER_NUMBER))
                    {
                        InsertIncomingIntoDatabase(doc);
                        aCount++;
                    }

                    fCount++;
                }
                FindClose(h);

                s.Stop();
                tsCount += s.ElapsedMilliseconds;
                taCount += aCount;
                Console.WriteLine($"{cli}");
                Console.WriteLine($"Files count/read/send\t| {tfCount}/{fCount}/{aCount} for {s.ElapsedMilliseconds}ms");
            }

            Console.WriteLine("========================================================");
            Console.WriteLine($"TOTAL send to db\t| {taCount} for {tsCount}ms");
            Console.Read();
            Console.Read();
            Console.Read();
        }





        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern IntPtr FindFirstFile(string lpFileName, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern bool FindNextFile(IntPtr hFindFile, out WIN32_FIND_DATA lpFindFileData);

        [DllImport("kernel32", CharSet = CharSet.Auto)]
        public static extern bool FindClose(IntPtr hFindFile);

        [DllImport("kernel32", SetLastError = true)]
        internal static extern bool ReadFile(SafeFileHandle hFile, Byte[] aBuffer, UInt32 cbToRead, UInt32 cbThatWereRead, IntPtr pOverlapped);

        [DllImport("kernel32.dll", SetLastError = true)]
        internal static extern SafeFileHandle CreateFile(string lpFileName, DesiredAccess dwDesiredAccess, ShareMode dwShareMode, IntPtr lpSecurityAttributes,
            CreationDisposition dwCreationDisposition, FlagsAndAttributes dwFlagsAndAttributes, IntPtr hTemplateFile);

        [DllImport("kernel32", SetLastError = true)]
        internal static extern Int32 CloseHandle(SafeFileHandle hObject);


    }



    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct WIN32_FIND_DATA
    {
        public uint dwFileAttributes;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftCreationTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastAccessTime;
        public System.Runtime.InteropServices.ComTypes.FILETIME ftLastWriteTime;
        public uint nFileSizeHigh;
        public uint nFileSizeLow;
        public uint dwReserved0;
        public uint dwReserved1;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
        public string cFileName;
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 14)]
        public string cAlternateFileName;
    }


    [Flags]
    public enum DesiredAccess : uint
    {
        GENERIC_READ = 0x80000000,
        GENERIC_WRITE = 0x40000000
    }
    [Flags]
    public enum ShareMode : uint
    {
        FILE_SHARE_NONE = 0x0,
        FILE_SHARE_READ = 0x1,
        FILE_SHARE_WRITE = 0x2,
        FILE_SHARE_DELETE = 0x4,

    }
    public enum CreationDisposition : uint
    {
        CREATE_NEW = 1,
        CREATE_ALWAYS = 2,
        OPEN_EXISTING = 3,
        OPEN_ALWAYS = 4,
        TRUNCATE_EXSTING = 5
    }
    [Flags]
    public enum FlagsAndAttributes : uint
    {
        FILE_ATTRIBUTES_ARCHIVE = 0x20,
        FILE_ATTRIBUTE_HIDDEN = 0x2,
        FILE_ATTRIBUTE_NORMAL = 0x80,
        FILE_ATTRIBUTE_OFFLINE = 0x1000,
        FILE_ATTRIBUTE_READONLY = 0x1,
        FILE_ATTRIBUTE_SYSTEM = 0x4,
        FILE_ATTRIBUTE_TEMPORARY = 0x100,
        FILE_FLAG_WRITE_THROUGH = 0x80000000,
        FILE_FLAG_OVERLAPPED = 0x40000000,
        FILE_FLAG_NO_BUFFERING = 0x20000000,
        FILE_FLAG_RANDOM_ACCESS = 0x10000000,
        FILE_FLAG_SEQUENTIAL_SCAN = 0x8000000,
        FILE_FLAG_DELETE_ON = 0x4000000,
        FILE_FLAG_POSIX_SEMANTICS = 0x1000000,
        FILE_FLAG_OPEN_REPARSE_POINT = 0x200000,
        FILE_FLAG_OPEN_NO_CALL = 0x100000
    }
}



