using System.Text;
using System.Text.Json;
using System.Net.Sockets;
using System.Diagnostics;

namespace sharptest
{
    class Program
    {
        static readonly HttpClient httpClient = new HttpClient();
        static async Task Main(string[] args)
        {
            TitleScreen();

            Console.ResetColor();

            Console.WriteLine("Type \"A\" to ping the five closest servers to you");
            Console.WriteLine("Type \"B\" to check the CAPPED download speed of the nearest server to you");


            Console.Write("Enter your choice: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "A":
                    await PingTopFiveServers.GetServersToPing();
                    break;
                case "B":
                    await DownloadFromClosestServer.DownloadSpeedTester();
                    break;
                default:
                    Console.WriteLine("Not a valid option.");
                break;
            }
                        
            Console.WriteLine("\n Press any key to exit.");
            Console.ReadKey();
        }
        public static void TitleScreen()
        {
            Console.ForegroundColor = ConsoleColor.Red;

            string ascii = @"
                 _____  __                    ______          __ 
                / ___/ / /_  ____ __________ /_  __/__  _____/ /_
                \__ \ / __ \/ __ `/ ___/ __ \ / / / _ \/ ___/ __/
                ___/ / / / / /_/ / /  / /_/ // / /  __(__  ) /_  
               /____/_/ /_/\__,_/_/  / .___/__/  \___/____/\__/  
                                    /_/                                                      
            ";

            Console.WriteLine(ascii);
        }
    }
}