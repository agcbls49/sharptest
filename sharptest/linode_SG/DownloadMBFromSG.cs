namespace sharptest
{
    using System.Diagnostics;
    using System.Net.Http;

    public static class DownloadOneHundredMB 
    {
        static readonly HttpClient client = new HttpClient();

        public static async Task DownloadSpeedInSG()
        {
            string dashes = "-----------------------------------";
            Console.WriteLine();
            Console.WriteLine(dashes);

            string linodeFileUrl = "http://speedtest.singapore.linode.com/100MB-singapore.bin";
            string savePath = DownloadPath.GetDownloadPath("100MB-singapore.bin");

            try
            {
                Console.WriteLine("Starting download...");

                Stopwatch stopwatch = Stopwatch.StartNew();
                long totalBytes = await DownloadDataInSG(linodeFileUrl, savePath);
                
                stopwatch.Stop();
                Console.WriteLine("Download complete successfully!");

                long elapsedMs = stopwatch.ElapsedMilliseconds;
                double mbps = totalBytes * 8.0 / 1_000_000.0 / (elapsedMs / 1000.0);

                Console.WriteLine(dashes);
                Console.WriteLine("Download Results:");
                Console.WriteLine($"Time: {elapsedMs} ms, SG Server Download Speed: {mbps:F2} Mbps");

                Console.WriteLine("Press any key to exit.");
                Console.ReadKey();
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Error finding website: {e.Message}");
            }
        }
        public static async Task<long> DownloadDataInSG(string linodeFileUrl, string savePath)
        {
            try
            {
                // send GET request from the Linode server in Singapore
                using Stream remoteStream = await client.GetStreamAsync(linodeFileUrl);

                // Write the data to local disk

                // FileShare.None tells the computer that no other process or code can open, 
                // read, or write to the file until the program is done and closes it
                using FileStream localStream = new FileStream(savePath, FileMode.Create, FileAccess.Write, FileShare.None);

                // copy the data from the server to the local disk
                await remoteStream.CopyToAsync(localStream);

                return localStream.Length;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
                return 0;
            }
        }
    }
}