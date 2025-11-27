using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Water : MonoBehaviour, IResource
{
    public string ResourceType => "Water";
    public float Hydration => 100f;
    public float Satiety => 0f;
    public Vector3 InteractionDistance => new Vector3(1.5f, 0.5f, 1.5f);


    public GameObject ResourceGameObject => this.gameObject;
    public float TimeToConsumeIt => 3f;


    public void DestroyGameobject()
    {
        Debug.Log("Drinking Water");
        //Destroy(gameObject);
    }

   
}
