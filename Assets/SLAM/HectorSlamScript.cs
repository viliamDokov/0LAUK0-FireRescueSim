using BaseSLAM;
using CoreSLAM;
using HectorSLAM.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HectorSlamScript : MonoBehaviour, ISlamComponent
{
    public LidarScan lidar;

    private static int _slamMapSize = 500;
    public int SlamMapSize => _slamMapSize;

    private static int _realMapSize = 200;
    public float RealMapSize => _realMapSize;

    private Vector3 _worldStartPose;
    public UnityEngine.Vector3 WorldStartPose => _worldStartPose;

    private System.Numerics.Vector3 _mapStartPose = new System.Numerics.Vector3(_realMapSize / 2, _realMapSize / 2, 0);
    public System.Numerics.Vector3 MapStartPose => _mapStartPose;

    public System.Numerics.Vector3 EstimatedPose => slamProcessor.MatchPose;

    public uint[] MapData => throw new NotImplementedException();

    private HectorSLAMProcessor slamProcessor;
    private CoreSLAMProcessor coreSlam;
    private int loops = 0;


    // Start is called before the first frame update
    void Start()
    {

        _worldStartPose = transform.position;

        System.Drawing.Point mapSize = new(SlamMapSize, SlamMapSize);
        slamProcessor = new HectorSLAMProcessor(RealMapSize / SlamMapSize, mapSize, MapStartPose, 1, 4) 
        {
            MinDistanceDiffForMapUpdate = 0.4f,
            MinAngleDiffForMapUpdate = MathEx.DegToRad(30)
        };
        slamProcessor.MapRep.Maps[0].EstimateIterations = 7;

        coreSlam = new CoreSLAMProcessor(RealMapSize, SlamMapSize, 64, MapStartPose, 0.1f, MathEx.DegToRad(10), 1000, 4)
        {
            HoleWidth = 2.0f
        };


        if (lidar != null)
        {
            lidar.ProcessData += UpdateSlam;
        }
    }

    public void UpdateSlam(List<BaseSLAM.Ray> rays)
    {
        var cloud = new ScanCloud()
        {
            Pose = System.Numerics.Vector3.Zero
        };

        List<ScanSegment> segments = new();
        ScanSegment segment = new()
        {
            IsLast = true,
            Pose = coreSlam.Pose,
            Rays = rays
        };



        segments.Add(segment);
        coreSlam.Update(segments);


        foreach (BaseSLAM.Ray ray in rays)
        {
            cloud.Points.Add(new System.Numerics.Vector2()
            {
                X = ray.Radius * MathF.Cos(ray.Angle),
                Y = ray.Radius * MathF.Sin(ray.Angle),
            });
        }
        

        var didUpdate = slamProcessor.Update(cloud, coreSlam.Pose, loops < 10);
        Debug.Log($"UPDATO!!! {slamProcessor.MatchPose.X} {slamProcessor.MatchPose.Y} {slamProcessor.MatchPose.Z} {didUpdate}");
        loops++;
    }

}
