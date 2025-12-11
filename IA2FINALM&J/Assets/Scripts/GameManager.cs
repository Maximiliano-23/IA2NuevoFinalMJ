using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;

public class GameManager : MonoBehaviour
{
    public CookAgent agent;
    public List<Node> _node;
    public Ingredient[] objets;
    // Start is called before the first frame update
    void Start()
    {
        SpawnObjectsOnRandomNodesNoRepeat();
    }

    // Update is called once per frame
    void Update()
    {
        /*if (Input.GetKeyDown(KeyCode.Space))
        {
            // Elegir nodo random
            Node randomNode = _node[Random.Range(0, _node.Count)];
            agent.GoToNode(randomNode);
        }*/
    }
    void SpawnObjectsOnRandomNodesNoRepeat()
    {
        List<Node> availableNodes = new List<Node>(_node);

        foreach (var obj in objets)
        {
            if (availableNodes.Count == 0)
            {
                Debug.LogWarning("No quedan nodos disponibles para spawnear.");
                break;
            }

            int index = Random.Range(0, availableNodes.Count);
            Node chosenNode = availableNodes[index];

            Instantiate(obj.gameObject, chosenNode.transform.position, Quaternion.identity);

            availableNodes.RemoveAt(index); // evitar repetir nodo
        }
    }

}
