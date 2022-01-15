using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RectTransform))]
public class CustomAutoLayout : MonoBehaviour
{
    public List<RectTransform> _cardsRectTransformList;

    [SerializeField] protected GameObject _cardPrefab;

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
