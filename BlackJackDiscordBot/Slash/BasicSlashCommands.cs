using DSharpPlus.SlashCommands;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using System;

namespace BlackJackDiscordBot.Slash
{
    public class BasicSlashCommands : ApplicationCommandModule 
    {
        public static BlackJackSimulator blackJackSimulator = new BlackJackSimulator();


        [SlashCommand("BlackJack", "Play a game of BlackJack")]
        public async Task playBlackJack(InteractionContext ctx)
        {
            await ctx.DeferAsync();

            var hitButton = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "hit", "Hit");
            var stayButton = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "stay", "Stay");
            var surrenderButton = new DiscordButtonComponent(DSharpPlus.ButtonStyle.Primary, "surrender", "Surrender");

            var embed = new DiscordEmbedBuilder
            {
                Title = "Blackjack Simulator",
                Description = $"This command was executed by {ctx.User.Username}",
                Color = DiscordColor.Blue
            };

            var embed2 = await blackJackSimulator.GameSetup();


            await ctx.EditResponseAsync(new DiscordWebhookBuilder()
                .AddEmbed(embed)
                .AddEmbed(embed2)
                .AddComponents(hitButton, stayButton, surrenderButton));
        }
    
    }

}
