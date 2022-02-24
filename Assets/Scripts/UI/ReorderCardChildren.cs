using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReorderCardChildren : MonoBehaviour
{
    [SerializeField] private Vector3Int[] _cardOrders;
    private GameObject[] _cardObjects;
    private int[] _tempIndexes;

    private void Start()
    {
        _cardObjects = new GameObject[transform.childCount];
        _tempIndexes = new int[_cardObjects.Length];

        for (int i = 0; i < transform.childCount; i++)
        {
            _cardObjects[i] = transform.GetChild(i).gameObject;
        }
    }

    public void ReorderChildren(int currentStep)
    {
        //We first determine which order the card children will follow, with the 3 possibilities written in _cardOrders
        int j = 0;

        switch (currentStep)
        {
            case 0: //x
                for(int i = 0; i < _tempIndexes.Length; i++)
                {
                    _tempIndexes[i] = _cardOrders[i].x;
                }
                break;

            case 1: //y
                for (int i = 0; i < _tempIndexes.Length; i++)
                {
                    _tempIndexes[i] = _cardOrders[i].y;
                }
                break;

            case 2: //z
                for (int i = 0; i < _tempIndexes.Length; i++)
                {
                    _tempIndexes[i] = _cardOrders[i].z;
                }
                break;

            default:
                Debug.Log("Deck cards children reorder error : int parameter must be either 0, 1 or 2.");
                break;
        }

        while (j <= _tempIndexes.Length)
        {
            //Then we give to each child its new sibling index
            for (int i = 0; i < _tempIndexes.Length; i++)
            {
                if (_tempIndexes[i] == j)
                {
                    _cardObjects[i].transform.SetSiblingIndex(j);
                    break;
                }
            }
            j++;
        }
    }
}
