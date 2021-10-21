using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIScripts.QDP {
    /// <summary>
    /// Script for interactions with the qubit display panel (QDP).
    /// </summary>
    public sealed class QDPScript : MonoBehaviour
    {
        // qubit desc
        [SerializeField] private RawImage _qubitRepRawImage;
        [SerializeField] private TextMeshProUGUI _qubitIndexText;
        [SerializeField] private TextMeshProUGUI _qubitStateText;

        // probabilites 
        [SerializeField] private PieChartScript _pieChart;
        [SerializeField] private TextMeshProUGUI _groundStateProbText;
        [SerializeField] private TextMeshProUGUI _excitedStateProbText;

        
    }
}

