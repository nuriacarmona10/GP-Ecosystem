using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    public Specie specie;

    

    public virtual void Init(LivingEntity mother = null)
    {

    }

    protected virtual void Die ()
    {
        EcosystemManager.Instance.HandleEntityDeath(this);
        Destroy(gameObject);
    }
}
