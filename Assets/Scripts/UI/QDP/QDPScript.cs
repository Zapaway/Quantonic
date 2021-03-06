using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UIScripts.QDP {
    /// <summary>
    /// Script for interactions with the qubit display panel (QDP).
    /// </summary>
    public sealed class QDPScript : Panel
    {
        // qubit desc
        [SerializeField] private RawImage _qubitRepRawImage;
        [SerializeField] private TextMeshProUGUI _qubitIndexText;
        [SerializeField] private TextMeshProUGUI _qubitStateText;

        // probabilites 
        [SerializeField] private PieChartScript _pieChart;
        [SerializeField] private TextMeshProUGUI _groundStateProbText;
        [SerializeField] private TextMeshProUGUI _excitedStateProbText;

        #region Getters and Setters
        /// <summary>
        /// Set the entire QDP.
        /// </summary>
        public void SetQDP(Texture qubitTexture, int qubitIndex, string qubitState, double groundStateProb, double excitedStateProb, Func<double, string> stringFunc = null) {
            _qubitRepRawImage.texture = qubitTexture;
            _qubitIndexText.SetText($"[{qubitIndex}]");
            _qubitStateText.SetText(qubitState);
            _updateProbabilities(groundStateProb, excitedStateProb, stringFunc);
        } 
        #endregion Getters and Setters

        #region Helpers
        private void _updateProbabilities(double groundStateProb, double excitedStateProb, Func<double, string> stringFunc = null) {
            Func<double, string> doubleToString = stringFunc ?? delegate (double x) { return $"{x}%"; };

            _groundStateProbText.SetText("0: " + doubleToString(groundStateProb));
            _excitedStateProbText.SetText("1: " + doubleToString(excitedStateProb));
            
            if (groundStateProb < 0) groundStateProb = 0;
            if (excitedStateProb < 0) excitedStateProb = 0;
            _pieChart.SetValues(new double[] {groundStateProb, excitedStateProb});
        }
        #endregion Helpers
    }
}

