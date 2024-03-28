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

    // public event Action<List<LidarReading>> LidarReadingEvent;
    private HectorSLAMProcessor hectorSlam;
    private int loops = 0;

    // Start is called before the first frame update
    void Start()
    {
        RobotLayer = 1 << LayerMask.NameToLayer("Robot");
        TargetLayers = ~RobotLayer;
        LidarStep = Mathf.PI*2 / NLidarRays;


        var startPose = new System.Numerics.Vector3(20.0f, 20.0f, 0.0f);

        // HectorSLAM
        int hsMapSide = 400;
        hectorSlam = new HectorSLAMProcessor(40.0f / hsMapSide, new System.Drawing.Point(hsMapSide, hsMapSide), startPose, 4, 4)
        {
            MinDistanceDiffForMapUpdate = 0.4f,
            MinAngleDiffForMapUpdate = MathEx.DegToRad(8)
        };

        // Set estimate iteransions counts for each map layer
        hectorSlam.MapRep.Maps[0].EstimateIterations = 7;
        hectorSlam.MapRep.Maps[1].EstimateIterations = 4;
        hectorSlam.MapRep.Maps[2].EstimateIterations = 4;
        hectorSlam.MapRep.Maps[3].EstimateIterations = 4;

    }

    // Update is called once per frame
    void Update()
    {
        ScanSegments(hectorSlam.MatchPose, out List<ScanSegment> scanSegments);
        ScanCloud scanCloud = new ScanCloud()
        {
            Pose = System.Numerics.Vector3.Zero
        };

        foreach (ScanSegment seg in scanSegments)
        {
            foreach (BaseSLAM.Ray ray in seg.Rays)
            {
                scanCloud.Points.Add(new System.Numerics.Vector2()
                {
                    X = ray.Radius * MathF.Cos(ray.Angle),
                    Y = ray.Radius * MathF.Sin(ray.Angle),
                });
            }
        }

        hectorSlam.Update(scanCloud, hectorSlam.MatchPose, loops < 10);
        loops++;
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
            float lidarAngle = angle + transform.rotation.eulerAngles.y; // Is the addition relevant?
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
