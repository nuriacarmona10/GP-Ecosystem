using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EcosystemManager : MonoBehaviour

{

    [Header("Populations")] 
    public Population[] initialPopulations;

    [System.Serializable] // creo la populacion en el editor 
    public struct Population
    {
        public GameObject prefab;  // Hace referencia al prefab de la entidad viva
        public int count;            // Cantidad de entidades vivas en la población
    }
    // Start is called before the first frame update
    void Start()
    {
        if(initialPopulations.Length > 0)
        {
            foreach (var specie in initialPopulations)
            {
                for (int i = 0; i < specie.count; i++)
                {
                    Vector3 randomSpawn = new Vector3(Random.Range(-24.5f, 24.5f), 0.25f, Random.Range(-24.5f, 24.5f));
                    GameObject cosita = Instantiate(specie.prefab, randomSpawn, Quaternion.identity);
                    Cosita cos = cosita.GetComponent<Cosita>();
                    cos.Init();
                }
                   
                
                
            }
        }
        else
        {
            Debug.Log("There is no population initialized");
        }


        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
