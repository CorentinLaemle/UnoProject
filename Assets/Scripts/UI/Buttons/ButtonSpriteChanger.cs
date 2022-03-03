using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonSpriteChanger : MonoBehaviour
{
    public bool _hasRotated;

    [SerializeField] private Sprite _stateSprite0;
    [SerializeField] private Sprite _stateSprite1;
    [SerializeField] private float _rotationDelay;
    [SerializeField] private float _rotationAmount;
    [SerializeField][Min(1)] private int _maxRotationSteps;

    private Image _buttonImage;
    private RectTransform _rT;
    private bool _isRotating;

    private void Awake()
    {
        _buttonImage = GetComponent<Image>();
        _rT = GetComponent<RectTransform>();
    }

    private void Start()
    {
        _isRotating = false;
        _hasRotated = true;
    }

    public void ChangeActiveSprite()
    {
        if(_buttonImage.sprite == _stateSprite0)
        {
            _buttonImage.sprite = _stateSprite1;
            return;
        }
        _buttonImage.sprite = _stateSprite0;
    }

    public void RotateActiveSprite()
    {
        if (!_isRotating)
        {
            _isRotating = true;
            StartCoroutine(Rotate());
        }
    }

    private IEnumerator Rotate()
    {
        bool isContinue = true;
        int currentRotSteps = 0;
        Vector3 rot = new Vector3(0, 0, _rotationAmount);
        
        while (isContinue)
        {
            yield return new WaitForSeconds(_rotationDelay);
            _rT.Rotate(rot);
            currentRotSteps++;

            if(currentRotSteps >= _maxRotationSteps)
            {
                isContinue = false;
            }
        }
        _rotationAmount *= -1;
        _isRotating = false;
        _hasRotated = !_hasRotated;
        yield break;
    }
}
