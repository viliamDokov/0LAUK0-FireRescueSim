using CoreSLAM;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    public int fireRadius = 100;
    public float fireDesaturation = 0.0001f;
    public static float TRAIL_TIME = 30.0f;

    public ComputeShader ImageDrawShader; 
    public ComputeShader HeatMapShader; 

    Gradient gradient = new Gradient();
    public RenderTexture slamMapTexture;
    public Texture2D heatMapTexture;
    public float MAX_TEMP = 250;
    public float TEMP_MIN_ALPHA = 0.2f;
    public float FADE_DURATION = 10f;

    private Gradient HeatMapGradient;
    private Color32[] heatMapColors;

    public ScaleDrawe heatScale;
    public RawImage heatScaleImage;
    private Texture2D heatScaleTexture;



    public int heatScaleWidth = 30;
    public int heatScaleHeigth= 100;

    public TextMeshProUGUI TempDisplay;
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log("STARTING!");
        slam = baseSlam;
        Size = slam.SlamMapSize;
        
        heatMap = new HeatMapData[Size, Size];

        slamMapTexture = new RenderTexture(Size, Size, 1);
        slamMapTexture.enableRandomWrite = true;
        targetImage.texture = slamMapTexture;
        slamMapTexture.Create();
        ImageDrawShader.SetTexture(0, "Result", slamMapTexture);
        ImageDrawShader.SetFloat("Resolution", Size);


        heatMapTexture = new Texture2D(Size, Size);
        heatMapColors = new Color32[Size*Size];
        heatOverlay.texture = heatMapTexture; 

        HeatMapGradient = new Gradient();
        HeatMapGradient.mode = GradientMode.PerceptualBlend;
        var colors = new GradientColorKey[]
        {
            new GradientColorKey(Color.blue, 0.0f),
            new GradientColorKey(Color.cyan, 0.25f),
            new GradientColorKey(Color.green, 0.5f),
            new GradientColorKey(Color.yellow, 0.75f),
            new GradientColorKey(Color.red, 1f),
        };
        var alphas = new GradientAlphaKey[]
        {
            new GradientAlphaKey(1.0f, colors[0].time),
            new GradientAlphaKey(1.0f, colors[1].time),
            new GradientAlphaKey(1.0f, colors[2].time),
            new GradientAlphaKey(1.0f, colors[3].time),
            new GradientAlphaKey(1.0f, colors[4].time),
        };
        HeatMapGradient.alphaKeys = alphas;
        HeatMapGradient.colorKeys = colors;

        heatScale.MIN = 0.0f;
        heatScale.MAX = MAX_TEMP;
        heatScale.heatScaleGradient = HeatMapGradient;

         
        heatScaleTexture = new Texture2D(heatScaleWidth, heatScaleHeigth);
        heatScaleImage.texture = heatScaleTexture;

        for (int i = 0; i < heatScaleHeigth; i++)
        {
            Color color = HeatMapGradient.Evaluate(i / (float)heatScaleHeigth);
            for (int j = 0; j < heatScaleWidth; j++)
            {
                heatScaleTexture.SetPixel(j, i, color);
            }
        }
        heatScaleTexture.Apply();

        //heatMapTexture = new RenderTexture(Size, Size, 1);
        //heatMapTexture.enableRandomWrite = true;
        //heatOverlay.texture = heatMapTexture;
        //heatMapTexture.Create();

        //Debug.Log("THIGN!!");
        //HeatMapShader.SetTexture(0, "Result", heatMapTexture);
        //HeatMapShader.SetFloat("Resolution", Size);

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
        ImageDrawShader.Dispatch(0, slamMapTexture.width / (int)x, slamMapTexture.height / (int)y, (int)z);
        DrawHeatData();
    }

    private void DrawHeatData()
    {
        float currentTime = Time.time;

        for (int i = 0; i < Size; i ++)
        {
            for (int j = 0; j < Size; j ++) 
            {
                if (heatMap[i,j].temp > 0) 
                {
                    var color = HeatMapGradient.Evaluate(heatMap[i, j].temp);
                    color.a = Math.Clamp(1 - (currentTime - heatMap[i, j].time) / FADE_DURATION,TEMP_MIN_ALPHA,1);
                    heatMapColors[(Size-1-i) + Size *j] = color;
                } else
                {
                    heatMapColors[(Size - 1 - i) + Size * j] = new Color(1.0f, 1.0f, 1.0f,1.0f);
                }
            }
        }
        heatMapTexture.SetPixels32(heatMapColors, 0);
        heatMapTexture.Apply();
    }

    public void updateHeatData(float x, float y)
    {
        var mapPoseHeat = slam.EstimatedPose * slam.scale;
        int mapPoseX = Mathf.RoundToInt(mapPoseHeat.X);
        int mapPoseY = Mathf.RoundToInt(mapPoseHeat.Y);

        float temp = heatData.GetCurrentHeatDataPoint(x, y);
        TempDisplay.text = $"Temp: {temp:0.} °C";
        Debug.Log($"HEATMAP: {heatMap.GetLength(0)}");

        for (int i = -fireRadius; i < fireRadius + 1; i++)
        {
            for (int j = -fireRadius; j < fireRadius + 1; j++)
            {
                if(i*i + j*j <= fireRadius*fireRadius) // Draw an actual circle
                {
                    var xIdx = Mathf.Clamp(mapPoseX + i, 0, heatMap.GetLength(0) - 1);
                    var yIdx = Mathf.Clamp(mapPoseY + j, 0, heatMap.GetLength(1) - 1);
                    var tempMapValue = temp / MAX_TEMP;
                    Debug.Log(tempMapValue);
                    heatMap[xIdx, yIdx].temp = tempMapValue;
                    heatMap[xIdx, yIdx].time = Time.time;
                }
            }
        }
    }
}
