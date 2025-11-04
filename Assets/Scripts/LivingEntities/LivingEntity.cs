using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    public Specie specie;

    public delegate void DeathAction(LivingEntity entity);

    public static event DeathAction OnEntityDied;

    public virtual void Init()
    {

    }

    protected virtual void Die ()
    {
        OnEntityDied?.Invoke(this);
        Destroy(gameObject);
    }
}
