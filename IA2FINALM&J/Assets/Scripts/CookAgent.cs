using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CookAgent : MonoBehaviour
{
    //Tengo refe de los ingredientes, para añadirlos a mi inventario cuando los coja
    public Inventory inventory;
    // Start is called before the first frame update
    void Start()
    {
        inventory = new Inventory();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
