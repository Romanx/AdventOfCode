using MoreLinq;

namespace DayTwentyTwo2020
{
    public class Challenge : Shared.Challenge
    {
        public override ChallengeInfo Info { get; } = new ChallengeInfo(new LocalDate(2020, 12, 22), "Crab Combat");

        public void PartOne(IInput input, IOutput output)
        {
            var (playerOneDeck, playerTwoDeck) = input.ParseGameState();

            while (playerOneDeck.Count > 0 && playerTwoDeck.Count > 0)
            {
                var playerOneCard = playerOneDeck.Dequeue();
                var playerTwoCard = playerTwoDeck.Dequeue();

                if (playerOneCard > playerTwoCard)
                {
                    playerOneDeck.Enqueue(playerOneCard);
                    playerOneDeck.Enqueue(playerTwoCard);
                }
                else if (playerTwoCard > playerOneCard)
                {
                    playerTwoDeck.Enqueue(playerTwoCard);
                    playerTwoDeck.Enqueue(playerOneCard);
                }
                else
                {
                    throw new InvalidOperationException("Rules are fuzzy on this!");
                }
            }

            var winningDeck = playerOneDeck.Count > 0
                ? playerOneDeck
                : playerTwoDeck;

            var total = winningDeck
                .Reverse()
                .Index(1)
                .Select(k => k.Key * k.Value)
                .Sum();

            output.WriteProperty("Winning Deck", string.Join(", ", winningDeck));
            output.WriteProperty("Total Points", total);
        }

        public void PartTwo(IInput input, IOutput output)
        {
            var (playerOneDeck, playerTwoDeck) = input.ParseGameState();

            PlayGame(playerOneDeck, playerTwoDeck);

            var winningDeck = playerOneDeck.Count > 0
                ? playerOneDeck
                : playerTwoDeck;

            var total = winningDeck
                .Reverse()
                .Index(1)
                .Select(k => k.Key * k.Value)
                .Sum();

            output.WriteProperty("Winning Deck", string.Join(", ", winningDeck));
            output.WriteProperty("Total Points", total);

            static GameState PlayGame(Queue<int> playerOneDeck, Queue<int> playerTwoDeck)
            {
                var deckStates = new HashSet<(string DeckOneState, string DeckTwoState)>();

                while (playerOneDeck.Count > 0 && playerTwoDeck.Count > 0)
                {
                    var (result, cards) = PlayRound(deckStates, playerOneDeck, playerTwoDeck);

                    if (result == GameState.PlayerOneWinsGame)
                        return result;

                    var winningDeck = result == GameState.PlayerOneWinsRound
                        ? playerOneDeck
                        : playerTwoDeck;

                    foreach (var card in cards)
                    {
                        winningDeck.Enqueue(card);
                    }
                }

                return playerOneDeck.Count > 0
                    ? GameState.PlayerOneWinsGame
                    : GameState.PlayerTwoWinsGame;
            }

            static (GameState, int[] cards) PlayRound(HashSet<(string DeckOneState, string DeckTwoState)> deckStates, Queue<int> playerOneDeck, Queue<int> playerTwoDeck)
            {
                var state = (string.Join(",", playerOneDeck), string.Join(",", playerTwoDeck));
                if (deckStates.Contains(state))
                {
                    return (GameState.PlayerOneWinsGame, Array.Empty<int>());
                }

                deckStates.Add(state);

                var playerOneCard = playerOneDeck.Dequeue();
                var playerTwoCard = playerTwoDeck.Dequeue();

                RoundState roundState;

                if (playerOneDeck.Count >= playerOneCard && playerTwoDeck.Count >= playerTwoCard)
                {
                    var copyDeckOne = new Queue<int>(playerOneDeck.Take(playerOneCard));
                    var copyDeckTwo = new Queue<int>(playerTwoDeck.Take(playerTwoCard));
                    var result = PlayGame(copyDeckOne, copyDeckTwo);

                    roundState = result == GameState.PlayerOneWinsGame
                        ? RoundState.PlayerOneWinsRound
                        : RoundState.PlayerTwoWinsRound;
                }
                else
                {
                    if (playerOneCard > playerTwoCard)
                    {
                        roundState = RoundState.PlayerOneWinsRound;
                    }
                    else if (playerTwoCard > playerOneCard)
                    {
                        roundState = RoundState.PlayerTwoWinsRound;
                    }
                    else
                    {
                        throw new InvalidOperationException("Someone copied a card!");
                    }
                }

                if (roundState == RoundState.PlayerOneWinsRound)
                {
                    return (GameState.PlayerOneWinsRound, new[] { playerOneCard, playerTwoCard });
                }
                else if (roundState == RoundState.PlayerTwoWinsRound)
                {
                    return (GameState.PlayerTwoWinsRound, new[] { playerTwoCard, playerOneCard });
                }
                else
                {
                    throw new InvalidOperationException("Someone copied a card!");
                }
            }
        }
    }

    enum GameState
    {
        NotSet,
        PlayerOneWinsGame,
        PlayerTwoWinsGame,
        PlayerOneWinsRound,
        PlayerTwoWinsRound
    }

    enum RoundState
    {
        NotSet,
        PlayerOneWinsRound,
        PlayerTwoWinsRound
    }

    internal static class ParseExtensions
    {
        public static (Queue<int> PlayerOne, Queue<int> PlayerTwo) ParseGameState(this IInput input)
        {
            var paragraphs = input.Lines.AsParagraphs();

            return (ParseDeck(paragraphs[0]), ParseDeck(paragraphs[1]));

            static Queue<int> ParseDeck(ReadOnlyMemory<ReadOnlyMemory<char>> readOnlyMemory)
            {
                var deck = new Queue<int>();

                foreach (var item in readOnlyMemory.Span[1..])
                {
                    deck.Enqueue(int.Parse(item.Span));
                }

                return deck;
            }
        }
    }
}
