using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DiscardManager))]
public class DiscardAutoLayout : CustomAutoLayout
{
    [SerializeField] private bool _isCardsRotationOn;
    private DiscardManager _myDiscardManager;

    protected override void Awake()
    {
        base.Awake();
        _myDiscardManager = GetComponent<DiscardManager>();
    }

    public void RepositionCardsInDiscard(int playerIndex)
    {
        _cardsNumber = _myDiscardManager._discardList.Count;

        int lastPlayedCardIndex = _cardsRectTransformList.Count - 1;
        float rotationDelta = Random.Range(-15, 15);

        _cardsRectTransformList[lastPlayedCardIndex].localPosition = Vector3.zero;

        //todo : en plus de changer l'orientation, pk pas poser la carte dans un cercle de rayon définissable
        if (_isCardsRotationOn)
        {
            switch (playerIndex)
            {
                case 0: //deck or main player
                    _cardsRectTransformList[lastPlayedCardIndex].localRotation = Quaternion.Euler(0, 0, rotationDelta);
                    break;
                case 1: //left
                    _cardsRectTransformList[lastPlayedCardIndex].localRotation = Quaternion.Euler(0, 0, rotationDelta - 90);
                    break;
                case 2: //top
                    _cardsRectTransformList[lastPlayedCardIndex].localRotation = Quaternion.Euler(0, 0, rotationDelta - 180);
                    break;
                case 3: //right
                    _cardsRectTransformList[lastPlayedCardIndex].localRotation = Quaternion.Euler(0, 0, rotationDelta + 90);
                    break;
            }
        }
    }
}
