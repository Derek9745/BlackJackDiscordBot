using DSharpPlus.CommandsNext;
using DSharpPlus.CommandsNext.Attributes;
using DSharpPlus.Entities;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace BlackJackDiscordBot.Commands
{
    internal class BlackJackCommands : BaseCommandModule
    {
        [Command("test")]

        public async Task MyFirstCommand(CommandContext ctx)
        {
            await ctx.Channel.SendMessageAsync($"Hello{ctx.User.Username}");
        }

        [Command("embed")]

        public async Task EmbededMessage(CommandContext ctx)
        {
            var message = new DiscordEmbedBuilder
            {
                Title = "this is my first Discord Embed",
                Description = $"This command was executed by {ctx.User.Username}",
                Color = DiscordColor.Blue
            };

            await ctx.Channel.SendMessageAsync(embed:message);
        }

        [Command("PlayBlackJack")]

        public async Task BlackJack(CommandContext ctx)
        {
            BlackJackSimulator blackJackSimulator = new BlackJackSimulator();
       
            var blackJackEmbed = new DiscordEmbedBuilder
            {
                Title ="Game Started!",
                Description = $"Player hand Dealt.\n" +
                $"{blackJackSimulator.gameSetup()}" +
                $"\n Hands have been dealt. What would you like to do? Please enter a number value for a command" +
                $"\n Current Deck size: {blackJackSimulator.getDeckSize()}",
                Color = DiscordColor.Blue
            };

            await ctx.Channel.SendMessageAsync(embed: blackJackEmbed);

           





        }

        
    }
}
