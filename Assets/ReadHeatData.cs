using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using UnityEngine.UI;

public class ReadHeatData : MonoBehaviour
{
    //public string[][] heat_values;
    private float[] timestamps;

    // [t][x][y][z] where t is time and x, y, z are coordinates
    private float[][,] heatValues;
    // private float deltaTime = 1;
    int x = 0;
    int y = 0;

    private float maxHeat = 0f;

    public float x_coef = 5;
    public float x_const = 50;
    public float y_coef = 5;
    public float y_const = 50;
    public bool inverted = false;
    
    public GameObject fireCubePrefab;

    private GameObject[,] fireCubes;

    public TextAsset heatValuesAsset;

    public int legendWidth = 30;
    public int legendHeight = 100;
    public RawImage targetImage;
    private Texture2D texture;

    public Gradient gradient = new Gradient();
    // Start is called before the first frame update
    void Start()
    {
        var colors = new GradientColorKey[2];
        colors[0] = new GradientColorKey(Color.cyan, 0.0f);
        colors[1] = new GradientColorKey(Color.red, 1.0f);
        var alphas = new GradientAlphaKey[2];
        alphas[0] = new GradientAlphaKey(1.0f, 0.0f);
        alphas[1] = new GradientAlphaKey(1.0f, 1.0f);

        gradient.SetKeys(colors, alphas);

        texture = new Texture2D(legendWidth, legendHeight);
        targetImage.texture = texture;

        ParseData();
        //Debug.Log(x + " " + y);
        Debug.Log(maxHeat);
        fireCubes = new GameObject[x,y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                fireCubes[i, j] = Instantiate(fireCubePrefab, new Vector3((float)i * x_coef - x_const, 2, (float)j * y_coef - y_const), Quaternion.identity);
                fireCubes[i, j].transform.localScale = new Vector3(x_coef, 0.1f, y_coef);
            }
        }

        for (int i = 0; i < legendHeight; i++)
        {
            Color color = gradient.Evaluate((float) i / (float)legendHeight);
            for (int j = 0; j < legendWidth; j++)
            {
                texture.SetPixel(j, i, color);
            }
        }
        texture.Apply();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                float temp = GetCurrentHeatDataPoint((float)i * x_coef - x_const, (float)j * y_coef - y_const);
                //Debug.Log(temp);
                fireCubes[i, j].GetComponent<Renderer>().material.SetColor("_Color", new Color(temp/100f, 0f, 0f, 0.7f));
                //Debug.Log(fireCubes[i, j].GetComponent<Renderer>().material.GetColor("_Color"));
            }
        }
    }

    void ParseData()
    {
        
        //TextAsset heatValuesAsset = Resources.Load("demofile2") as TextAsset;

        string[] dataLines = heatValuesAsset.text.Split('\n');
        string[] dataDimensions = dataLines[0].Split(", ");
        int timestampCount = int.Parse(dataDimensions[0]);

        if(inverted)
        {
            y = int.Parse(dataDimensions[1]);
            x = int.Parse(dataDimensions[2]);
        }
        else
        {
            x = int.Parse(dataDimensions[1]);
            y = int.Parse(dataDimensions[2]);
        }
        //string[] roomDimenstions = dataLines[1].Split(", ");
        //x_coef = (float)(x - 1) / int.Parse(roomDimenstions[0]);
        //y_coef = (float)(y - 1) / int.Parse(roomDimenstions[1]);
        //Debug.Log(x_coef +" "+ y_coef);
        string[] timestampsString = dataLines[2].Split(", ");

        heatValues = new float[timestampCount][,];
        timestamps = new float[timestampCount];
        //Debug.Log(timestampCount);
        //Debug.Log(x + " " +  y);
        for (int j = 0; j < timestampCount; j++)
        {
            //Debug.Log(timestampsString[j]);
            timestamps[j] = float.Parse(timestampsString[j]);

        }

        for (int j = 0; j < timestampCount; j++)
        {
            heatValues[j] = new float[x, y];
            string[] temperatures = dataLines[j + 3].Split(", ");

            for (int i = 0; i < x; i++)
            {
                for(int k = 0; k < y; k++)
                {
                    float temp = float.Parse(temperatures[i + k * x]);
                    heatValues[j][i, k] = temp;
                    maxHeat = Mathf.Max(maxHeat, temp);
                    //Debug.Log(heatValues[j][i, 0, k]);
                }
                
            }

        }
    }
    public int NormalizeX(float x)
    {
        return Mathf.RoundToInt((x + x_const) / x_coef);
    }
    public int NormalizeZ(float z)
    {
        return Mathf.RoundToInt((z + y_const) / y_coef);
    }

    public float GetCurrentHeatDataPoint(float x, float z)
    {
        float time = Time.time;
        for(int i = 0; i < timestamps.Length - 1; i++)
        {
            if(timestamps[i] < time && time < timestamps[i + 1])
            {
                //Debug.Log(NormalizeX(x) + " " + NormalizeY(y) + " " + NormalizeZ(z));
                //Debug.Log(heatValues[timestamps.Length - 1][NormalizeX(x), NormalizeY(y), NormalizeZ(z)]);
                return heatValues[i][NormalizeX(x),NormalizeZ(z)];
            }
        }
        return heatValues[timestamps.Length - 1][NormalizeX(x), NormalizeZ(z)];
    }

    public int[] GetDimensions()
    {
        return new int[] { x, y };
    }
}
