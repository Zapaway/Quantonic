using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class PieChartScript : MonoBehaviour
{
    [SerializeField] private Image[] _pieLayers;

    /// <summary>
    /// Automatically change the pie chart to reflect the changes in value.
    /// </summary>
    public void SetValues(double[] values) {
        double total = values.Sum();
        double currSum = 0;

        for (int i = 0; i < values.Length; ++i) {
            if (values[i] == 0) {
                _pieLayers[i].fillAmount = 0;
                continue;
            }

            currSum += values[i];
            _pieLayers[i].fillAmount = (float)(currSum/total);
        }
    }
}
