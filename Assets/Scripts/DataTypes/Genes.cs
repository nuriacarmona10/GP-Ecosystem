using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genes
{
    public int inventorySlots;
    

    public Genes( int inventorySlots )
    {
        this.inventorySlots = inventorySlots;
    }
    public static Genes RandomGenes()
    {
        float randomReproductionHunger = Random.Range( 0, 50 );
        int randomInventorySlots = Random.Range( 1, 4 );

        return new Genes(randomInventorySlots);
    }
}
