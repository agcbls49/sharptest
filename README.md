# sharptest

A C# console app that pings nearby Philippine speed test servers, picks the fastest one, then downloads/uploads data from it using 8 parallel connections.

> [!NOTE]
> **The result reflects that SERVER'S OWN SPEED LIMIT**, not necessarily your real internet speed. It uses TCP because of Ookla protocol which the servers from the JSON list understand.
> This is **Philippines ONLY** and results will vary depending on location. It does not support other countries and the server list in the JSON file of the project was curated by me.

Servers List from [William Yap](https://williamyaps.github.io/wlmjavascript/servercli.html).

## Singapore Server Download Test

The program also includes separate 100 MB and 1 GB download tests using a **Linode server located in Singapore**.

The Singapore server is:

`http://speedtest.singapore.linode.com/`

The available tests are:

- **D** — 100 MB download test from the Singapore server
- **E** — 1 GB download test from the Singapore server

These tests download the file from the Singapore Linode server and save it locally to the `TestingSHARPTEST` folder.

The program checks for the folder in these locations:

- `C:\TestingSHARPTEST`
- `D:\TestingSHARPTEST`

If the folder already exists in either location, it uses that location. If neither exists, the program tries to create it on C first, then D if C cannot be used.