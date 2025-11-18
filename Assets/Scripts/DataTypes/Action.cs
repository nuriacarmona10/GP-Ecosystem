using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action 
{
    public CreatureActions action;
    public int priority;
    public Func<bool> condition;



    public Action (CreatureActions act, int pri, Func<bool> cond )
    {
        action = act;
        priority = pri;
        condition = cond;
    }
}
