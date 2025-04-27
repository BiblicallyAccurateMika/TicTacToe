using TicTacToe.Version1;
using TicTacToe.Version2;
using IInteractionResponse_1 = TicTacToe.Version1.IInteractionResponse;
using MoveRequest_1 = TicTacToe.Version1.MoveRequest;
using MoveRequest_2 = TicTacToe.Version2.MoveRequest;
using MoveResponse_1 = TicTacToe.Version1.MoveResponse;
using MoveResponse_2 = TicTacToe.Version2.MoveResponse;

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
            Console.WriteLine("0. Exit");
            Console.WriteLine("1. Play Game (Version 1)");
            Console.WriteLine("2. Play Game (Version 2)");
            
            Console.Write("Selection: ");
            var input = Console.ReadLine();

            switch (input)
            {
                case "0":
                    exit = true;
                    break;
                case "1":
                    runGame1();
                    break;
                case "2":
                    runGame2();
                    break;
                default:
                    Console.WriteLine("Invalid input");
                    break;
            }
        } while (!exit);
    }

    private static void runGame1()
    {
        var processManager = new ProcessManager();
        IInteractionResponse_1? response = null;

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
                        case MoveRequest_1:
                            drawBoard(result.NewState.Board, result.NewState.IsPlayer0Turn);
                            Console.Write("Move: ");
                            var input = Console.ReadLine();
                            var move = getCoordsFromInput(input);
                            response = new MoveResponse_1(move.Item1, move.Item2);
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

    private static void runGame2()
    {
        var processManager = new BoardProcessManager();
        InteractionResponse? response = null;

        do
        {
            try
            {
                processManager.Run(response);

                drawBoard(processManager.State.Board, !processManager.State.IsPlayerXTurn);

                switch (processManager.Request)
                {
                    case null: continue;
                    case MoveRequest_2:
                        Console.Write("Move: ");
                        var input = Console.ReadLine();
                        var move = getCoordsFromInput(input);
                        response = new MoveResponse_2(move.Item2, move.Item1);
                        break;
                    default: throw new ArgumentOutOfRangeException();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Fehler: {e}");
                response = null;
                Thread.Sleep(1000);
            }
        } while (processManager.Request != null);
        
        var winner = processManager.State.Winner!.Value ? Player1 : Player0;
        Console.WriteLine($"Player {winner} Wins!");
    }

    private static void drawBoard(bool?[] Board, bool player0Turn)
    {
        var player = player0Turn ? Player0 : Player1;
        
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
        
        var b = Board.Select(x => x.HasValue ? (x.Value ? "X" : "0") : " ").ToArray();
        
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