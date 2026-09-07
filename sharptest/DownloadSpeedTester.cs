namespace sharptest
{
    using System.Text;
    using System.Text.Json;
    using System.Net.Sockets;
    using System.Diagnostics;

    public static class DownloadFromClosestServer
    {
        static readonly HttpClient httpClient = new HttpClient();
        public static async Task DownloadSpeedTester()
        {
            await DownloadData();
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        public static async Task DownloadData()
        {
            string dashes = "-----------------------------------";

            // read the json file containing the public servers
            string jsonText = File.ReadAllText("servers.json");

            // make the deserializer ignore case
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

            // deserialize json and map it to C# objects
            // null-coalescing or ?? operator provides a backup if .Deserialize returns null
            List<SpeedTestServer> servers = JsonSerializer.Deserialize<List<SpeedTestServer>>(jsonText, options) ?? throw new InvalidOperationException("JSON data is null.");

            // list to hold each server paired with its distance
            var serverDistances = new List<(SpeedTestServer server, double distance)>();

            try
            {   
                // get public IP address
                string publicIPAdd = await httpClient.GetStringAsync("https://api.ipify.org");
                Console.WriteLine("Public IP Address: " + publicIPAdd);
                
                // send IP address to geo locator
                string geolocRawJsonResponse = await httpClient.GetStringAsync($"http://ip-api.com/json/{publicIPAdd}");
                GeoLocation myLocation = JsonSerializer.Deserialize<GeoLocation>(geolocRawJsonResponse, options) ?? throw new InvalidOperationException("JSON data is null.");
                Console.WriteLine($"Your location: {myLocation.City}, Lat: {myLocation.Lat}, Long: {myLocation.Lon}"); 

                // iterate through list of servers in the JSON file
                for (int i = 0; i < servers.Count; i++)
                {
                    SpeedTestServer server = servers[i];
                    double distance = HaversineFormula.CalculateDistance(myLocation.Lat, myLocation.Lon, server.Lat, server.Long);
                    serverDistances.Add((server, distance));
                }

                // sort the completed list with smallest distance first
                var sortedDistances = serverDistances.OrderBy(x => x.distance).ToList();

                // take the 5 closest servers 
                var closestServers = sortedDistances.Take(5).ToList();

                // call the function which will test all 5 servers
                var pingResults = new List<(SpeedTestServer server, long pingMs)>();
                
                foreach (var entry in closestServers)
                {
                    Console.WriteLine($"Connecting to server {entry.server.Host}:{entry.server.Port} ...");
                    long pingMs = await PingTopFiveServers.PingAllServers(entry.server.Host, entry.server.Port);
                    pingResults.Add((entry.server, pingMs));
                }

                // sort by actual measured ping time
                var sortedPingResults = pingResults.OrderBy(x => x.pingMs).ToList();

                foreach (var result in sortedPingResults)
                {
                    Console.WriteLine(dashes);
                    Console.WriteLine("Ping Results:");
                    Console.WriteLine($"City: {result.server.City}, Host: {result.server.Isp}, Ping: {result.pingMs} ms");
                }

                var bestServer = sortedPingResults[0].server;

                Console.WriteLine($"Connecting to server {bestServer.Host}:{bestServer.Port} ...");

                // start the time
                Stopwatch stopwatch = Stopwatch.StartNew();

                // run the download code 4 times
                var task1 = DownloadDataOnTheServer(bestServer.Host, bestServer.Port);
                var task2 = DownloadDataOnTheServer(bestServer.Host, bestServer.Port);
                var task3 = DownloadDataOnTheServer(bestServer.Host, bestServer.Port);
                var task4 = DownloadDataOnTheServer(bestServer.Host, bestServer.Port);
                var task5 = DownloadDataOnTheServer(bestServer.Host, bestServer.Port);
                var task6 = DownloadDataOnTheServer(bestServer.Host, bestServer.Port);
                var task7 = DownloadDataOnTheServer(bestServer.Host, bestServer.Port);
                var task8 = DownloadDataOnTheServer(bestServer.Host, bestServer.Port);

                // combine all 4 download results and sum them together
                long[] results = await Task.WhenAll(task1, task2, task3, task4, task5, task6, task7, task8);
                
                // stop time
                stopwatch.Stop();

                // store time and combine the 4 results 
                long elapsedMs = stopwatch.ElapsedMilliseconds;
                long totalBytes = results.Sum();

                double mbps = (totalBytes * 0.008) / elapsedMs;

                Console.WriteLine(dashes);
                Console.WriteLine("Download Results:");
                Console.WriteLine($"City: {bestServer.City}, Host: {bestServer.Isp}, Total Bytes: {totalBytes}, Time: {elapsedMs} ms, Server Download Speed Capped At: {mbps:F2} Mbps");            
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong: ", e.Message);
            }
        }
        public static async Task<long> DownloadDataOnTheServer(string serverIP, int serverPort)
        {
            try
            {
                // create tcp client to reach out to a server
                using TcpClient tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIP, serverPort);
                Console.WriteLine("Connected to the server successfully! \n");

                // allows to send or receive data from a stream socket
                using NetworkStream networkStream = tcpClient.GetStream();

                // send a message to the server which is to download data
                // 20MB
                string messageToSend = "DOWNLOAD 20000000\n";
                byte[] sendBuffer = Encoding.UTF8.GetBytes(messageToSend);

                // converts text message into bytes so it can actually be transmitted to server
                await networkStream.WriteAsync(sendBuffer, 0, sendBuffer.Length);

                // read response back from the server
                // 64kb buffer
                byte[] receiveBuffer = new byte[65536];

                // stores the bytes received from the server
                long totalBytesReceived = 0;
                long targetBytes = 20000000;

                while (totalBytesReceived < targetBytes)
                {
                    int bytesReadThisChunk = await networkStream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);

                    if (bytesReadThisChunk == 0)
                    {
                        break;
                    }

                    totalBytesReceived += bytesReadThisChunk;
                    Console.WriteLine($"BYTES READ THIS CHUNK: {bytesReadThisChunk}");
                }

                return totalBytesReceived;
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Http Request Error: {e.Message}");
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Socket Error: {e.Message}");
            }

            // Error occurred
            return 0;
        }
        public class SpeedTestServer
        {
            public string City { get; set; } = string.Empty;
            public string Isp { get; set; } = string.Empty;
            public string Host { get; set; } = string.Empty;
            public int Port { get; set; }
            public double Lat { get; set; }
            public double Long { get; set; }
        }
        public class GeoLocation
        {
            public string Status { get; set; } = string.Empty;
            public string City { get; set; } = string.Empty;
            public double Lat { get; set; }
            // geolocation api uses lon instead of long 😡
            public double Lon { get; set; }
        }
    }
}