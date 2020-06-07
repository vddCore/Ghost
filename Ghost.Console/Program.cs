using Ghost.Xenus;
using System.Threading.Tasks;
using Terminal = System.Console;

namespace Ghost.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new Client(false);
            client.ChatStarted += Client_ChatStarted;
            client.RawPacketReceived += Client_RawPacketReceived;
            client.RawPacketSent += Client_RawPacketSent;
            client.ChatMessageReceived += Client_ChatMessageReceived;

            await client.Connect();

            while (true)
            {
                Terminal.Write("> ");
                var input = Terminal.ReadLine();
                await client.SendJson(input);
            }
        }

        private static void Client_ChatMessageReceived(object sender, ChatMessageEventArgs e)
        {
            Terminal.WriteLine(e.Message.Body);
        }

        private static void Client_RawPacketReceived(object sender, RawDataEventArgs e)
        {
            if (e.RawPacket.Contains("count"))
                return;
            
            Terminal.WriteLine($" -> {e.RawPacket}");
        }

        private static void Client_RawPacketSent(object sender, RawDataEventArgs e)
        {
            Terminal.WriteLine($" <- {e.RawPacket}");
        }

        private static void Client_ChatStarted(object sender, ChatEventArgs e)
        {
            Terminal.WriteLine($"Conv started! {e.ChatInfo.ID}, {e.ChatInfo.Key}, {e.ChatInfo.IsFlagged}");
        }
    }
}
