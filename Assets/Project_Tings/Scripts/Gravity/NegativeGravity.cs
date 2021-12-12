using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NegativeGravity : MonoBehaviour
{
    [SerializeField] private float gravityForce = 10.0f;
    private Rigidbody _rb;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();

        if (_rb.useGravity)
            _rb.useGravity = false;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        _rb.AddForce(Vector3.up * _rb.mass * gravityForce);
    }
}
