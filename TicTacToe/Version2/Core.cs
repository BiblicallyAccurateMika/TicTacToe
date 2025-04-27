namespace TicTacToe.Version2;

public abstract record StateHolder;
public abstract record InteractionRequest;
public abstract record InteractionResponse;

public abstract class ProcessManager<TState>(TState? initialState = null)
    where TState : StateHolder, new()
{
    public record ProcessResult(TState NewState, InteractionRequest? Request = null);
    public record Process(Func<InteractionResponse?, bool> Check, Func<InteractionResponse?, ProcessResult> Action);
    
    protected abstract Process[] Processes { get; }
    private Process? _process;
    
    public TState State { get; private set; } = initialState ?? new TState();
    public InteractionRequest? Request { get; private set; }

    public void Run(InteractionResponse? request = null)
    {
        if (_process == null)
        {
            _process = Processes.FirstOrDefault(p => p.Check(request));
            if (_process == null) return;
        }
        
        var result = _process.Action(request);
        State = result.NewState;
        Request = result.Request;
            
        if (result.Request is not null) return;
        _process = null;
        Run();
    }
}