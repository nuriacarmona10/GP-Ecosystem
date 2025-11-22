using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour, IResource
{
    public float energy;
    public string ResourceType => "Food";
    public float Hydration => 20f;
    public float Satiety => 30f;

    public float InteractionDistance => 1.5f;
    public GameObject ResourceGameObject => this.gameObject;
    public float TimeToConsumeIt => 3f;


    // Start is called before the first frame update
    void Start()
    {
    }

    
    // Update is called once per frame
    public void DestroyGameobject()
    {
        //Debug.Log("Eating Apple");
       // this.gameObject.SetActive(false); // que cojones pasa tio, asi si que funciona pero si lo destruyo algo rompo
        Destroy(this.gameObject);
        Destroy(this);
    }


}
