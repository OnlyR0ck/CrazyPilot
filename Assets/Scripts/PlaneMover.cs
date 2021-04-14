using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public class PlaneMover : MonoBehaviour
{
    private Rigidbody _planeRigidbody;
    [SerializeField] private Transform _centerOfMass;

    [Header("Move Parameters")] private float currentSteerAngle;
    [SerializeField] private float _speed;
    [SerializeField] private float _maxSpeed;
    [SerializeField] private float _steering;

    //Input
    [SerializeField] private FixedJoystick _joystick;
    private Vector2 _direction;

    //Other
    private IEnumerator _coroutine;

    [Header("Final Flight")] [SerializeField]
    private float _height;

    [SerializeField] private Vector3 _angle;
    [SerializeField, Range(0.5f, 2)] private float _duration;

    private int _speedStep = 3;
    
    public static event Action CollectibleIsTaken;
    public static event Action<int> SpeedIsChanged;
    public static event Action<bool> LevelWasEnded;

    void Start()
    {
        _planeRigidbody = GetComponent<Rigidbody>();
        _planeRigidbody.centerOfMass = _centerOfMass.localPosition;
        StartCoroutine(CheckSpeed());

    }

    private void FixedUpdate()
    {
        _planeRigidbody.AddRelativeForce(Vector3.forward * (_direction.y * _speed), ForceMode.Acceleration);
        //_planeRigidbody.AddRelativeTorque(Vector3.up * (_direction.x * _steering), ForceMode.VelocityChange);
        _planeRigidbody.DORotate(Vector3.up * (_steering * _direction.x), 0.0f).SetRelative();
    }
    
    

    private void Update()
    {
        GetInput();
        ClampSpeed();
    }

    /*private void Movement()
    {
        transform.DOLocalMoveZ(_speed * _direction.y * Time.deltaTime, 0.0f);
        transform.DORotate(Vector3.up * (_steering * _direction.x * Time.deltaTime), 0.0f).SetRelative();
    }*/

    private void GetInput()
    {
        _direction = _joystick.Direction;
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

    IEnumerator CheckSpeed()
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
        transform.DORotate(_angle, _duration).SetEase(Ease.InOutSine).SetRelative().onComplete += () =>
        {
            transform.DORotate(-_angle, _duration).SetEase(Ease.Linear).SetRelative();
        };
    }
    
}
