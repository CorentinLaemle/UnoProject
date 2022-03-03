using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonOpenMenu : MonoBehaviour
{
    [Tooltip("This script is to be placed on the button that will open the menu." +
        "All buttons that are part of the menu except the one this script is on should be referenced in this array.")][SerializeField] private RectTransform[] _buttons;
    [Tooltip("Final coordinates of the buttons in the buttons array are to be written here")][SerializeField] private Vector2[] _lowButtonCoord;
    [SerializeField] private int _framesBetweenButtonMoves;
    [SerializeField] private int _buttonMovesAmount;
    
    [Tooltip("This filed contains the button one clicks on to roll in/out the menu")][SerializeField] private RectTransform _menuButton;
    private Vector2[] _highButtonCoord;
    private bool _isMenuShrinked;

    private void Awake()
    {
        _highButtonCoord = new Vector2[_buttons.Length];
    }

    void Start()
    {
        CustomGameEvents.GetInstance().OnFirstShuffleEnded += ButtonMenuChangeState;

        for(int i = 0; i < _buttons.Length; i++)
        {
            _highButtonCoord[i] = new Vector2(_lowButtonCoord[i].x, _menuButton.anchoredPosition.y);
            _buttons[i].anchoredPosition = _highButtonCoord[i];
            _buttons[i].gameObject.SetActive(false);
        }
        _isMenuShrinked = true;
    }

    public void ButtonMenuChangeState()
    {
        StartCoroutine(ChangeMenuState());
    }

    private IEnumerator ChangeMenuState()
    {
        int loops = 0;

        if (_isMenuShrinked)
        {
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].gameObject.SetActive(true);
            }

            while(loops < _buttonMovesAmount)
            {
                for (int i = 0; i < _buttons.Length; i++)
                {
                    float delta = _highButtonCoord[i].y - _lowButtonCoord[i].y;

                    float x = _lowButtonCoord[i].x;
                    float y = _highButtonCoord[i].y - delta * (loops + 1) / _buttonMovesAmount;

                    Vector2 v2 = new Vector2(x, y);

                    _buttons[i].anchoredPosition = v2;
                    
                    if(i == _buttons.Length - 1)
                    {
                        loops++;
                    }

                    yield return new WaitForSeconds(_framesBetweenButtonMoves * Time.deltaTime);
                }
            }
        }
        else
        {
            while (loops < _buttonMovesAmount)
            {
                for (int i = 0; i < _buttons.Length; i++)
                {
                    float delta = _highButtonCoord[i].y - _lowButtonCoord[i].y;

                    float x = _highButtonCoord[i].x;
                    float y = _lowButtonCoord[i].y + delta  * (loops + 1) / _buttonMovesAmount;

                    Vector2 v2 = new Vector2(x, y);

                    _buttons[i].anchoredPosition = v2;

                    if (i == _buttons.Length - 1)
                    {
                        loops++;
                    }

                    yield return new WaitForSeconds(_framesBetweenButtonMoves * Time.deltaTime);
                }
            }
            for (int i = 0; i < _buttons.Length; i++)
            {
                _buttons[i].gameObject.SetActive(false);
            }
        }
        _isMenuShrinked = !_isMenuShrinked;
        yield break;
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnFirstShuffleEnded -= ButtonMenuChangeState;
    }
}
