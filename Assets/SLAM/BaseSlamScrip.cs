using BaseSLAM;
using CoreSLAM;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BaseSlamScript : MonoBehaviour, ISlamComponent
{
    public LidarScan lidar;
    
    public int _slamMapSize = 350;
    public float _realMapSize = 200;
    private Vector3 _worldStartPose;
    private System.Numerics.Vector3 _mapStartPose;
    public int SlamMapSize => _slamMapSize;

    public float RealMapSize => _realMapSize;

    public UnityEngine.Vector3 WorldStartPose => _worldStartPose;

    public System.Numerics.Vector3 MapStartPose => _mapStartPose;

    public uint[] MapData => ConvertData();
    private uint[] mapData;
    public System.Numerics.Vector3 EstimatedPose => SLAMProcessor.Pose;

    private CoreSLAMProcessor SLAMProcessor;
    

    // Start is called before the first frame update
    void Start()
    {
        _mapStartPose = new System.Numerics.Vector3(_realMapSize / 2, _realMapSize / 2, 0);
        mapData = new uint[_slamMapSize * _slamMapSize];


        SLAMProcessor = new CoreSLAMProcessor(RealMapSize, SlamMapSize, 64, MapStartPose, 0.1f, MathEx.DegToRad(10), 2000, 4)
        {
            HoleWidth = 2.0f
        };

        _worldStartPose = transform.position;

        if (lidar != null)
        {
            lidar.ProcessData += UpdateSlam;
        }
    }

    private uint[] ConvertData()
    {
        
        for (int i = 0; i < mapData.Length; i++)
        {
            mapData[i] = (uint)(SLAMProcessor.HoleMap.Pixels[i] >> 8);
            //if (result[i] > 100)
            //{
            //    Debug.Log($"BIGGER {i}");
            //}
        }
        return mapData;
    }

    public void UpdateSlam(List<BaseSLAM.Ray> rays)
    {
        //Debug.Log("UPDATING SLAM");
        List<ScanSegment> segments = new();
        ScanSegment segment = new() { 
            IsLast = true,
            Pose = SLAMProcessor.Pose,
            Rays = rays
        };



        segments.Add(segment);
        SLAMProcessor.Update(segments);
    }
}
