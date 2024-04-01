using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class MoveWheels : MonoBehaviour
{
    public float speedScale = 1000; 
    private HingeJoint RightWheelJoint; 
    private HingeJoint LeftWheelJoint;
    private Transform body;
    private Transform heatSensor;
    public GameObject minimap;

    // Start is called before the first frame update
    void Start()
    {
        heatSensor = gameObject.transform.Find("Cube");
        body = transform.Find("Cube");
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

        if (forwardInput < 1f && forwardInput > -1f) {
            forwardInput = 0f;
        }

        if (leftrightInput < 1f && leftrightInput > -1f)
        {
            leftrightInput = 0f;
        }

        var LeftMotor = LeftWheelJoint.motor;
        LeftMotor.targetVelocity = speedScale * forwardInput - speedScale * leftrightInput;
        LeftWheelJoint.motor = LeftMotor;

        var RightMotor = RightWheelJoint.motor;
        RightMotor.targetVelocity = speedScale * forwardInput + speedScale * leftrightInput;
        RightWheelJoint.motor = RightMotor;

        //Debug.Log($"({LeftMotor.targetVelocity} {RightMotor.targetVelocity})");
        //Debug.Log($"({forwardInput} {leftrightInput})");
        
        minimap.GetComponent<LidarDrawer>().updateHeatData(heatSensor.position.x, heatSensor.position.z);
    }


    private void SetJointSpeed(HingeJoint joint, float speed) {
        JointMotor motor = joint.motor;
        motor.targetVelocity = speed;
        joint.motor = motor;
    }
}
