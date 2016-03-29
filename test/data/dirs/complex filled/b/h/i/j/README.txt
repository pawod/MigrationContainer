————————————————————————
WINDOWS
————————————————————————
You need to have the following things prepared in order to run the 8TrackGrabber:

- .NET Framework 4.5
  web installer: http://www.microsoft.com/en-us/download/details.aspx?id=42643

- WinPcap Driver
  The driver, that enables the grabber to sniff the incoming/outgoing network traffic between you and the 8Tracks server.
  download: http://www.winpcap.org/install/default.htm

- copy the SharpPcap.dll from the WIN directory to the bin directory


————————————————————————
OS X
————————————————————————
You need to have the following things prepared in order to run the 8TrackGrabber:

- Mono Framework
  You will need either the MRE or the SDK of the Mono framework in order to run this tool. 
  download: http://www.mono-project.com/download/

- LibPcap Driver
  That's a driver, that enables the grabber to sniffing the incoming/outgoing network traffic between you and the 8Tracks server.
  download: http://www.tcpdump.org/#latest-release
  Unpack it and follow its instructions inside the README to install it.

- copy the "SharpPcap.dll" and "SharpPcap.dll.config" from the OS X directory to the "../bin/Release" directory


————————————————————————
HOW TO USE
————————————————————————
1. Run the EightTracksGrabber.exe (inside "../bin/Release" directory)
   WINDOWS: simply double click the file
   OS X: open a new terminal window, navigate to the "Release" directory and run following command: "mono EightTracksGrabber.exe"

2. A list of available network devices will appear. Choose the network device you are currently using to connect with the internet, by entering the number of the device ID. In most cases you will use either the WiFi or Ethernet adapter. On MAC OS X it is mostly called "en0".

3. Any time you want to use the grabber make sure you are not visiting through HTTPS: Just remove the "https://" prefix in front of the URL and you will be listening through HTTP. Else the grabber won’t be able to track the traffic between you and the server. 

4. Pick a playlist on 8Tracks.com and play it. The grabber will download it, while you listen to it. You can find the downloaded tracks by default in the 8Tracks folder on your desktop. Remember, that the tool must be started, before you open a playlist the first time so it can read the current playlist’s name and the user who created it.

5. Report your errors and logfiles to me so I can further improve this tool. The logfile will be placed inside the log folder once you start the tool (right next to the .exe). If you are missing some feature let me know, I will add it to my TODO list.


————————————————————————
CONFIGURATION (OPTIONAL)
————————————————————————
If you wish to change the directory you can do so by adding the path to the EightTracksGrabber.exe.config (use a text editor) to the "downloadsFolder" key.

Example: <add key="downloadsFolder" value="C:\someDirectory\My8TracksDownloads"/>