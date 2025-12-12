using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

public class GOAPManager
{
    Pathfinding _pf;
    GameManager _gm;
    MixType bakeType = MixType.None;

    public GOAPManager(GameManager gm)
    {
        _pf = new();
        _gm = gm;
    }

    List<GOAPActions> GetActions()
    {
            return new List<GOAPActions>()
        {
            // --- Elegir mezcla que lo haga al final de calcular todo para saber cual le conviene antes de arrancar a caminar ---
            new GOAPActions("Pickup Vanilla")
            .SetCost(2)
            .Precondition(x => x.state.AvailableVanilla >= 1 && x.state.currentIngredient == MixType.None)
            .Effect(x =>
            {
                x.state.currentIngredient = MixType.Vanilla;
                x.state.ActionsLeftToCook--;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(_gm.agent.PickUpVanilla),

            new GOAPActions("Pickup Chocolate")
            .SetCost(2)
            .Precondition(x => x.state.AvailableChocolate >= 1 && x.state.currentIngredient == MixType.None)
            .Effect(x =>
            {
                x.state.currentIngredient = MixType.Chocolate;
                x.state.ActionsLeftToCook--;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(_gm.agent.PickUpChocolate),

            new GOAPActions("Pickup Strawberry")
            .SetCost(2)
            .Precondition(x => x.state.AvailableStrawberry >= 1 && x.state.currentIngredient == MixType.None)
            .Effect(x =>
            {
                x.state.currentIngredient = MixType.Strawberry;
                x.state.ActionsLeftToCook--;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(_gm.agent.PickUpStrawberrys),


            // --- Mezclar ---
            new GOAPActions("Mix Ingredients")
            .SetCost(3)
            .Precondition(x =>
                x.state.currentIngredient !=MixType.None && x.state.currentMix == MixType.None)
            .Effect(x =>
            {
                x.state.currentMix = x.state.currentIngredient ;
                x.state.currentIngredient = MixType.None;
                x.state.ActionsLeftToCook--;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(_gm.agent.MixIngredients),

            // --- Comer ---
             new GOAPActions("Eat Strawberry")
            .SetCost(1)
            .Precondition(x => 
               x.state.currentCake == MixType.Strawberry)
            .Effect(x =>
            {
                x.state.hunger -= 20;
                x.state.currentCake = MixType.None;
              
                x.state.ActionsLeftToCook--;
                return x;
            })
            .SetBehaviour(()=>_gm.agent.Eat("Strawberry")),
              new GOAPActions("Eat Vanilla")
            .SetCost(1)
            .Precondition(x => 
               x.state.currentCake == MixType.Vanilla)
            .Effect(x =>
            {
                x.state.hunger -= 10;
                  x.state.currentCake = MixType.None;
                
                x.state.ActionsLeftToCook--;
                return x;
            })
            .SetBehaviour(()=>_gm.agent.Eat("Vanilla")),
                new GOAPActions("Eat Chocolate")
            .SetCost(1)
            .Precondition(x => 
               x.state.currentCake == MixType.Chocolate)
            .Effect(x =>
            {
                x.state.hunger -= 30;
                x.state.currentCake = MixType.None;
                
                x.state.ActionsLeftToCook--;
                return x;
            })
            .SetBehaviour(()=>_gm.agent.Eat("Chocolate")),

            // --- Hornear ---
             new GOAPActions("Bake Cake")
            .SetCost(2)
            .Precondition(x => x.state.currentlyBakingCake == MixType.None &&
                x.state.currentMix != MixType.None)
            .Effect(x =>
            {
                bakeType = x.state.currentMix;
                x.state.currentlyBakingCake = x.state.currentMix;
                x.state.currentMix = MixType.None;
                x.state.hunger += 0.1f;
                x.state.ActionsLeftToCook = 4;
                return x;
            })
            .SetBehaviour(()=>_gm.agent.BakeCake(bakeType)),


             new GOAPActions("Take out Cake")
            .SetCost(2)
            .Precondition(x =>
                x.state.currentlyBakingCake != MixType.None && x.state.ActionsLeftToCook <= 0 )
            .Effect(x =>
            {
                bakeType = x.state.currentMix;
                x.state.currentCake = x.state.currentlyBakingCake;
                x.state.currentlyBakingCake = MixType.None;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(()=>_gm.agent.TakeCake(bakeType)),

            new GOAPActions("Buy Ingredient Vanilla")
            .SetCost(2)
            .Precondition(x => x.state.ShopOpen &&
                x.state.currentIngredient == MixType.None &&
                x.state.Coins>= 2)
            .Effect(x =>
            {
                x.state.currentIngredient = MixType.Vanilla;
                x.state.Coins -= 2;
                x.state.ActionsLeftToCook--;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(_gm.agent.PickUpVanilla),
           new GOAPActions("Buy Ingredient Chocolate")
            .SetCost(2)
            .Precondition(x => x.state.ShopOpen &&
                x.state.currentIngredient == MixType.None &&
                x.state.Coins>= 6)
            .Effect(x =>
            {
                x.state.currentIngredient = MixType.Chocolate;
                x.state.Coins -= 6;
                x.state.ActionsLeftToCook--;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(_gm.agent.PickUpChocolate),
            new GOAPActions("Buy Ingredient Strawberry")
            .SetCost(2)
            .Precondition(x => x.state.ShopOpen &&
                x.state.currentIngredient == MixType.None &&                
                x.state.Coins>= 4)
            .Effect(x =>
            {
                x.state.currentIngredient = MixType.Strawberry;
                x.state.Coins -= 4;
                x.state.ActionsLeftToCook--;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(_gm.agent.PickUpStrawberrys),
           new GOAPActions("Buy Cake Chocolate")
            .SetCost(2)
            .Precondition(x => x.state.ShopOpen && x.state.currentCake == MixType.None &&
                x.state.Coins>= 20)
            .Effect(x =>
            {
                x.state.currentCake = MixType.Chocolate;
                
                x.state.Coins -= 20;
                x.state.ActionsLeftToCook--;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(()=>_gm.agent.BakeCake(bakeType)),
           new GOAPActions("Buy Cake Vanilla")
            .SetCost(2)
            .Precondition(x => x.state.ShopOpen && x.state.currentCake == MixType.None &&
                x.state.Coins>= 10)
            .Effect(x =>
            {
                x.state.currentCake = MixType.Vanilla;
                
                x.state.Coins -= 10;
                x.state.ActionsLeftToCook--;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(()=>_gm.agent.BakeCake(bakeType)),
           new GOAPActions("Buy Cake Strawberry")
            .SetCost(2)
            .Precondition(x => x.state.ShopOpen && x.state.currentCake == MixType.None &&
                x.state.Coins>= 15)
            .Effect(x =>
            {
                x.state.currentCake = MixType.Strawberry;
                
                x.state.Coins -= 15;
                x.state.ActionsLeftToCook--;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(()=>_gm.agent.BakeCake(bakeType)),

             new GOAPActions("Wait for Cake")
            .SetCost(1)
            .Precondition(x => x.state.currentlyBakingCake != MixType.None && x.state.ActionsLeftToCook > 0)
            .Effect(x =>
            {
                x.state.ActionsLeftToCook--;
                x.state.hunger += 0.1f;
                return x;
            })
            .SetBehaviour(_gm.agent.ActiveZzz),
        };
    }


    public bool TryGetPlan(WorldState initialState, out List<GOAPActions> plan)
    {
        Func<WorldState, int> heuristic = x =>
        {
            //Debug.Log("asas");
            return 0;
        };

        Func<WorldState, bool> objective = x =>
        {
            Debug.Log(x.state.hunger);
            return x.state.hunger <= 0 ;
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
