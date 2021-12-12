using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LRWallGravity : MonoBehaviour
{
    [SerializeField] private bool isRight = true;
    [SerializeField] private float gravityForce = 10.0f;
    
    private Rigidbody _rb;
    private float _gravityController = 1;
        
    void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (_rb.useGravity)
            _rb.useGravity = false;

        if (isRight)
            _gravityController = 1;
        else
            _gravityController = -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.AddForce((_gravityController *  Vector3.right) * _rb.mass * gravityForce);
    }
}
