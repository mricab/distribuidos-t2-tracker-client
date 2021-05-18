using System;
using System.Threading;
using System.IO;
using System.Net;
using System.Net.Sockets;
using FixedSizedQueue;
using GPSGenerator;
using TrackerPackage;

namespace client
{
    class Program
    {
        // Propieties
        static int deviceNumber;        // Client's ID.
        static GPS GPSMngr;             // GPS
        static FixedSizedQueue<TrackerPkg> buffer = new FixedSizedQueue<TrackerPkg>();   // Queue of Packages
        static int locatorDelay;        // locationDelay
        static int senderDelay;         // sendDelay
        static string serverIP;         // Server's IP
        static int serverPort;          // Server's Port Number

        static void Main(string[] args)
        {
            // Initialization
            Random rnd = new Random();
            deviceNumber = rnd.Next(0,255);
            GPSMngr = new GPS();
            buffer.Limit = 4;       // Set queue (capacity)
            locatorDelay = 10000;    // Set locatorDelay
            senderDelay = 3000;     // Set sendDelay
            serverIP = "127.0.0.1"; // Server's IP
            serverPort = 7777;      // Server's Port

            // Threads
            var loc = new Thread(new ThreadStart(locator));
            var sen = new Thread(new ThreadStart(sender));
            var lis = new Thread(new ThreadStart(listener));

            // Start threads
            Console.WriteLine("Starting Device Nº {0}.", deviceNumber);            
            loc.Start();
            sen.Start();
            lis.Start();

            // End
            Console.WriteLine("Client up!");
        }

        // Locator
        static void locator() 
        {
            DateTime now;
            DateTime last = System.DateTime.Now;

            while(true) {
                now = System.DateTime.Now;
                if( now >= last.AddMilliseconds(locatorDelay) ) {
                    Console.WriteLine("\tStoring Location at {0}", now.ToString());
                    string location = GPSMngr.getLocation();
                    String timeStamp = now.ToString("yyyyMMddHHmmssffff");
                    TrackerPkg package = new TrackerPkg(0, deviceNumber, timeStamp, location);
                    buffer.Enqueue(package);
                    last = last.AddMilliseconds(locatorDelay);
                }
            }
        }

        // Sender
        static void sender()
        {
            DateTime now;
            DateTime last = System.DateTime.Now;

            UdpClient client = new UdpClient();   
            client.Connect(new IPEndPoint(IPAddress.Parse(serverIP),serverPort));
         
            while(true) {
                now = System.DateTime.Now;
                if(now >= last.AddMilliseconds(senderDelay) && buffer.Count()>0) {
                    try
                    {
                        Console.WriteLine("\tSending Package at {0}", now.ToString());
                        TrackerPkg package = buffer.TryPeek();
                        byte[] pkgBytes = package.GetBytes();
                        client.Send(pkgBytes,pkgBytes.Length);
                    }
                    catch (System.Exception)
                    {
                        // See: https://github.com/dotnet/runtime/issues/923
                        Console.WriteLine("\t\t(Sending aborted by O.S.)");
                    }
                    last = last.AddMilliseconds(locatorDelay); 
                }
            }
        }

        // Listener
        static void listener()
        {
            while(true) {
                UdpClient client = new UdpClient(9999);
                IPEndPoint remoteIp = new IPEndPoint(IPAddress.Parse(serverIP),0);
                byte[] receivedBytes = client.Receive(ref remoteIp);
                    client.Close();
                TrackerPkg response = new TrackerPkg(receivedBytes);
                TrackerPkg package = buffer.TryPeek();
                if (package.type == 1 && package.message_id == response.message_id) {
                    buffer.TryDequeue();
                }
                Console.WriteLine("\tLast package delivered sucessfully (Removed from queue).");
            }          
        }
    }
}