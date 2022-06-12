using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeckManager : MonoBehaviour
{
    private static DeckManager _instance;

    //The decklist that contains all the 108 cards. Mustn't be modified at playtime !
    [SerializeField] private DeckList _unoDeckList;
    [SerializeField] private DiscardManager _discardManager;
    [SerializeField] private GameObject _myOutline;
    [SerializeField] private AudioSource _source;
    [SerializeField] private Animator _animator;
    [SerializeField] private AnimationClip _shuffleAnim;

    [SerializeField] private List<Card> _cardsList; //This is the actual in-game list of cards actually in the deck
    private List<Card> _bufferDeckList;             //This list is used as an intermediary list when shuffling the deck ; it should be empty at all times except when shuffling

    private int _deckCardsAmount;
    private bool _isFirstShuffle;
    private Button _myButton;

    public static DeckManager GetInstance()
    {
        return _instance;
    }
    private void Awake()
    {
        if (_instance != null)
        {
            Destroy(this);
        }
        _instance = this;

        _myButton = _myOutline.GetComponent<Button>();
    }

    private void Start()
    {
        CustomGameEvents.GetInstance().OnPlayerMustDraw += ActivateButton;

        AudioManager.GetInstance().SetAudioSource(_source, "SHUFFLE");

        _myOutline.SetActive(false);
        PrepareDeck(); //This should be the only method related to cards business called on Start() from a manager script
    }

    private void PrepareDeck() //This method is called at the start of the game.
    {
        _isFirstShuffle = true;
        _cardsList = new List<Card>();
        _bufferDeckList = new List<Card>();

        for (int i = 0; i < _unoDeckList.cards.Count; i++)
        {
            _bufferDeckList.Add(_unoDeckList.cards[i]);
        }
        InitShuffle();
    }

    private void InitShuffle()
    {
        CustomGameEvents.GetInstance().ShuffleStart(); //This event is listened to by the GameManager script, which will forbid the players from interacting with the game while the deck is shuffling

        if(!_isFirstShuffle)
        {
            //on rajoute dans _bufferDeckList les cartes de la d�fausse (sauf la derni�re), et les �ventuelles cartes restantes dans la biblioth�que
            if (_discardManager._discardList.Count > 1)
            {
                int discardCards = _discardManager._discardList.Count;

                for(int i = 0; i < discardCards - 1; i++)
                {
                    _bufferDeckList.Add(_discardManager.DeleteCardFromDiscard());
                }
            }
            if(_cardsList.Count != 0)
            {
                int deckCards = _cardsList.Count;

                for(int j = 0; j < deckCards; j++)
                {
                    _bufferDeckList.Add(_cardsList[0]);
                    _cardsList.RemoveAt(0);
                }
            }
        }
        _deckCardsAmount = _bufferDeckList.Count;
        BeginAnimation();
    }

    private void BeginAnimation()
    {
        _animator.Play(_shuffleAnim.name);
        Invoke(nameof(PlaySound), 0.3f); //the animation needs a bit of time to be ready when the sound begins
    }

    private void PlaySound()
    {
        AudioManager.GetInstance().PlayPitchedSound(_source);

        Invoke(nameof(Shuffle), _shuffleAnim.length);
    }

    private void Shuffle()
    {
        int minRandom = 0;
        int maxRandom = _deckCardsAmount;
        int index;

        for(int i = 0; i < _deckCardsAmount; i++)
        {
            index = Random.Range(minRandom, maxRandom);
            _cardsList.Add(_bufferDeckList[index]);
            _bufferDeckList.RemoveAt(index);
            maxRandom--;
        }

        if(_isFirstShuffle)
        {
            _isFirstShuffle = false;
            CustomGameEvents.GetInstance().FirstShuffleEnded();
            return;
        }
        CustomGameEvents.GetInstance().ShuffleEnd();
    }

    public bool CheckDrawPossible(int cardNumber)
    {
        if(cardNumber <= _cardsList.Count)
        {
            return true;
        }
        else
        {
            InitShuffle();
            return false;
        }
    }

    public Card DrawOneCard()
    {
        _myOutline.SetActive(false);
        _myButton.interactable = false;

        Card drawnCard = _cardsList[0];
        _cardsList.RemoveAt(0);
        return drawnCard;
    }

    private void ActivateButton()
    {
        if(UnoGameMaster.GetInstance().ActivePlayer == UnoGameMaster.GetInstance().MainPlayerIndex)
        {
            _myOutline.SetActive(true);
            _myButton.interactable = true;
        }
    }

    private void OnDestroy()
    {
        CustomGameEvents.GetInstance().OnPlayerMustDraw -= ActivateButton;
    }
}
