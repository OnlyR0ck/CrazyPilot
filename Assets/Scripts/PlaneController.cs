using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}
     
public class PlaneController : MonoBehaviour 
{
    public List<AxleInfo> axleInfos; 
    public float maxMotorTorque;
    public float maxSteeringAngle;
    private Rigidbody _planeRigidbody;
    [SerializeField] private Transform _centerOfMass;
    
    //Input
    private Vector3 _startMousePosition;
    private Vector3 _currentMousePosition;
    private Vector3 _deltaMouse;

    private Vector3 _deltaMouseNormalized;
    

    private void Start()
    {
        _planeRigidbody = GetComponent<Rigidbody>();
        _planeRigidbody.centerOfMass = _centerOfMass.localPosition;
    }

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0) {
            return;
        }
     
        Transform visualWheel = collider.transform.GetChild(0);

        collider.GetWorldPose(out var position, out var rotation);
     
        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }
     
    public void FixedUpdate()
    {
        float motor = maxMotorTorque * _deltaMouseNormalized.y;
        float steering = maxSteeringAngle * _deltaMouseNormalized.x;
     
        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }
            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }
    }

    private void Update()
    {
        GetInput();
    }

    private void GetInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _startMousePosition = Input.mousePosition;
        }
        else if (Input.GetKey(KeyCode.Mouse0))
        {
            _currentMousePosition = Input.mousePosition;
            _deltaMouse = _currentMousePosition - _startMousePosition;
            _deltaMouseNormalized = _deltaMouse.normalized;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _startMousePosition = Vector3.zero;
            _currentMousePosition = Vector3.zero;
        }
    }
}
