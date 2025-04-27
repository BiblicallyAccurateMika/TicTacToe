namespace TicTacToe.Version2;

public record BoardState(bool?[] Board, bool IsPlayerXTurn) : StateHolder
{
    public bool? Winner
    {
        get
        { 
            // horizontal checks
            for (int i = 0; i < 3; i++)
            {
                var index = i * 3;

                if (Board[index].HasValue && Board[index + 1].HasValue && Board[index + 2].HasValue)
                {
                    var columnValue = (Board[index].Value ? 1 : 0) +
                                      (Board[index + 1].Value ? 1 : 0) +
                                      (Board[index + 2].Value ? 1 : 0);
                    
                    if (columnValue == 3) return true;
                    if (columnValue == 0) return false;
                }
            }
            
            // vertical checks
            for (int i = 0; i < 3; i++)
            {
                var index = i;

                if (Board[index].HasValue && Board[index + 3].HasValue && Board[index + 6].HasValue)
                {
                    var columnValue = (Board[index].Value ? 1 : 0) +
                                      (Board[index + 3].Value ? 1 : 0) +
                                      (Board[index + 6].Value ? 1 : 0);
                    
                    if (columnValue == 3) return true;
                    if (columnValue == 0) return false;
                }
            }
            
            // diagonal checks
            if (Board[0].HasValue && Board[4].HasValue && Board[8].HasValue)
            {
                var columnValue = (Board[0].Value ? 1 : 0) +
                                  (Board[4].Value ? 1 : 0) +
                                  (Board[8].Value ? 1 : 0);
                    
                if (columnValue == 3) return true;
                if (columnValue == 0) return false;
            }
            if (Board[2].HasValue && Board[4].HasValue && Board[6].HasValue)
            {
                var columnValue = (Board[2].Value ? 1 : 0) +
                                  (Board[4].Value ? 1 : 0) +
                                  (Board[6].Value ? 1 : 0);
                    
                if (columnValue == 3) return true;
                if (columnValue == 0) return false;
            }
            
            return null;
        }
    }

    public BoardState() : this(new bool?[9], false) { }
}

public record MoveRequest : InteractionRequest;
public record MoveResponse(int X, int Y) : InteractionResponse;

public class BoardProcessManager : ProcessManager<BoardState>
{
    protected override Process[] Processes =>
    [
        new(_ => !State.Winner.HasValue, response =>
        {
            switch (response)
            {
                case null: return new ProcessResult(State, new MoveRequest());
                case MoveResponse moveResponse:
                    if (State.Board[Manager.BoardCoordinateToIndex(moveResponse.X, moveResponse.Y)].HasValue)
                    {
                        throw new InvalidMoveException();
                    }
                    var newBoard = State.Board;
                    newBoard[Manager.BoardCoordinateToIndex(moveResponse.X, moveResponse.Y)] = State.IsPlayerXTurn;
                    return new ProcessResult(new BoardState(newBoard, !State.IsPlayerXTurn));
                default: throw new ArgumentException("Invalid response");
            }
        })
    ];
}