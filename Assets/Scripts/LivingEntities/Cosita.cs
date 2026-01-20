using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Security.Cryptography;
using TMPro;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.Port;
using static UnityEditor.PlayerSettings;

public class Cosita : LivingEntity
{
    
     public float speed;
     public float hydrated;
     public float sated;
     public Vector3 interactionBetweenCositasRange;
     public float reproductionHunger;
     public float reproductionHungerRate;
     public bool hasPassedReproCooldown;
     public bool isBaby;
     public Genes genes;
     public ActionManager actionManager;
     public Renderer cositaRenderer;
     public Cosita mom;
    





    public IResource resourceTarget;
    public List<IResource> inventoryList;

    public Cosita neighbourCositaInNeed;

    
    public NavMeshAgent Agent;

    //private Vector3 desperateDirection;

    //public Vector3 currentDirection; // Esto seguramente borrar junto con los metodos viejos de Walk y del movimiento de cosita en general
    //public Vector3 currentRandomPoint;
    // public Transform currentRandomPositionGoing;

    //BoxCollider boxColliderCosita;
    // private float walkingTimer;              // Contador de tiempo

    //Reproduction

    public int count = 0;

    ///UI
    [Header("UI")]
    public HUDBar hungerBar;
    public HUDBar waterBar;
    public HUDBar reproductionBar;
    public TMP_Text goalUI;
    public TMP_Text targetUI;
    public TMP_Text ThirstyUI;
    public GameObject inventorySlotUI;
    public GameObject SlotPrefab;
    public GameObject appleSlotPrefab;
    //public TMP_Text satedUI;

    public CreatureActions actionDoing = CreatureActions.Idle;
    public List<Cosita> childs;

    //DEBUG
    public TMP_Text debugUI;


  
    public bool isBusy = false;

    private int timeToDeathByHungerAndThirsty = 1;



    public override void Init(LivingEntity mother = null)
    {
        if (mother)
        {
            mom = mother as Cosita;
            genes = new Genes(mom.genes); // I'm passing down same genes as mother , pasar directamente gen
        }
        else
        {
            genes = new Genes();

        }

        childs = new List<Cosita>();
        //OnDrawGizmos();



        //Debug.Log("INIT COSITA");
        specie = Specie.Cosita;
        resourceTarget = null;
        //boxColliderCosita = GetComponent<BoxCollider>();
        hungerBar.SetMaxValue(100);
        waterBar.SetMaxValue(100);
        reproductionBar.SetMaxValue(100);

       

        AddInventorySlots();



        //genes = new Genes(0, 0); // I will replace them in the next line;
        //genes = genes.RandomGenes();

        //reproductionHunger = 0f;

        //currentRandomPoint = GetRandomPoint();
        UpdateUICosita();

        //ThirstyUI.text = "Thirsty: " + hydrated.ToString();
        //ChooseNextAction();

        if (isBaby) {
            StartCoroutine(GrowTimer(genes.timeToGrow));
        }

    }
    public void Reproduce()
    {
        Debug.Log("Me reproduzco");
        hasPassedReproCooldown = false;
        isBusy = true;
        StartCoroutine(ReproductionCooldown(3f));
       
    }
    // Update is called once per frame
    void Update()
    {
        //Increase hunger and thirst over time
        sated -= Time.deltaTime * 1 / timeToDeathByHungerAndThirsty; // ahora mismo tarda 100 segundos en morir
        hydrated -= Time.deltaTime * 1 / timeToDeathByHungerAndThirsty;

        //Esto no deberia ir aquí
        //goalUI.text = actionDoing.ToString();
        //if (target)
        //{
        //    targetUI.text = "Target: " + target.name;
        //}
        //else {
        //    targetUI.text = "Target: None";

        //}


        // Verificar si el cooldown se ha terminado
        //debugUI.text = "Haciendo algo: " + isDoingAction.ToString();
        if (!isBusy)
        {
            SensingEnvironment();
            Act();
            UpdateUICosita();

        }

       

        if (hydrated <= 0 || sated <= 0)
        {
            Die();
            Debug.Log("Me MORIII");
        }

        UpdateReproductionHunger();


        
       


    }
    public void UpdateReproductionHunger()
    {
        float delta = reproductionHungerRate * Time.deltaTime;

        if (hydrated > 50f && sated > 50f)
        {
            reproductionHunger = Mathf.Min(reproductionHunger + delta, 100f);
        }
        else if (hydrated < 50f || sated < 50f)
        {
            reproductionHunger = Mathf.Max(reproductionHunger - delta, 0f);
        }

    }

    IEnumerator GrowTimer(float time)
    {
        

        // Espera 5 segundos antes de permitir la siguiente acción

        yield return new WaitForSeconds(time);
        transform.localScale = transform.localScale * 2;
        genes.sensingRange = genes.sensingRange * 2;




    }

    IEnumerator ReproductionCooldown(float time)
    {


        // Espera 5 segundos antes de permitir la siguiente acción

        yield return new WaitForSeconds(time);
        EcosystemManager.Instance.HandleEntityBorn(this);
        reproductionHunger = reproductionHunger / 2;
        hasPassedReproCooldown = true;
        isBusy = false;
        




    }

    public void ToShare(IResource resourceToShare)
    {
        if (neighbourCositaInNeed != null)
        {

            neighbourCositaInNeed.ToReceive(resourceToShare);
            neighbourCositaInNeed = null;

            StartCoroutine(BusyCoolDown(3f));

        }

    }
    public void ToReceive(IResource resourceReceived)
    {
        this.actionDoing = CreatureActions.Receiving;
        if (resourceReceived!=null)
        {
            this.AddResourceToInventory(resourceReceived);


        }
    }
    public void SensingEnvironment()
    {
        // Inicia la acción

        //if busy then return 

        //if (hydrated < 50f && resourceTarget == null ) // si tengo sed y no tengo asignado ya un target pues voy a buscar agua
        //{

        //}
        //else if ( sated < 60f && resourceTarget == null)   // si no he encontrado agua pero tengo hambre, busco comida
        //{

        if (hydrated < 90f )
            SearchForResource("Water");
        if (sated < 90f && inventoryList.Count < genes.inventorySlots) 
            SearchForResource("Food");

        SearchForMom();

        if(inventoryList.Count >= genes.inventorySlots / 2 && neighbourCositaInNeed == false)
        {
            SearchForNeighbours();
        }


    }
    public void SearchForMom()
    {
        Collider[] objectsDetected = Physics.OverlapSphere(transform.position, genes.sensingRange);
        foreach (Collider collider in objectsDetected)
        {
            Cosita cosita = collider.GetComponent<Cosita>();
            if (cosita != null && cosita == mom)
            {
                neighbourCositaInNeed = cosita;
                return;
            }
            else
            {
                neighbourCositaInNeed = null;
            }
        }
    }
    public void SearchForNeighbours()
    {
        Collider[] objectsDetected = Physics.OverlapSphere(transform.position, genes.sensingRange);
        foreach (Collider collider in objectsDetected)
        {
            Cosita cosita = collider.GetComponent<Cosita>();
            if (cosita != null && cosita.inventoryList.Count < cosita.genes.inventorySlots)
            {
                neighbourCositaInNeed = cosita;
                return;
            }
            else
            {
                neighbourCositaInNeed = null;
            }
        }
    }

    public void RemoveAppleFromInventory(Apple apple)
    {
        inventoryList.Remove(apple);
        //Debug.Log("Tengo estos hijos" + inventorySlotUI.transform.childCount.ToString());
        //debugUI.text = inventorySlotUI.transform.childCount.ToString();

        for (int i = inventorySlotUI.transform.childCount - 1; i >= 0; i--) // empiezo de atrás a delante
        {
            Transform child = inventorySlotUI.transform.GetChild(i);

            // Verifica si el hijo tiene hijos
            if (child.childCount > 0)
            {
                Destroy(child.GetChild(0).gameObject);
                return;
            }
        }

    }
    public IResource SensingResources(string thingWanted) // I pass a string with the name of the thing I want
    {
        Collider[] objectsDetected = Physics.OverlapSphere(transform.position, genes.sensingRange);

        IResource closestResource = null;
        float closestDistance = Mathf.Infinity;  // Comenzamos con una distancia muy grande

        foreach (Collider collider in objectsDetected)
        {
            IResource resource = collider.GetComponent<IResource>();
            if (resource != null && resource.ResourceType == thingWanted)
            {
                // Calculamos la distancia desde el objeto que llama al método hasta el objeto detectado
                float distance = Vector3.Distance(transform.position, collider.transform.position);

                // Si esta distancia es la más cercana, actualizamos el recurso más cercano
                if (distance < closestDistance)
                {
                    closestResource = resource;
                    closestDistance = distance;  // Actualizamos la distancia más cercana
                }
            }
        }

        return closestResource;  // Devolvemos el recurso más cercano o null si no se encontró ninguno

    }

    public void UpdateUICosita()
    {
        goalUI.text = actionDoing.ToString();
        ThirstyUI.text = "Thirsty: " + hydrated.ToString();
        hungerBar.SetSliderValue(sated);
        waterBar.SetSliderValue(hydrated);
        //debugUI.text = "Path Active: " + Agent.hasPath + " | Distancia Restante: " + Agent.remainingDistance;
        reproductionBar.SetSliderValue(reproductionHunger);



        //debugUI.text = inventoryList.Count.ToString();


        //if (target)
        //{
        //    targetUI.text = "Target: " + target.name;
        //}
        //else
        //{
        //    targetUI.text = "Target: None";

        //}

    }
    public void AddInventorySlots()
    {
        inventoryList = new List<IResource>();
        inventoryList.Capacity = genes.inventorySlots;

        for (int i = 0; i<genes.inventorySlots; i++) {

            Instantiate(SlotPrefab, inventorySlotUI.transform);

        }
    }
    void OnDrawGizmos()
    {
        // Configura el color del radar (por ejemplo, semi-transparente y rojo)
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);  // Rojo con algo de transparencia

        // Dibuja una esfera en la posición del objeto (el radar de 15m)
        Gizmos.DrawSphere(transform.position, genes.sensingRange);
    }
    
    private void SearchForResource(string resourceName)
    {
        IResource resourceFound = SensingResources(resourceName);
        if (resourceFound != null && resourceFound.ResourceGameObject != null)
        {
            //actionDoing = resourceName.Equals("Water") ? CreatureActions.GoingToWater : CreatureActions.GoingToFood;
            
                //actionDoing = CreatureActions.GoingToFood;
            
                //actionDoing = CreatureActions.GoingToWater;
                resourceTarget = resourceFound;
                //Agent.ResetPath(); // con esto va directamente sin terminar de hacer el path que tenia marcado antes de encontrar agua

            //resourceTargetUI.text = resourceTarget.name.ToString();

        }
        //else
        //{
        //    resourceTarget = null; 
        //    actionDoing = CreatureActions.Exploring; // aqui iba walking desesperatly

        //}

    }

   
    public Vector3 GetRandomDirection()
    {
        // random direcction in XZ 
        Vector3 randomDirection = new Vector3(
            Random.Range(-1f, 1f), // X
            0f,                     // Y = 0
            Random.Range(-1f, 1f)  // Z
        ).normalized; // Normaliza para que solo sea dirección

        return randomDirection;

    }
    public void Act()
    {
        actionManager.ExecuteAction();
      

    }
    public void AddResourceToInventory(IResource res)
    {
        if (res as Apple)
        {
            isBusy = true;
            inventoryList.Add(res);
            foreach (Transform slot in inventorySlotUI.transform)
            {
                // Verificar si el slot no tiene hijos
                if (slot.childCount == 0)
                {
                    // Instanciar el prefab de la manzana en este slot vacío
                    Instantiate(appleSlotPrefab, slot);

                    // Si deseas agregar una imagen o un sprite a la manzana instanciada, puedes hacerlo así:
                    // appleSlotPrefab.GetComponent<Image>().sprite = appleSprite; // Asigna el sprite de la manzana si es necesario

                    break; // Terminar el ciclo ya que hemos encontrado un slot vacío
                }
            }
            StartCoroutine(BusyCoolDown(3f));
            resourceTarget.DestroyGameobject();
            resourceTarget = null;
            //Agent.ResetPath();
        }
    }
    
    public IEnumerator ConsumingResourceCooldown(IResource resource)
    {


        // Espera 5 segundos antes de permitir la siguiente acción

        hydrated += resource.Hydration;
        if (hydrated > 100)
            hydrated = 100;
        sated += resource.Satiety;
        if (sated > 100)
            sated = 100;

        yield return new WaitForSeconds(resource.TimeToConsumeIt);
        isBusy = false;

    }
    public IEnumerator BusyCoolDown(float time)
    {
        isBusy = true;
        yield return new WaitForSeconds(time);
        isBusy = false;

    }

   
    public void MoveDesesperatly()
    {
        if (Agent.hasPath)
            return;

        Vector3 dir = Vector3.forward;
        Agent.SetDestination(dir);

    }

    public void MoveToTarget(Vector3 target)
    {
        if (Agent.hasPath)
            return;

       

        Debug.Log("Me muevo hacia un target");
        Agent.SetDestination(target);
    }
    public void MoveToRandomPoint()
    {
        debugUI.text = "Path Active: " + Agent.hasPath + " | Distancia Restante: " + Agent.remainingDistance;

        Vector3 randomPoint;

        if (Agent.hasPath)
            return;

    
        if(GetRandomPoint(out randomPoint))
        {

            if (Agent.SetDestination(randomPoint))
            {
            debugUI.text = "Escojo un nuevo punto" + randomPoint;

            }

        }
        //debugUI.text = "remaining distance" + Agent.remainingDistance;


    }
    //public Vector3 GetRandomPoint()
    //{
    //    for (int i = 0; i < 10; i++) // hasta 10 intentos
    //    {
    //        Vector3 randomPoint = transform.position + Random.insideUnitSphere * genes.sensingRange;
    //        randomPoint.y = transform.position.y;

    //        if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
    //        {
    //            return hit.position;
    //        }
    //    }

    //    // Si no encontró ninguno, devuélvelo a su posición actual (no al (0,0,0))
    //    return transform.position;
    //}
    bool GetRandomPoint(out Vector3 result)
    {
        for (int i = 0; i <30; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * genes.sensingRange * 2;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;

                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }

    bool GetFarRandomPoint(out Vector3 result)
    {
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * genes.sensingRange*4;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }


    public bool AreNear(GameObject objectToCheck, Vector3 range)
    {

        return System.Math.Abs(this.transform.position.x - objectToCheck.transform.position.x) <= range.x && System.Math.Abs(this.transform.position.y - objectToCheck.transform.position.y) <= range.y && System.Math.Abs(this.transform.position.z - objectToCheck.transform.position.z) <= range.z;

    }

   
}
