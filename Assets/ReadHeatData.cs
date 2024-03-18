using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;

public class ReadHeatData : MonoBehaviour
{
    //public string[][] heat_values;
    private float[] timestamps;

    // [t][x][y][z] where t is time and x, y, z are coordinates
    private float[][,,] heatValues;
    private float deltaTime = 1;
    int x = 0;
    int y = 0;
    public GameObject fireCubePrefab;

    private GameObject[,] fireCubes;
    // Start is called before the first frame update
    void Start()
    {
        ParseData();
        fireCubes = new GameObject[x,y];
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                fireCubes[i, j] = Instantiate(fireCubePrefab, new Vector3((float)i / 2, 0, (float)j / 2), Quaternion.identity);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < x; i++)
        {
            for (int j = 0; j < y; j++)
            {
                float temp = GetCurrentHeatDataPoint((float)i / 2, 0, (float)j / 2);
                //Debug.Log(temp);
                fireCubes[i, j].GetComponent<Renderer>().material.SetColor("_Color", new Color(temp/100f, 0f, 0f, 0.7f));
                //Debug.Log(fireCubes[i, j].GetComponent<Renderer>().material.GetColor("_Color"));
            }
        }
    }

    void ParseData()
    {
        
        TextAsset heatValuesAsset = Resources.Load("demofile2") as TextAsset;

        string[] dataLines = heatValuesAsset.text.Split('\n');
        string[] dataDimensions = dataLines[0].Split(", ");
        int timestampCount = int.Parse(dataDimensions[0]);
        x = int.Parse(dataDimensions[1]);
        y = int.Parse(dataDimensions[2]);
        string[] timestampsString = dataLines[2].Split(", ");

        heatValues = new float[timestampCount][,,];
        timestamps = new float[timestampCount];

        for (int j = 0; j < timestampCount; j++)
        {
            //Debug.Log(timestampsString[j]);
            timestamps[j] = float.Parse(timestampsString[j]);

        }

        for (int j = 0; j < timestampCount; j++)
        {
            heatValues[j] = new float[x, 1, y];
            string[] temperatures = dataLines[j + 3].Split(", ");

            for (int i = 0; i < x; i++)
            {
                for(int k = 0; k < y; k++)
                {
                    //Debug.Log(j + " " + i + " " + k);
                    heatValues[j][i, 0, k] = float.Parse(temperatures[i + k * y]);
                    //Debug.Log(heatValues[j][i, 0, k]);
                }
                
            }

        }
    }
    private int NormalizeX(float x)
    {
        return Mathf.RoundToInt(x * 2);
    }
    private int NormalizeY(float y)
    {
        //return Mathf.RoundToInt(y * );
        return 0;
    }
    private int NormalizeZ(float z)
    {
        return Mathf.RoundToInt(z * 2);
    }

    public float GetCurrentHeatDataPoint(float x, float y, float z)
    {
        float time = Time.time;
        for(int i = 0; i < timestamps.Length - 1; i++)
        {
            if(timestamps[i] < time && time < timestamps[i + 1])
            {
                return heatValues[i][NormalizeX(x),NormalizeY(y),NormalizeZ(z)];
            }
        }
        //Debug.Log(NormalizeX(x) + " " + NormalizeY(y) + " " + NormalizeZ(z));
        return heatValues[timestamps.Length - 1][NormalizeX(x), NormalizeY(y), NormalizeZ(z)];
    }
}
