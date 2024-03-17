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
    // Start is called before the first frame update
    void Start()
    {
        ParseData();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void ParseData()
    {
        timestamps = new float[2];
        TextAsset heatValuesAsset = Resources.Load("test") as TextAsset;
        string[] timestampDataPoints = heatValuesAsset.text.Split('\n');
        heatValues = new float[2][,,];
        for (int j = 0; j < 2; j++)
        {
            heatValues[j] = new float[timestampDataPoints.Length, timestampDataPoints.Length, timestampDataPoints.Length];
            for (int i = 2; i < timestampDataPoints.Length; i++)
            {
                string[] datapoint = timestampDataPoints[i].Split(',');
                

                //Debug.Log(datapoint[0] + " " + datapoint[1] + " " + datapoint[2]);
                //Debug.Log(float.Parse(datapoint[0], NumberStyles.Float) + " " + float.Parse(datapoint[1], NumberStyles.Float) + " " + float.Parse(datapoint[2], NumberStyles.Float));
                //Debug.Log(NormalizeX(float.Parse(datapoint[0], NumberStyles.Float)) + " " + NormalizeY(float.Parse(datapoint[1], NumberStyles.Float)) + " " + NormalizeZ(float.Parse(datapoint[2], NumberStyles.Float)));
                //Debug.Log(float.Parse(datapoint[3], NumberStyles.Float));
                heatValues[j][NormalizeX(float.Parse(datapoint[0], NumberStyles.Float)), NormalizeY(float.Parse(datapoint[1], NumberStyles.Float)), NormalizeZ(float.Parse(datapoint[2], NumberStyles.Float))] = float.Parse(datapoint[3], NumberStyles.Float) + 1;
                timestamps[j] = j*5;
            }
                
        }
        //Debug.Log(heatValues[0][0, 0, 0]);
        //Debug.Log(2.5 + heatValues[0][0, 0, 0]);
        //for (int j = 0; j < 1; j++)

        //Console.WriteLine(heatValues.GetType().Name);
        //string[] heat_value_rows = heatValues.text.Split('\n');
        //heat_values = new string[heat_value_rows.Length][];

            //for (int i = 2; i < heat_value_rows.Length; i++)
            //{
            //    heat_values[i] = heat_value_rows[i].Split(',');
            //}

            //for (int i = 0; i < heatValues.Length; i++)
            //{
            //    for (int j = 0; j < heatValues[i].Length; j++)
            //    {
            //        Debug.Log(heatValues[i][j]);
            //    }
            //}
    }
    private int NormalizeX(float x)
    {
        return Mathf.RoundToInt(x * 10);
    }
    private int NormalizeY(float y)
    {
        return Mathf.RoundToInt(y * 10);
    }
    private int NormalizeZ(float z)
    {
        return Mathf.RoundToInt(z * 10);
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
        Debug.Log(NormalizeX(x) + " " + NormalizeY(y) + " " + NormalizeZ(z));
        return heatValues[timestamps.Length - 1][NormalizeX(x), NormalizeY(y), NormalizeZ(z)];
    }
}
