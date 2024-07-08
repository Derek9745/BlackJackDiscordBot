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

        
    }
}
