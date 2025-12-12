using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

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
    public int hunger;

    public GameObject Oven, SuperMarket;

    public MixType CurrentCake;
    public MixType CurrentMix;
    public MixType CurrentlyBakingCake;
    public MixType currentIngredient;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(this);
        SpawnObjectsOnRandomNodesNoRepeat();
    }

    void Start()
    {

        StartingWorld = new();

        StartingWorld.state.hunger = hunger;
        StartingWorld.state.ActionsLeftToCook = 0;
        StartingWorld.state.AvailableChocolate = ChocolateCount;
        StartingWorld.state.AvailableStrawberry = StrawberryCount;
        StartingWorld.state.AvailableVanilla = VanillaCount;
        StartingWorld.state.Coins = CoinsCount;
        StartingWorld.state.currentCake = MixType.None;
        StartingWorld.state.currentMix = MixType.None;
        StartingWorld.state.currentlyBakingCake = MixType.None;
        StartingWorld.state.ShopOpen = true;
        StartingWorld.state.currentIngredient = currentIngredient;
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

            Chocolate.Add(Instantiate(Chocolateprefab, chosenNode.transform.position, Quaternion.identity));

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

            Strawberry.Add(Instantiate(Strawberryprefab, chosenNode.transform.position, Quaternion.identity));

            availableNodes.RemoveAt(index);
        }
    }

    public Node ObtenerNodoCercano(Vector3 position)
    {
        Node closestNode = null;
        float closestDistance = float.MaxValue;

        foreach (Node node in GameManager.instance._node)
        {
            float distance = Vector3.Distance(position, node.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestNode = node;
            }
        }

        return closestNode;
    }

    public Node FinalNode(Vector3 vector3)
    {
        return _node.OrderBy(x => Vector3.Distance(vector3, x.transform.position)).First();
    }

}
