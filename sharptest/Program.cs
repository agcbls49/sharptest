using System.IO;

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
            Console.WriteLine("Type \"C\" to check the CAPPED upload speed of the nearest server to you");
            Console.WriteLine("Type \"D\" to run a 100 MB download test (Singapore server)");
            Console.WriteLine("Type \"E\" to run a 1 GB download test (Singapore server)");

            Console.WriteLine();
            Console.Write("Enter your choice: ");
            string input = Console.ReadLine();

            switch (input)
            {
                case "A" or "a":
                    await PingTopFiveServers.GetServersToPing();
                    break;
                case "B" or "b":
                    await DownloadFromClosestServer.DownloadSpeedTester();
                    break;
                case "C" or "c":
                    await UploadFromClosestServer.UploadSpeedTester();
                    break;
                case "D" or "d":
                    DownloadPath.CreateDirectory();
                    await DownloadOneHundredMB.DownloadSpeedInSG();
                    break;
                case "E" or "e":
                    DownloadPath.CreateDirectory();
                    await DownloadOneGB.DownloadSpeedInSG();
                    break;
                default:
                    Console.WriteLine("Not a valid option.");
                break;
            }
            Console.WriteLine();
            Console.WriteLine("Press any key to exit.");
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