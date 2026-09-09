namespace sharptest
{
    using System.Text;
    using System.Text.Json;
    using System.Net.Sockets;
    using System.Diagnostics;

    public static class PingTopFiveServers
    {
        static readonly HttpClient httpClient = new HttpClient();
        public static async Task PingServers(string[] args)
        {
            // ping 5 servers 
            await GetServersToPing();

            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
        public static async Task GetServersToPing()
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
            Console.WriteLine();
            Console.WriteLine(dashes);

            try
            {   
                // get public IP address
                string publicIPAdd = await httpClient.GetStringAsync("https://api.ipify.org");
                Console.WriteLine("Public IP Address: " + publicIPAdd);
                
                // send IP address to geo locator
                string geolocRawJsonResponse = await httpClient.GetStringAsync($"http://ip-api.com/json/{publicIPAdd}");
                GeoLocation myLocation = JsonSerializer.Deserialize<GeoLocation>(geolocRawJsonResponse, options) ?? throw new InvalidOperationException("JSON data is null.");
                Console.WriteLine($"Your location: {myLocation.City}, Lat: {myLocation.Lat}, Long: {myLocation.Lon}"); 
                Console.WriteLine(dashes);

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
                Console.WriteLine();

                foreach (var entry in closestServers)
                {
                    Console.WriteLine($"Connecting to server {entry.server.Host}:{entry.server.Port} ...");
                    long pingMs = await PingAllServers(entry.server.Host, entry.server.Port);
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
            }
            catch (Exception e)
            {
                Console.WriteLine("Something went wrong: ", e.Message);
            }
            }
        // code for pinging servers
        public static async Task<long> PingAllServers(string serverIP, int serverPort)
        {
            try 
            {
                // create tcp client to reach out to a server
                using TcpClient tcpClient = new TcpClient();
                await tcpClient.ConnectAsync(serverIP, serverPort);
                Console.WriteLine("Connected to the server successfully! \n");

                // allows to send or receive data from a stream socket
                using NetworkStream networkStream = tcpClient.GetStream();

                // send a message to the server (in this case a hi)
                string messageToSend = "HI\n";
                byte[] sendBuffer = Encoding.UTF8.GetBytes(messageToSend);  
                
                // converts text message into bytes so it can actually be transmitted to server
                await networkStream.WriteAsync(sendBuffer, 0, sendBuffer.Length);

                // read response back from the server
                byte[] receiveBuffer = new byte[1024];

                // stores the bytes received from the server
                int bytesRead = await networkStream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);

                if (bytesRead > 0)
                {
                    // convert raw bytes from server to text
                    string responseMessage = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
                    
                    // send another message (in this case a ping) to the server 
                    string pingMessage = "PING " + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "\n";
                    byte[] pingBuffer = Encoding.UTF8.GetBytes(pingMessage);

                    // create the timer
                    Stopwatch stopwatch = Stopwatch.StartNew();

                    // record the timestamp when sending
                    DateTime requestSentTime = DateTime.UtcNow;
                    
                    // converts text message into bytes so it can actually be transmitted to server
                    await networkStream.WriteAsync(pingBuffer, 0, pingBuffer.Length);

                    // stores the bytes received from the server
                    int pingBytesRead = await networkStream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);

                    if(pingBytesRead > 0)
                    {
                        // stop timer after receiving a response from the server
                        stopwatch.Stop();
                        DateTime requestCompletedTime = DateTime.UtcNow;

                        // convert raw bytes from server to text
                        string pingResponse = Encoding.UTF8.GetString(receiveBuffer, 0, pingBytesRead);

                        // Get the elapsed time of ping sent to pong received
                        long milliseconds = stopwatch.ElapsedMilliseconds;

                        return milliseconds;
                    }
                } 
                else
                {
                    Console.WriteLine("Server closed the connection without responding.");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine($"Http Request Error: {e.Message}");
            }
            catch (SocketException e)
            {
                Console.WriteLine($"Socket Error: {e.Message}");
            }
            // Error occured
            return -1;
        }
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