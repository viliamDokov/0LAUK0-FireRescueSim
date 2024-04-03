using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWheels2 : MonoBehaviour
{
    public float speedScale = 200;
    public GameObject gameController;
    private HingeJoint RightWheelJoint; 
    private HingeJoint LeftWheelJoint;

    // Start is called before the first frame update
    void Start()
    {
        int nChildren = gameObject.transform.childCount;
        for(int i = 0; i < nChildren; i++){
            GameObject child = gameObject.transform.GetChild(i).gameObject;
            if (child.name == "WheelLeft") {
                LeftWheelJoint = child.GetComponent<HingeJoint>();
            }
            if (child.name == "WheelRight") {
                RightWheelJoint =  child.GetComponent<HingeJoint>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        float forwardInput = Input.GetAxis("Vertical");
        float leftrightInput = Input.GetAxis("Horizontal");

        var LeftMotor = LeftWheelJoint.motor;
        LeftMotor.targetVelocity = -speedScale * forwardInput - speedScale * leftrightInput;
        LeftWheelJoint.motor = LeftMotor;

        var RightMotor = RightWheelJoint.motor;
        RightMotor.targetVelocity = -speedScale * forwardInput + speedScale * leftrightInput;
        RightWheelJoint.motor = RightMotor;

        //Debug.Log($"({LeftMotor.targetVelocity} {RightMotor.targetVelocity})");
        //Debug.Log($"({forwardInput} {leftrightInput})");
        Transform sensor = gameObject.transform.Find("Cube");
        //Debug.Log(Time.time);

        //Debug.Log(gameController.GetComponent<ReadHeatData>().GetCurrentHeatDataPoint(sensor.position.x, 0, sensor.position.z));
    }
}
