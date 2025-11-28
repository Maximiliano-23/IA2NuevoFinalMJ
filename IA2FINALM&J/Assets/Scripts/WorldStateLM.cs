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
        // FLOAT
        public float mixtureTemperature; // antes enemyHp

        // INTS
        public int eggsCount;           // antes arrows
        public int eggsAvailable;       // antes arrowsAvailable

        // BOOLS
        public bool ingredientsDetected; // antes detected
        public bool eggNearby;           // antes arrowNearby
        public bool ovenReachable;       // antes enemyReachable
        public bool bowlNearby;          // antes enemyNearby
        public bool canRestock;          // antes canRetreat

        // ENUM
        public MixType selectedMix;      // antes equippedWeapon

        public State Clone()
        {
            return new State()
            {
                mixtureTemperature = this.mixtureTemperature,
                eggsCount = this.eggsCount,
                eggsAvailable = this.eggsAvailable,
                ingredientsDetected = this.ingredientsDetected,
                eggNearby = this.eggNearby,
                ovenReachable = this.ovenReachable,
                bowlNearby = this.bowlNearby,
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