using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Cosita : LivingEntity
{
    
    // Propiedades básicas
    [SerializeField] public float health;     // Salud de la cosita
    [SerializeField] public float speed;
    [SerializeField] public int hydrated;
    [SerializeField] public int sated;
    [SerializeField] public float sensingRange;

    public Transform target;
    Vector3 desperateDirection;
    BoxCollider boxColliderCosita;


    ///UI
    public HUDBar hungerBar;

    public TMP_Text targetUI;
    public TMP_Text goalUI;
    public TMP_Text ThirstyUI;
    //public TMP_Text satedUI;
    CreatureActions actionDoing = CreatureActions.Idle;

    //DEBUG
    public TMP_Text debugUI;
    int cont = 0;


    private float coolDownActionChoice= 5.0f;  // The cooldown duration (e.g., 5 second)
    private float currentCoolDownActionChoice = 0.0f;  // The current cooldown time left
    private bool isDoingAction = false;

   

    public void Init()
    {
        //Debug.Log("INIT COSITA");
        specie = Specie.Cosita;
        target = null;
        boxColliderCosita = GetComponent<BoxCollider>();
        hungerBar.SetMaxValue(sated);
        UpdateUICosita();
       

        //ThirstyUI.text = "Thirsty: " + hydrated.ToString();
        ChooseNextAction();



    }
    // Update is called once per frame
    void Update()
    {
        // Increase hunger and thirst over time
        //sated -= Time.deltaTime * 1 / timeToDeathByHungerAndThirsty;
        //hydrated -= Time.deltaTime * 1 / timeToDeathByHungerAndThirsty;

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
        if(hydrated < 0)
        {
            Die();
            Debug.Log("Me MORIII");
        }
        
    }

  
    IEnumerator ActionCooldown()
    {
        
                // Inicia la acción
                isDoingAction = true;
                // Espera 5 segundos antes de permitir la siguiente acción
                yield return new WaitForSeconds(5f);
                isDoingAction = false;
       
            

           
    }
    public void ChooseNextAction()
    {

        if (hydrated < 50f && !target)
        {
            FindWater();
        }
        else
        {
            Walk();
        }
        UpdateUICosita();
        StartCoroutine(ActionCooldown());
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

    private void FindWater()
    {
        actionDoing = CreatureActions.LookingForWater;
        debugUI.text = "Entrando numero: " + cont;

        Transform thingWanted = SensingEnvironment("Water");

        if (thingWanted)
        {
            target = thingWanted;
            targetUI.text = target.name.ToString();


        }
        else
        {
            Walk();
        }
        //else // si no esta en mi rango de visión
        //{
        //    //Aqui tengo que poner quiza otro comportamiento en vez de Walk randomly, quiza andar en linea recta.
        //    //Pero este blucle de ejecúción no está muy bien porque no tiene mucha lógica.
        //    Walk();
        //}
    }
    private void FindFood()
    {
        actionDoing = CreatureActions.LookingForFood;

    }
    public Vector3 GetRandomDirection()
    {
        //Elijo aleatoriamente una dirección en la x y z
        int randomDir = Random.Range(0, 3); // 0: UP, 1: LEFT, 2: DOWN, 3: RIGHT:
        switch (randomDir)
        {
            case 0: return new Vector3 (0,0,1);
            case 1: return new Vector3 (-1,0,0);
            case 2: return new Vector3 (0,0,-1);
            case 3: return new Vector3 (1,0,1);
            default: return Vector3.zero;

        }

    }

    public void Walk()
    {
        hydrated = hydrated - 5;
        sated = sated - 5;
        cont++;

        if (!target && actionDoing == CreatureActions.LookingForWater) // I didnt see water in my range of view and im thirsty
        {
            //choose one direction and go in the same direction until I see water or I die
            if (desperateDirection == Vector3.zero)
            {
                desperateDirection = GetRandomDirection();

            }

            transform.position += desperateDirection * speed; // Moving the object multiplying the dir by the speed



        }
        else if (!target)
        {  // I only walk without direcction

            desperateDirection = Vector3.zero; // Now I have a target and Im not desperate any more
            actionDoing = CreatureActions.Exploring;

            //Vector3 RandomTargetMov = new Vector3(Random.Range(-2f, 2f), 0.25f, Random.Range(2f, 2f));
            //transform.position = transform.position + RandomTargetMov;

            Vector3 direction = GetRandomDirection(); // Obtener la dirección aleatoria
            transform.position += direction * speed; // Mover el objeto multiplicando la dirección por la velocidad
        }

        else if (target)  // I have a target 
        {
            Vector3 targetPositionIgnoringY = new Vector3 ( target.position.x, transform.position.y, target.position.z );
            // Mover hacia el target
            transform.position = Vector3.MoveTowards(transform.position, targetPositionIgnoringY, speed);

            // Calcular la distancia en X y Z sin signo
            //float distanceX = Mathf.Abs(transform.position.x - target.position.x);
            //float distanceZ = Mathf.Abs(transform.position.z - target.position.z);

            float radius = Mathf.Max(boxColliderCosita.size.x, boxColliderCosita.size.y, boxColliderCosita.size.z) / 2f;
            Collider[] objectsDetected = Physics.OverlapSphere(transform.position, radius);

            foreach (Collider collider in objectsDetected)
            {
                if (collider.CompareTag(target.gameObject.tag))
                {
                    Debug.Log("BEBIENDO AGUA");

                    hydrated = 100;
                    target = null;
                    actionDoing = CreatureActions.Drinking;
                    break;
                }
            }


            // Verificar si la distancia es menor o igual a 1 en ambos ejes
            //if (distanceX <= 1f && distanceZ <= 1f)
            //{
            //    Debug.Log("BEBIENDO AGUA");
               
            //    hydrated = 100f;
            //    target = null;
            //    actionDoing = CreatureActions.Drinking;
            //    UpdateUICosita();
            //}
            

            //transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed);
            // Calcula el valor 't' entre 0 y 1
            //float t = Mathf.Clamp01(timeElapsed / duration);

            // Interpola linealmente la posición del objeto hacia el objetivo
            // transform.position = Vector3.Lerp(transform.position, target.position, t);
            // interpolar paso a paso para que cosita vaya a su target;
        }

            

       


    }
}
