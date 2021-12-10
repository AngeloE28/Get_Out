using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement2 : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private Rigidbody playerController;    
    [SerializeField] private float rayLength = 5.0f;
    [SerializeField] private Transform[] groundChecks;
    [SerializeField] private float moveSpeed = 6.0f;
    [SerializeField] private float RotationSpeed = 1.0f;
    [SerializeField] private float smoothVal = 20.0f;
    [SerializeField] private float gravityForce = 10.0f;        
    private Vector3 _groundNormal, _myNormal;    

    [Header("Ground Check")]
    [SerializeField] private LayerMask GroundLayers;
    [SerializeField] private float groundOffset = 0.5f;
    [SerializeField] private float collisionRadius = 0.5f;
    private bool _isGrounded;    


    [Header("Cinemachine")]
    [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
    [SerializeField] private GameObject CinemachineCameraTarget;
    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField] private float TopClamp = 90.0f;
    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField] private float BottomClamp = -90.0f;
        
    private PlayerInputActions _playerInputActions;
    private GameObject _mainCamera;

    // cinemachine
    private float _rotationVelocity;
    private float _cinemachineTargetPitch;

    private const float _threshold = 0.01f;

    private void Awake()
    {
        _playerInputActions = new PlayerInputActions();
        _playerInputActions.Enable();

        // get a reference to our main camera
        if (_mainCamera == null)
        {
            _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
        }
        playerController = GetComponent<Rigidbody>();        
    }

    // Start is called before the first frame update
    void Start()
    {
        _myNormal = transform.up;
        playerController.freezeRotation = true;        
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        GroundCheck();
    }


    private void FixedUpdate()
    {
        Vector3 targetGroundDir = FloorAngle();
        _groundNormal = Vector3.Lerp(_groundNormal, targetGroundDir, smoothVal * Time.deltaTime);
        playerController.AddForce(-gravityForce * playerController.mass * targetGroundDir);
    }

    private void LateUpdate()
    {
        CameraRotation();
    }
    private void Move()
    {
        float sprintInput = _playerInputActions.Player.Sprint.ReadValue<float>();
        Vector2 moveInput = _playerInputActions.Player.Move.ReadValue<Vector2>();

        float speedMult;
        if (sprintInput == 0)
            speedMult = 1.0f;
        else
            speedMult = 1.5f;

        if (!_isGrounded)
        {
            // Reset normal so player doesn't fall forever
            _groundNormal = Vector3.up;
        }        

        _myNormal = Vector3.Lerp(_myNormal, _groundNormal, smoothVal * Time.deltaTime);

        Vector3 forward = Vector3.Cross(transform.right, _myNormal);

        Quaternion targetRot = Quaternion.LookRotation(forward, _myNormal);
        transform.rotation = Quaternion.Lerp(transform.rotation, targetRot, smoothVal * Time.deltaTime);

        transform.Translate(moveInput.x * moveSpeed * speedMult * Time.deltaTime,
                            0, 
                            moveInput.y * moveSpeed * speedMult * Time.deltaTime);
    }

    private void CameraRotation()
    {
        // if there is an input
        Vector2 lookInput = _playerInputActions.Player.Look.ReadValue<Vector2>();
        if (lookInput.sqrMagnitude >= _threshold)
        {
            _cinemachineTargetPitch += lookInput.y * RotationSpeed * Time.deltaTime;
            _rotationVelocity = lookInput.x * RotationSpeed * Time.deltaTime;

            // clamp our pitch rotation
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

            // Update Cinemachine camera target pitch
            CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);

            // rotate the player left and right
            transform.Rotate(Vector3.up * _rotationVelocity);
        }
    }

    private void GroundCheck()
    {
        Vector3 pos = transform.position + (-_myNormal * groundOffset);
        _isGrounded = Physics.CheckSphere(pos, collisionRadius, GroundLayers);
    }

    private Vector3 FloorAngle()
    {      
        Vector3 hitDir = transform.up;
        for (int i = 0; i < groundChecks.Length; i++)
        {
            RaycastHit hit;
            Physics.Raycast(groundChecks[i].position, -groundChecks[i].transform.up, out hit, rayLength, GroundLayers);
            Debug.DrawRay(groundChecks[i].position, -groundChecks[i].transform.up * rayLength, Color.red);
            if (hit.transform != null)
                hitDir += hit.normal;
        }

        Debug.DrawLine(transform.position, transform.position + (hitDir.normalized * rayLength / 2));

        return hitDir.normalized;
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }

}
