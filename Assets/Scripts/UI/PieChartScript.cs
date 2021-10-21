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
    public void SetValues(float[] values) {
        float total = values.Sum();
        float currSum = 0;

        for (int i = 0; i < values.Length; ++i) {
            currSum += values[i];
            _pieLayers[i].fillAmount = currSum/total;
        }
    }
}
