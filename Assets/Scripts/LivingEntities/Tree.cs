using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : LivingEntity
{

    public GameObject applePrefab;
    public List<GameObject> appleSpawnPoint;
    public List<Apple> apples;


    public float appleRespawnTime = 10f;
    public bool isRespawnTime = false;
    //public float ripeAppleTime = 10f;

    // Delegado para notificar cuando una manzana madure
    public delegate void AppleRipeEvent(Apple apple);
    public event AppleRipeEvent OnAppleRipe;

    // Start is called before the first frame update
    public override void Init(LivingEntity mother = null)
    {
        specie = Specie.Tree;
       

        
    }


    private void OnEnable()
    {
        OnAppleRipe += LaunchApple; // Suscribe el método de lanzamiento a la notificación de madurez
    }

    // Este método se llama cuando se destruye el objeto para desuscribir el delegado
    private void OnDisable()
    {
        OnAppleRipe -= LaunchApple; // Desuscribe el método de lanzamiento
    }

    public void LaunchApple(Apple apple)
    {

    }
    //public IEnumerable 

    public void SpawnApples ()
    {

         foreach (GameObject spawnPoint in appleSpawnPoint)
         {
                GameObject a = Instantiate(applePrefab, spawnPoint.transform);
                Apple apple = a.GetComponent<Apple>();

                if (apple != null)
                {
                    apple.Init(this);
                    apples.Add(apple);
                    StartCoroutine(apple.RipeTime());
                }

         }
    }

    public IEnumerator SpawnTimeCoolDown(float time)
    {
        yield return new WaitForSeconds(time);
        isRespawnTime = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(isRespawnTime)
        {
            SpawnApples();
           
        }
    }
}
