using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genes
{
    public int inventorySlots;
    public Color genColor;
    public float sensingRange;
    public float timeToGrow;




    public Genes(Genes motherGenes = null)
    {
        sensingRange = 6;
        timeToGrow = 10;

        if (motherGenes!=null)
        {
            this.inventorySlots = motherGenes.inventorySlots;

        }
        else  // random genes
        {
            this.inventorySlots = Random.Range(1, 4);
        }

        genColor = Color.red;

        if (this.inventorySlots == 2)
        {
            genColor = Color.green;
        }
        else if(this.inventorySlots == 3 ) 
        {
            genColor = Color.blue;

        }

    }
    
    //public static Genes RandomGenes()
    //{
    //    
    //}

}
