using System.Collections;
using UnityEngine;
using DG.Tweening;

public class CollectibleController : MonoBehaviour
{
    [SerializeField] private GameObject _fuel;
    [SerializeField] private ParticleSystem _pickupParticles;
    [SerializeField] private GameObject _bottomCircle;

    [Header("Animations")] 
    [SerializeField] private Ease _ease;

    [SerializeField, Range(0.0f, 5f)] private float _moveSpeed;
    [SerializeField, Range(0.0f, 5f)] private float _rotateSpeed;

    private bool _isPicked = false;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(MoveAnimation());
        StartCoroutine(RotateAnimation());
    }

    public void CollisionHandler()
    {
        GetComponent<BoxCollider>().enabled = false;
        _fuel.SetActive(false);
        _bottomCircle.SetActive(false);
        _isPicked = true;
        StartCoroutine(PickUp());
    }
    

    IEnumerator PickUp()
    {
        _pickupParticles.Play();
        yield return new WaitForSeconds(_pickupParticles.main.duration);
    }

    IEnumerator MoveAnimation()
    {
        while (!_isPicked)
        {
            _fuel.transform.DOMoveY(3, _moveSpeed).SetEase(_ease);
            yield return new WaitForSeconds(_moveSpeed);
            _fuel.transform.DOMoveY(1, _moveSpeed).SetEase(_ease);
            yield return new WaitForSeconds(_moveSpeed);
        }
    }

    IEnumerator RotateAnimation()
    {
        while (!_isPicked)
        {
            _fuel.transform.DORotate(new Vector3(0, 360, 0), _rotateSpeed, RotateMode.FastBeyond360).SetRelative();
            yield return new WaitForSeconds(_rotateSpeed);
        }
    }
}
