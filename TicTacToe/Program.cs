using TicTacToe.Version1;

namespace TicTacToe;

class Program
{
    private const string Player0 = "0";
    private const string Player1 = "X";

    private static void Main(string[] args)
    {
        Console.WriteLine("Tic-Tac-Toe!");
        
        var exit = false;

        do
        {
            Console.WriteLine("1. Play Game");
            Console.WriteLine("2. Exit");
            
            Console.Write("Selection: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "1":
                    runGame();
                    break;
                case "2":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        } while (!exit);
    }

    private static void runGame()
    {
        var processManager = new ProcessManager();
        IInteractionResponse? response = null;

        while (!processManager.CurrentState.Winner.HasValue)
        {
            try
            {
                var result = processManager.Process(response);
                
                if (result.IsComplete)
                {
                    response = null;
                }
                else
                {
                    switch (result.InteractionRequest)
                    {
                        case MoveRequest:
                            drawBoard(result.NewState);
                            Console.Write("Move: ");
                            var input = Console.ReadLine();
                            var move = getCoordsFromInput(input);
                            response = new MoveResponse(move.Item1, move.Item2);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Fehler: {e}");
                response = null;
                Thread.Sleep(1000);
            }
        }
        
        var winner = processManager.CurrentState.Winner.Value ? Player1 : Player0;
        
        Console.WriteLine($"Player {winner} Wins!");
    }

    private static void drawBoard(BoardState boardState)
    {
        var player = boardState.IsPlayer0Turn ? Player0 : Player1;
        
        Console.Clear();
        Console.WriteLine($"Current Turn: {player}");
        
        /*  x 1 2 3
           y ┌─┬─┬─┐
           1 │ │ │ │
             ├─┼─┼─┤
           2 │ │ │ │
             ├─┼─┼─┤
           3 │ │ │ │
             └─┴─┴─┘ */
        
        var b = boardState.Board.Select(x => x.HasValue ? (x.Value ? "X" : "0") : " ").ToArray();
        
        Console.WriteLine(" x 1 2 3");
        Console.WriteLine("y ┌─┬─┬─┐");
        Console.WriteLine($"1 │{b[0]}│{b[1]}│{b[2]}│");
        Console.WriteLine("  ├─┼─┼─┤");
        Console.WriteLine($"2 │{b[3]}│{b[4]}│{b[5]}│");
        Console.WriteLine("  ├─┼─┼─┤");
        Console.WriteLine($"3 │{b[6]}│{b[7]}│{b[8]}│");
        Console.WriteLine("  └─┴─┴─┘");
    }

    private static Tuple<int, int> getCoordsFromInput(string? input)
    {
        if (String.IsNullOrWhiteSpace(input))
        {
            throw new ArgumentException($"'{nameof(input)}' cannot be null or whitespace.", nameof(input));
        }
        
        if (input.Length != 2)
        {
            throw new ArgumentException("Input must contain 2 characters");
        }

        if (!Int32.TryParse(input, out _))
        {
            throw new ArgumentException("Input must be a number");
        }
        
        var x = Int32.Parse(input.Substring(0, 1));
        var y = Int32.Parse(input.Substring(1, 1));

        if (x >= 1 && x <= 3 && y >= 1 && y <= 3)
        {
            return new Tuple<int, int>(x-1, y-1); // move to 0-indexed coords
        }
        
        throw new ArgumentException("Input must be between 1 and 3");
    }
}