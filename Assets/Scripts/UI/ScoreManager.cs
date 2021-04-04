using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class ScoreManager : MonoBehaviour
{
    #region Fields

    private RectTransform _rectTransform;
    private TextMeshProUGUI _scoreTMP;
    private float _score;

    [SerializeField] private int _scoreToAdd;

    [Header("Animations\n")] 
    
    [SerializeField] private float _scaleFactor;

    [SerializeField] private float _scaleScoreDuration;
    [SerializeField] private Color _scoreEndColor; 
    [SerializeField] private Color _scoreStartColor; 
    [SerializeField] private float _scoreColorDuration;

    [Header("Bonus System\n")] 
    
    [SerializeField] private float _bonusTime;
    [SerializeField] private int _multiplier;
    [SerializeField] private TextMeshProUGUI _multiplierTMP;
    private bool _isBonusActive = false;
    private float _bonusTimeTemp;
    
    #endregion
    
    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _scoreTMP = GetComponent<TextMeshProUGUI>();
        
        _scoreTMP.color = _scoreStartColor;
        _scoreTMP.text = "0";
    }

    private void OnEnable()
    {
        Destroyable.BuildingIsDestroyed += UpdateScore;
        Destroyable.BuildingIsDestroyed += AnimateScore;
        Destroyable.BuildingIsDestroyed += Bonuses;
    }

    private void OnDisable()
    {
        Destroyable.BuildingIsDestroyed -= UpdateScore;
        Destroyable.BuildingIsDestroyed -= AnimateScore;
        Destroyable.BuildingIsDestroyed -= Bonuses;

    }
    private void AnimateScore()
    {
        _rectTransform.DOScale(1.5f, 0.1f);
        _scoreTMP.DOColor(_scoreEndColor, 0.1f);
        _rectTransform.DOScale(1f, _scaleScoreDuration);
        _scoreTMP.DOColor(_scoreStartColor, _scoreColorDuration);
    }
    private void UpdateScore()
    {
        _score += _scoreToAdd * _multiplier;
        _scoreTMP.text = $"{_score}";
    }

    private void UpdateMultiplierText()
    {
        _multiplierTMP.text = $"x{_multiplier}";
    }

    private void Bonuses()
    {
        if (_isBonusActive)
        {
            _bonusTimeTemp = _bonusTime;
            _multiplier++;
            UpdateMultiplierText();
        }
        else
        {
            _bonusTimeTemp = _bonusTime;
            _isBonusActive = true;
            StartCoroutine(Timer());
        }
    }

    private IEnumerator Timer()
    {
        while (_bonusTimeTemp > 0)
        {
            yield return new WaitForFixedUpdate();
            _bonusTimeTemp -= Time.fixedDeltaTime;
        }
        
        _isBonusActive = false;
        _multiplier = 1;
        
        UpdateMultiplierText();
    }
}
