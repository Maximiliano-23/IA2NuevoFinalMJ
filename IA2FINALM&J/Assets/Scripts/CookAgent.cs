using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using IA2; // tu namespace de EventFSM/State/Transition

public class CookAgent : MonoBehaviour
{
    // refs
    public Inventory inventory;
    Pathfinding pathfinding;
    GameManager gm;
    GOAPManager goap;

    // movement
    List<Vector3> currentPath;
    int pathIndex = 0;
    public float speed = 3f;

    // planning
    List<GOAPActions> currentPlan;
    int planIndex = 0;
    public WorldState worldState = new WorldState();

    // FSM: usamos string como trigger/input
    EventFSM<string> fsm;
    State<string> idleState, moveState, pickupState, interactState, waitState;

    void Start()
    {
        inventory = GetComponent<Inventory>();
        if (inventory == null) inventory = gameObject.AddComponent<Inventory>();
        pathfinding = new Pathfinding();
        gm = FindObjectOfType<GameManager>();
        goap = new GOAPManager(gm);

        BuildStatesAndFSM();
    }

    void Update()
    {
        // actualiza la FSM (llama al Update del estado actual)
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
        // 1) crear estados
        idleState = new State<string>("Idle");
        moveState = new State<string>("Move");
        pickupState = new State<string>("Pickup");
        interactState = new State<string>("Interact");
        waitState = new State<string>("Wait");

        // 2) configurar eventos de lifecycle (Enter/Update/Exit)
        idleState.OnEnter += (inpt) => {
            // cuando entramos en Idle intentamos planear
            TryPlan();
        };
        idleState.OnUpdate += () => {
            // Si hay plan y no se está ejecutando ninguna acción, comenzamos
            if (currentPlan != null && planIndex < currentPlan.Count)
            {
                // disparar transición a movimiento / ejecución
                fsm.Feed("DO_ACTION");
            }
        };

        moveState.OnEnter += (inpt) => {
            // Al entrar a Move, tomar la acción actual y pedir movimiento
            StartMoveToCurrentActionTarget();
        };

        // pickup: en Enter ejecutamos la recolección (simulado como coroutine breve)
        pickupState.OnEnter += (inpt) => {
            StartCoroutine(DoPickupCoroutine());
        };

        interactState.OnEnter += (inpt) => {
            StartCoroutine(DoInteractCoroutine());
        };

        waitState.OnEnter += (inpt) => {
            StartCoroutine(DoWaitCoroutine(2f));
        };

        // 3) crear diccionarios de transiciones para cada estado
        // Idle: DO_ACTION -> Move
        var idleTransitions = new Dictionary<string, Transition<string>>() {
            { "DO_ACTION", new Transition<string>("DO_ACTION", moveState) }
        };
        idleState.Configure(idleTransitions);

        // Move: ARRIVED -> Interact, FOUND_OBJECT -> Pickup
        var moveTransitions = new Dictionary<string, Transition<string>>() {
            { "ARRIVED", new Transition<string>("ARRIVED", interactState) },
            { "FOUND_OBJECT", new Transition<string>("FOUND_OBJECT", pickupState) }
        };
        moveState.Configure(moveTransitions);

        // Pickup: DONE -> Idle
        var pickupTransitions = new Dictionary<string, Transition<string>>() {
            { "DONE", new Transition<string>("DONE", idleState) }
        };
        pickupState.Configure(pickupTransitions);

        // Interact: DONE -> Idle
        var interactTransitions = new Dictionary<string, Transition<string>>() {
            { "DONE", new Transition<string>("DONE", idleState) }
        };
        interactState.Configure(interactTransitions);

        // Wait: DONE -> Idle
        var waitTransitions = new Dictionary<string, Transition<string>>() {
            { "DONE", new Transition<string>("DONE", idleState) }
        };
        waitState.Configure(waitTransitions);

        // 4) crear la FSM con estado inicial Idle
        fsm = new EventFSM<string>(idleState);
    }

    // ------------------------------
    // PLANNING (usa tu GOAPManager.TryGetPlan)
    // ------------------------------
    void TryPlan()
    {
        // ensure worldState is up-to-date (por ejemplo, worldState.state.ingredientsDetected etc.)
        if (goap.TryGetPlan(worldState, out List<GOAPActions> plan))
        {
            currentPlan = plan;
            planIndex = 0;
            Debug.Log("Plan obtenido con " + plan.Count + " acciones.");
        }
        else
        {
            currentPlan = null;
            Debug.Log("No se pudo generar plan");
        }
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

    void StartMoveToCurrentActionTarget()
    {
        if (currentPlan == null || planIndex >= currentPlan.Count)
        {
            // nada que hacer
            fsm.Feed("DONE");
            return;
        }

        GOAPActions action = currentPlan[planIndex];

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
    }

    // ------------------------------
    // ACTION IMPLEMENTATIONS (coroutines simuladas)
    // ------------------------------
    IEnumerator DoPickupCoroutine()
    {
        // ejemplo simple: esperar, recoger, actualizar estado
        yield return new WaitForSeconds(0.5f);

        // actualizar WorldState e inventario según corresponda
        worldState.state.hasIngredients = true;

        // avanzar plan
        planIndex++;

        // volver a Idle
        fsm.Feed("DONE");
    }

    IEnumerator DoInteractCoroutine()
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
    }

    IEnumerator DoWaitCoroutine(float seconds)
    {
        float t = 0f;
        while (t < seconds) { t += Time.deltaTime; yield return null; }
        fsm.Feed("DONE");
    }
}
