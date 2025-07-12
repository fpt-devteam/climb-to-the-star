using UnityEngine;

public class StateMachine
{
    public IState CurrentState => currentState;

    public IState PreviousState => previousState;

    private IState currentState;

    private IState previousState;

    public void Initialize(IState initialState)
    {
        ChangeState(initialState);
    }

    public void Update()
    {
        if (currentState == null)
            return;

        currentState.Update();

        IState nextState = currentState.CheckTransitions();

        if (nextState != null && nextState != currentState)
        {
            ChangeState(nextState);
        }
    }

    public void ChangeState(IState newState)
    {
        if (newState == null)
            return;

        currentState?.Exit();

        currentState = newState;

        currentState.Enter();
    }

    public void ForceState(IState newState)
    {
        ChangeState(newState);
    }
}
