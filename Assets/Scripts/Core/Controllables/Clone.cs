using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;

using Managers;

public sealed class Clone : Controllable
{
    protected override void Awake()
    {
        base.Awake();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        SoundManager.Instance.StageSounds.PlayExplosionSFX();
        _clear();
    }
}
