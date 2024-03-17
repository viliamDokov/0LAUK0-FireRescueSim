using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LidarVisualizer : MonoBehaviour
{
    // THIS IS CRAZY SLOW;
    public LidarScan Lidar;
    public GameObject LidarBall;

    private List<GameObject> previosBalls = new();
    // Start is called before the first frame update
    void Start()
    {
        if (Lidar != null)
        {
            Lidar.LidarReadingEvent += HandleLidarReadings;
        }
    }

    private void HandleLidarReadings(List<LidarReading> readings) 
    {
        foreach(var ball in previosBalls) {
            Destroy(ball);
        }
        previosBalls = new();
        foreach(LidarReading reading in readings) 
        {
            previosBalls.Add(Instantiate(LidarBall, reading.point, Quaternion.identity));
        }
    }
}
