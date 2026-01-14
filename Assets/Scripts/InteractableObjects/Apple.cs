using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Apple : MonoBehaviour, IResource
{
    public float energy;
    public string ResourceType => "Food";
    public float Hydration => 20f;
    public float Satiety => 30f;
    public Vector3 InteractionDistance => new Vector3(1.5f, 0.5f, 1.5f);
    public float TimeToConsumeIt => 3f;

    public GameObject ResourceGameObject => this.gameObject;

    private float TimeToMature;
    private float TimeToSpawn;

    public AppleTree parentTree;


    public bool isRipe;

    

    // Delegado para notificar cuando una manzana madure
    public delegate void AppleRipeEvent(Apple apple);
    public event AppleRipeEvent OnAppleRipe;
    public delegate void AppleRespawnEvent();
    public event AppleRespawnEvent OnAppleSpawn;

    public void Init(AppleTree parent)
    {
        parentTree = parent;
        isRipe = false;
        TimeToMature = Random.Range(6, 20);
        TimeToSpawn = 30;


}
public void Start()
    {
        StartCoroutine(RipeTime());



    }



    // Update is called once per frame
    public void DestroyGameobject()
    {
        //Debug.Log("Eating Apple");
        foreach (Cosita cosita in FindObjectsOfType<Cosita>())  // Recorre todas las Cositas en la escena
        {
            if (cosita.resourceTarget == this)  // Si la Cosita tiene asignado este recurso
            {
                cosita.resourceTarget = null;  // Limpia la referencia
            }
        }
        parentTree.UnsubscribeAppleEvent(this);
        parentTree.apples.Remove(this);
        DestroyImmediate(this.gameObject); // de verdad esto lo ha arreglado todo, i cant not believe
        //this.gameObject.SetActive(true);


        //this.gameObject.SetActive(false); // que cojones pasa tio, asi si que funciona pero si lo destruyo rompo todo
        //Si este recurso(Apple) es el que está asignado en la cosita, limpiamos la referencia.


        //StartCoroutine(DestroyCountdown(5f));

        // Luego destruye el objeto, ya que la referencia ya ha sido limpiada
        //Destroy(this.gameObject);

        //Destroy(this);
    }

    public IEnumerator RipeTime()
    {
        yield return new WaitForSeconds(TimeToMature);
        isRipe = true;
        OnAppleRipe?.Invoke(this); // Llama al delegado para notificar que la manzana está madura
    }
    
    public IEnumerator RespawnTime()
    {

        yield return new WaitForSeconds(TimeToSpawn);
        OnAppleSpawn?.Invoke(); // Llama al delegado para notificar que la manzana está madura


    }


}
