using BlackJackDiscordBot.Commands;
using BlackJackDiscordBot.Slash;
using DSharpPlus;
using DSharpPlus.CommandsNext;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus.Interactivity;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DSharpPlus.Entities;
using DSharpPlus.EventArgs;

namespace BlackJackDiscordBot
{
    public class Program
    {
       
        private static DiscordClient Client { get; set; }
        private static CommandsNextExtension Commands { get; set; }
        private static SlashCommandsExtension SlashCommands { get; set; }
        

        static async Task Main(string[] args)
        {
           
            var jsonReader = new JSONReader();
            await jsonReader.ReadJSON();
            

            var discordConfig = new DiscordConfiguration()
            {
                Intents = DiscordIntents.All,
                Token = jsonReader.token,
                TokenType = TokenType.Bot,
                AutoReconnect = true
            };

            Client = new DiscordClient(discordConfig);

            //sets defaults for interactivity based events
            Client.UseInteractivity(new InteractivityConfiguration()
            {
                Timeout = TimeSpan.FromMinutes(2)
            });

            Client.Ready += Client_Ready;
            Client.ComponentInteractionCreated += Client_ComponentInteractionCreated;

            var commandsConfig = new CommandsNextConfiguration()
            {
                StringPrefixes = new string[] { jsonReader.prefix },
                EnableMentionPrefix = true,
                EnableDms = true,
                EnableDefaultHelp = false
            };

            Commands = Client.UseCommandsNext(commandsConfig);
            Commands.RegisterCommands<BlackJackCommands>();

            SlashCommands = Client.UseSlashCommands();
            SlashCommands.RegisterCommands<BasicSlashCommands>();

            await Client.ConnectAsync();
            await Task.Delay(-1);

        }

        private static Task Client_Ready(DiscordClient sender, ReadyEventArgs args)
        {
            return Task.CompletedTask;
        }

        private static async Task Client_ComponentInteractionCreated(DiscordClient sender, ComponentInteractionCreateEventArgs args)
        {
            switch (args.Interaction.Data.CustomId)
            {
                case "hit":
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{args.User.Username} has chosen to hit. Adding another card to the hand."));
                    var embed = await BasicSlashCommands.blackJackSimulator.Hit(args);
                    await args.Interaction.CreateFollowupMessageAsync(new DiscordFollowupMessageBuilder().AddEmbed(embed));
                    break;
                case "stay":
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{args.User.Username} has chosen to stay"));
                    await BasicSlashCommands.blackJackSimulator.DealerTurn(args);
                    break;
                case "surrender":
                    await args.Interaction.CreateResponseAsync(InteractionResponseType.ChannelMessageWithSource, new DiscordInteractionResponseBuilder().WithContent($"{args.User.Username} has chosen to surrender"));
                    break;
            }

        }


      
    }
}
