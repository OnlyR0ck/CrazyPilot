using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProgressManager : MonoBehaviour
{
    #region Fields

    [SerializeField] private Image[] _cellsArray;
    [SerializeField] private AnnouncersController _announcersControllerScript;

    
    [SerializeField] private int _available;
    private int _active;
    [SerializeField] private int _collectibles;

    [SerializeField] private Color _activeColor;
    [SerializeField] private Color _unavailableColor;
    [SerializeField] private Color _inactiveColor;

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < _available; i++)
        {
            _cellsArray[i].color = _inactiveColor;
        }
        for (int i = 0; i < _collectibles; i++)
        {
            _cellsArray[i + _available].color = _unavailableColor;
        }
    }

    private void OnEnable()
    {
        HoverCarControl.SpeedIsChanged += ChangeSpeedHandler;
        HoverCarControl.CollectibleIsTaken += HandleChangeLimit;
    }

    private void OnDisable()
    {
        HoverCarControl.SpeedIsChanged -= ChangeSpeedHandler;
        HoverCarControl.CollectibleIsTaken -= HandleChangeLimit;
    }
    private void ChangeSpeedHandler(int actives)
    {
        if (actives != _active)
        {
            StartCoroutine(actives > _active ? IncreaseSpeedProgress(actives) : DecreaseSpeedProgress(actives));
        }
        

    }

    IEnumerator IncreaseSpeedProgress(int actives)
    {
        actives = Mathf.Min(actives, _available);
        for (int i = _active; i < actives; i++)
        {
            yield return new WaitForSeconds(0.1f);
            _cellsArray[i].color = _activeColor;
        }
        
        _active = actives;
    }
    
    IEnumerator DecreaseSpeedProgress(int actives)
    {
        for (int i = Mathf.Min(_active, _cellsArray.Length - 1); i >= actives; i--)
        {
            yield return new WaitForSeconds(0.1f);
            _cellsArray[i].color = _inactiveColor;
        }
        
        _active = actives;
    }

    private void HandleChangeLimit()
    {
        _cellsArray[_available].color = _inactiveColor;
        _available++;
        if (_available == 10)
        {
            _announcersControllerScript.StartAnnouncer("Speed Up For Takeoff");
        }
    }
    
}
