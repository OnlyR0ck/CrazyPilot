using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;

public class PlaneController : MonoBehaviour
{
    private Rigidbody _planeRigidbody;
    [SerializeField] private Transform _centerOfMass;
    

    //Input
    private Vector3 _startMousePosition;
    private Vector3 _currentMousePosition;
    private Vector3 _deltaMouse;
    private Vector3 _deltaMouseNormalized;

    //Other
    private IEnumerator _coroutine;

    [Header("Final Flight")] [SerializeField]
    private float _height;

    [SerializeField] private Vector3 _angle;
    [SerializeField, Range(0.5f, 2)] private float _duration;

    [Header("Move Parameters")] private float currentSteerAngle;
    private float currentbreakForce;
    private bool isBreaking;
    private int _speedStep = 3;

    [SerializeField] private float motorForce;
    [SerializeField] private float breakForce;
    [SerializeField] private float maxSteerAngle;

    [SerializeField] private WheelCollider frontLeftWheelCollider;
    [SerializeField] private WheelCollider frontRightWheelCollider;
    [SerializeField] private WheelCollider rearLeftWheelCollider;
    [SerializeField] private WheelCollider rearRightWheelCollider;
    [SerializeField] private int _maxSpeed;

    public static event Action CollectibleIsTaken;
    public static event Action<int> SpeedIsChanged;
    public static event Action<bool> LevelWasEnded;

    private void Start()
    {
        _planeRigidbody = GetComponent<Rigidbody>();
        _planeRigidbody.centerOfMass = _centerOfMass.localPosition;

        StartCoroutine(PrintSpeed());
    }

    private void FixedUpdate()
    {
        HandleMotor();
        HandleSteering();
    }


    private void HandleMotor()
    {
        frontLeftWheelCollider.motorTorque = _deltaMouseNormalized.y * motorForce;
        frontRightWheelCollider.motorTorque = _deltaMouseNormalized.y * motorForce;
        currentbreakForce = isBreaking ? breakForce : 0f;
        ApplyBreaking();
    }

    private void ApplyBreaking()
    {
        frontRightWheelCollider.brakeTorque = currentbreakForce;
        frontLeftWheelCollider.brakeTorque = currentbreakForce;
        rearLeftWheelCollider.brakeTorque = currentbreakForce;
        rearRightWheelCollider.brakeTorque = currentbreakForce;
    }

    private void HandleSteering()
    {
        currentSteerAngle = maxSteerAngle * _deltaMouseNormalized.x;
        frontLeftWheelCollider.steerAngle = currentSteerAngle;
        frontRightWheelCollider.steerAngle = currentSteerAngle;
    }

    /*private void UpdateWheels()
    {
        UpdateSingleWheel(frontLeftWheelCollider, frontLeftWheelTransform);
        UpdateSingleWheel(frontRightWheelCollider, frontRightWheeTransform);
        UpdateSingleWheel(rearRightWheelCollider, rearRightWheelTransform);
        UpdateSingleWheel(rearLeftWheelCollider, rearLeftWheelTransform);
    }*/

    private void UpdateSingleWheel(WheelCollider wheelCollider, Transform wheelTransform)
    {
        Vector3 pos;
        Quaternion rot;
        wheelCollider.GetWorldPose(out pos, out rot);
        wheelTransform.rotation = rot;
        wheelTransform.position = pos;
    }


    private void Update()
    {
        GetInput();
        ClampSpeed();
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

    private void ClampSpeed()
    {
        _planeRigidbody.velocity = Vector3.ClampMagnitude(_planeRigidbody.velocity, _maxSpeed);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Collectible"))
        {
            _maxSpeed += _speedStep;
            CollectibleIsTaken?.Invoke();
            other.GetComponent<CollectibleController>().CollisionHandler();
        }
    }

    IEnumerator PrintSpeed()
    {
        var previous = 0;
        while (true)
        {
            yield return new WaitForSeconds(1);

            var currentSpeed = (int) _planeRigidbody.velocity.magnitude / 3;
            if (currentSpeed != previous)
            {
                if (_coroutine != null) StopCoroutine(_coroutine);

                SpeedIsChanged?.Invoke(currentSpeed);
                previous = currentSpeed;

                switch (currentSpeed)
                {
                    case 10:
                        _coroutine = LevelChangeHandler(2);
                        StartCoroutine(_coroutine);
                        break;
                    case 0:
                        _coroutine = LevelChangeHandler(5);
                        StartCoroutine(_coroutine);
                        break;
                }
            }
        }
    }

    IEnumerator LevelChangeHandler(int delay)
    {
        yield return new WaitForSeconds(delay);
        if (delay == 2)
        {
            LevelWasEnded?.Invoke(true);
            Debug.Log("Win");
            Flight();
        }
        else if (delay == 5)
        {
            LevelWasEnded?.Invoke(false);
            Debug.Log("Lose");
        }
    }

    private void Flight()
    {
        _planeRigidbody.useGravity = false;
        _planeRigidbody.DOMoveY(_height, _duration).SetEase(Ease.Linear);
        //transform.DOMoveY(_height, _duration).SetEase(Ease.Linear);
        transform.DORotate(_angle, _duration).SetEase(Ease.InOutSine).SetRelative().onComplete += () =>
        {
            transform.DORotate(-_angle, _duration).SetEase(Ease.Linear).SetRelative();
        };
    }
}
