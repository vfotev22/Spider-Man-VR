using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class EndScreenActivate : MonoBehaviour
{
    public GameObject endscreen;

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            // Activates the hidden object
            endscreen.SetActive(true);
            Debug.Log("touched end zone");
        }
    }
}