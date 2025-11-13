using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tree : LivingEntity
{
    // Start is called before the first frame update
    public override void Init(LivingEntity mother = null)
    {
        specie = Specie.Tree;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
