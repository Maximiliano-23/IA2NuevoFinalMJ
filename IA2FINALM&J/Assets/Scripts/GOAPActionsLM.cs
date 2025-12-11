using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GOAPActions
{
    public string Name { get; private set; }

    public Func<WorldState, bool> Preconditions = delegate { return true; };
    public Func<WorldState, WorldState> Effects = delegate { return default; };

    public int Cost { get; private set; }

    // TARGETS
    public Node targetNode;
    public Vector3? targetPosition;
    public GameObject targetObject;

    // ACTION BEHAVIOUR
    public Action<CookAgent> agentBehaviour;

    public GOAPActions(string name)
    {
        Name = name;
    }

    public GOAPActions SetCost(int cost)
    {
        Cost = cost;
        return this;
    }

    // Permite SetBehaviour con Action<CookAgent>
    public GOAPActions SetBehaviour(Action<CookAgent> behaviour)
    {
        agentBehaviour = behaviour;
        return this;
    }

    // PERMITE también usar lambdas sin parámetros => SetBehaviour(() => {...})
    public GOAPActions SetBehaviour(Action behaviour)
    {
        agentBehaviour = (agent) => behaviour();
        return this;
    }

    public GOAPActions Precondition(Func<WorldState, bool> prec)
    {
        Preconditions = prec;
        return this;
    }

    public GOAPActions Effect(Func<WorldState, WorldState> effe)
    {
        Effects = effe;
        return this;
    }
}
