using BaseSLAM;
using CoreSLAM;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;

public struct LidarReading {
    public float distance;
    public float angle;
    public Vector3 point;
    public LidarReading(float distance , float angle, Vector3 point)
    {
        this.point = point;
        this.distance = distance;
        this.angle = angle;
    }
}


public class LidarScan : MonoBehaviour
{
    // The collision layer of the robot  
    private int RobotLayer;
    // Colision layers of everything we want to detect with our lidar
    private int TargetLayers;

    public int NLidarRays = 180;
    private float LidarStep;
    // private float 

    // public event Action<List<LidarReading>> LidarReadingEvent;
    private int loops = 0;

    public CoreSLAMProcessor SLAMProcessor;
    public ushort[] MapData
    {
        get => SLAMProcessor?.HoleMap.Pixels;
    }

    public int MapSize = 350;
    public System.Numerics.Vector3 mapStartPose = new System.Numerics.Vector3(100.0f, 100.0f, 0.0f);
    public Vector3 worldStartPose;

    // Start is called before the first frame update
    void Start()
    {
        RobotLayer = 1 << LayerMask.NameToLayer("Robot");
        TargetLayers = ~RobotLayer;
        LidarStep = Mathf.PI*2 / NLidarRays;
        worldStartPose = transform.position;

        SLAMProcessor = new CoreSLAMProcessor(200.0f, MapSize, 64, mapStartPose, 0.1f, MathEx.DegToRad(10), 1000, 4)
        {
            HoleWidth = 2.0f
        };
    }

    public System.Numerics.Vector3 WorldPoseToMapPose(Vector3 worldPose)
    {
        worldPose = worldPose - worldStartPose;
        var pixelsPerMeter = SLAMProcessor.HoleMap.Scale; 
        return new System.Numerics.Vector3((mapStartPose.X - worldPose.x )* pixelsPerMeter, (mapStartPose.Y + worldPose.z) * pixelsPerMeter, 0);
    }

    void Update()
    {
        ScanSegments(SLAMProcessor.Pose, out List<ScanSegment> segments);
        SLAMProcessor.Update(segments);
        //for(int i = 0; i < 10; i++)
        //{
        //    Debug.Log(SLAMProcessor.HoleMap.Pixels[i]);
        //}
    }

     private void ScanSegments(System.Numerics.Vector3 estimatedPose, out List<ScanSegment> segments)
    {
        ScanSegment scanSegment = new ScanSegment()
        {
            Pose = estimatedPose,
            IsLast = true
        };


        for (float angle = 0f; angle < Mathf.PI * 2; angle += LidarStep)
        {
            float lidarAngle = angle;
            Vector3 direction = new Vector3(Mathf.Sin(lidarAngle), 0, Mathf.Cos(lidarAngle));
            RaycastHit hit;
            if (Physics.Raycast(
                origin: transform.position,
                direction: transform.TransformDirection(direction),
                hitInfo: out hit,
                maxDistance: 100,
                layerMask: TargetLayers)
            )
            {
                scanSegment.Rays.Add(new BaseSLAM.Ray(angle, hit.distance));
            }
        }

        segments = new List<ScanSegment>()
             {
                 scanSegment
             };
    }
}
