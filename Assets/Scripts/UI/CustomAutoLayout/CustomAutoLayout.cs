using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CustomAutoLayout : MonoBehaviour
{
    [SerializeField] protected GameObject _cardPrefab;

    public List<RectTransform> _cardsRectTransformList;

    protected RectTransform _rectTransform;
    protected RectTransform _cardRectTransform;
    protected int _cardsNumber;

    protected virtual void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _cardRectTransform = _cardPrefab.GetComponent<RectTransform>();
        _cardsRectTransformList = new List<RectTransform>();
    }
}
