using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class MobileEnemy : Enemy
{   
    /// <summary>
    /// For each frame, this should be called.
    /// </summary>
    protected abstract void _move();
}
