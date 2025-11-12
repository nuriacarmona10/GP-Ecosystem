using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genes
{
    public float reproductionHunger;
    public int inventorySlots;

    public Genes(float reproductionHunger, int inventorySlots )
    {
        this.reproductionHunger = reproductionHunger;
        this.inventorySlots = inventorySlots;
    }
    public Genes RandomGenes()
    {
        float randomReproductionHunger = Random.Range( 0, 100 );
        int randomInventorySlots = Random.Range( 0, 6 );

        return new Genes(reproductionHunger, randomInventorySlots);
    }
}
