using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScaleDrawe : MonoBehaviour
{
    // Start is called before the first frame update

    public float MIN;
    public float MAX;
    public int NLabels;
    public TextMeshProUGUI LabelPrefab;
    private List<TextMeshProUGUI> Labels;
    public Gradient heatScaleGradient;


    void Start()
    {
        Labels = new List<TextMeshProUGUI>();

        float STEP_SIZE = (MAX - MIN) / (NLabels - 1);
        float value = MAX;
        for(int i = 0; i< NLabels; i++) 
        {
            var label = Instantiate(LabelPrefab, transform);
            label.alignment = TextAlignmentOptions.MidlineLeft;
            label.text = $"{value:0.## °C}";
            Labels.Add(label);

            value -= STEP_SIZE;
        }

        Labels[0].alignment = TextAlignmentOptions.TopLeft;
        Labels[NLabels - 1].alignment = TextAlignmentOptions.BottomLeft;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
