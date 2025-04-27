namespace TicTacToe.Version1;

#region State

public record BoardState(bool?[] Board, bool IsPlayer0Turn)
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
    
    public static BoardState Empty() => new BoardState(new bool?[9], false);
}

#endregion

#region Requests, Responses, Exceptions

public record MoveRequest() : IInteractionRequest;
public record MoveResponse(int X, int Y) : IInteractionResponse;

#endregion

#region Processes

public class MoveProcess : IProcess<BoardState>
{
    public bool CanExecute(BoardState currentState)
    {
        return !currentState.Winner.HasValue;
    }

    public HandleResult<BoardState> Handle(BoardState currentState, IInteractionResponse? response)
    {
        ArgumentNullException.ThrowIfNull(currentState);
        ArgumentNullException.ThrowIfNull(currentState.Board);

        switch (response)
        {
            case null: return handleInit(currentState);
            case MoveResponse moveResponse: return handleMoveResponse(currentState, moveResponse);
            default: throw new ArgumentException("Invalid response");
        }
    }

    private static HandleResult<BoardState> handleInit(BoardState currentState)
    {
        var request = new MoveRequest();
        return new HandleResult<BoardState>(currentState, false, request);
    }
    
    private static HandleResult<BoardState> handleMoveResponse(BoardState currentState, MoveResponse moveResponse)
    {
        if (currentState.Board[Manager.BoardCoordinateToIndex(moveResponse.X, moveResponse.Y)].HasValue)
        {
            throw new InvalidMoveException();
        }
        var newBoard = currentState.Board;
        newBoard[Manager.BoardCoordinateToIndex(moveResponse.X, moveResponse.Y)] = currentState.IsPlayer0Turn ? false : true;
        return new HandleResult<BoardState>(new BoardState(newBoard, !currentState.IsPlayer0Turn), true);
    }
}

#endregion

public class ProcessManager : IProcessManager<BoardState>
{
    private readonly List<IProcess<BoardState>> _processes;
    private IProcess<BoardState>? _currentProcess = null;
    
    public BoardState CurrentState { get; private set; }
    public IInteractionRequest? CurrentRequest { get; private set; }
    
    public ProcessManager(BoardState? initialState = null, IEnumerable<IProcess<BoardState>>? processes = null)
    {
        initialState ??= BoardState.Empty();
        processes ??= [new MoveProcess()];
        
        CurrentState = initialState;
        _processes = processes.ToList();
    }
    
    public HandleResult<BoardState> Process(IInteractionResponse? response = null)
    {
        if (_currentProcess == null)
        {
            _currentProcess = _processes.FirstOrDefault(p => p.CanExecute(CurrentState));
            if (_currentProcess == null)
                return new HandleResult<BoardState>(CurrentState, true);
        }

        var result = _currentProcess.Handle(CurrentState, response);
        CurrentState = result.NewState;
        CurrentRequest = result.InteractionRequest;

        if (result.IsComplete)
        {
            _currentProcess = null;
        }
        
        return result;
    }
}
