using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public List<Ingredient> items = new List<Ingredient>();

    public void Add(Ingredient obj)
    {
        items.Add(obj);
        obj.OnInventoryAdd();
    }

    public void Remove(Ingredient obj)
    {
        items.Remove(obj);
        obj.OnInventoryRemove();
    }

    /*public bool HasItem(string id)
    {
        return items.Any(i => i.id == id);
    }*/
}
