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

    public Transform target;
    Vector3 desperateDirection;
    public Vector3 currentDirection; // Esto seguramente borrar junto con los metodos viejos de Walk y del movimiento de cosita en general
    public Vector3 currentRandomPoint;
    public Transform currentRandomPositionGoing;

    BoxCollider boxColliderCosita;
    private float walkingTimer;              // Contador de tiempo
  



    ///UI

    public HUDBar hungerBar;
    public HUDBar waterBar;
    public TMP_Text goalUI;


    public TMP_Text targetUI;
    public TMP_Text ThirstyUI;
    //public TMP_Text satedUI;
    CreatureActions actionDoing = CreatureActions.Idle;

    //DEBUG
    public TMP_Text debugUI;


    private float coolDownActionChoice= 5.0f;  // The cooldown duration (e.g., 5 second)
    private float currentCoolDownActionChoice = 0.0f;  // The current cooldown time left
    private bool isDoingAction = false;

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
        currentDirection = transform.forward;
        currentRandomPoint = Vector3.zero;
        UpdateUICosita();

        //ThirstyUI.text = "Thirsty: " + hydrated.ToString();
        //ChooseNextAction();



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
        if (!isDoingAction)
        {
            ChooseNextAction();


        }
        else
        {
            Act();
            UpdateUICosita();
        }
        if (hydrated <= 0 || sated <= 0)
        {
            Die();
            Debug.Log("Me MORIII");
        }
        
    }

  
    IEnumerator ActionCooldown()
    {
        
                
                // Espera 5 segundos antes de permitir la siguiente acción

                yield return new WaitForSeconds(5f);
                isDoingAction = false;
       
            

           
    }
    public void ChooseNextAction()
    {
        // Inicia la acción
        isDoingAction = true; // maybe remove this

        //if busy then return 

        if (hydrated < 50f )
        {
            SearchForResource("Water");
        }
        else if ( sated < 50f )
        {
            SearchForResource("Food");

        }
        else
        {
            actionDoing = CreatureActions.Exploring;
        }
        

        
        //StartCoroutine(ActionCooldown());
    }

    public Transform SensingEnvironment(string thingWanted) // I pass a string with the name of the tag to the method
    {
        Collider[] objectsDetected = Physics.OverlapSphere(transform.position, sensingRange);

        foreach (Collider collider in objectsDetected)
        {
            Debug.Log("COLISIONO CON:" +  collider.gameObject.name);
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
            actionDoing = CreatureActions.WalkingDesperately;

        }

    }

    //private void FindWater()
    //{

    //    Transform thingWanted = SensingEnvironment("Water");

    //    if (thingWanted)
    //    {
    //        target = thingWanted;
    //        targetUI.text = target.name.ToString();
    //        actionDoing = CreatureActions.GoingToWater;


    //    }
    //    else
    //    {
    //        actionDoing = CreatureActions.WalkingDesperately;

    //    }


    //}
    //private void FindFood()
    //{
    //    Transform thingWanted = SensingEnvironment("Food");
    //    if (thingWanted)
    //    {
    //        target = thingWanted;
    //        targetUI.text = target.name.ToString();
    //        actionDoing = CreatureActions.GoingToFood;


    //    }
    //    else
    //    {
    //        actionDoing = CreatureActions.WalkingDesperately;

    //    }


    //}
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
                    hydrated = 100;
                    StartCoroutine(ActionCooldown());
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
                    sated = 100;
                    StartCoroutine(ActionCooldown());
                }
                MoveToTarget(target);
                break;

            case CreatureActions.WalkingDesperately:
                if (desperateDirection == Vector3.zero)
                {
                    desperateDirection = GetRandomDirection();

                }
                transform.Translate(desperateDirection * Time.deltaTime * speed);
                ChooseNextAction(); // quiero que si es que tiene hambre o sed y encuentra algo que vaya hacia el
                break;

            case CreatureActions.Exploring:
                if (agent.remainingDistance < 0.5f)
                {
                    agent.ResetPath();
                }
                MoveToRandomPoint();
                break;
        }

    }
    public void MoveToTarget(Transform target)
    {
        Vector3 targetPositionIgnoringY = new Vector3(target.position.x, transform.position.y, target.position.z);

        transform.position = Vector3.MoveTowards(
            transform.position,
            targetPositionIgnoringY,
            speed * Time.deltaTime
        );


        //currentDirection = (targetPositionIgnoringY - transform.position).normalized;
    }
    public void MoveToRandomPoint()
    {
        
        if(agent.hasPath)
        {
            currentRandomPoint = GetRandomPoint();
        }
        
         agent.SetDestination(currentRandomPoint);
        

       
    }
    public Vector3 GetRandomPoint()
    {
        Vector3 randomPoint = transform.position + Random.insideUnitSphere * sensingRange;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
        {
            return hit.position;
        }
        return Vector3.zero;
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

    public void Walk()
    {
        //I only walk without direcction
       
        desperateDirection = Vector3.zero; //Im not desperate any more

        transform.Translate(currentDirection * Time.deltaTime * speed);
        Debug.Log("Ando a una velocidad de:" + currentDirection * Time.deltaTime * speed);
        //transform.position += smoothedDir * speed;
        // Aumenta el contador de tiempo
        walkingTimer += Time.deltaTime;

        // Si ya pasaron 3 segundos, cambia la dirección
        if (walkingTimer >= 3f)
        {
            Vector3 randomdirection = GetRandomDirection();
            Vector3 smoothedDir = Vector3.Lerp(currentDirection, randomdirection, 0.5f).normalized;
            currentDirection = smoothedDir;
            walkingTimer = 0f; // Reinicia el contador
            isDoingAction = false;
        }


       

    }

    public bool WillHaveGround (Vector3 futurePosition)
    {
        if(Physics.Raycast(futurePosition, Vector3.down, out RaycastHit hit, 2f))
        {
            if( hit.collider.CompareTag("Ground") )
            {
                return true;
            }
        }
        return false;
    }



    //public void Walk()
    //{
    //    hydrated = hydrated - 5;
    //    sated = sated - 5;
    //    cont++;

    //    if (!target && ( actionDoing == CreatureActions.LookingForWater || actionDoing == CreatureActions.LookingForFood)) // I didnt see water in my range of view and im thirsty
    //    {
    //        //choose one direction and go in the same direction until I see water or I die
    //        if (desperateDirection == Vector3.zero)
    //        {
    //            desperateDirection = GetRandomDirection();

    //        }

    //        transform.position += desperateDirection * speed; // Moving the object multiplying the dir by the speed



    //    }

    //    else if (!target)
    //    {  // I only walk without direcction

    //        desperateDirection = Vector3.zero; // Now I have a target and Im not desperate any more
    //        actionDoing = CreatureActions.Exploring;

    //        //Vector3 RandomTargetMov = new Vector3(Random.Range(-2f, 2f), 0.25f, Random.Range(2f, 2f));
    //        //transform.position = transform.position + RandomTargetMov;

    //        Vector3 randomdirection = GetRandomDirection(); // Obtener la dirección aleatoria
    //        Vector3 smoothedDir = Vector3.Lerp(currentDirection, randomdirection, 0.3f);


    //        transform.position += smoothedDir * speed * Time.deltaTime; // Mover el objeto multiplicando la dirección por la velocidad

    //        currentDirection = smoothedDir;
    //    }

    //    else if (target)  // I have a target 
    //    {
    //        Vector3 targetPositionIgnoringY = new Vector3(target.position.x, transform.position.y, target.position.z);

    //        transform.position = Vector3.MoveTowards(
    //            transform.position,
    //            targetPositionIgnoringY,
    //            speed
    //        );
    //        currentDirection = (targetPositionIgnoringY - transform.position).normalized;


    //        // Calcular la distancia en X y Z sin signo
    //        //float distanceX = Mathf.Abs(transform.position.x - target.position.x);
    //        //float distanceZ = Mathf.Abs(transform.position.z - target.position.z);

    //        float radius = Mathf.Max(boxColliderCosita.size.x, boxColliderCosita.size.y, boxColliderCosita.size.z) / 2f;
    //        Collider[] objectsDetected = Physics.OverlapSphere(transform.position, radius);

    //        foreach (Collider collider in objectsDetected)
    //        {
    //            if (collider.CompareTag(target.gameObject.tag))
    //            {
    //                if (target.gameObject.tag.Equals("Water"))
    //                {
    //                    actionDoing = CreatureActions.Drinking;
    //                    hydrated = 100;

    //                }
    //                else if (target.gameObject.tag.Equals("Food"))
    //                {
    //                    actionDoing = CreatureActions.Eating;
    //                    sated = 100;
    //                }

    //                target = null;
    //                break;
    //            }
    //        }


    //        // Verificar si la distancia es menor o igual a 1 en ambos ejes
    //        //if (distanceX <= 1f && distanceZ <= 1f)
    //        //{
    //        //    Debug.Log("BEBIENDO AGUA");

    //        //    hydrated = 100f;
    //        //    target = null;
    //        //    actionDoing = CreatureActions.Drinking;
    //        //    UpdateUICosita();
    //        //}


    //        //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed);
    //        // Calcula el valor 't' entre 0 y 1
    //        //float t = Mathf.Clamp01(timeElapsed / duration);

    //        // Interpola linealmente la posición del objeto hacia el objetivo
    //        // transform.position = Vector3.Lerp(transform.position, target.position, t);
    //        // interpolar paso a paso para que cosita vaya a su target;
    //    }






    //}
}
