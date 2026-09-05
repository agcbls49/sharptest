using System.Text;
using System.Text.Json;
using System.Net.Sockets;

namespace sharptest
{
    class Program
    {
        static readonly HttpClient httpClient = new HttpClient();
        static async Task Main(string[] args)
        {
            string serverIP = "speedtst-tim.asianvision.com.ph";
            int serverPort = 8080;

            try
            {
                // get public IP address
                string publicIPAdd = await httpClient.GetStringAsync("https://api.ipify.org");
                Console.WriteLine("Public IP Address: " + publicIPAdd);

                // send IP address to geo locator
                string geolocRawJsonResponse = await httpClient.GetStringAsync($"http://ip-api.com/json/{publicIPAdd}");
                
                using JsonDocument document = JsonDocument.Parse(geolocRawJsonResponse);
                var options = new JsonSerializerOptions { WriteIndented = true };
                string formattedJson = JsonSerializer.Serialize(document, options);
                Console.WriteLine(formattedJson);

                Console.WriteLine($"Connecting to server {serverIP}:{serverPort} ...");

                // create tcp client to reach out to a server
                using TcpClient tcpClient = new TcpClient();
                tcpClient.Connect(serverIP, serverPort);
                Console.WriteLine("Connected to the server successfully!");

                // allows to send or receive data from a streacm socket
                using NetworkStream networkStream = tcpClient.GetStream();

                // send a message to the server (in this case a hi)
                string messageToSend = "HI\n";
                byte[] sendBuffer = Encoding.UTF8.GetBytes(messageToSend);  
                
                // converts text message into bytes so it can actually be transmitted to server
                await networkStream.WriteAsync(sendBuffer, 0, sendBuffer.Length);
                Console.WriteLine($"Sent: {messageToSend}");

                // read response back from the server
                byte[] receiveBuffer = new byte[1024];

                // stores the bytes received from the server
                int bytesRead = await networkStream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);

                if (bytesRead > 0)
                {
                    // convert raw bytes from server to text
                    string responseMessage = Encoding.UTF8.GetString(receiveBuffer, 0, bytesRead);
                    Console.WriteLine($"Received from server: {responseMessage}");
                    
                    // send another message (in this case a ping) to the server 
                    string pingMessage = "PING " + DateTimeOffset.UtcNow.ToUnixTimeMilliseconds() + "\n";
                    byte[] pingBuffer = Encoding.UTF8.GetBytes(pingMessage);
                    
                    // converts text message into bytes so it can actually be transmitted to server
                    await networkStream.WriteAsync(pingBuffer, 0, pingBuffer.Length);
                    Console.WriteLine($"Sent: {pingMessage}");

                    // stores the bytes received from the server
                    int pingBytesRead = await networkStream.ReadAsync(receiveBuffer, 0, receiveBuffer.Length);

                    if(pingBytesRead > 0)
                    {
                        // convert raw bytes from server to text
                        string pingResponse = Encoding.UTF8.GetString(receiveBuffer, 0, pingBytesRead);
                        Console.WriteLine($"Received from server: {pingResponse}");
                        
                    }
                } 
                else
                {
                    Console.WriteLine("Server closed the connection without responding.");
                }
            }
            catch (HttpRequestException e)
            {
                Console.WriteLine("Http Request Error: ", e.Message);
            }
            catch (SocketException e)
            {
                Console.WriteLine("Socket Error: ", e.Message);
            }
            
            Console.WriteLine("Press any key to exit.");
            Console.ReadKey();
        }
    }
    public class SpeedTestServer
    {
        public string City { get; set; }
        public string Isp { get; set; }
        public string Host { get; set; }
        int Port { get; set; }
        public double Lat { get; set; }
        public double Lon { get; set; }
    }
}