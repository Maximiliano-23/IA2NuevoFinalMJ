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
        //BuildStatesAndFSM();
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
       
        //fsm.Update();
        //
        //if (currentPath != null && pathIndex < currentPath.Count)
        //{
        //    Vector3 targetPos = currentPath[pathIndex];
        //    transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);
        //
        //    if (Vector3.Distance(transform.position, targetPos) < 0.15f)
        //    {
        //        pathIndex++;
        //        if (pathIndex >= currentPath.Count)
        //        {
        //            fsm.Feed("ARRIVED");
        //        }
        //    }
        //}

        TravelThroughPath();
        if (_pathToFollow.Count() <= 0)
        {
            //TravelThroughPath().First();
        }
    }

    //void BuildStatesAndFSM()
    //{
    //    var idleState = new State<string>("Idle");
    //    var moveState = new State<string>("Move");
    //    var pickupState = new State<string>("Pickup");
    //    var interactState = new State<string>("Interact");
    //
    //    StateConfigurer.Create(idleState).
    //        SetTransition("Idle", idleState).
    //        SetTransition("Move", moveState).
    //        SetTransition("Pickup", pickupState).
    //        SetTransition("Interact", interactState).
    //        Done();
    //    StateConfigurer.Create(moveState).
    //        SetTransition("Idle", idleState).
    //        SetTransition("Move", moveState).
    //        SetTransition("Pickup", pickupState).
    //        SetTransition("Interact", interactState).
    //        Done();
    //    StateConfigurer.Create(pickupState).
    //        SetTransition("Idle", idleState).
    //        SetTransition("Move", moveState).
    //        SetTransition("Pickup", pickupState).
    //        SetTransition("Interact", interactState).
    //        Done();
    //    StateConfigurer.Create(interactState).
    //        SetTransition("Idle", idleState).
    //        SetTransition("Move", moveState).
    //        SetTransition("Pickup", pickupState).
    //        SetTransition("Interact", interactState).
    //        Done();
    //    //Idle
    //    idleState.OnEnter += x => {
    //
    //        //TryPlan();
    //    };
    //    idleState.OnUpdate += () => {
    //
    //        if (currentPlan != null && planIndex < currentPlan.Count)
    //        {
    //            fsm.Feed("Move");
    //        }
    //    };
    //    idleState.OnExit += x =>
    //    {
    //        Debug.Log("finishIdle");
    //    };
    //
    //    //Move
    //    moveState.OnEnter += x => material.color = Color.blue;
    //    moveState.OnUpdate += () =>
    //    {
    //        if (_pathToFollow.Count > 0)
    //        {
    //            pathfinding.AStar(GameManager.instance.ObtenerNodoCercano(this.transform.position),);
    //            _pathToFollow
    //            TravelThroughPath();
    //        }
    //        else 
    //        { 
    //            
    //        }
    //    };
    //    idleState.OnExit += x =>
    //    {
    //        Debug.Log("finishMove");
    //    };
    //
    //    //pickup
    //    pickupState.OnEnter += x => material.color = Color.white;
    //    interactState.OnEnter += x => material.color = Color.green;
    //
    //    // pickup: en Enter ejecutamos la recolección (simulado como coroutine breve)
    //    pickupState.OnEnter += (inpt) => {
    //        StartCoroutine(DoPickupCoroutine());
    //    };
    //
    //    interactState.OnEnter += (inpt) => {
    //        //StartCoroutine(DoInteractCoroutine());
    //    };
    //
    //
    //   
    //    fsm = new EventFSM<string>(idleState);
    //}
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
        changePath = true;
        var current = currentPlan.First();
        current();
    }

    public void Move(Vector3 dir)
    {
        transform.LookAt(transform.position + dir, Vector3.back);
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

    /*void StartMoveToCurrentActionTarget()
    {
        if (currentPlan == null || planIndex >= currentPlan.Count)
        {
            // nada que hacer
            fsm.Feed("DONE");
            return;
        }

        //GOAPActions action = currentPlan[planIndex];

        // Necesitamos decidir destino físico según nombre de la acción
        // EJEMPLO SIMPLE: si la acción es "Go to Bowl" mover al primer nodo que tenga bowlNearby true
        Node targetNode = null;

        if (action.Name.Contains("Bowl"))
        {
            // buscar nodo asociado al bowl (ejemplo sencillo: primer nodo con bowlNearby flag en WorldState)
            // adaptalo a cómo tú realmente referencies bowl/oven en escena
            targetNode = gm._node.Find(n => Vector3.Distance(n.transform.position, transform.position) > 0f);
        }
        else if (action.Name.Contains("Oven"))
        {
            targetNode = gm._node[0]; // placeholder; adaptá a nodo horno real
        }
        else
        {
            // por defecto: moverse al nodo más cercano (o a un nodo random)
            targetNode = GetClosestNode();
        }

        Node start = GetClosestNode();
        currentPath = pathfinding.AStar(start, targetNode);
        pathIndex = 0;

        if (currentPath == null)
        {
            Debug.LogWarning("No se encontró path para la acción: " + action.Name);
            // pasar a siguiente / replanificar
            planIndex++;
            fsm.Feed("DONE");
        }
    }*/

    IEnumerator DoPickupCoroutine()
    {
        // ejemplo simple: esperar, recoger, actualizar estado
        yield return new WaitForSeconds(0.5f);

        // actualizar WorldState e inventario según corresponda
        

        // avanzar plan
        planIndex++;

        // volver a Idle
        fsm.Feed("DONE");
    }

    /*IEnumerator DoInteractCoroutine()
    {
        GOAPActions action = currentPlan != null && planIndex < currentPlan.Count ? currentPlan[planIndex] : null;
        if (action != null)
        {
            // ejecutar comportamiento opcional (si el action tiene agenteBehaviour)
            action.agentBehaviour?.Invoke(this);

            // aplicar efectos del action sobre el worldState
            worldState = action.Effects(worldState);

            // simular tiempo de ejecución
            yield return new WaitForSeconds(1f);

            planIndex++;
        }
        else
        {
            yield return null;
        }

        fsm.Feed("DONE");
    }*/

    IEnumerator DoWaitCoroutine(float seconds)
    {
        float t = 1f;
        while (t < seconds) { t += Time.deltaTime; yield return null; }
        fsm.Feed("DONE");
    }
    
}
