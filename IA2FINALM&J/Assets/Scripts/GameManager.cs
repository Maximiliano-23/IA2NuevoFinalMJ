using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public CookAgent agent;
    public WorldState StartingWorld;
    public List<Node> _node;
    [SerializeField] Ingredient Vanillaprefab, Chocolateprefab, Strawberryprefab;
    [HideInInspector] public List<Ingredient> Chocolate, Vanilla, Strawberry;
    public int ChocolateCount;
    public int VanillaCount;
    public int StrawberryCount;
    public int CoinsCount;

    public MixType CurrentCake;
    public MixType CurrentMix;
    public MixType CurrentlyBakingCake;

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

        SpawnObjectsOnRandomNodesNoRepeat();

        StartingWorld = new ();

        StartingWorld.state.ActionsLeftToCook = 0;
        StartingWorld.state.AvailableChocolate = ChocolateCount;
        StartingWorld.state.AvailableStrawberry = StrawberryCount;
        StartingWorld.state.AvailableVanilla = VanillaCount;
        StartingWorld.state.Coins = CoinsCount;
        StartingWorld.state.currentCake = MixType.None;
        StartingWorld.state.currentMix = MixType.None;
        StartingWorld.state.currentlyBakingCake = MixType.None;
        StartingWorld.state.ShopOpen = true;
    }
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
      
    }
    void SpawnObjectsOnRandomNodesNoRepeat()
    {
        List<Node> availableNodes = new List<Node>(_node);

        for (int i = 0; i < VanillaCount; i++)
        {
            if (availableNodes.Count == 0)
            {
                Debug.LogWarning("No quedan nodos disponibles para spawnear.");
                break;
            }

            int index = Random.Range(0, availableNodes.Count);
            Node chosenNode = availableNodes[index];

            Vanilla.Add(Instantiate(Vanillaprefab, chosenNode.transform.position, Quaternion.identity));

            availableNodes.RemoveAt(index);
        }
        for (int i = 0; i < ChocolateCount; i++)
        {
            if (availableNodes.Count == 0)
            {
                Debug.LogWarning("No quedan nodos disponibles para spawnear.");
                break;
            }

            int index = Random.Range(0, availableNodes.Count);
            Node chosenNode = availableNodes[index];

            Vanilla.Add(Instantiate(Chocolateprefab, chosenNode.transform.position, Quaternion.identity));

            availableNodes.RemoveAt(index);
        }
        for (int i = 0; i < StrawberryCount; i++)
        {
            if (availableNodes.Count == 0)
            {
                Debug.LogWarning("No quedan nodos disponibles para spawnear.");
                break;
            }

            int index = Random.Range(0, availableNodes.Count);
            Node chosenNode = availableNodes[index];

            Vanilla.Add(Instantiate(Strawberryprefab, chosenNode.transform.position, Quaternion.identity));

            availableNodes.RemoveAt(index);
        }
    }

}
