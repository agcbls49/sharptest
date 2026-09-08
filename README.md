# sharptest

A C# console app that pings nearby Philippine speed test servers, picks the fastest one, then downloads/uploads data from it using 8 parallel connections.

> [!NOTE]
> **The result reflects that SERVER'S OWN SPEED LIMIT**, not necessarily your real internet speed. It uses TCP because of Ookla protocol which the servers from the JSON list understand.
> This is **Philippines ONLY** and results will vary depending on location. It does not support other countries and the server list in the JSON file of the project was curated by me.

Servers List from [William Yap](https://williamyaps.github.io/wlmjavascript/servercli.html).