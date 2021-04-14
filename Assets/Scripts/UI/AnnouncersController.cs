using System.Collections;
using UnityEngine;
using DG.Tweening;
using TMPro;

public class AnnouncersController : MonoBehaviour
{

    [SerializeField] private TextMeshProUGUI _announcerTMP;
    [SerializeField] private RectTransform _announcerRectTransform;
    [SerializeField] private float _animationDuration;
    private Vector3 _originalPosition;
    private Vector3 _endPosition;

    private void Start()
    {
        _announcerRectTransform = GetComponent<RectTransform>();
        _originalPosition = _announcerRectTransform.anchoredPosition;
        _endPosition = new Vector3(0, 580);
        
        StartAnnouncer("Collect fuel!");
    }
    public void StartAnnouncer(string text)
    {
        _announcerTMP.text = $"{text}";
        gameObject.SetActive(true);
        StartCoroutine(ShowAnnouncer());

    }

    IEnumerator ShowAnnouncer()
    {
        _announcerRectTransform.DOAnchorPos(_endPosition, _animationDuration).SetEase(Ease.OutBounce);
        yield return new WaitForSeconds(_animationDuration + 1);
        _announcerRectTransform.DOAnchorPos(_originalPosition, _animationDuration).SetEase(Ease.InBounce);
        yield return new WaitForSeconds(_animationDuration);
        gameObject.SetActive(false);
    }
}
