using PacketDotNet;
using SharpPcap;

namespace PhotonPackageParser.Example
{
    class Program
    {
        private readonly PhotonParser photonParser;
        private ILiveDevice device;

        public Program()
        {
            photonParser = new ExampleParser();
        }

        public static void Main(string[] args)
        {
            new Program().Start().Wait();
        }

        private async Task Start()
        {
            var devices = CaptureDeviceList.Instance;
            if (devices.Count == 0)
            {
                Console.WriteLine("No network devices found.");
                return;
            }

            Console.WriteLine("Available network devices:");
            for (int i = 0; i < devices.Count; i++)
            {
                Console.WriteLine($"{i}: {devices[i].Description}");
            }

            Console.Write("Select a device by number: ");
            if (!int.TryParse(Console.ReadLine(), out int selectedDeviceIndex) || selectedDeviceIndex < 0 || selectedDeviceIndex >= devices.Count)
            {
                Console.WriteLine("Invalid selection.");
                return;
            }

            device = devices[selectedDeviceIndex];

            device.OnPacketArrival += PacketHandler;
            device.Open(DeviceModes.Promiscuous);

            Console.WriteLine("Packet reception started. Press 'Q' to exit...");

            var captureTask = Task.Run(() => device.StartCapture());

            while (true)
            {
                var key = Console.ReadKey(intercept: true).Key;
                if (key == ConsoleKey.Q)
                {
                    break;
                }
            }

            device.StopCapture();
            device.Close();
            Console.WriteLine("Packet reception stopped.");

            await captureTask;
        }

        private void PacketHandler(object sender, PacketCapture e)
        {
            var rawPacket = e.GetPacket();
            var packet = Packet.ParsePacket(rawPacket.LinkLayerType, rawPacket.Data);

            var udpPacket = packet.Extract<UdpPacket>();
            if (udpPacket != null && (udpPacket.SourcePort == 5056 || udpPacket.DestinationPort == 5056))
            {
                Console.WriteLine($"Received UDP packet with length: {udpPacket.PayloadData.Length} bytes");
                photonParser.ReceivePacket(udpPacket.PayloadData);
            }
        }
    }
}