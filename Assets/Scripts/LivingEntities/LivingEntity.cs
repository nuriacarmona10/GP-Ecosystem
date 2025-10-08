using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    protected Specie specie;
    
    public virtual void Init()
    {

    }

    protected virtual void Die ()
    {
        Destroy(gameObject);
    }
}
