using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.collider.CompareTag("door"))
        {
            Debug.Log("Perdu !");
        }
    }
}
