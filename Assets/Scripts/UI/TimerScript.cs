using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TimerScript : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _timerText;
    private float _seconds = 10;

    private void Update() {
        _seconds -= Time.deltaTime;
        _timerText.text = $"{Mathf.FloorToInt(_seconds)} µs";
    }
}
