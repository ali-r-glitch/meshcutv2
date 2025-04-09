using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class sword2 : MonoBehaviour

{
    public GameObject hilt;
    public GameObject shaft;
    public GameObject pointer;
    // Start is called before the first frame update
  

    // Update is called once per frame
    void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            pointer.transform.position = hit.point;
            hilt.transform.LookAt(pointer.transform.position);
        }
    }
}