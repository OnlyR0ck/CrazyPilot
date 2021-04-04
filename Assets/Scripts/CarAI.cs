using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
public class CarAI : MonoBehaviour
{
    private NavMeshAgent _agent;

    [SerializeField] private List<Transform> _walkPointsList;
    private Queue<Transform> _walkPoints;
    
    private Transform _currentWalkPoint;
    private Vector3 _distanceToTheWalkPoint;
    private bool _walkPointSet;

    [SerializeField] private ParticleSystem _explosionParticles;
    
    // Start is called before the first frame update
    void Start()
    {
        _agent = GetComponent<NavMeshAgent>();
        _walkPoints = new Queue<Transform>();

        foreach (var pos in _walkPointsList)
        {
            _walkPoints.Enqueue(pos);
        }
    }

    private void Patrolling()
    {
        if (!_walkPointSet)
        {
            _currentWalkPoint = _walkPoints.Dequeue();
            _walkPointSet = true;
        }
        else if(_agent.enabled) _agent.SetDestination(_currentWalkPoint.position);

        _distanceToTheWalkPoint = transform.position - _currentWalkPoint.position;

        if (_distanceToTheWalkPoint.magnitude < 1)
        {
            _walkPointSet = false;
            _walkPoints.Enqueue(_currentWalkPoint);
        }
    }
    void Update()
    {
        Patrolling();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Player"))
        {
            _agent.enabled = false;
            GetComponent<Rigidbody>().AddForce(other.transform.forward * 10, ForceMode.VelocityChange);
            StartCoroutine(ExplodeCar());
        }
    }

    IEnumerator ExplodeCar()
    {
        _explosionParticles.Play();
        yield return new WaitForSeconds(2);
        gameObject.SetActive(false);
    }
}
