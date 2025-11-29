using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ingredient : ObjectsBase
{
    public override void OnInventoryAdd()
    {
        //WorldState.Instance.SetState("HasIngredient", true);
        gameObject.SetActive(false);
    }

    public override void OnInventoryRemove()
    {
        Destroy(gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
