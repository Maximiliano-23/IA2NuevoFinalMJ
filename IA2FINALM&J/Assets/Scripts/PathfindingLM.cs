using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class Pathfinding
{
    readonly List<Vector3> EMPTY = new List<Vector3>();

    public List<Vector3> AStar(Node start, Node end)
    {
        if (start == null) return default;
        var frontier = new PriorityQueue<Node>();
        frontier.Enqueue(start, 0);
        var cameFrom = new Dictionary<Node, Node>();
        cameFrom.Add(start, null);

        var costSoFar = new Dictionary<Node, int>();
        costSoFar.Add(start, 0);

        while (frontier.Count > 0)
        {
            var current = frontier.Dequeue();

            if (current == end)
            {
                var path = new List<Vector3>();
                while (current != start)
                {
                    //path.Add(current.characterPos.position);
                    current = cameFrom[current];
                }
                //path.Add(start.characterPos.position);
                path.Reverse();
                return path;
            }

            foreach (var item in current.Neighbors)
            {
                if (item.isBlocked) continue;

                int newCost = costSoFar[current];

                if (!costSoFar.ContainsKey(item) || newCost < costSoFar[current])
                {
                    //frontier.Enqueue(item, newCost + Heuristic(end.characterPos.position, item.characterPos.position));
                    cameFrom[item] = current;
                    costSoFar[item] = newCost;
                }
            }

        }
        return default;
    }

    public List<WorldState> AStarGOAP(WorldState start, IEnumerable<GOAPActions> allActions, Func<WorldState, int> heuristic, Func<WorldState, bool> goalMet)
    {
        if (start == null) return default;
        var frontier = new PriorityQueue<WorldState>();
        frontier.Enqueue(start, 0);
        var cameFrom = new Dictionary<WorldState, WorldState>();
        cameFrom.Add(start, null);

        var costSoFar = new Dictionary<WorldState, int>();
        costSoFar.Add(start, 0);

        int watchdog = 0;

        while (frontier.Count > 0)
        {
            watchdog++;

            if (watchdog > 20000) return default;

            var current = frontier.Dequeue();

            if (goalMet(current))
            {
                var path = new List<WorldState>();
                while (current != start)
                {
                    path.Add(current);
                    current = cameFrom[current];
                }
                path.Add(current);
                path.Reverse();
                return path;
            }

            foreach (var item in GetGOAPNeighbors(current, allActions))
            {
                int newCost = costSoFar[current] + item.Item2;

                if (!costSoFar.ContainsKey(item.Item1) || newCost < costSoFar[current])
                {
                    frontier.Enqueue(item.Item1, newCost + heuristic(item.Item1));
                    cameFrom[item.Item1] = current;
                    costSoFar[item.Item1] = newCost;
                }
            }
        }

        return default;
    }

    List<Tuple<WorldState, int>> GetGOAPNeighbors(WorldState current, IEnumerable<GOAPActions> allActions)
    {
        return allActions
            .Where(a => a.Preconditions(current))
            .Aggregate(new List<Tuple<WorldState, int>>(), (possibleList, action) =>
            {
                var newState = new WorldState(current, action);
                newState = action.Effects(newState);

                possibleList.Add(new Tuple<WorldState, int>(newState, action.Cost));

                return possibleList;
            });
    }

    /*public List<Vector3> ThetaStar(Node start, Node end, LayerMask wallLayer)
    {
        if (start == null || end == null) return EMPTY;
        var path = AStar(start, end);

        if (path.Count == 0) return EMPTY;

        int current = 0;
        while (current + 2 < path.Count)
        {
            if (path[current].InLineOfSightOf(path[current + 2], wallLayer))
                path.RemoveAt(current + 1);
            else current++;
        }

        return path;
    }*/

    float Heuristic(Vector3 start, Vector3 end)
    {
        return Vector3.Distance(start, end);
    }


}
