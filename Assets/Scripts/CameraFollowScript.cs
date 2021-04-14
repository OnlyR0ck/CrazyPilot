using Cinemachine;
using DG.Tweening;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    private CinemachineVirtualCamera _mainCamera;
    
    [Header("Shake Options")]
    [SerializeField] private float _shakeDuration;
    [SerializeField] private float _shakePower;
    [SerializeField] private int _vibrato;
    [SerializeField] private float _randomness;

    // Start is called before the first frame update
    void Start()
    {
        _mainCamera = GetComponent<CinemachineVirtualCamera>();
    }

    private void OnEnable()
    {
        Destroyable.BuildingIsDestroyed += ShakeCamera;
    }

    private void OnDisable()
    {
        Destroyable.BuildingIsDestroyed -= ShakeCamera;
    }

    void ShakeCamera()
    {
        
    }
}
