using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// Collision/triggering does something to the object.
/// </summary>
internal interface ICollision
{
    /// <summary>
    /// For use in OnCollision2D or OnTrigger2D
    /// </summary>
    UniTask TouchedAction(GameObject gameObject);
}
