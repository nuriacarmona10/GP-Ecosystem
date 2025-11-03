using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
        public Specie specie;
    }

    public List<Cosita> cositas;
    public List<Tree> trees;


    // Start is called before the first frame update
    void Start()
    {
        trees = new List<Tree>();
        cositas = new List<Cosita>();

        List<LivingEntity> aux = new List<LivingEntity>();

        if (initialPopulations.Length > 0)
        {
            foreach (var population in initialPopulations)
            {
               
                for (int i = 0; i < population.count; i++)
                {
                    Vector3 randomSpawn = new Vector3(Random.Range(-24.5f, 24.5f), 0.25f, Random.Range(-24.5f, 24.5f));
                    GameObject entity = Instantiate(population.prefab, randomSpawn, Quaternion.identity);
                    LivingEntity livingEntity = entity.GetComponent<LivingEntity>();
                    aux.Add(livingEntity);
                    livingEntity.Init();
                }

                if (population.specie == Specie.Cosita)
                {
                    cositas = aux.Cast<Cosita>().ToList();
                }
                else if (population.specie == Specie.Tree)
                {
                    trees = aux.Cast<Tree>().ToList();
                }

            }
        }
        else
        {
            Debug.Log("There is no population initialized");
        }


        
    }

}
