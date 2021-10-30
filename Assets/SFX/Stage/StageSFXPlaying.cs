using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class StageSFXPlaying : MonoBehaviour
{
    [SerializeField] private AudioSource _jumpingSFX;
    [SerializeField] private AudioSource _pickupSFX;
    [SerializeField] private AudioSource _waveShootSFX;
    [SerializeField] private AudioSource _explosionSFX; 
    [SerializeField] private AudioSource _blipSelectSFX;
    [SerializeField] private AudioSource _checkpointActivatedSFX;
    [SerializeField] private AudioSource _enemyExplosionSFX; 

    public void PlayJumpSFX() {
        _jumpingSFX.Play();
    }

    public void PlayPickupSFX() {
        _pickupSFX.Play();
    }

    public void PlayWaveShootSFX() {
        _waveShootSFX.Play();
    }

    public void PlayExplosionSFX() {
        _explosionSFX.Play();
    }

    public void PlayBlipSelectSFX() {
        _blipSelectSFX.Play();
    }

    public void PlayCheckpointTouchedSFX() {
        _checkpointActivatedSFX.Play();
    }

    public void PlayEnemyExplosionSFX() {
        _enemyExplosionSFX.Play();
    }
}
