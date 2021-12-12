using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{    
    [SerializeField] private Renderer btnRenderer;

    [ColorUsage(true, true)]
    [SerializeField] private Color newEColor;
    [SerializeField] private Color newColor;

    private PlayerMovement _player;
    private bool _isPressed = false;

    private void Awake()
    {        
        _isPressed = false;
    }

    private void LateUpdate()
    {
        if (_player != null)
        {
            float interactInput = _player.GetPlayerInputActions().Player.Interact.ReadValue<float>();
            if (interactInput != 0)
                _isPressed = true;
        }

        if(_isPressed)
        {
            btnRenderer.material.SetColor("_BaseColor", newColor);
            btnRenderer.material.SetColor("_EmissionColor", newEColor);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        other.gameObject.TryGetComponent<PlayerMovement>(out var player);
        if (player != null)
        {
            _player = player;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        other.gameObject.TryGetComponent<PlayerMovement>(out var player);
        if (player != null)
        {
            _player = null;
        }
    }

    public bool BtnPressed()
    {
        return _isPressed;
    }
}
