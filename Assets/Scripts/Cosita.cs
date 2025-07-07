using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using TMPro;

public class Cosita : LivingEntity
{
    // Propiedades básicas
    public float health = 100f;     // Salud de la cosita
    public float speed = 1.0f;
    public float hydrated = 100f;
    public float sated = 100f;
    public TMP_Text goalUI;
    public TMP_Text ThirstyUI;


    private float coolDownWalk = 5.0f;  // The cooldown duration (e.g., 5 second)
    private float currentCoolDownWalk = 0.0f;  // The current cooldown time left
    private bool IsWalking = false;

   

    public void Init()
    {
        Debug.Log("INIT COSITA");
        specie = Specie.Cosita;
        ChooseNextAction();
        ThirstyUI.text = "Thirsty: " + hydrated.ToString();



    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public void ChooseNextAction()
    {
        if(hydrated < 50f)
        {
            FindWater();
        }
        else
        {
            Walk();
        }

    }

    private void FindWater()
    {
        goalUI.text = "Finding Water";
    }

    public void Walk()
    {
        goalUI.text = "Walk randomly";

        if (!IsWalking)
        {
            //Vector3 RandomTargetMov = new Vector3(Random.Range(-2f, 2f), 0.25f, Random.Range(2f, 2f));
            //transform.position = transform.position + RandomTargetMov;

            IsWalking = true;
            int randomDir = Random.Range(0, 3);
            switch (randomDir) // 0: UP, 1: LEFT, 2: DOWN, 3: RIGHT:
            {
                case 0:
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + speed);
                    break;
                case 1:
                    transform.position = new Vector3(transform.position.x - speed, transform.position.y, transform.position.z);
                    break;
                case 2:
                    transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - speed);
                    break;
                case 3:
                    transform.position = new Vector3(transform.position.x + speed, transform.position.y, transform.position.z);
                    break;

            }

            hydrated = hydrated - 10;
            ThirstyUI.text = "Thirsty: " + hydrated.ToString();




            //currentCoolDownWalk += currentCoolDownWalk * Time.deltaTime;
            //currentCoolDownWalk += Time.deltaTime;

        }
        else
        {
            Debug.Log("Walking: " + IsWalking);
            currentCoolDownWalk += Time.deltaTime;

            if (currentCoolDownWalk >= coolDownWalk)
            {
                Debug.Log("He terminado mi movimiento");
                IsWalking = false;
                currentCoolDownWalk = 0;
                ChooseNextAction();


            }


        }

    }
}
