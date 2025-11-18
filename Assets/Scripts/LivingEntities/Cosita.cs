using System.Collections;
using System.Collections.Generic;
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
    
    [SerializeField] public float health;     // Salud de la cosita
    [SerializeField] public float speed;
    [SerializeField] public float hydrated;
    [SerializeField] public float sated;
    [SerializeField] public float sensingRange;
    [SerializeField] public float reproductionHunger;
    [SerializeField] public float reproductionHungerRate;
    [SerializeField] public bool hasPassedReproCooldown;
    [SerializeField] public Genes genes;
    [SerializeField] public ActionManager actionManager;



    public IResource resourceTarget;
    //public IResource resource;


    public List<IResource> inventoryList;
    public NavMeshAgent Agent;

    //private Vector3 desperateDirection;

    //public Vector3 currentDirection; // Esto seguramente borrar junto con los metodos viejos de Walk y del movimiento de cosita en general
    //public Vector3 currentRandomPoint;
   // public Transform currentRandomPositionGoing;

    //BoxCollider boxColliderCosita;
    // private float walkingTimer;              // Contador de tiempo

    //Reproduction
   


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

    //DEBUG
    public TMP_Text debugUI;


  
    public bool isBusy = false;

    private int timeToDeathByHungerAndThirsty = 1;



    public override void Init(LivingEntity mother = null)
    {
        if (mother != null)
        {
            Cosita cositaMom = mother as Cosita;
            genes = new Genes(cositaMom.genes.inventorySlots); // I'm passing down same genes as mother , pasar directamente gen


        }
        else
        {
            genes = Genes.RandomGenes();
        }
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

    

    IEnumerator ReproductionCooldown(float time)
    {


        // Espera 5 segundos antes de permitir la siguiente acción

        yield return new WaitForSeconds(time);
        EcosystemManager.Instance.HandleEntityBorn(this);
        reproductionHunger = reproductionHunger / 2;
        hasPassedReproCooldown = true;
        isBusy = false;
        




    }
    public void SensingEnvironment()
    {
        // Inicia la acción

        //if busy then return 

        if (hydrated < 50f )
        {
            SearchForResource("Water");
            
        }
        else if ( sated < 60f )   
        {
            
            SearchForResource("Food");
            

        }
        


        else if (hasPassedReproCooldown && reproductionHunger > 55f)
        {
            Agent.ResetPath(); // quiero que se quede parado
            //actionDoing = CreatureActions.Cloning;
        }
        else
        {
            //target = null;
            //actionDoing = CreatureActions.Exploring;
        }
        

        
        //StartCoroutine(ActionCooldown());
    }
  
    public IResource SensingResources(string thingWanted) // I pass a string with the name of the thing I want
    {
        Collider[] objectsDetected = Physics.OverlapSphere(transform.position, sensingRange);

        foreach (Collider collider in objectsDetected)
        {
            IResource resource = collider.GetComponent<IResource>();
            if (resource != null && resource.ResourceType == thingWanted )
            {
                return resource;

            }
        }

        return null;

    }

    public void UpdateUICosita()
    {
        goalUI.text = actionDoing.ToString();
        ThirstyUI.text = "Thirsty: " + hydrated.ToString();
        hungerBar.SetSliderValue(sated);
        waterBar.SetSliderValue(hydrated);
        reproductionBar.SetSliderValue(reproductionHunger);



        debugUI.text = inventoryList.Count.ToString();


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
        Gizmos.DrawSphere(transform.position, sensingRange);
    }
    
    private void SearchForResource(string resourceName)
    {
        IResource resourceFound = SensingResources(resourceName);
        if (resourceFound != null)
        {
            //actionDoing = resourceName.Equals("Water") ? CreatureActions.GoingToWater : CreatureActions.GoingToFood;
            
                //actionDoing = CreatureActions.GoingToFood;
            
                //actionDoing = CreatureActions.GoingToWater;
                resourceTarget = resourceFound;

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
        //switch(actionDoing)
        //{
        //    case CreatureActions.GoingToWater:
        //        if (AreNear(resourceTarget.ResourceGameObject, 1.5f))
        //        {
                    
        //            actionDoing = CreatureActions.Drinking;
        //            Agent.ResetPath();
        //            isBusy = true;
        //            StartCoroutine(DrinkingCooldown(resourceTarget.TimeToConsumeIt));

                  
        //        }
        //        else
        //        {
        //            MoveToTarget(resourceTarget.ResourceGameObject);

        //        }
        //        break;

        //    case CreatureActions.GoingToFood:

        //        if(AreNear(resourceTarget.ResourceGameObject, 2f))
        //        {
        //            AddResourceToInventory(resourceTarget);
        //            resourceTarget.Consume();
        //            isBusy = true;
        //            StartCoroutine(EatingCooldown(resourceTarget.TimeToConsumeIt));
        //            resourceTarget = null;
        //            Agent.ResetPath();



        //        }
        //        else
        //        {
        //            MoveToTarget(resourceTarget.ResourceGameObject);

        //        }
        //        break;

        //    case CreatureActions.Eating:
                
        //            if (inventoryList.Count>0) { 
                       
        //                Debug.Log("Lo he hecho");
        //                Apple apple = inventoryList[0] as Apple;

        //                sated += apple.Satiety;
        //                if (sated > 100)
        //                    sated = 100;

        //                isBusy = true;
        //                StartCoroutine(EatingCooldown(apple.TimeToConsumeIt));
        //                //apple.Consume();
        //                Debug.Log("Quito la manzanita de mi inventario porque me la comi");
        //                inventoryList.Remove(apple);

        //                Debug.Log("Tengo estos hijos" + inventorySlotUI.transform.childCount.ToString());
        //                debugUI.text = inventorySlotUI.transform.childCount.ToString();

        //                for (int i = inventorySlotUI.transform.childCount - 1; i >= 0; i--) // empiezo de atrás a delante
        //                {
        //                    Transform child = inventorySlotUI.transform.GetChild(i);

        //                    // Verifica si el hijo tiene hijos
        //                    if (child.childCount > 0)
        //                    {
        //                        Destroy(child.GetChild(0).gameObject);
        //                        break;
        //                    }
        //                }
        //                //    Transform LastChild = inventorySlotUI.transform.GetChild(inventorySlotUI.transform.childCount - 1).GetChild(0);
        //                // Debug.Log("Soy el hijo" + LastChild.name);

        //                //if (LastChild != null)
        //                //{
        //                //    //LastChild.gameObject.SetActive(false);
        //                //    Destroy(LastChild.gameObject);
        //                //    Debug.Log("He destruido a appleSlot");
        //                //    break;


        //                //}
        //        }

                
               
                
               
        //        break;

        //    case CreatureActions.WalkingDesperately:
        //        if (Agent.remainingDistance < 0.5f)
        //        {
        //            Agent.ResetPath();
        //            //currentRandomPoint = Vector3.zero;
        //        }
        //        MoveToRandomPoint();
        //        //Agent.Move(desperateDirection * Time.deltaTime * speed);
        //        //ChooseNextAction(); // quiero que si es que tiene hambre o sed y encuentra algo que vaya hacia el
        //        break;
        //    case CreatureActions.Cloning:
        //        Reproduce();
        //        break;
        //    case CreatureActions.Exploring:
        //        if (Agent.remainingDistance < 0.5f)
        //        {
        //            Agent.ResetPath();
        //            //currentRandomPoint = Vector3.zero;
        //        }
        //        MoveToRandomPoint();
        //        //ChooseNextAction();
        //        break;
        //}

    }
    public void AddResourceToInventory(IResource res)
    {
        if (res as Apple)
        {
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
        }
    }
    public IEnumerator EatingCooldown(float time)
    {


        // Espera 5 segundos antes de permitir la siguiente acción
        yield return new WaitForSeconds(time);
       

        isBusy = false;


    }
    public IEnumerator DrinkingCooldown(float time)
    {


        // Espera 5 segundos antes de permitir la siguiente acción
        isBusy = true;
        hydrated += resourceTarget.Hydration;
        if (hydrated > 100)
            hydrated = 100;
        resourceTarget = null;
        yield return new WaitForSeconds(time);
        isBusy = false;






    }

    public void MoveToTarget(GameObject target)
    {
        //Vector3 targetPositionIgnoringY = new Vector3(target.position.x, transform.position.y, target.position.z);
        //Vector3 dir = (targetPositionIgnoringY - transform.position).normalized;

        if (Agent.hasPath)
            return;

        if (target)
            Agent.SetDestination(target.transform.position);
        //Agent.Move(dir * speed * Time.deltaTime);


        //transform.position = Vector3.MoveTowards(
        //    transform.position,
        //    targetPositionIgnoringY,
        //    speed * Time.deltaTime
        //);


        //currentDirection = (targetPositionIgnoringY - transform.position).normalized;
    }
    public void MoveDesesperatly()
    {
        if (Agent.hasPath)
            return;

        Vector3 dir = Vector3.forward;
        Agent.SetDestination(dir);

    }
    public void MoveToRandomPoint()
    {
        Vector3 randomPoint;

        if (Agent.hasPath)
            return;

        if(GetRandomPoint(out randomPoint))
        {
        
            Agent.SetDestination(randomPoint);

        }

        
       

        //     walkingTimer += Time.deltaTime;

        //// Si ya pasaron 3 segundos, cambia la dirección
        //if (walkingTimer >= 3f)
        //{
        //    Vector3 randomdirection = GetRandomDirection();
        //    Vector3 smoothedDir = Vector3.Lerp(currentDirection, randomdirection, 0.5f).normalized;
        //    currentDirection = smoothedDir;
        //    walkingTimer = 0f; // Reinicia el contador
        //}






    }
    //public Vector3 GetRandomPoint()
    //{
    //    for (int i = 0; i < 10; i++) // hasta 10 intentos
    //    {
    //        Vector3 randomPoint = transform.position + Random.insideUnitSphere * sensingRange;
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
        for (int i = 0; i < 30; i++)
        {
            Vector3 randomPoint = transform.position + Random.insideUnitSphere * sensingRange*2;
            if (NavMesh.SamplePosition(randomPoint, out NavMeshHit hit, 2f, NavMesh.AllAreas))
            {
                result = hit.position;
                return true;
            }
        }
        result = Vector3.zero;
        return false;
    }


    public bool AreNear(GameObject objectToCheck, float range)
    {

        return System.Math.Abs(this.transform.position.x - objectToCheck.transform.position.x) <= range && System.Math.Abs(this.transform.position.z - objectToCheck.transform.position.z) <= range;
        //float radius = Mathf.Max(boxColliderCosita.size.x, boxColliderCosita.size.y, boxColliderCosita.size.z) / 2f;
        //Collider[] objectsDetected = Physics.OverlapSphere(transform.position, radius);

        //foreach (Collider collider in objectsDetected)
        //{
        //    if (collider.CompareTag(target.gameObject.tag))
        //    {
        //        if (target.gameObject.tag.Equals("Water"))
        //        {
        //            actionDoing = CreatureActions.Drinking;
        //            hydrated = 100;

        //        }
        //        else if (target.gameObject.tag.Equals("Food"))
        //        {
        //            actionDoing = CreatureActions.Eating;
        //            sated = 100;
        //        }

        //        target = null;
        //        break;
        //    }
        //}
    }

    //public void Walk()
    //{
    //    //I only walk without direcction
       
    //    desperateDirection = Vector3.zero; //Im not desperate any more

    //    transform.Translate(currentDirection * Time.deltaTime * speed);
    //    Debug.Log("Ando a una velocidad de:" + currentDirection * Time.deltaTime * speed);
    //    //transform.position += smoothedDir * speed;
    //    // Aumenta el contador de tiempo
    //    walkingTimer += Time.deltaTime;

    //    // Si ya pasaron 3 segundos, cambia la dirección
    //    if (walkingTimer >= 3f)
    //    {
    //        Vector3 randomdirection = GetRandomDirection();
    //        Vector3 smoothedDir = Vector3.Lerp(currentDirection, randomdirection, 0.5f).normalized;
    //        currentDirection = smoothedDir;
    //        walkingTimer = 0f; // Reinicia el contador
    //    }


       

    //}

   



   
}
