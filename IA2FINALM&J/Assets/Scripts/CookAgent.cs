using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA2;
using System.Data;
using System.Linq;
using System; 

public class CookAgent : MonoBehaviour
{
    public Inventory inventory;
    Pathfinding pathfinding;
    GameManager gm;
    GOAPManager goap;
    Material material;

    public List<Ingredient> Strawberry;
    public List<Ingredient> Chocolate;
    public List<Ingredient> Vanilla;

    List<Vector3> currentPath;
    Vector3 FinalPos;
    int pathIndex = 0;
    public float speed = 3f;
    public List<Vector3> _pathToFollow = new List<Vector3>();

    List<Action> currentPlan;
    int planIndex = 0;
    public WorldState worldState = new WorldState();

    EventFSM<string> fsm;

    [SerializeField] GameObject BowlWithVanilla, BowlWithChocolate, BowlWithStrawberrys, Bowl, CakeVanilla, CakeChocolate, CakeStrawberry, zzzObject;

    [SerializeField] Animator anim;
    public enum HunterMoves { Idle, Move, Pickup, Interact }
    private EventFSM<HunterMoves> _myFsm;

    Pathfinding _path;

    bool changePath=false;

    private void Awake()
    {

    }

    void Start()
    {
        Strawberry = GameManager.instance.Strawberry;
        Chocolate = GameManager.instance.Strawberry;
        Vanilla = GameManager.instance.Strawberry;
        pathfinding = new Pathfinding();
        gm = FindObjectOfType<GameManager>();
        goap = new GOAPManager(gm);
        material = GetComponent<Renderer>().material;


        TryPlan();
        StartCoroutine(WaitToNextAction());
    }

    void Update()
    {
       

        TravelThroughPath();
        if (_pathToFollow.Count() <= 0)
        {
           
        }
    }

    public void PickUpVanilla() 
    {
        Debug.Log("PickUpVanilla");
        var closestVanilla = GameManager.instance.Vanilla.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();
        _pathToFollow = pathfinding.AStar(GameManager.instance.FinalNode(transform.position),GameManager.instance.FinalNode(closestVanilla.transform.position));
        StartCoroutine(WaitToReach(() => { 
            BowlWithVanilla.SetActive(true);
            GameManager.instance.Vanilla.Remove(closestVanilla);
            Destroy(closestVanilla.gameObject); }));
        changePath = true;


    }
    public void PickUpChocolate()
    {
        Debug.Log("PickUpChocolate");
        var closestChocolate = GameManager.instance.Chocolate.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();
        _pathToFollow = pathfinding.AStar(GameManager.instance.FinalNode(transform.position), GameManager.instance.FinalNode(closestChocolate.transform.position));
        StartCoroutine(WaitToReach(() => { 
            BowlWithChocolate.SetActive(true);
            GameManager.instance.Chocolate.Remove(closestChocolate);
            Destroy(closestChocolate.gameObject); }));
        changePath = true;

    }
    public void PickUpStrawberrys()
    {
        Debug.Log("PickUpStrawberrys");

        var closestStrawberry = GameManager.instance.Chocolate.OrderBy(x => Vector3.Distance(transform.position, x.transform.position)).First();

        _pathToFollow = pathfinding.AStar(GameManager.instance.FinalNode(transform.position), GameManager.instance.FinalNode(closestStrawberry.transform.position));
        
        StartCoroutine(WaitToReach(() => { 
            BowlWithStrawberrys.SetActive(true); 
            GameManager.instance.Strawberry.Remove(closestStrawberry);
            Destroy(closestStrawberry.gameObject); }));

        changePath = true;

    }
    public void Eat(string whatEat)
    {
        Debug.Log("Eat");
        switch (whatEat)
        {
            case "Vanilla":
                CakeVanilla.SetActive(false);
                break;
            case "Chocolate":
                CakeChocolate.SetActive(false);
                break;
            case "Strawberry":
                CakeStrawberry.SetActive(false);
                break;
            default:
                break;
        }
        StartCoroutine(WaitToNextAction());
    }
    public void BakeCake(MixType type)
    {
        Debug.Log("BakeCake");
        _pathToFollow = pathfinding.AStar(GameManager.instance.FinalNode(transform.position), GameManager.instance.FinalNode(GameManager.instance.Oven.transform.position));
        StartCoroutine(WaitToReach(()=>{
            Bowl.SetActive(false);
            switch (type)
            {
                case MixType.Vanilla:
                    BowlWithVanilla.SetActive(false);
                    break;
                case MixType.Chocolate:
                    BowlWithChocolate.SetActive(false);
                    break;
                case MixType.Strawberry:
                    BowlWithStrawberrys.SetActive(false);
                    break;
                default:
                    break;
            }
        }));
        changePath = true;

    }
    IEnumerator WaitToReach(Action action)
    {
        yield return new WaitWhile(() => _pathToFollow.Count() > 0);
        action();
    }
    public void ActiveZzz()
    {
        Debug.Log("ActiveZzz");
        zzzObject.SetActive(true);
        StartCoroutine(WaitToNextAction());
    }
    public void MixIngredients()
    {
        Debug.Log("MixIngredients");
        anim.SetBool("Mix",true);
        StartCoroutine(WaitToNextAction());
    }
    public void TakeCake(MixType type)
    {
        Debug.Log("TakeCake");
        switch (type)
        {
            case MixType.Vanilla:
                CakeVanilla.SetActive(true);
                zzzObject.SetActive(false);
                break;
            case MixType.Chocolate:
                CakeChocolate.SetActive(true);
                zzzObject.SetActive(false);
                break;
            case MixType.Strawberry:
                CakeStrawberry.SetActive(true);
                zzzObject.SetActive(false);
                break;
            default:
                break;
        }
        StartCoroutine(WaitToNextAction());
        _pathToFollow = pathfinding.AStar(GameManager.instance.FinalNode(transform.position), GameManager.instance.FinalNode(GameManager.instance.Oven.transform.position));
        changePath = true;
    }
    void TryPlan()
    {
        if (goap.TryGetPlan(GameManager.instance.StartingWorld, out List<GOAPActions> plan))
        {
            currentPlan = plan.Select(x => x.agentBehaviour).ToList();
            planIndex = 0;
            var goapActiiiions = plan;
            foreach (var item in goapActiiiions)
            {
                Debug.Log(item.Name);
            }
            Debug.Log("Plan obtenido con " + plan.Count + " acciones.");
        }
        else
        {
            currentPlan = null;
            Debug.Log("No se pudo generar plan");
        }
    }

    public void TravelThroughPath()
    {
        if (changePath && _pathToFollow.Count() <= 0)
        {
            changePath = false;
            StartCoroutine(WaitToNextAction());
        }
        if (_pathToFollow == null || _pathToFollow.Count == 0) return;
            Vector3 posTarget = _pathToFollow[0];
            Vector3 dir = posTarget - transform.position;
            if (dir.magnitude < 0.05f)
            {
                //SetPosition(posTarget);
                _pathToFollow.RemoveAt(0);
            }

            Move(dir);
        
    }
    IEnumerator WaitToNextAction()
    {
        yield return new WaitForSeconds(2f);
        if(currentPlan.Count()>0)
        {
            var current = currentPlan.First();
            current();
            currentPlan.RemoveAt(0);
        }
    }

    public void Move(Vector3 dir)
    {
        transform.LookAt(transform.position + dir, Vector3.up);
        transform.position += dir.normalized * speed * Time.deltaTime;
    }

    Node GetClosestNode()
    {
        Node best = null;
        float minDist = Mathf.Infinity;
        foreach (Node n in gm._node)
        {
            float d = Vector3.Distance(transform.position, n.transform.position);
            if (d < minDist) { minDist = d; best = n; }
        }
        return best;
    }


    IEnumerator DoPickupCoroutine()
    {
        yield return new WaitForSeconds(0.5f);


        planIndex++;


        fsm.Feed("DONE");
    }


    IEnumerator DoWaitCoroutine(float seconds)
    {
        float t = 1f;
        while (t < seconds) { t += Time.deltaTime; yield return null; }
        fsm.Feed("DONE");
    }
    
}
