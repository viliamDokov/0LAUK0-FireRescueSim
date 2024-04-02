using CoreSLAM;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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
    public int fireScaler = 3;

    // Start is called before the first frame update
    void Start()
    {
        Size = Lidar.MapSize;
        texture = new Texture2D(Size, Size);
        targetImage.texture = texture;
        Vector2 dimensions = texture.Size();
        //int[] dimensions = heatData.GetDimensions();
        //heatMapX = dimensions[0] * fireScaler;
        //heatMapZ = dimensions[1] * fireScaler;
        //Debug.Log(dimensions[0] + " " +  dimensions[1]);
        //heatMap = new float[dimensions[0] * fireScaler, dimensions[1]  * fireScaler];
        heatMap = new float[(int)dimensions.x, (int)dimensions.y];
    }

    // Update is called once per frame
    void Update()
    {
        ushort[] GrayValues = Lidar.MapData;
        var mapPose = Lidar.WorldPoseToMapPose(Lidar.transform.position);
        var estimatedPose = Lidar.SLAMProcessor.Pose;
        estimatedPose = estimatedPose * Lidar.SLAMProcessor.HoleMap.Scale;
        //Debug.Log($"ESTIMADE: {estimatedPose.X} {estimatedPose.Y }");

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
                else if (heatMap[x, y] > 0)
                {
                    color = new Color(heatMap[x, y] / 100f, 0, 0, 1);
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
        texture.Apply();
    }

    public void updateHeatData(float x, float y)
    {
        //int normalizedX = heatData.NormalizeX(x);
        //int normalizedZ = heatData.NormalizeZ(y);

        var mapPoseHeat = Lidar.WorldPoseToMapPose(new Vector3(x, 0, y));
        int mapPoseX = Mathf.RoundToInt(mapPoseHeat.X);
        int mapPoseY = Mathf.RoundToInt(mapPoseHeat.Y);

        float temp = heatData.GetCurrentHeatDataPoint(x, 0, y);
        //Debug.Log(temp);

        for (int i = -fireScaler; i < fireScaler + 1; i++)
        {
            for (int j = -fireScaler; j < fireScaler + 1; j++)
            {
                heatMap[mapPoseX + i, mapPoseY + j] = Mathf.Max(heatMap[mapPoseX + i, mapPoseY + j], temp * (1 - ( fireScaler + i) / (2 * fireScaler) ) );
            }
        }

        //SetFireColor(temp);
    }
}
