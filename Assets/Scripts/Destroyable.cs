using System;
using UnityEngine;

public class Destroyable : MonoBehaviour
{
    public static event Action BuildingIsDestroyed;
    /*
    private void OnCollisionEnter(Collision other)
    {
        var parent = other.transform.parent.parent;
        
        if (parent != null)
        {
            if (parent.CompareTag("Destroyable"))
            {
                BuildingIsDestroyed?.Invoke();
                parent.GetChild(0).gameObject.SetActive(false);
                parent.GetChild(1).gameObject.SetActive(true);
            }
        }
    }
    */

    private void OnTriggerEnter(Collider other)
    {
        var parent = other.transform.parent;

        if (parent != null)
        {
            if (parent.CompareTag("Destroyable"))
            {
                BuildingIsDestroyed?.Invoke();
                parent.GetChild(0).gameObject.SetActive(false);
                parent.GetChild(1).gameObject.SetActive(true);
            }
        }
    }
}
