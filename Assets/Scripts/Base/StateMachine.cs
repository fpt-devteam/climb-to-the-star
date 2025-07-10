using System;
using System.Collections.Generic;
using UnityEngine;

public class StateMachine
{
    public StateNode current;

    public StateNode previous;

    Dictionary<Type, StateNode> nodes;

    public StateMachine()
    {
        nodes = new Dictionary<Type, StateNode>();
    }

    public void Update()
    {
        var transition = GetTransition();

        if (transition != null)
        {
            ChangeState(transition.To);
            return;
        }

        current.State?.Update();
    }

    public void FixedUpdate()
    {
        var transition = GetTransition();

        if (transition != null)
        {
            ChangeState(transition.To);
            return;
        }

        current.State?.FixedUpdate();
    }

    public void SetState(IState state)
    {
        var node = GetOrAddNode(state);
        current = node;
        current.State?.OnEnter();
    }

    public void AddTransition(IState from, IState to, IPredicate condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    private void ChangeState(IState state)
    {
        if (current.State == state)
            return;

        var previousState = current.State;
        var nextState = nodes[state.GetType()].State;

        previousState?.OnExit();
        nextState?.OnEnter();

        current = nodes[state.GetType()];
    }

    private ITransition GetTransition()
    {
        foreach (var transition in current.Transitions)
        {
            if (transition.Condition.Evaluate())
            {
                return transition;
            }
        }

        return null;
    }

    private StateNode GetOrAddNode(IState state)
    {
        var node = nodes.GetValueOrDefault(state.GetType());

        if (node == null)
        {
            node = new StateNode(state);
            nodes.Add(state.GetType(), node);
        }

        return node;
    }
}

public class StateNode
{
    public IState State { get; }

    public HashSet<ITransition> Transitions { get; }

    public StateNode(IState state)
    {
        State = state;
        Transitions = new HashSet<ITransition>();
    }

    public void AddTransition(IState to, IPredicate condition)
    {
        Transitions.Add(new Transition(to, condition));
    }
}
