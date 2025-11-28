using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Node : MonoBehaviour
{
    public LayerMask Wall;
    public LayerMask nodeLayer;
    private Renderer _renderer;

    public bool isOccupied = false;

    public List<Node> _neighbors;
    private int _cost;
    public float size;
    public int Cost { get => _cost; }
    public Color costColor = Color.green - new Color(0, 0.3f, 0);
    void Start()
    {
        _renderer = GetComponent<Renderer>();
        _neighbors = new List<Node>();
        SetCost(1);
        DetectNeighbors();

    }

    // Update is called once per frame
    void Update()
    {

    }
    private void DetectNeighbors()
    {
        //Debug.Log("Entre");
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, size, nodeLayer);
        foreach (var hitCollider in hitColliders)
        {
            Node node = hitCollider.GetComponent<Node>();
            if (node != null && node != this && InLineOfSight(node.transform.position))
            {
                _neighbors.Add(node);
            }
        }
    }

    public List<Node> Neighbors
    {
        get
        {
            return _neighbors;
        }
    }
    public bool isBlocked;

    /*public void Initialize(Coordinates coords, Grid grid)
    {
        _coordinates = coords;
        _grid = grid;
        _renderer = GetComponent<Renderer>();
        _textMesh = GetComponentInChildren<TextMeshPro>();
        SetCost(1);
    }*/
    public bool InLineOfSight(Vector3 target)
    {
        Vector3 dir = target - transform.position;
        return !Physics.Raycast(transform.position, dir, dir.magnitude, Wall);
    }
    public bool InLineOfSight(Vector3 target, Vector3 FinalNode)
    {
        Vector3 dir = target - FinalNode;
        return !Physics.Raycast(FinalNode, dir, dir.magnitude, Wall);
    }
    public void SetBlock(bool block)
    {
        isBlocked = block;
        ChangeColor(block ? Color.black : Color.white);
        gameObject.layer = block ? 6 : 0;
    }

    public void ChangeColor(Color color)
    {
        _renderer.material.color = color;
    }
    public void SetCost(int cost)
    {
        _cost = Mathf.Clamp(cost, 1, 99);
        /*if (!isBlocked && _renderer != null)
        {
            ChangeColor(_cost == 1 ? Color.white : costColor);
        }*/
    }
}
