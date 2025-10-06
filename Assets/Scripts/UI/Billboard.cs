using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{

    private Transform cam;

    public void Start()
    {
        cam = Camera.main.transform;
    }
    public void LateUpdate()
    {
        transform.LookAt(transform.position + cam.forward); 
    }
}
