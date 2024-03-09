using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveWheels : MonoBehaviour
{
    public float speedScale = 1000; 
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
        float horizontalInput = Input.GetAxis("CarTurn"); 
        float verticalInput = Input.GetAxis("CarForward");

        if (horizontalInput == -1) {
            SetJointSpeed(LeftWheelJoint, 1*speedScale);
            SetJointSpeed(RightWheelJoint, -1* speedScale);
        } else if (horizontalInput == 1) {
            SetJointSpeed(LeftWheelJoint, -1*speedScale);
            SetJointSpeed(RightWheelJoint, 1*speedScale);
        } else if (verticalInput == 1) {
            SetJointSpeed(LeftWheelJoint, 1*speedScale);
            SetJointSpeed(RightWheelJoint, 1*speedScale);
        } else if (verticalInput == -1) {
            SetJointSpeed(LeftWheelJoint, -1*speedScale);
            SetJointSpeed(RightWheelJoint, -1*speedScale);
        } else if (verticalInput ==  0 && horizontalInput == 0) {
            SetJointSpeed(LeftWheelJoint,0); 
            SetJointSpeed(RightWheelJoint,0); 
        }

        // Debug.Log($"Speed: ({LeftWheelJoint.motor.targetVelocity} {RightWheelJoint.motor.targetVelocity})");
        // Debug.Log($"INPUTS: ({horizontalInput} {verticalInput})");
    }


    private void SetJointSpeed(HingeJoint joint, float speed) {
        JointMotor motor = joint.motor;
        motor.targetVelocity = speed;
        joint.motor = motor;
    }
}
