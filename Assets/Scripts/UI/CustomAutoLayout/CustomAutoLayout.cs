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

    protected int _cardHeight;
    protected int _cardWidth;
    protected int _cardsNumber;

    protected virtual void Awake()
    {
        _rectTransform = gameObject.GetComponent<RectTransform>();
        _cardRectTransform = _cardPrefab.GetComponent<RectTransform>();
        _cardsRectTransformList = new List<RectTransform>();

        _cardHeight = (int)_cardRectTransform.rect.height;
        _cardWidth = (int)_cardRectTransform.rect.width;
    }
}
