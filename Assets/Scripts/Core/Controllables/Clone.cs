using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

public sealed class Clone : Controllable
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override async UniTask Start()
    {
        await base.Start();
    }
}
