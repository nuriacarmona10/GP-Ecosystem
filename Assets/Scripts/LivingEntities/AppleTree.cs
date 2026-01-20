using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppleTree : LivingEntity
{

    public GameObject applePrefab;
    public List<GameObject> appleSpawnPoints;
    public List<Apple> apples;


    public float appleRespawnTime = 10f;
    //public bool isRespawnTime;
    //public float ripeAppleTime = 10f;



    // Start is called before the first frame update
    public override void Init(LivingEntity mother = null)
    {
        specie = Specie.Tree;

        //isRespawnTime = true;

    }

    void Start()
    {

        SpawnInitApples();

    }

    public void SubscribeAppleEvent(Apple apple)
    {
        apple.OnAppleRipe += LaunchApple; // suscribe el método de lanzamiento
        //apple.OnAppleSpawn += SpawnApple; // suscribe el método de lanzamiento

    }

    public void UnsubscribeAppleEvent(Apple apple)
    {
        apple.OnAppleRipe -= LaunchApple; // Desuscribe el método de lanzamiento

    }

    public void SpawnSpecificApple(Apple apple)
    {

    }

    public void LaunchApple(Apple apple)
    {
        // Ejemplo de acción que puede realizarse al madurar la manzana
        apple.gameObject.GetComponent<Rigidbody>().useGravity = true;
        apple.gameObject.GetComponent<Rigidbody>().isKinematic = false;
        //Debug.Log("La manzana " + apple.gameObject.name + " está madura y se lanza.");
        StartCoroutine(SpawnApple(30f));

        // Aquí puedes agregar la lógica de lo que pasa con la manzana madura
    }
    //public IEnumerable 

    public void SpawnInitApples()
    {
        foreach (GameObject spawnPoint in appleSpawnPoints)
        {
            if (spawnPoint.transform.childCount == 0) // if there is not an apple already there 
            {
                GameObject a = Instantiate(applePrefab, spawnPoint.transform);
                Apple apple = a.GetComponent<Apple>();
                apple.Init(this);
                SubscribeAppleEvent(apple);
                apples.Add(apple);
                StartCoroutine(apple.RipeTime());

            }
        }
    }


    public IEnumerator SpawnApple(float timeToSpawn)
    {
        yield return new WaitForSeconds(timeToSpawn);

        foreach (GameObject spawnPoint in appleSpawnPoints)
        {
            if (spawnPoint.transform.childCount == 0)
            {
                GameObject a = Instantiate(applePrefab, spawnPoint.transform);
                Apple apple = a.GetComponent<Apple>();
                apple.Init(this);
                SubscribeAppleEvent(apple);
                apples.Add(apple);
                StartCoroutine(apple.RipeTime());

            }
        }



        //int indexRandomFreeSpot = appleSpawnPoints.IndexOf(freeSpawnPoints[randomIndex]);








    }



}
