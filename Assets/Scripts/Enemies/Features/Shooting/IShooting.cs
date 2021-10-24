using UnityEngine;
using Cysharp.Threading.Tasks;

/// <summary>
/// It can shoot.
/// </summary>
internal interface IShooting
{
    /// <summary>
    /// For use in AI-related methods.
    /// </summary>
    UniTask ShootAction(GameObject gameObject);
}
