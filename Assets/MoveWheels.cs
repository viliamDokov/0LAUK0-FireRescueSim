using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWheels : MonoBehaviour
{
    public float speedScale = 10; 
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
        float rightInput = Input.GetAxis("CarRight"); 
        float leftInput = Input.GetAxis("CarLeft");

        var LeftMotor = LeftWheelJoint.motor;
        LeftMotor.targetVelocity += -speedScale * leftInput;
        LeftWheelJoint.motor = LeftMotor;

        var RightMotor = RightWheelJoint.motor;
        RightMotor.targetVelocity += -speedScale * rightInput;
        RightWheelJoint.motor = RightMotor;

        Debug.Log($"({LeftMotor.targetVelocity} {RightMotor.targetVelocity})");
        Debug.Log($"({rightInput} {leftInput})");
    }
}
