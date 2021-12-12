using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pads : MonoBehaviour
{
    [SerializeField] string keyName = "Key";

    private bool _isActuated = false;

    private void Awake()
    {
        _isActuated = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == keyName)
            _isActuated = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == keyName)
            _isActuated = false;
    }

    public bool GetActuatedState()
    {
        return _isActuated;
    }
}
