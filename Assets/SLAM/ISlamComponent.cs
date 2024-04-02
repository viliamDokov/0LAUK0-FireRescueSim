using BaseSLAM;
using CoreSLAM;
using HectorSLAM.Main;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Tracing;
using Unity.VisualScripting;
using UnityEngine;


public interface ISlamComponent
{
    public int SlamMapSize { get; }
    public float RealMapSize { get; }
    public float scale => SlamMapSize / RealMapSize; // m/pixels
    public Vector3 WorldStartPose { get; }
    public System.Numerics.Vector3 MapStartPose { get; }
    public System.Numerics.Vector3 EstimatedPose { get; }
    public abstract void UpdateSlam(List<BaseSLAM.Ray> cloud);
    public byte[] MapData { get; }

    public System.Numerics.Vector3 WorldPoseToMapPose(Vector3 worldPose)
    {
        worldPose = worldPose - WorldStartPose;
        return new System.Numerics.Vector3((MapStartPose.X - worldPose.x) * scale, (MapStartPose.Y + worldPose.z) * scale, 0);
    }

}
