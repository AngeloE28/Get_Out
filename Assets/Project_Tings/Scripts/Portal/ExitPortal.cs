using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitPortal : MonoBehaviour
{
    private bool overLapped = false;

    private void Awake()
    {
        overLapped = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.TryGetComponent<PlayerMovement>(out var player);
        if (player != null)
            overLapped = true;
    }

    public bool PlayerLeft()
    {
        return overLapped;
    }
}
