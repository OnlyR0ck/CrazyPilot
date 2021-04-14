using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class SpawnCollectables : MonoBehaviour
{
    private List<Transform> _points;

    [SerializeField] private GameObject _collectablePrefab;
    
    // Start is called before the first frame update
    void Start()
    {
        _points = new List<Transform>();
        for (int i = 0; i < transform.childCount; i++)
        {
            _points.Add(transform.GetChild(i));
        }
        for (int i = 0; i < 5; i++)
        {
            var index = Random.Range(0, _points.Count);
            Instantiate(_collectablePrefab, _points[index].position, transform.rotation);
            _points.RemoveAt(index);
        }
    }
}
