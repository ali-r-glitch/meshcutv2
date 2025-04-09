using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tear : MonoBehaviour
{
    public GameObject tear;


    private void OnCollisionEnter(Collision other)
    {
        other.gameObject.GetComponent<Rigidbody>();
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
