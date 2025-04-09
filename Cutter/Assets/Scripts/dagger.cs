using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class dagger : MonoBehaviour
{
    // Start is called before the first frame update
 
    public GameObject pointer;
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        MoveDagger();
    }

    void MoveDagger()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            pointer.transform.position = hit.point;
           
        }
    }
}
