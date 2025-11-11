using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;
using UnityEditor.Experimental.GraphView;
using UnityEngine.AI;

public class Cosita : LivingEntity
{
    
    [SerializeField] public float health;     // Salud de la cosita
    [SerializeField] public float speed;
    [SerializeField] public float hydrated;
    [SerializeField] public float sated;
    [SerializeField] public float sensingRange;
    [SerializeField] public float reproductionHunger;
    [SerializeField] public float reproductionHungerRate;
    [SerializeField] public bool  hasPassedReproCooldown;


    public Transform target;
    public Vector3 desperateDirection;

    //public Vector3 currentDirection; // Esto seguramente borrar junto con los metodos viejos de Walk y del movimiento de cosita en general
    //public Vector3 currentRandomPoint;
   // public Transform currentRandomPositionGoing;

    BoxCollider boxColliderCosita;
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
    //public TMP_Text satedUI;

    CreatureActions actionDoing = CreatureActions.Idle;

    //DEBUG
    public TMP_Text debugUI;


    private float coolDownActionChoice= 5.0f;  // The cooldown duration (e.g., 5 second)
    private float currentCoolDownActionChoice = 0.0f;  // The current cooldown time left
    private bool isBusy = false;

    private int timeToDeathByHungerAndThirsty = 1;
    public NavMeshAgent agent;



    public override void Init()
    {
        //Debug.Log("INIT COSITA");
        specie = Specie.Cosita;
        target = null;
        boxColliderCosita = GetComponent<BoxCollider>();
        hungerBar.SetMaxValue(100);
        waterBar.SetMaxValue(100);
        reproductionBar.SetMaxValue(100);
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
            ChooseNextAction();
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
        else if (hydrated < 50f && sated < 50f)
        {
            reproductionHunger = Mathf.Max(reproductionHunger - delta, 0f);
        }

    }

    IEnumerator EatingCooldown(float time)
    {


        // Espera 5 segundos antes de permitir la siguiente acción
        isBusy = true;
        yield return new WaitForSeconds(time);
        sated = 100;
        target = null;
        isBusy = false;


    }
    IEnumerator DrinkingCooldown(float time)
    {


        // Espera 5 segundos antes de permitir la siguiente acción
        isBusy = true;
        yield return new WaitForSeconds(time);
        hydrated = 100;
        target = null;
        isBusy = false;


    }

    IEnumerator ReproductionCooldown(float time)
    {


        // Espera 5 segundos antes de permitir la siguiente acción

        yield return new WaitForSeconds(time);
        hasPassedReproCooldown = true;
        isBusy = false;
        EcosystemManager.Instance.HandleEntityBorn(this.transform);
        reproductionHunger = reproductionHunger / 2;




    }
    public void ChooseNextAction()
    {
        // Inicia la acción

        //if busy then return 

        if (hydrated < 50f )
        {
            SearchForResource("Water");
            if (target == null && sated < 50f)
            {
                SearchForResource("Food");
            }
        }
        else if ( sated < 50f )
        {
            SearchForResource("Food");

        }
        else if (hasPassedReproCooldown && reproductionHunger > 50f )
        {
            agent.ResetPath(); // quiero que se quede parado
            actionDoing = CreatureActions.Cloning;
        }
        else
        {
            target = null;
            actionDoing = CreatureActions.Exploring;
        }
        

        
        //StartCoroutine(ActionCooldown());
    }
  
    public Transform SensingEnvironment(string thingWanted) // I pass a string with the name of the tag to the method
    {
        Collider[] objectsDetected = Physics.OverlapSphere(transform.position, sensingRange);

        foreach (Collider collider in objectsDetected)
        {
            if (collider.CompareTag(thingWanted))
            {
                return collider.transform;

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

        if (target)
        {
            targetUI.text = "Target: " + target.name;
        }
        else
        {
            targetUI.text = "Target: None";

        }

    }
    void OnDrawGizmos()
    {
        // Configura el color del radar (por ejemplo, semi-transparente y rojo)
        Gizmos.color = new Color(1f, 0f, 0f, 0.5f);  // Rojo con algo de transparencia

        // Dibuja una esfera en la posición del objeto (el radar de 15m)
        Gizmos.DrawSphere(transform.position, sensingRange);
    }
    
    private void SearchForResource(string resource)
    {
        Transform thingWanted = SensingEnvironment(resource);
        if (thingWanted)
        {
            actionDoing = resource.Equals("Water") ? CreatureActions.GoingToWater : CreatureActions.GoingToFood;
            target = thingWanted;
            targetUI.text = target.name.ToString();

        }
        else
        {
            target = null;
            actionDoing = CreatureActions.WalkingDesperately;

        }

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
        switch(actionDoing)
        {
            case CreatureActions.GoingToWater:
                if (AreNear(target, 1.5f))
                {
                    actionDoing = CreatureActions.Drinking;
                    agent.ResetPath();

                    StartCoroutine(DrinkingCooldown(3f));

                  
                }
                else
                {
                    MoveToTarget(target);

                }
                break;

            case CreatureActions.GoingToFood:

                if(AreNear(target, 2f))
                {
                    actionDoing = CreatureActions.Eating;
                    agent.ResetPath();

                    StartCoroutine(EatingCooldown(3f));

                    sated = 100;
                    target = null;
                }
                MoveToTarget(target);
                break;

            case CreatureActions.WalkingDesperately:
                if (agent.remainingDistance < 0.5f)
                {
                    agent.ResetPath();
                    //currentRandomPoint = Vector3.zero;
                }
                MoveToRandomPoint();
                //agent.Move(desperateDirection * Time.deltaTime * speed);
                //ChooseNextAction(); // quiero que si es que tiene hambre o sed y encuentra algo que vaya hacia el
                break;
            case CreatureActions.Cloning:
                Reproduce();
                break;
            case CreatureActions.Exploring:
                if (agent.remainingDistance < 0.5f)
                {
                    agent.ResetPath();
                    //currentRandomPoint = Vector3.zero;
                }
                MoveToRandomPoint();
                //ChooseNextAction();
                break;
        }

    }
    public void MoveToTarget(Transform target)
    {
        //Vector3 targetPositionIgnoringY = new Vector3(target.position.x, transform.position.y, target.position.z);
        //Vector3 dir = (targetPositionIgnoringY - transform.position).normalized;

        if (agent.hasPath)
            return;

        agent.SetDestination(target.position);
        //agent.Move(dir * speed * Time.deltaTime);


        //transform.position = Vector3.MoveTowards(
        //    transform.position,
        //    targetPositionIgnoringY,
        //    speed * Time.deltaTime
        //);


        //currentDirection = (targetPositionIgnoringY - transform.position).normalized;
    }
    public void MoveDesesperatly()
    {
        if (agent.hasPath)
            return;

        Vector3 dir = Vector3.forward;
        agent.SetDestination(dir);

    }
    public void MoveToRandomPoint()
    {
        Vector3 randomPoint;

        if (agent.hasPath)
            return;

        if(GetRandomPoint(out randomPoint))
        {
        
            agent.SetDestination(randomPoint);

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


    public bool AreNear(Transform objectToCheck, float range)
    {

        return System.Math.Abs(this.transform.position.x - objectToCheck.position.x) <= range && System.Math.Abs(this.transform.position.z - objectToCheck.position.z) <= range;
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
