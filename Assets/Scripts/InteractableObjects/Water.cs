using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour, IResource
{
    public string ResourceType => "Water";
    public float hydration => 100f;
    public float satiety => 0f;

    public GameObject gameObjecto => this.gameObject;
    public float timeToConsumeIt => 3f;


    public void Consume()
    {
        Debug.Log("Drinking Water");
        //Destroy(gameObject);
    }

   
}
