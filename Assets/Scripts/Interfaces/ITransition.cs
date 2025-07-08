using System;
using UnityEngine;

public interface ITransition
{
    IState To { get; }

    IPredicate Condition { get; }
}
