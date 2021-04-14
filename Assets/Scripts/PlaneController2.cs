using System;
using UnityEngine;
using UnityStandardAssets.Vehicles.Aeroplane;
using UnityStandardAssets.Vehicles.Car;


public class PlaneController2 : MonoBehaviour
{
    private Vector3 _startMousePosition;
    private Vector3 _currentMousePosition;
    private Vector3 _deltaMouse;
    private Vector3 _deltaMouseNormalized;
    private Rigidbody _planeRigidbody;
    private float _clampSpeed;

    private AeroplaneController _controller;


    private void Start()
    {
        _planeRigidbody = GetComponent<Rigidbody>();
        _controller = GetComponent<AeroplaneController>();
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

    private void LerpSpeed()
    {
        _planeRigidbody.velocity = Vector3.ClampMagnitude(_planeRigidbody.velocity, _clampSpeed);
    }

    private void Update()
    {
        GetInput();

        _controller.Move(_deltaMouseNormalized.y, _deltaMouseNormalized.y, 0, 0, false);
        LerpSpeed();
    }
}
