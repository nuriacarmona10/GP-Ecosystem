using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour
{
    protected Specie specie;
    

    protected virtual void Die ()
    {
        Destroy(gameObject);
    }
}
