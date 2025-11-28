using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GOAPManager
{
    Pathfinding _pf;
    GameManager _gm;

    public GOAPManager(GameManager gm)
    {
        _pf = new();
        _gm = gm;
    }

    List<GOAPActions> GetActions()
    {
            return new List<GOAPActions>()
        {
            // --- Elegir mezcla ---
            new GOAPActions("Choose Vanilla")
            .SetCost(1)
            .Precondition(x => x.state.selectedMix != MixType.Vanilla)
            .Effect(x =>
            {
                x.state.selectedMix = MixType.Vanilla;
                return x;
            }),

            new GOAPActions("Choose Chocolate")
            .SetCost(1)
            .Precondition(x => x.state.selectedMix != MixType.Chocolate)
            .Effect(x =>
            {
                x.state.selectedMix = MixType.Chocolate;
                return x;
            }),

            new GOAPActions("Choose Strawberry")
            .SetCost(1)
            .Precondition(x => x.state.selectedMix != MixType.Strawberry)
            .Effect(x =>
            {
                x.state.selectedMix = MixType.Strawberry;
                return x;
            }),

            // --- Detectar ingredientes ---
            new GOAPActions("Detect Ingredients")
            .SetCost(1)
            .Precondition(x => !x.state.ingredientsDetected)
            .Effect(x =>
            {
                x.state.ingredientsDetected = true;
                return x;
            }),

            // --- Buscar huevos ---
            new GOAPActions("Search for Eggs")
            .SetCost(2)
            .Precondition(x => x.state.eggsAvailable > 0 && !x.state.eggNearby)
            .Effect(x =>
            {
                x.state.eggNearby = true;
                return x;
            }),

            new GOAPActions("Pick Egg")
            .SetCost(1)
            .Precondition(x => x.state.eggNearby && x.state.eggsAvailable > 0)
            .Effect(x =>
            {
                x.state.eggsAvailable--;
                x.state.eggsCount++;
                x.state.eggNearby = false;
                return x;
            }),

            // --- Ir al bowl ---
            new GOAPActions("Go to Bowl")
            .SetCost(1)
            .Precondition(x => !x.state.bowlNearby)
            .Effect(x =>
            {
                x.state.bowlNearby = true;
                return x;
            }),

            // --- Mezclar ---
            new GOAPActions("Mix Ingredients")
            .SetCost(2)
            .Precondition(x =>
                x.state.ingredientsDetected &&
                x.state.eggsCount > 0 &&
                x.state.bowlNearby)
            .Effect(x =>
            {
                // aquí no cambia nada del estado más que decir que mezcla está lista
                // o podrías agregar un flag mixReady
                return x;
            }),

            // --- Ir al horno ---
            new GOAPActions("Go to Oven")
            .SetCost(1)
            .Precondition(x => !x.state.ovenReachable)
            .Effect(x =>
            {
                x.state.ovenReachable = true;
                return x;
            }),

            // --- Hornear ---
            new GOAPActions("Bake Cake")
            .SetCost(3)
            .Precondition(x =>
                x.state.ovenReachable &&
                x.state.ingredientsDetected &&
                x.state.eggsCount > 0)
            .Effect(x =>
            {
                x.state.mixtureTemperature = 180f;
                return x;
            })
        };
    }


    public bool TryGetPlan(WorldState initialState, out List<GOAPActions> plan)
    {
        Func<WorldState, int> heuristic = x =>
        {
            return Mathf.RoundToInt(180 - x.state.mixtureTemperature);
        };

        Func<WorldState, bool> objective = x =>
        {
            return x.state.mixtureTemperature >= 180f;
        };

        var worldStatePath = _pf.AStarGOAP(initialState, GetActions(), heuristic, objective);

        if (worldStatePath != default)
        {
            plan = worldStatePath.Skip(1).Select(x => x.generatingAction).ToList();
            return true;
        }
        else
        {
            plan = default;
            return false;
        }
    }
}
