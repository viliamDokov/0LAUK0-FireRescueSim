using BaseSLAM;
using CoreSLAM;
using HectorSLAM.Main;
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

    public delegate void ReadingHandler(List<BaseSLAM.Ray> cloud);
    public event ReadingHandler ProcessData;

    // Start is called before the first frame update
    void Start()
    {
        RobotLayer = 1 << LayerMask.NameToLayer("Robot");
        TargetLayers = ~RobotLayer;
        LidarStep = Mathf.PI*2 / NLidarRays;
    }


    void Update()
    {
        ScanSegments(out List<BaseSLAM.Ray> segments);
        if (ProcessData != null) {
            ProcessData(segments);
        }
    }


    //private HectorSLAMProcessor SetupHecoeSlam()
    //{
    //    var result = new HectorSLAMProcessor(RealMapSize / hsMapSize, new System.Drawing.Point(hsMapSize, hsMapSize), hsMapStartPose, 4, 4)
    //    {
    //        MinDistanceDiffForMapUpdate = 0.4f,
    //        MinAngleDiffForMapUpdate = MathEx.DegToRad(8)
    //    };

    //    result.MapRep.Maps[0].EstimateIterations = 7;
    //    result.MapRep.Maps[1].EstimateIterations = 4;
    //    result.MapRep.Maps[2].EstimateIterations = 4;
    //    result.MapRep.Maps[3].EstimateIterations = 4;

    //    return result;
    //}

    //private ScanCloud SegmentsToCloud(List<ScanSegment> segments)
    //{
    //    ScanCloud scanCloud = new ScanCloud()
    //    {
    //        Pose = System.Numerics.Vector3.Zero
    //    };

    //    foreach (ScanSegment seg in segments)
    //    {
    //        foreach (BaseSLAM.Ray ray in seg.Rays)
    //        {
    //            scanCloud.Points.Add(new System.Numerics.Vector2()
    //            {
    //                X = ray.Radius * MathF.Cos(ray.Angle),
    //                Y = ray.Radius * MathF.Sin(ray.Angle),
    //            });
    //        }
    //    }

    //    return scanCloud;
    //}

     private void ScanSegments(out List<BaseSLAM.Ray> rays)
    {
        rays = new();

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
                rays.Add(new BaseSLAM.Ray(angle, hit.distance));
            }
        }
    }
}
