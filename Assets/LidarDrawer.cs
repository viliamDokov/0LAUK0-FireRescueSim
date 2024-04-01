using CoreSLAM;
using System.Collections;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class LidarDrawer : MonoBehaviour
{

    public RawImage targetImage;
    private Texture2D texture;
    private int Size = 100;
    public Color color = Color.cyan;
    public LidarScan Lidar;
    private CoreSLAMProcessor SLAMProcessor;

    private float[,] heatMap;
    private int heatMapX;
    private int heatMapZ;
    public ReadHeatData heatData;
    public int fireScaler = 12;

    // Start is called before the first frame update
    void Start()
    {
        Size = Lidar.MapSize;
        texture = new Texture2D(Size, Size);
        targetImage.texture = texture;
        int[] dimensions = heatData.GetDimensions();
        heatMapX = dimensions[0] * fireScaler;
        heatMapZ = dimensions[1] * fireScaler;
        Debug.Log(dimensions[0] + " " +  dimensions[1]);
        heatMap = new float[dimensions[0] * fireScaler, dimensions[1]  * fireScaler];
    }

    // Update is called once per frame
    void Update()
    {
        ushort[] GrayValues = Lidar.MapData;
        var mapPose = Lidar.WorldPoseToMapPose(Lidar.transform.position);
        var estimatedPose = Lidar.SLAMProcessor.Pose;
        estimatedPose = estimatedPose * Lidar.SLAMProcessor.HoleMap.Scale;
        Debug.Log($"ESTIMADE: {estimatedPose.X} {estimatedPose.Y }");

        for (int x = 0; x < texture.width; x++)
        {
            for(int y = 0; y < texture.height;y++)
            {
                Color color;
                if( Mathf.Abs(x - mapPose.X) < 2 && Mathf.Abs(y - mapPose.Y) < 2)
                {
                    color = Color.red;
                } 
                else if (Mathf.Abs(x - estimatedPose.X) < 2 && Mathf.Abs(y - estimatedPose.Y) < 2)
                {
                    color = Color.green;
                }
                else
                {
                    ushort alpha = GrayValues[x + Size * y];
                    byte alphaByte = (byte)(alpha >> 8);
                    color = new Color32(255, 255, 255, alphaByte);
                }

                
                texture.SetPixel(Size - x, y, color);
            }
        }

        for (int x = 0; x < heatMapX; x++)
        {
            for (int y = 0; y < heatMapZ; y++)
            {
                if (heatMap[x,y] > 0)
                {
                    texture.SetPixel(x, y, new Color(heatMap[x, y] / 100f, 0, 0, 1));
                }
            }
        }
        texture.Apply();
    }

    public void updateHeatData(float x, float z)
    {
        int normalizedX = heatData.NormalizeX(x);
        int normalizedZ = heatData.NormalizeZ(z);
        Debug.Log(x + " " + z);
        Debug.Log(normalizedX + " " + normalizedZ);
        float temp = heatData.GetCurrentHeatDataPoint(x, 0, z);
        Debug.Log(temp);

        for (int i = 0; i < fireScaler * 2; i++)
        {
            for (int j = 0; j < fireScaler * 2; j++)
            {
                heatMap[normalizedX * fireScaler + i, normalizedZ * fireScaler + j] = temp;
            }
        }

        //SetFireColor(temp);
    }
}
