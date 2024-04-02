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
        float forwardInput = Input.GetAxisRaw("Vertical");
        float leftrightInput = Input.GetAxisRaw("Horizontal");

        if (forwardInput < 1f && forwardInput > -1f) {
            forwardInput = 0f;
        }

        if (leftrightInput < 1f && leftrightInput > -1f)
        {
            leftrightInput = 0f;
            float angVelocity = body.GetComponent<Rigidbody>().angularVelocity.y;
            float newAngVelocity;
            if (angVelocity > 0f)
            {
                newAngVelocity = Mathf.Max(0f, angVelocity - 0.2f);
            } else
            {
                newAngVelocity = Mathf.Min(0f, angVelocity + 0.2f);
            }
            body.GetComponent<Rigidbody>().angularVelocity = new Vector3(0, newAngVelocity, 0);
        }

        var LeftMotor = LeftWheelJoint.motor;
        LeftMotor.targetVelocity = speedScale * forwardInput - speedScale * leftrightInput;
        LeftWheelJoint.motor = LeftMotor;

        var RightMotor = RightWheelJoint.motor;
        RightMotor.targetVelocity = speedScale * forwardInput + speedScale * leftrightInput;
        RightWheelJoint.motor = RightMotor;

        //if( forwardInput == 0f && leftrightInput == 0f)
        //{
        //    body.GetComponent<Rigidbody>().velocity = Vector3.zero;
        //    LeftMotor.force = 0;
        //    RightMotor.force = 0;
        //}

        Debug.Log(body.GetComponent<Rigidbody>().angularVelocity);
        //Debug.Log(body.GetComponent<Rigidbody>().velocity);
        //Debug.Log(body.GetComponent<Rigidbody>().);
        Debug.Log($"({LeftMotor.targetVelocity} {RightMotor.targetVelocity})");
        Debug.Log($"({LeftMotor.force} {RightMotor.force})");
        //Debug.Log($"({forwardInput} {leftrightInput})");

        minimap.GetComponent<LidarDrawer>().updateHeatData(heatSensor.position.x, heatSensor.position.z);
    }


    private void SetJointSpeed(HingeJoint joint, float speed) {
        JointMotor motor = joint.motor;
        motor.targetVelocity = speed;
        joint.motor = motor;
    }
}
