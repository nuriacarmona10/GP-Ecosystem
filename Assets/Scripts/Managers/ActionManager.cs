using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.UIElements.Experimental;

public class ActionManager : MonoBehaviour
{
    public List<Action> actionList;
    private Cosita cosita;

    public void Start()
    {
        cosita = GetComponent<Cosita>();

        if(cosita == null )
        {
            Debug.Log("Mi cosita es null");
        }
       
        //cosita = GetComponent<Cosita>();
        actionList = new List<Action>
        {
            //Priorities makes no sense, I can use their position in the Array to determine the priority but maybe in the future I can calculate it depending on the context of cosita
            new Action(CreatureActions.Drinking, 1, () => cosita.hydrated < 50 && cosita.resourceTarget != null && cosita.AreNear(cosita.resourceTarget.ResourceGameObject, cosita.resourceTarget.InteractionDistance)),
            new Action(CreatureActions.Eating, 2, () =>  cosita.sated < 30 && cosita.inventoryList.Count > 0 ),
            new Action(CreatureActions.AddingFoodToInventory, 4, () => cosita.inventoryList.Count < cosita.genes.inventorySlots && cosita.resourceTarget != null && // tengo problemas porque se acceden a objetos una vez están ya borrados
                                                                      cosita.resourceTarget.ResourceGameObject != null &&
                                                                      cosita.AreNear(cosita.resourceTarget.ResourceGameObject, cosita.resourceTarget.InteractionDistance)){
            },
            new Action(CreatureActions.GoingToWater, 5,() => cosita.hydrated < 50 && cosita.resourceTarget != null),
            new Action(CreatureActions.GoingToFood, 6, () => cosita.sated < 60 && cosita.resourceTarget != null && cosita.inventoryList.Count < cosita.genes.inventorySlots),
            new Action(CreatureActions.Exploring,7, () => true)
        }; 
    }

    public void ExecuteAction()
    {
        foreach (var action in actionList)
        {
            if (action == null)
                return;

            //Debug.Log("Esta es la primera accion"+ action.action.ToString());
            Debug.Log("Estoy esta acttion" + action.action.ToString());

            if (cosita.resourceTarget != null)
            {
                Debug.Log("Esta es mi resource target" + cosita.resourceTarget);
                Debug.Log("Esta es mi gameobject" + cosita.resourceTarget.ResourceGameObject);
            }
            else
                Debug.Log("Estamos jodidos nene");


            if (action.condition())
            {
                Debug.Log("Accion que es true es: " + action.action.ToString());

                switch (action.action)
                {
                    case CreatureActions.GoingToWater:
                        cosita.actionDoing = CreatureActions.GoingToWater;
                        cosita.MoveToTarget(cosita.resourceTarget.ResourceGameObject);

                        return;

                    case CreatureActions.GoingToFood:

                        cosita.actionDoing = CreatureActions.GoingToFood;
                        cosita.MoveToTarget(cosita.resourceTarget.ResourceGameObject);

                        return;

                    case CreatureActions.Drinking:

                        cosita.isBusy = true;
                        cosita.actionDoing = CreatureActions.Drinking;
                        cosita.Agent.ResetPath();
                        StartCoroutine(cosita.ConsumingResourceCooldown(cosita.resourceTarget));
                        cosita.resourceTarget = null;


                        return;

                    case CreatureActions.AddingFoodToInventory:
                        cosita.isBusy = true;
                        cosita.actionDoing = CreatureActions.AddingFoodToInventory;
                        cosita.AddResourceToInventory(cosita.resourceTarget);
                        StartCoroutine(cosita.ConsumingResourceCooldown(cosita.resourceTarget));
                        cosita.resourceTarget.DestroyGameobject();
                        cosita.resourceTarget = null;

                        cosita.Agent.ResetPath();

                        return;

                    case CreatureActions.Eating:


                        cosita.actionDoing = CreatureActions.Eating;

                        //Debug.Log("Lo he hecho");
                        Apple apple = cosita.inventoryList[0] as Apple; // I grab first apple


                        cosita.isBusy = true;
                        StartCoroutine(cosita.ConsumingResourceCooldown(apple as IResource));
                        //apple.Consume();
                        //Debug.Log("Quito la manzanita de mi inventario porque me la comi");
                        cosita.inventoryList.Remove(apple);
                        //Debug.Log("Tengo estos hijos" + cosita.inventorySlotUI.transform.childCount.ToString());
                        //debugUI.text = inventorySlotUI.transform.childCount.ToString();

                        for (int i = cosita.inventorySlotUI.transform.childCount - 1; i >= 0; i--) // empiezo de atrás a delante
                        {
                            Transform child = cosita.inventorySlotUI.transform.GetChild(i);

                            // Verifica si el hijo tiene hijos
                            if (child.childCount > 0)
                            {
                                Destroy(child.GetChild(0).gameObject);
                                return;
                            }
                        }

                        return;

                    case CreatureActions.WalkingDesperately:
                        if (cosita.Agent.remainingDistance < 0.5f)
                        {
                            cosita.Agent.ResetPath();
                            //currentRandomPoint = Vector3.zero;
                        }
                        cosita.MoveToRandomPoint();
                        //Agent.Move(desperateDirection * Time.deltaTime * speed);
                        //ChooseNextAction(); // quiero que si es que tiene hambre o sed y encuentra algo que vaya hacia el
                        return;
                    case CreatureActions.Cloning:
                        cosita.actionDoing = CreatureActions.Cloning;
                        cosita.Reproduce();
                        return;
                    case CreatureActions.Exploring:
                        cosita.actionDoing = CreatureActions.Exploring;

                        if (cosita.Agent.remainingDistance < 0.5f)
                        {
                            cosita.Agent.ResetPath();
                            //currentRandomPoint = Vector3.zero;
                        }
                        cosita.MoveToRandomPoint();
                        //ChooseNextAction();
                        return;
                }
            }
        }
    }
}
