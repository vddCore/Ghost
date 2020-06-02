using Ghost.Xenus;
using System.Threading.Tasks;
using Terminal = System.Console;

namespace Ghost.Console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var client = new Client();
            client.ChatStarted += Client_ChatStarted;
            client.RawPacketReceived += Client_RawPacketReceived;
            client.RawPacketSent += Client_RawPacketSent;
            client.ChatMessageReceived += Client_ChatMessageReceived;

            await client.Connect();

            while (true)
            {
                var input = Terminal.ReadLine();

                switch (input)
                {
                    case "_sas":
                        await client.StartNewChat();
                        break;

                    case "_pmsg":
                        await client.SendMessage("Ohai! Jak się masz? :D");
                        break;

                    case "_mtyp:0":
                        await client.SendTypingState(false);
                        break;

                    case "_mtyp:1":
                        await client.SendTypingState(true);
                        break;

                    case "_distalk":
                        await client.EndChat();
                        break;
                }
            }
        }

        private static void Client_ChatMessageReceived(object sender, ChatMessageEventArgs e)
        {
            Terminal.WriteLine(e.Message.Body);
        }

        private static void Client_RawPacketReceived(object sender, RawDataEventArgs e)
        {
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
