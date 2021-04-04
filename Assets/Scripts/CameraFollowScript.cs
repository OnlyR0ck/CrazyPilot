using DG.Tweening;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    [SerializeField] private Transform _target;
    [SerializeField] [Range(0f,1f)] private float _smooth;
    [SerializeField] [Range(0f,1f)] private float _turnSmoothVelocity;
    [SerializeField] [Range(0f,1f)] private float _turnSmoothTime;
    [SerializeField] private Transform _rotator;
    private Camera _mainCamera;
    

    private Vector3 _offset;

    private Vector3 _desiredPosition;
    private Vector3 _smoothedPosition;
    
    [Header("Shake Options")]
    [SerializeField] private float _shakeDuration;
    [SerializeField] private float _shakePower;
    [SerializeField] private int _vibrato;
    [SerializeField] private float _randomness;

    // Start is called before the first frame update
    void Start()
    {
        _offset = transform.position - _target.position;
        _mainCamera = GetComponent<Camera>();
    }

    private void OnEnable()
    {
        Destroyable.BuildingIsDestroyed += ShakeCamera;
    }

    private void OnDisable()
    {
        Destroyable.BuildingIsDestroyed -= ShakeCamera;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        //FollowPlayer();
        //Rotate();
    }

    private void FollowPlayer()
    {
        _desiredPosition = _target.position + _offset;
        _smoothedPosition = Vector3.Lerp(transform.position, _desiredPosition, _smooth);
        transform.position = _smoothedPosition;
    }

    private void Rotate()
    {
        transform.rotation = Quaternion.Euler(transform.eulerAngles.x, _target.eulerAngles.y, transform.eulerAngles.z);
    }
    
    
    private void RotateAround(){

        // orientation as an angle when projected onto the XZ plane
        // this functionality is modularise into a separate method because
        // I use it elsewhere
        float playerAngle = AngleOnXZPlane (_target);
        float cameraAngle = AngleOnXZPlane (transform);

        // difference in orientations
        float rotationDiff = Mathf.DeltaAngle(cameraAngle, playerAngle);

        // rotate around target by time-sensitive difference between these angles
        transform.RotateAround(_target.position, Vector3.up, rotationDiff * Time.deltaTime);
    }

// Find the angle made when projecting the rotation onto the xz plane.
// You could pass in the rotation as a parameter instead of the transform.
    private float AngleOnXZPlane(Transform item){

        // get rotation as vector (relative to parent)
        Vector3 direction = item.rotation * _target.forward;

        // return angle in degrees when projected onto xz plane
        return Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;
    }
    
    void ShakeCamera()
    {
        _mainCamera.DOShakeRotation(_shakeDuration, _shakePower, _vibrato, _randomness);
    }
}
