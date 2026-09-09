namespace sharptest
{
    public static class DownloadPath 
    {
        public static void CreateDirectory()
        {
            string defaultPath = @"C:\TestingSHARPTEST";
            string anotherDrivePath = @"D:\TestingSHARPTEST";

            // if folder exists on C drive, use it
            if (Directory.Exists(defaultPath))
            {
                Console.WriteLine("Singapore server downloaded files will be in C Drive");
                return;
            }
            // if folder exists on D drive, use it
            if (Directory.Exists(anotherDrivePath))
            {
                Console.WriteLine("Singapore server downloaded files will be in D Drive");
                return;
            }

            // neither folder exists so try to create on C drive first
            try
            {
                Directory.CreateDirectory(defaultPath);
                Console.WriteLine("Singapore server downloaded files will be in C Drive");
                return;
            }
            catch
            {
                // if C drive failed try D drive
                try
                {
                    Directory.CreateDirectory(anotherDrivePath);
                    Console.WriteLine("Singapore server downloaded files will be in D Drive");
                    return;
                }
                catch
                {
                    // Both drives failed
                    Console.WriteLine("Please make sure a drive is available for the download folder.");
                    // Environment.Exit(1) is used  to immediately stop a running application and 
                    // signal to the operating system that the program closed due to an error
                    Environment.Exit(1);
                }
            }
        }
        public static string GetDownloadPath(string fileName)
        {
            // this is for download location of files downloaded from the server
            // either c drive or d drive for 100 MB or 1 GB
            string defaultPath = $@"C:\TestingSHARPTEST\{fileName}";
            string anotherDrivePath = $@"D:\TestingSHARPTEST\{fileName}";

            // if folder exists on C drive use C path
            if (Directory.Exists(@"C:\TestingSHARPTEST"))
            {
                return defaultPath;
            }

            // if folder exists on D drive use D path
            if (Directory.Exists(@"D:\TestingSHARPTEST"))
            {
                return anotherDrivePath;
            }

            // neither folder exists
            Console.WriteLine("Please make sure a drive is available for the download folder.");
            // Environment.Exit(1) is used  to immediately stop a running application and 
            // signal to the operating system that the program closed due to an error
            Environment.Exit(1);

            return string.Empty;
        }
    }
}