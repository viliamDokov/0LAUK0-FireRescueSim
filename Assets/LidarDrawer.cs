using CoreSLAM;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UI;

public class LidarDrawer : MonoBehaviour
{

    public RawImage targetImage;
    public RawImage heatOverlay;

    private Texture2D texture;
    private Texture2D heatOverlayTexture;

    private int Size = 100;
    public Color color = Color.cyan;
    public BaseSlamScript baseSlam;
    public HectorSlamScript hectorSlam;
    private ISlamComponent slam;
    public Transform RobotPosition;

    private float[,] heatMap;
    private float[,] heatMapDesaturation;
    private int heatMapX;
    private int heatMapZ;
    public ReadHeatData heatData;
    public int fireScaler = 3;
    public float fireDesaturation = 0.0001f;

    Gradient gradient = new Gradient();
    // Start is called before the first frame update
    void Start()
    {

        gradient = heatData.gradient;
        slam = baseSlam;
        Size = slam.SlamMapSize;
        texture = new Texture2D(Size, Size);
        targetImage.texture = texture;
        heatOverlayTexture = new Texture2D(Size, Size);
        heatOverlay.texture = heatOverlayTexture;
        
        Vector2 dimensions = texture.Size();
        //int[] dimensions = heatData.GetDimensions();
        //heatMapX = dimensions[0] * fireScaler;
        //heatMapZ = dimensions[1] * fireScaler;
        //Debug.Log(dimensions[0] + " " +  dimensions[1]);
        //heatMap = new float[dimensions[0] * fireScaler, dimensions[1]  * fireScaler];
        heatMap = new float[(int)dimensions.x, (int)dimensions.y];
        heatMapDesaturation = new float[(int)dimensions.x, (int)dimensions.y];


    }

    // Update is called once per frame
    void Update()
    {
        byte[] GrayValues = slam.MapData;
        var mapPose = slam.WorldPoseToMapPose(RobotPosition.position);
        var estimatedPose = slam.EstimatedPose;
        estimatedPose = estimatedPose * slam.scale;
        //Debug.Log($"POSATO {mapPose.X} {mapPose.Y}");

        //Debug.Log($"ESTIMADE: {texture.width} {texture.height }");
        for (int x = 0; x < texture.width; x++)
        {
            for(int y = 0; y < texture.height;y++)
            {
                Color color;
                if (x < 10 && y < 10)
                {
                    color = Color.cyan;
                }
                else if( Mathf.Abs(x - mapPose.X) < 2 && Mathf.Abs(y - mapPose.Y) < 2)
                {
                    color = Color.red;
                } 
                else if (Mathf.Abs(x - estimatedPose.X) < 2 && Mathf.Abs(y - estimatedPose.Y) < 2)
                {
                    color = Color.green;
                }
                else
                {
                    byte alpha = GrayValues[x + Size * y];
                    color = new Color32(255, 255, 255, alpha);
                }
                texture.SetPixel(Size - x, y, color);

                if (heatMap[x, y] > 0)
                {
                    //color = new Color(heatMap[x, y] / 100f, 0.2f, 0.2f, heatMapDesaturation[x, y]);
                    color = gradient.Evaluate(heatMap[x, y]/150f);
                    color.a = heatMapDesaturation[x, y];
                    heatOverlayTexture.SetPixel(Size - x, y, color);
                } else
                {
                    heatOverlayTexture.SetPixel(Size - x, y, new Color(0f,0f,0f,0f));
                }
                if (heatMapDesaturation[x, y] > 0)
                {
                    heatMapDesaturation[x, y] -= fireDesaturation;
                }
                
            }
        }
        texture.Apply();
        heatOverlayTexture.Apply();
    }

    public void updateHeatData(float x, float y)
    {
        //int normalizedX = heatData.NormalizeX(x);
        //int normalizedZ = heatData.NormalizeZ(y);

        var mapPoseHeat = slam.EstimatedPose * slam.scale;
        int mapPoseX = Mathf.RoundToInt(mapPoseHeat.X);
        int mapPoseY = Mathf.RoundToInt(mapPoseHeat.Y);

        float temp = heatData.GetCurrentHeatDataPoint(x, y);
        Debug.Log(temp);

        for (int i = -fireScaler; i < fireScaler + 1; i++)
        {
            for (int j = -fireScaler; j < fireScaler + 1; j++)
            {
                var xIdx = Mathf.Clamp(mapPoseX + i, 0, heatMap.GetLength(0)-1); 
                var yIdx = Mathf.Clamp(mapPoseY + j, 0, heatMap.GetLength(1)-1);
                heatMap[xIdx, yIdx] = temp * (float)(1 - (float)(Mathf.Abs(i) + Mathf.Abs(j)) / (float)(10 * fireScaler));
                heatMapDesaturation[xIdx, yIdx] = 1f;
            }
        }

        //SetFireColor(temp);
    }
}
