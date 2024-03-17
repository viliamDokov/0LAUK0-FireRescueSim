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

    public event Action<List<LidarReading>> LidarReadingEvent;


    // Start is called before the first frame update
    void Start()
    {
        RobotLayer = 1 << LayerMask.NameToLayer("Robot");
        TargetLayers = ~RobotLayer;
        LidarStep = Mathf.PI*2 / NLidarRays;
    }

    // Update is called once per frame
    void Update()
    {
        List<LidarReading> hitData = new(NLidarRays);
        for(float angle = 0f; angle < Mathf.PI * 2; angle += LidarStep ) {
            Vector3 direction = new Vector3(Mathf.Sin(angle),0, Mathf.Cos(angle));

            RaycastHit hit;
            if (Physics.Raycast(
                origin: transform.position,
                direction: transform.TransformDirection(direction),
                hitInfo: out hit,
                maxDistance: 100,
                layerMask: TargetLayers)
            ) {
                LidarReading reading = new(hit.distance, angle, hit.point);
                hitData.Add(reading);
            }
        }

        LidarReadingEvent?.Invoke(hitData);
    }
}
