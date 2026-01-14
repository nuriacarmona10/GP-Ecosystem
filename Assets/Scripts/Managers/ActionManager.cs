using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation.Samples;
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



        //cosita = GetComponent<Cosita>();
        actionList = new List<Action>
        {
            //Priorities makes no sense, I can use their position in the Array to determine the priority but maybe in the future I can calculate it depending on the context of cosita
            new Action(CreatureActions.Drinking, 1, () => cosita.hydrated < 50 && cosita.resourceTarget != null && cosita.resourceTarget.ResourceType == "Water" &&
                                                    cosita.AreNear(cosita.resourceTarget.ResourceGameObject, cosita.resourceTarget.InteractionDistance)),
            new Action(CreatureActions.Eating, 2, () =>  cosita.sated < 50 && cosita.inventoryList.Count > 0 ),
            //new Action(CreatureActions.LookingForWater, 6, () => ( (cosita.hydrated <30 || cosita.sated <30) && cosita.resourceTarget == null) || (cosita.hydrated < 30 && cosita.resourceTarget != null && cosita.resourceTarget.ResourceType == "Food") || (cosita.sated<30 && cosita.resourceTarget != null && cosita.resourceTarget.ResourceType == "Water" ) ),
            new Action(CreatureActions.Cloning,3, () =>  cosita.reproductionHunger > 80 ),

            new Action(CreatureActions.AddingFoodToInventory, 5, () =>
                                        cosita.inventoryList.Count < cosita.genes.inventorySlots &&
                                        cosita.resourceTarget != null &&
                                        cosita.resourceTarget.ResourceGameObject != null &&  // Verifica que el ResourceGameObject no esté destruido
                                         cosita.resourceTarget.ResourceType == "Food" && 
                                        cosita.AreNear(cosita.resourceTarget.ResourceGameObject, cosita.resourceTarget.InteractionDistance)
                                        ),
            new Action(CreatureActions.GoingToWater, 6,() => cosita.hydrated < 60 && cosita.resourceTarget != null && cosita.resourceTarget.ResourceType == "Water"),
            new Action(CreatureActions.GoingToFood, 7, () => cosita.sated < 75 && cosita.resourceTarget != null && cosita.resourceTarget.ResourceGameObject != null
                                                            && cosita.resourceTarget.ResourceType == "Food" && cosita.inventoryList.Count < cosita.genes.inventorySlots),
            //new Action(CreatureActions.Sharing, 4, () => cosita.inventoryList.Count > cosita.genes.inventorySlots/2  && cosita.neighbourCositaInNeed != null && cosita.sated>60 && cosita.hydrated>60
                                                    //&& cosita.AreNear(cosita.neighbourCositaInNeed.gameObject, cosita.neighbourCositaInNeed.interactionBetweenCositasRange )), // has to have at least half of his inventory full
           // new Action(CreatureActions.GoingToNeighbour, 8, () => cosita.neighbourCositaInNeed != null && cosita.sated > 60 && cosita.hydrated>60 && cosita.inventoryList.Count > cosita.genes.inventorySlots/2  ),
            new Action(CreatureActions.Exploring, 9, () => true)
        }; 
    }

    public void ExecuteAction()
    {
        foreach (var action in actionList)
        {
            if (action == null)
                return;

           

            if (action.condition())
            {

                switch (action.action)
                {
                    case CreatureActions.GoingToWater:
                        cosita.actionDoing = CreatureActions.GoingToWater;
                        cosita.MoveToTarget(cosita.resourceTarget.ResourceGameObject.transform.position);

                        return;

                    case CreatureActions.GoingToFood:

                        cosita.actionDoing = CreatureActions.GoingToFood;
                        cosita.MoveToTarget(cosita.resourceTarget.ResourceGameObject.transform.position);

                        return;

                    case CreatureActions.Drinking:

                        cosita.isBusy = true;
                        cosita.actionDoing = CreatureActions.Drinking;
                        cosita.Agent.ResetPath();
                        StartCoroutine(cosita.ConsumingResourceCooldown(cosita.resourceTarget));
                        cosita.resourceTarget = null;


                        return;
                    

                    case CreatureActions.AddingFoodToInventory:

                        cosita.actionDoing = CreatureActions.AddingFoodToInventory;
                        cosita.AddResourceToInventory(cosita.resourceTarget);
                        cosita.Agent.ResetPath();

                        return;

                  
                    case CreatureActions.GoingToNeighbour:

                        Debug.Log("TENGO GANAS DE COMPARTIR");

                        cosita.actionDoing = CreatureActions.GoingToNeighbour;
                        //if (cosita.Agent.remainingDistance < 0.5f)
                        //{
                        //    cosita.Agent.ResetPath();
                        //}
                        cosita.MoveToTarget(cosita.neighbourCositaInNeed.transform.position);

                        return;

                    case CreatureActions.Sharing:
                        cosita.actionDoing = CreatureActions.Sharing;
                        
                        if(cosita.inventoryList[cosita.inventoryList.Count - 1] != null)
                        {
                            IResource resourceToshare = cosita.inventoryList[cosita.inventoryList.Count - 1];
                            cosita.ToShare(resourceToshare);
                        }
                        
                        //cosita.neighbourCositaInNeed = null;
                        Debug.Log("ESTOY COMPARTIENDO");

                        return;

                    case CreatureActions.Eating:


                        cosita.actionDoing = CreatureActions.Eating;

                        //Debug.Log("Lo he hecho");
                        Apple apple = cosita.inventoryList[0] as Apple; // I grab first apple


                        cosita.isBusy = true;
                        //cosita.Agent.ResetPath();
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

                   
                    case CreatureActions.Cloning:
                        cosita.actionDoing = CreatureActions.Cloning;
                        cosita.Reproduce();
                        return;
                    case CreatureActions.Exploring:
                        cosita.actionDoing = CreatureActions.Exploring;
                        if (!cosita.Agent.hasPath)
                        {
                            cosita.MoveToRandomPoint();

                        }
                       


                            //if (cosita.Agent.remainingDistance < 0.5f)
                            //{
                            //    cosita.Agent.ResetPath();
                            //    //currentRandomPoint = Vector3.zero;
                            //}
                            //ChooseNextAction();
                            return;

                   
                }
            }
        }
    }
}
