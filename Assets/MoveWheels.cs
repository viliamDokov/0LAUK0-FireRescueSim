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

    public GameObject minimap;
    public GameObject gameController;
    // Start is called before the first frame update
    void Start()
    {
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

        var LeftMotor = LeftWheelJoint.motor;
        LeftMotor.targetVelocity = speedScale * forwardInput - speedScale * leftrightInput;
        LeftWheelJoint.motor = LeftMotor;

        var RightMotor = RightWheelJoint.motor;
        RightMotor.targetVelocity = speedScale * forwardInput + speedScale * leftrightInput;
        RightWheelJoint.motor = RightMotor;

        //Debug.Log($"({LeftMotor.targetVelocity} {RightMotor.targetVelocity})");
        //Debug.Log($"({forwardInput} {leftrightInput})");
        Transform sensor = gameObject.transform.Find("Cube");
        //Debug.Log(Time.time);

        float temp = gameController.GetComponent<ReadHeatData>().GetCurrentHeatDataPoint(sensor.position.x, 0, sensor.position.z);
        SetFireColor(temp);
    }


    private void SetFireColor(float temp)
    {
        int x = (int)body.position.x;
        int y = (int)body.position.z;
        for (int i = -1; i < 2; i++)
        {
            for (int j = -1; j < 2; j++)
            {
                minimap.GetComponent<MinimapDrawer>().Paint(x + i, y + j, new Color(temp / 100f, 0, 0));
            }
        }
        
    }

    private void SetJointSpeed(HingeJoint joint, float speed) {
        JointMotor motor = joint.motor;
        motor.targetVelocity = speed;
        joint.motor = motor;
    }
}
