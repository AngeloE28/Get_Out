using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBWallGravity : MonoBehaviour
{
    [SerializeField] private bool isForward = true;
    [SerializeField] private float gravityForce = 10.0f;
    
    private Rigidbody _rb;
    private float _gravityController = 1;


    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (_rb.useGravity)
            _rb.useGravity = false;

        if (isForward)
            _gravityController = 1;
        else
            _gravityController = -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {       
        _rb.AddForce((_gravityController * Vector3.forward) * _rb.mass * gravityForce);
    }
}
