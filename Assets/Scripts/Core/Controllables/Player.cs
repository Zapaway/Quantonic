using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class Player : Controllable
{
    protected override void Awake()
    {
        base.Awake();

        _addQubitFromPrefab(0);  // all players should start off with one qubit
    }
}
