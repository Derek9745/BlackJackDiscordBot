
using DSharpPlus.Entities;
using DSharpPlus.Interactivity.Extensions;
using DSharpPlus;
using DSharpPlus.SlashCommands;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using DSharpPlus.EventArgs;
using System.Runtime.Remoting.Messaging;
//using static BlackJackSimulator;


namespace BlackJackDiscordBot
{
    public class BlackJackSimulator
    {
        public List<Player> currentPlayerList = new List<Player>();
        Dealer dealer = new Dealer(new List<Card>(), "Dealer", 0);
        public Deck currentDeck = new Deck();
        public Player currentPlayer = null;
        public List<Card> cardList = new List<Card>();

        public enum Suit { Heart, Diamond, Spade, Club };
        public enum Rank { Ace = 11, Two = 2, Three = 3, Four = 4, Five = 5, Six = 6, Seven = 7, Eight = 8, Nine = 9, Ten = 10, Jack = 10, Queen = 10, King = 10 };

        public class Deck
        {
            public List<Card> deck = new List<Card>();
            private static Random rng = new Random();

            public void loadDeck(List<Card> cardList)
            {
                deck.Clear();
                deck.AddRange(cardList);
                shuffleDeck();
            }

            public void shuffleDeck()
            {
                // uses fisher-yates shuffle
                int n = deck.Count;
                while (n > 1)
                {
                    n--;
                    int k = rng.Next(n + 1);
                    Card value = deck[k];
                    deck[k] = deck[n];
                    deck[n] = value;
                }
            }
        }

        public class Card
        {
            public Rank Rank;
            public Suit Suit;
            public bool isVisible;

            public Card(Rank rank, Suit suit)
            {
                Suit = suit;
                Rank = rank;
            }
        }

        public class Player
        {
            public List<Card> hand = new List<Card>();
            public string playerName;
            public int currentCardValue;

            public Player(List<Card> hand, string playerName, int currentCardValue)
            {
                this.hand = hand;
                this.playerName = playerName;
                this.currentCardValue = currentCardValue;
            }
        }

        public class Dealer : Player
        {
            public Dealer(List<Card> hand, string playerName, int currentCardValue) : base(hand, playerName, currentCardValue) { }
        }

        public async Task<DiscordEmbedBuilder> GameSetup()
        {
            Player player = new Player(new List<Card>(), "Player", 0);
            currentPlayerList.Add(player);
            currentPlayerList.Add(dealer);
            currentPlayer = currentPlayerList[0];
            generateCards(cardList);
            currentDeck.loadDeck(cardList);
            dealHands(currentPlayerList, currentDeck);
            calculatePlayerCardValue(currentPlayer);

           
            var embed = new DiscordEmbedBuilder()
            {
                Title = "Game Started",
                Description = $"Starting deck size: 52" +
                $"\n Current deck size: {currentDeck.deck.Count}" +
                $"\n {currentPlayer.playerName} hand:--- current card value:{currentPlayer.currentCardValue}"
            };

            return embed;
        }

        public void generateCards(List<Card> cardlist)
        {
            foreach (Suit suit in Enum.GetValues(typeof(Suit)))
            {
                foreach (Rank rank in Enum.GetValues(typeof(Rank)))
                {
                    //add new card combination to the cardlist
                    cardList.Add(new Card(rank, suit));
                }
            }
        }

        public void dealHands(List<Player> currentPlayerList, Deck currentDeck)
        {
            foreach (var player in currentPlayerList)
            {
                player.hand.Clear();
                player.hand.Add(currentDeck.deck[0]);
                currentDeck.deck.RemoveAt(0);
                player.hand.Add(currentDeck.deck[0]);
                currentDeck.deck.RemoveAt(0);
            }
        }

        public async Task<DiscordEmbedBuilder> Hit(ComponentInteractionCreateEventArgs args)
        {
            currentPlayer.hand.Add((currentDeck.deck[0]));
            currentDeck.deck.RemoveAt(0);
            var cardValue = calculatePlayerCardValue(currentPlayer);

            if (cardValue< 21)
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Description = "Current deck size: " + Convert.ToString(currentDeck.deck.Count) +
                    $"\n {args.User.Username} hand: ------ current card value:{cardValue}"
                };

                return embed;

            }
            else if (cardValue == 21)
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Title = "21! Congratulations, You Win!",
                    Description = "Current deck size: " + Convert.ToString(currentDeck.deck.Count) +
                    $"\n {args.User.Username} hand:  ---- current card value:{cardValue}"
                };

                return embed;
                
            }
            else if (cardValue > 21)
            {
                var embed = new DiscordEmbedBuilder()
                {
                    Title = "You Exceeded 21! You Lose!",
                    Description = "Current deck size: " + Convert.ToString(currentDeck.deck.Count) +
                    $"\n {args.User.Username} hand:  ----- current card value:{cardValue}"
                };

                return embed;
            }
            else
            {
                // Handle other cases if necessary, or return null if no embed is needed.
                return null;
            }
        }

        

        public string getDeckSize()
        {
            string deckSize = Convert.ToString(currentDeck.deck.Count);
            return deckSize;
        }

        public int calculatePlayerCardValue(Player currentPlayer)
        {
            currentPlayer.currentCardValue = 0;
            foreach (var card in currentPlayer.hand)
            {
                //get the currentcardvalue of the players hand, using default ace values of 11
                currentPlayer.currentCardValue += Convert.ToInt32(card.Rank);
            }
            //if the hand value is over 21, check again for any aces, and reduce the players hand value by 10 for each one until the player is at or below 21
            if (currentPlayer.currentCardValue > 21)
            {
                foreach (var card in currentPlayer.hand)
                {
                    if (card.Rank == Rank.Ace)
                    {
                        currentPlayer.currentCardValue -= 10;
                        if (currentPlayer.currentCardValue <= 21)
                        {
                            break;
                        }
                    }
                }
            }
            return currentPlayer.currentCardValue;
        }
   
        public async Task<DiscordEmbedBuilder> DealerTurn(ComponentInteractionCreateEventArgs args)
        {

            while (dealer.currentCardValue <= 16)
            {
                dealer.hand.Add(currentDeck.deck[0]);
                currentDeck.deck.RemoveAt(0);
                calculatePlayerCardValue(dealer);
                await Task.Delay(1000);
            }


            var embed = new DiscordEmbedBuilder()
            {
                Description = $"Dealer hand: ---- current card value: {dealer.currentCardValue}"
            };

            if (dealer.currentCardValue > 21)
            {
                embed.Title = "Dealer Exceeded 21! You Win!";
            }
            else if (dealer.currentCardValue >= currentPlayer.currentCardValue)
            {
                embed.Title = "Dealer Wins!";
            }
            else
            {
                embed.Title = "You Win!";
            }

            return embed;
        }

    }
    
}