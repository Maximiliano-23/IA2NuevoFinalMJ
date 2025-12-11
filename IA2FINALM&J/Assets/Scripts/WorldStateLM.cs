using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum MixType
{
    Vanilla,
    Chocolate,
    Strawberry
}
public class WorldState
{
    public struct State
    {

        public float mixtureTemperature; 


        public int Coins;
        //public int BoxAvailable;
        public int BoxCount;


        //public bool ingredientsDetected; 
        //public bool BoxNearby;           
        //public bool ovenReachable;       
        public bool bowlNearby;          
        public bool canRestock;          
        public bool hasIngredients;
        public bool mixReady;
        public bool cakeReady;


        public float hunger;
        //public bool supermarketNearby;


        public MixType selectedMix;     

        public State Clone()
        {
            return new State()
            {
                mixtureTemperature = this.mixtureTemperature,
                Coins = this.Coins, 
                //BoxAvailable = this.BoxAvailable,
                BoxCount = this.BoxCount,
                //ingredientsDetected = this.ingredientsDetected,
                //BoxNearby = this.BoxNearby,
                //ovenReachable = this.ovenReachable,
                //bowlNearby = this.bowlNearby,
                canRestock = this.canRestock,
                selectedMix = this.selectedMix,
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