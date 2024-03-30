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

    public int MapSize = 256;

    // Start is called before the first frame update
    void Start()
    {
        RobotLayer = 1 << LayerMask.NameToLayer("Robot");
        TargetLayers = ~RobotLayer;
        LidarStep = Mathf.PI*2 / NLidarRays;
        

        var startPose = new System.Numerics.Vector3(100.0f, 100.0f, 0.0f);
        SLAMProcessor = new CoreSLAMProcessor(200.0f, MapSize, 64, startPose, 0.1f, MathEx.DegToRad(10), 1000, 4)
        {
            HoleWidth = 2.0f
        };
    }

    // Update is called once per frame
<<<<<<< HEAD
    // void Update()
    // {
    //     ScanSegments(hectorSlam.MatchPose, out List<ScanSegment> scanSegments);
    //     ScanCloud scanCloud = new ScanCloud()
    //     {
    //         Pose = System.Numerics.Vector3.Zero
    //     };

    //     foreach (ScanSegment seg in scanSegments)
    //     {
    //         foreach (BaseSLAM.Ray ray in seg.Rays)
    //         {
    //             scanCloud.Points.Add(new System.Numerics.Vector2()
    //             {
    //                 X = ray.Radius * MathF.Cos(ray.Angle),
    //                 Y = ray.Radius * MathF.Sin(ray.Angle),
    //             });
    //         }
    //     }

    //     hectorSlam.Update(scanCloud, hectorSlam.MatchPose, loops < 10);
    //     loops++;
    // }
=======
    void Update()
    {
        ScanSegments(SLAMProcessor.Pose, out List<ScanSegment> segments);
        SLAMProcessor.Update(segments);
        for(int i = 0; i < 10; i++)
        {
            Debug.Log(SLAMProcessor.HoleMap.Pixels[i]);
        }
    }
>>>>>>> 7f019f6 (First iteration of SLAM)

    // private void ScanSegments(System.Numerics.Vector3 estimatedPose, out List<ScanSegment> segments)
    // {
    //     ScanSegment scanSegment = new ScanSegment()
    //     {
    //         Pose = estimatedPose,
    //         IsLast = true
    //     };

<<<<<<< HEAD
    //     for (float angle = 0f; angle < Mathf.PI * 2; angle += LidarStep)
    //     {
    //         float lidarAngle = angle + transform.rotation.eulerAngles.y; // Is the addition relevant?
    //         Vector3 direction = new Vector3(Mathf.Sin(lidarAngle), 0, Mathf.Cos(lidarAngle));
=======
        for (float angle = 0f; angle < Mathf.PI * 2; angle += LidarStep)
        {
            float lidarAngle = angle; // Is the addition relevant?
            Vector3 direction = new Vector3(Mathf.Sin(lidarAngle), 0, Mathf.Cos(lidarAngle));
>>>>>>> 7f019f6 (First iteration of SLAM)

    //         RaycastHit hit;
    //         if (Physics.Raycast(
    //             origin: transform.position,
    //             direction: transform.TransformDirection(direction),
    //             hitInfo: out hit,
    //             maxDistance: 100,
    //             layerMask: TargetLayers)
    //         )
    //         {
    //             scanSegment.Rays.Add(new BaseSLAM.Ray(angle, hit.distance));
    //         }
    //     }

    //     segments = new List<ScanSegment>()
    //     {
    //         scanSegment
    //     };
    // }
}
