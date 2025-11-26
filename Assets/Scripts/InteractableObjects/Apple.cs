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
    public float InteractionDistance => 1.5f;
    public GameObject ResourceGameObject => this.gameObject;
    public float TimeToConsumeIt => 3f;

    public float TimeToMature;

    public Tree parentTree;

    public bool isRipe;

    public void Init(Tree parent)
    {
        parentTree = parent;
        isRipe = false;
        TimeToMature = Random.Range(6, 10);
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
        //parentTree.OnAppleRipe?.Invoke(this); // Llama al delegado
    }


}
