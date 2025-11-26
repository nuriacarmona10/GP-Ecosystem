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

    [Header("PREFABS")]
    public GameObject applePrefab;

    [Header("Populations")] 
    public Population[] initialPopulations;

    [System.Serializable] // creo la populacion en el editor 
    public struct Population
    {
        public GameObject prefab;  // Hace referencia al prefab de la entidad viva
        public int count;            // Cantidad de entidades vivas en la población
    }


    public static EcosystemManager Instance;
    [HideInInspector] public List<Cosita> cositas;
    [HideInInspector] public List<Tree> trees;
    [HideInInspector] public GameObject cositaPrefab;
    [HideInInspector] public GameObject treePrefab;


    private void Awake()
    {
        Instance = this;
    }
    

   
    // Start is called before the first frame update
    void Start()
    {
        trees = new List<Tree>();
        cositas = new List<Cosita>();


        if (initialPopulations.Length > 0)
        {
            foreach (var popu in initialPopulations)
            {
                
               
                for (int i = 0; i < popu.count; i++)
                {
                    Vector3 randomSpawn = new Vector3(Random.Range(-24.5f, 24.5f), 0.25f, Random.Range(-24.5f, 24.5f));
                    GameObject entity = Instantiate(popu.prefab, randomSpawn, Quaternion.identity);
                    LivingEntity livingEntity = entity.GetComponent<LivingEntity>();
                    if (livingEntity is Cosita cos)
                    {
                        cos.Init();
                        cositaPrefab = popu.prefab; // Esto no esta bien aqui 
                        cositas.Add(cos);

                    }
                    else if (livingEntity is Tree tree)
                    {
                        tree.Init();
                        treePrefab = popu.prefab; // Esto no esta bien aqui 
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
    public void HandleEntityBorn(Cosita mother)
    {
        GameObject entity = Instantiate(cositaPrefab, mother.transform.position, Quaternion.identity);
        LivingEntity livingEntity = entity.GetComponent<LivingEntity>();
        if (livingEntity is Cosita cos)
        {
            cos.Init(mother);

            cositas.Add(cos);
        }
        else if (livingEntity is Tree tree)
        {   
            tree.Init();
            trees.Add(tree);
        }
        UpdateHud();   

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

    public void UpdateHud()
    {
        cositasCountText.text = cositas.Count.ToString();
        treesCountText.text = trees.Count.ToString();
    }

}
