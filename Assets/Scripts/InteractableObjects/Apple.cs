using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour, IResource
{
    public float energy;
    public string ResourceType => "Food";
    public float hydration => 100f;
    public float satiety => 30f;
    public GameObject gameObjecto => this.gameObject;
    public float timeToConsumeIt => 3f;


    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    public void Consume()
    {
        Debug.Log("Eating Apple");
        Destroy(this.gameObject);
    }


}
