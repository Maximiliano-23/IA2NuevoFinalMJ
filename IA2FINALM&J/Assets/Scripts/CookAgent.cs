using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA2;
using System.Data;
using System.Linq;
using System; // tu namespace de EventFSM/State/Transition

public class CookAgent : MonoBehaviour
{
    // refs
    public Inventory inventory;
    Pathfinding pathfinding;
    GameManager gm;
    GOAPManager goap;
    Material material;

    public List<Ingredient> Strawberry;
    public List<Ingredient> Chocolate;
    public List<Ingredient> Vanilla;

    // movement
    List<Vector3> currentPath;
    Vector3 FinalPos;
    int pathIndex = 0;
    public float speed = 3f;
    public List<Vector3> _pathToFollow = new List<Vector3>();

    // planning
    List<Action> currentPlan;
    int planIndex = 0;
    public WorldState worldState = new WorldState();

    // FSM: usamos string como trigger/input
    EventFSM<string> fsm;

    [SerializeField] GameObject BowlWithVanilla, BowlWithChocolate, BowlWithStrawberrys, Bowl, CakeVanilla, CakeChocolate, CakeStrawberry, zzzObject;

    [SerializeField] Animator anim;
    public enum HunterMoves { Idle, Move, Pickup, Interact }
    private EventFSM<HunterMoves> _myFsm;

    private void Awake()
    {
        BuildStatesAndFSM();
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




    }

    void Update()
    {
       
        fsm.Update();

        // si estamos moviéndonos, avanzar en el path
        // (la lógica de movimiento también puede ejecutarse desde el estado Move).
        if (currentPath != null && pathIndex < currentPath.Count)
        {
            Vector3 targetPos = currentPath[pathIndex];
            transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

            if (Vector3.Distance(transform.position, targetPos) < 0.15f)
            {
                pathIndex++;
                if (pathIndex >= currentPath.Count)
                {
                    // llegada al destino
                    fsm.Feed("ARRIVED");
                }
            }
        }
    }

    // ------------------------------
    // BUILD FSM (usando State<T>.Configure)
    // ------------------------------
    void BuildStatesAndFSM()
    {
        var idleState = new State<string>("Idle");
        var moveState = new State<string>("Move");
        var pickupState = new State<string>("Pickup");
        var interactState = new State<string>("Interact");

        StateConfigurer.Create(idleState).
            SetTransition("Idle", idleState).
            SetTransition("Move", moveState).
            SetTransition("Pickup", pickupState).
            SetTransition("Interact", interactState).
            Done();
        StateConfigurer.Create(moveState).
            SetTransition("Idle", idleState).
            SetTransition("Move", moveState).
            SetTransition("Pickup", pickupState).
            SetTransition("Interact", interactState).
            Done();
        StateConfigurer.Create(pickupState).
            SetTransition("Idle", idleState).
            SetTransition("Move", moveState).
            SetTransition("Pickup", pickupState).
            SetTransition("Interact", interactState).
            Done();
        StateConfigurer.Create(interactState).
            SetTransition("Idle", idleState).
            SetTransition("Move", moveState).
            SetTransition("Pickup", pickupState).
            SetTransition("Interact", interactState).
            Done();
        //Idle
        idleState.OnEnter += x => {

            material.color = Color.red;
            TryPlan();
        };
        idleState.OnUpdate += () => {

            if (currentPlan != null && planIndex < currentPlan.Count)
            {
                fsm.Feed("Move");
            }
        };
        idleState.OnExit += x =>
        {
            Debug.Log("finishIdle");
        };

        //Move
        moveState.OnEnter += x => material.color = Color.blue;
        moveState.OnUpdate += () =>
        {
            if (_pathToFollow.Count > 0)
            {
                TravelThroughPath();
            }
            else 
            { 
                
            }
        };
        idleState.OnExit += x =>
        {
            Debug.Log("finishMove");
        };

        //pickup
        pickupState.OnEnter += x => material.color = Color.white;
        interactState.OnEnter += x => material.color = Color.green;

        // pickup: en Enter ejecutamos la recolección (simulado como coroutine breve)
        pickupState.OnEnter += (inpt) => {
            StartCoroutine(DoPickupCoroutine());
        };

        interactState.OnEnter += (inpt) => {
            //StartCoroutine(DoInteractCoroutine());
        };


       
        fsm = new EventFSM<string>(idleState);
    }
    //private void SendInputToFSM(HunterMoves inp) => _myFsm.SendInput(inp);
    public void PickUpVanilla() 
    {
        BowlWithVanilla.SetActive(true);
    }
    public void PickUpChocolate()
    {
        BowlWithChocolate.SetActive(true);
    }
    public void PickUpStrawberrys()
    {
        BowlWithStrawberrys.SetActive(true);
    }
    public void Eat(string whatEat)
    {
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
    }
    public void BakeCake(MixType type)
    {
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
    }
    public void ActiveZzz()
    {
        zzzObject.SetActive(true);
    }
    public void MixIngredients()
    {
        anim.SetBool("Mix",true);
    }
    public void TakeCake(MixType type)
    {
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
    }
    void TryPlan()
    {
        // ensure worldState is up-to-date (por ejemplo, worldState.state.ingredientsDetected etc.)
        if (goap.TryGetPlan(worldState, out List<GOAPActions> plan))
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

    public void Move(Vector3 dir)
    {
        transform.LookAt(transform.position + dir, Vector3.back);
        transform.position += dir.normalized * speed * Time.deltaTime;
    }

    // ------------------------------
    // MOVEMENT / helpers
    // ------------------------------
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
