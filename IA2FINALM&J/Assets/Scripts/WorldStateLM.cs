using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MixType
{
    Vanilla,
    Chocolate,
    Strawberry,
    None
}
public class WorldState
{
    public struct State
    {
        public MixType currentCake;
        public MixType currentMix;
        public MixType currentlyBakingCake;

        public int ActionsLeftToCook;

        public int Coins;
        public int AvailableChocolate;
        public int AvailableStrawberry;
        public int AvailableVanilla;

        public bool ShopOpen;


        public float hunger;


        public MixType currentIngredient;     

        public State Clone()
        {
            return new State()
            {
                Coins = this.Coins, 
                ActionsLeftToCook = this.ActionsLeftToCook,
                currentCake = this.currentCake,
                currentMix = this.currentMix,
                currentlyBakingCake = this.currentlyBakingCake,
                AvailableChocolate = this.AvailableChocolate,
                AvailableStrawberry = this.AvailableStrawberry,
                AvailableVanilla = this.AvailableVanilla,
                ShopOpen = this.ShopOpen,
                hunger = this.hunger,
                currentIngredient = this.currentIngredient,
            };
        }
    }

    public State state;

    public GOAPActions generatingAction;

    public WorldState(GOAPActions action = null)
    {
        generatingAction = action;
        state = new();
    }

    public WorldState(WorldState newState, GOAPActions action = null)
    {
        generatingAction = action;
        state = newState.state.Clone();
    }
}