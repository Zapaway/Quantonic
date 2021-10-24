using UnityEngine;
using Cysharp.Threading.Tasks;

using Managers;

[RequireComponent(typeof(WaveMoveScript))]
public sealed class ControllableWaveScript : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other) {
        if (other.CompareTag("EnemyWave")) {
            Destroy(gameObject);
            Destroy(other.gameObject);
        }
        else if (other.tag.Contains("Enemy") && other.tag.Contains("Killable")) {
            Destroy(gameObject);
            other.GetComponent<IDeath>().DeathAction();
        }
    }
}

