using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeckManager : MonoBehaviour
{
    private static DeckManager _instance;
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
    }

    //The decklist that contains all the 108 cards. Mustn't be modified at playtime !
    [SerializeField] private DeckList _unoDeckList;
    [SerializeField] private DiscardManager _discardManager;

    private List<Card> _bufferDeckList; //This list is used as an intermediary list when shuffling the deck ; it should be empty at all times except when shuffling
    private List<Card> _cardsList;      //This is the actual in-game list of cards actually in the deck

    private int _deckCardsAmount;
    private bool _isFirstShuffle;

    private void Start()
    {
        //This should be the only method from a manager script called on Start(), the rest follow the order imposed by the calling of custom game events
        PrepareDeck();
    }

    //This method is called at the start of the game.
    private void PrepareDeck()
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

    public void InitShuffle()
    {
        if(!_isFirstShuffle)
        {
            //on rajoute dans _bufferDeckList les cartes de la défausse (sauf la dernière), et les éventuelles cartes restantes dans la bibliothèque
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
        Shuffle(_isFirstShuffle);
        _isFirstShuffle = false;
    }

    private void Shuffle(bool isFirstShuffle)
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

        if(isFirstShuffle)
        { 
            CustomGameEvents.GetInstance().FirstShuffleEnded(); 
        }
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
        Card drawnCard = _cardsList[0];
        _cardsList.RemoveAt(0);
        return drawnCard;
    }
}
