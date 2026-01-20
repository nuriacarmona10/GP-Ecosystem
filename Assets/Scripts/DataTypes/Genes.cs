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
        timeToGrow = 60;

        if (motherGenes!=null) // Cosita has a mom
        {
            this.inventorySlots = motherGenes.inventorySlots;
            MutateInventorySlot();

        }
        else  // random genes
        {
            this.inventorySlots = Random.Range(1, 4);
        }

        UpdateGenColor();

    }

    public void UpdateGenColor()
    {
        genColor = Color.red;

        if (this.inventorySlots == 2)
        {
            genColor = Color.green;
        }
        else if (this.inventorySlots == 3)
        {
            genColor = Color.blue;

        }
        else if(this.inventorySlots == 4)
        {
            genColor = Color.yellow;
        }
    }

    public void MutateInventorySlot()
    {
        // Probabilidad de mutación (por ejemplo, 10%)
        float mutationChance = 0.1f;  // 10% de probabilidad de mutar
        float mutationDecision = Random.Range(0f, 1f);  // Generamos un número entre 0 y 1

        if (mutationDecision < mutationChance)
        {
            // Decidimos si añadir o quitar un slot
            int mutationType = Random.Range(0, 2);  // 0 para quitar, 1 para añadir

            if (mutationType == 0 && inventorySlots > 1)  // No se puede quitar si ya es 1
            {
                inventorySlots--;  // Reducimos un slot
            }
            else if (mutationType == 1)  // No se puede añadir si ya es 3
            {
                inventorySlots++;  // Añadimos un slot
            }

            // Actualizar genColor dependiendo del nuevo número de slots
            UpdateGenColor();
        }
    }

}
