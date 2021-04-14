using System;
using UnityEngine;
using System.Collections;

public class HoverCarControl : MonoBehaviour
{
    [SerializeField] private FixedJoystick _joystick;
    Rigidbody body;
    float deadZone = 0.1f;
    public float groundedDrag = 3f;
    public float maxVelocity = 50;
    public float hoverForce = 1000;
    public float gravityForce = 1000f;
    public float hoverHeight = 1.5f;
    public GameObject[] hoverPoints;

    public float forwardAcceleration = 8000f;
    public float reverseAcceleration = 4000f;
    float thrust = 0f;

    public float turnStrength = 1000f;
    float turnValue = 0f;

    public ParticleSystem[] dustTrails = new ParticleSystem[2];

    int layerMask;
    private IEnumerator _coroutine;
    private int _maxSpeed = 16;
    private int _speedStep = 3;

    public static event Action CollectibleIsTaken;
    public static event Action<int> SpeedIsChanged;
    public static event Action<bool> LevelWasEnded;

    void Start()
    {
        body = GetComponent<Rigidbody>();
        body.centerOfMass = Vector3.down;

        layerMask = 1 << LayerMask.NameToLayer("player");
        layerMask = ~layerMask;
        
        StartCoroutine(CheckSpeed());
    }

// Uncomment this to see a visual indication of the raycast hit points in the editor window
//  void OnDrawGizmos()
//  {
//
//    RaycastHit hit;
//    for (int i = 0; i < hoverPoints.Length; i++)
//    {
//      var hoverPoint = hoverPoints [i];
//      if (Physics.Raycast(hoverPoint.transform.position, 
//                          -Vector3.up, out hit,
//                          hoverHeight, 
//                          layerMask))
//      {
//        Gizmos.color = Color.blue;
//        Gizmos.DrawLine(hoverPoint.transform.position, hit.point);
//        Gizmos.DrawSphere(hit.point, 0.5f);
//      } else
//      {
//        Gizmos.color = Color.red;
//        Gizmos.DrawLine(hoverPoint.transform.position, 
//                       hoverPoint.transform.position - Vector3.up * hoverHeight);
//      }
//    }
//  }

    void Update()
    {
        GetInput();
        ClampSpeed();
    }

    void FixedUpdate()
    {
        //  Do hover/bounce force
        RaycastHit hit;
        bool grounded = false;
        for (int i = 0; i < hoverPoints.Length; i++)
        {
            var hoverPoint = hoverPoints[i];
            if (Physics.Raycast(hoverPoint.transform.position, -Vector3.up, out hit, hoverHeight, layerMask))
            {
                body.AddForceAtPosition(Vector3.up * (hoverForce * (1.0f - (hit.distance / hoverHeight))),
                    hoverPoint.transform.position);
                grounded = true;
            }
            /*else
            {
                // Self levelling - returns the vehicle to horizontal when not grounded and simulates gravity
                if (transform.position.y > hoverPoint.transform.position.y)
                {
                    body.AddForceAtPosition(hoverPoint.transform.up * gravityForce, hoverPoint.transform.position);
                }
                else
                {
                    body.AddForceAtPosition(hoverPoint.transform.up * -gravityForce, hoverPoint.transform.position);
                }
            }*/
        }

        var emissionRate = 0;
        if (grounded)
        {
            body.drag = groundedDrag;
            emissionRate = 10;
        }
        else
        {
            body.drag = 0.1f;
            thrust /= 100f;
            turnValue /= 100f;
        }
        for (int i = 0; i < dustTrails.Length; i++)
        {
            var emission = dustTrails[i].emission;
            emission.rate = new ParticleSystem.MinMaxCurve(emissionRate);
        }

        // Handle Forward and Reverse forces
        if (Mathf.Abs(thrust) > 0)
            body.AddForce(transform.forward * thrust, ForceMode.Acceleration);

        // Handle Turn forces
        if (turnValue > 0)
        {
            body.AddRelativeTorque(Vector3.up * (turnValue * turnStrength));
        }
        else if (turnValue < 0)
        {
            body.AddRelativeTorque(Vector3.up * (turnValue * turnStrength));
        }

        // Limit max velocity
        /*if (body.velocity.sqrMagnitude > (body.velocity.normalized * maxVelocity).sqrMagnitude)
        {
            body.velocity = body.velocity.normalized * maxVelocity;
        }*/
    }

    private void GetInput()
    {
        // Main Thrust
        thrust = 0.0f;
        float acceleration = _joystick.Vertical;
        if (acceleration > deadZone)
            thrust = acceleration * forwardAcceleration;
        else if (acceleration < -deadZone)
            thrust = acceleration * reverseAcceleration;
 
        // Turning
        turnValue = 0.0f;
        float turnAxis = _joystick.Horizontal;
        if (Mathf.Abs(turnAxis) > deadZone)
            turnValue = turnAxis;
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

            var currentSpeed = (int) body.velocity.magnitude / _speedStep;
            if (currentSpeed != previous)
            {
                if (_coroutine != null) StopCoroutine(_coroutine);

                SpeedIsChanged?.Invoke(currentSpeed);
                previous = currentSpeed;
                Debug.Log(currentSpeed);
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
        }
        else if (delay == 5)
        {
            LevelWasEnded?.Invoke(false);
            Debug.Log("Lose");
        }
    }
    
    private void ClampSpeed()
    {
        body.velocity = Vector3.ClampMagnitude(body.velocity, _maxSpeed);
    }
}
