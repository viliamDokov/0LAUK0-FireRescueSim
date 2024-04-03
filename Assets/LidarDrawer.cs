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

    private struct HeatMapData
    {
        public float temp;
        public float time;
    }

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

    private HeatMapData[,] heatMap;
    private int heatMapX;
    private int heatMapZ;
    public ReadHeatData heatData;
    public int fireScaler = 3;
    public float fireDesaturation = 0.0001f;
    public static float TRAIL_TIME = 30.0f;

    public ComputeShader ImageDrawShader; 

    Gradient gradient = new Gradient();
    public RenderTexture testTexture;
    
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
        heatMap = new HeatMapData[(int)dimensions.x, (int)dimensions.y];

        testTexture = new RenderTexture(Size, Size, 1);
        testTexture.enableRandomWrite = true;
        targetImage.texture = testTexture;
        testTexture.Create();
        ImageDrawShader.SetTexture(0, "Result", testTexture);
        ImageDrawShader.SetFloat("Resolution", Size);
    }

    // Update is called once per frame
    void Update()
    {
        //UpdateImage();

        var robotMapPose = slam.EstimatedPose * slam.scale;

        var computeBuffer = new ComputeBuffer(slam.MapData.Length, sizeof(uint));
        computeBuffer.SetData(slam.MapData);
        ImageDrawShader.SetBuffer(0, "MapData", computeBuffer);
        ImageDrawShader.SetInt("RobotX", (int)robotMapPose.X);
        ImageDrawShader.SetInt("RobotY", (int)robotMapPose.Y);
        ImageDrawShader.GetKernelThreadGroupSizes(0, out uint x, out uint y, out uint z);
        ImageDrawShader.Dispatch(0, testTexture.width / (int)x, testTexture.height / (int)y, (int)z);
    }


    private void UpdateImage()
    {
        uint[] GrayValues = slam.MapData;
        var mapPose = slam.WorldPoseToMapPose(RobotPosition.position);
        var estimatedPose = slam.EstimatedPose;
        estimatedPose = estimatedPose * slam.scale;
        var currTime = Time.time;

        //Debug.Log($"POSATO {mapPose.X} {mapPose.Y}");

        //Debug.Log($"ESTIMADE: {texture.width} {texture.height }");
        for (int x = 0; x < texture.width; x++)
        {
            for (int y = 0; y < texture.height; y++)
            {
                Color color;
                if (x < 10 && y < 10)
                {
                    color = Color.cyan;
                }
                else if (Mathf.Abs(x - mapPose.X) < 2 && Mathf.Abs(y - mapPose.Y) < 2)
                {
                    color = Color.red;
                }
                else if (Mathf.Abs(x - estimatedPose.X) < 2 && Mathf.Abs(y - estimatedPose.Y) < 2)
                {
                    color = Color.green;
                }
                else
                {
                    uint alpha = GrayValues[x + Size * y];
                    color = new Color32(255, 255, 255, (byte) (alpha>>8));
                }
                texture.SetPixel(Size - x, y, color);

                if (currTime - heatMap[x, y].time < TRAIL_TIME && heatMap[x, y].temp > 0)
                {
                    //color = new Color(heatMap[x, y] / 100f, 0.2f, 0.2f, heatMapDesaturation[x, y]);
                    color = gradient.Evaluate(heatMap[x, y].temp / 150f);
                    var alpha = Math.Clamp(TRAIL_TIME - (currTime - heatMap[x, y].time), 0, TRAIL_TIME) / TRAIL_TIME;
                    color.a = alpha;
                    heatOverlayTexture.SetPixel(Size - x, y, color);
                }
                else
                {
                    heatOverlayTexture.SetPixel(Size - x, y, new Color(0f, 0f, 0f, 0f));
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
                var tempMapValue = temp * (float)(1 - (float)(Mathf.Abs(i) + Mathf.Abs(j)) / (float)(10 * fireScaler));
                heatMap[xIdx, yIdx] = new HeatMapData{ temp = tempMapValue, time = Time.time };
            }
        }

        //SetFireColor(temp);
    }
}
