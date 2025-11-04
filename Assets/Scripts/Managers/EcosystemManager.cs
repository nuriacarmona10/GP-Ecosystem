using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UnityEngine;

public class EcosystemManager : MonoBehaviour

{

    [Header("UI")]
    public TMP_Text cositasCountText;
    public TMP_Text cositasInitializedCountText;

    public TMP_Text treesCountText;
    public TMP_Text treesInitializedCountText;

    [Header("Populations")] 
    public Population[] initialPopulations;

    [System.Serializable] // creo la populacion en el editor 
    public struct Population
    {
        public GameObject prefab;  // Hace referencia al prefab de la entidad viva
        public int count;            // Cantidad de entidades vivas en la población
    }

    [HideInInspector] public static List<Cosita> cositas;
    [HideInInspector] public static List<Tree> trees;



    private void OnEnable()
    {
        LivingEntity.OnEntityDied += HandleEntityDeath;
    }

    private void OnDisable()
    {
        LivingEntity.OnEntityDied -= HandleEntityDeath;
    }

    public void HandleEntityDeath(LivingEntity entity)
    {
        if (cositas.Contains(entity))
        {
            cositas.Remove((Cosita)entity);
        }
        else if (trees.Contains(entity))
        {
            trees.Remove((Tree)entity);
        }
        UpdateHud();
    }
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
                    if (livingEntity is Cosita cos)
                    {
                        cositas.Add(cos);
                    }
                    else if (livingEntity is Tree tree)
                    {
                        trees.Add(tree);
                    }
                }


            }

            cositasInitializedCountText.text = cositas.Count.ToString();
            treesInitializedCountText.text = cositas.Count.ToString();
            UpdateHud();
        }
        else
        {
            Debug.Log("There is no population initialized");
        }
        
    }

    public void UpdateHud()
    {
        cositasCountText.text = cositas.Count.ToString();
        treesCountText.text = trees.Count.ToString();
    }

}
