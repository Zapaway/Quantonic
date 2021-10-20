using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PieChartScript : MonoBehaviour
{
    [SerializeField] private Image[] _pieLayers;
    private float[] _values;
    public float[] Values {
        get => Values;
        set {
            // automatically change the pie chart to reflect the changes in value
            
        }
    }
}
