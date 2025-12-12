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

    public Node targetNode;
    public Vector3? targetPosition;
    public GameObject targetObject;

    public Action agentBehaviour;

    public GOAPActions(string name)
    {
        Name = name;
    }

    public GOAPActions SetCost(int cost)
    {
        Cost = cost;
        return this;
    }


    public GOAPActions SetBehaviour(Action behaviour)
    {
        agentBehaviour = behaviour;
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
