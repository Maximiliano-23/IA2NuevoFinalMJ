using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookAgent : MonoBehaviour
{
    //Tengo refe de los ingredientes, para añadirlos a mi inventario cuando los coja
    public Inventory inventory;

    Pathfinding pathfinding;
    GameManager gm;

    List<Vector3> currentPath;
    int pathIndex = 0;

    public float speed = 3f;
    // Start is called before the first frame update
    void Start()
    {
        inventory = new Inventory();
        pathfinding = new Pathfinding();
        gm = FindObjectOfType<GameManager>();
        //gm = new GameManager();
    }

    // Update is called once per frame
    void Update()
    {
        FollowPath();
    }
    public void SearchBoxOfCoins()
    {

    }
    public void SearchInredients(Ingredient ingredient)
    {

    }
    // -------------------------------------------------------------------
    //   PASO 1 — Elegir un nodo destino y pedir la ruta A*
    // -------------------------------------------------------------------
    public void GoToNode(Node target)
    {
        Node start = GetClosestNode();

        currentPath = pathfinding.AStar(start, target);

        if (currentPath == null)
        {
            Debug.Log("No se pudo encontrar un camino.");
            return;
        }

        pathIndex = 0;
    }

    void FollowPath()
    {
        if (currentPath == null || pathIndex >= currentPath.Count)
            return;

        Vector3 targetPos = currentPath[pathIndex];

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            pathIndex++;
        }
    }


    // -------------------------------------------------------------------
    //  Obtener el nodo más cercano al agente
    // -------------------------------------------------------------------
    Node GetClosestNode()
    {
        Node best = null;
        float minDist = Mathf.Infinity;

        foreach (Node n in gm._node)
        {
            float d = Vector3.Distance(transform.position, n.transform.position);
            if (d < minDist)
            {
                minDist = d;
                best = n;
            }
        }

        return best;
    }
}
