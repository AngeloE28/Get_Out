using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
	[Header("Player")]
	[SerializeField] private Rigidbody playerController;
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField] private float MoveSpeed = 4.0f;
    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField] private float SprintSpeed = 6.0f;
    [Tooltip("Rotation speed of the character")]
    [SerializeField] private float RotationSpeed = 2.0f;
    [Tooltip("Acceleration and deceleration")]
    [SerializeField] private float SpeedChangeRate = 10.0f;
    [SerializeField] private bool analogMovement = false;
	[SerializeField] private float collisionRadius = 0.5f;

	[Header("Wall Stick")]
	[SerializeField] private float rayLength = 10.0f;
	[SerializeField] private Transform[] groundChecks;
	[SerializeField] private float downwardForce = 12.0f;
	[SerializeField] private float gravityOnWall = 10.0f;
	[SerializeField] private float gravityBuildUp = 2.0f;
	[SerializeField] private float gravityRotationSpeed = 10.0f;
	private float _gravityTotal;

	[Space(10)]
    //[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    //[SerializeField] private float GravityForce = -15.0f;	
	[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
	[SerializeField] private float FallTimeout = 0.15f;

	[Header("Player Grounded")]
	[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
	[SerializeField] private bool Grounded = true;
	[Tooltip("Useful for rough ground")]
	[SerializeField] private float GroundedOffset = -0.14f;
	[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
	[SerializeField] private float GroundedRadius = 0.5f;
	[Tooltip("What layers the character uses as ground")]
	[SerializeField] private LayerMask GroundLayers;

	[Header("Cinemachine")]
	[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
	[SerializeField] private GameObject CinemachineCameraTarget;
	[Tooltip("How far in degrees can you move the camera up")]
	[SerializeField] private float TopClamp = 90.0f;
	[Tooltip("How far in degrees can you move the camera down")]
	[SerializeField] private float BottomClamp = -90.0f;

	// cinemachine
	private float _cinemachineTargetPitch;

	// player
	private float _speed;
	private float _rotationVelocity;
	private float _verticalVelocity;
	private float _terminalVelocity = 53.0f;

	// timeout deltatime
	private float _fallTimeoutDelta;

	private CharacterController _controller;
	private PlayerInputActions _playerInputActions;
	private GameObject _mainCamera;

	private Vector3 _groundDir, _targetDir, _moveDir;

	private const float _threshold = 0.01f;

    private void Awake()
    {
		_playerInputActions = new PlayerInputActions();
		_playerInputActions.Enable();
		_groundDir = transform.up;
		_gravityTotal = 5.0f;


		// get a reference to our main camera
		if (_mainCamera == null)
		{
			_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
		}
		playerController = GetComponentInChildren<Rigidbody>();
		playerController.transform.parent = null;
	}

    // Start is called before the first frame update
    void Start()
    {
		_controller = GetComponent<CharacterController>();

		// reset our timeouts on start
		_fallTimeoutDelta = FallTimeout;
	}

    // Update is called once per frame
    void Update()
    {
		transform.position = playerController.position;
		ApplyGravity();
		GroundedCheck(-_groundDir);
		Move();
	}

    private void FixedUpdate()
    {
		playerController.AddForce(-_gravityTotal * playerController.mass * FloorAngle());
	}

    private void LateUpdate()
	{
		CameraRotation();
	}

	private void GroundedCheck(Vector3 dir)
	{
		// set sphere position, with offset
		Vector3 pos = transform.position + (dir * GroundedOffset);
		Grounded = Physics.CheckSphere(pos, collisionRadius, GroundLayers);		
        
		//Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
		//Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
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

	private void Move()
	{		
		float sprintInput = _playerInputActions.Player.Sprint.ReadValue<float>();
		Vector2 moveInput = _playerInputActions.Player.Move.ReadValue<Vector2>();
		bool moved = false;

		Vector3 camForward = _mainCamera.transform.forward;
		Vector3 camRight = _mainCamera.transform.right;

		Vector3 forward = camForward * moveInput.y;
		Vector3 right = camRight * moveInput.x;

        Vector3 inputDirection = (forward + right).normalized;


		if (moveInput == Vector2.zero)
		{
			_targetDir = transform.forward;
			_speed = 0.0f;
		}
		else
		{
			_targetDir = inputDirection;
			_speed = MoveSpeed;
			moved = true;
		}

		if (_targetDir == Vector3.zero)
			_targetDir = transform.forward;

		Vector3 targetGroundDir = FloorAngle();
		_groundDir = Vector3.Lerp(_groundDir, targetGroundDir, gravityRotationSpeed * Time.deltaTime);

		RotatePlayer(targetGroundDir, gravityRotationSpeed);
		//RotateMesh(_targetDir, MoveSpeed);		

		float speedMult;
		if (sprintInput == 0)
			speedMult = 1.0f;
		else
			speedMult = 1.5f;

		// move the player
		Vector3 currentVelocity = playerController.velocity;
		if(!moved)
        {
			_speed = _speed * 0.8f;
			_moveDir = Vector3.Lerp(transform.forward, playerController.velocity.normalized, 12 * Time.deltaTime);
        }
		else
        {
			_moveDir = transform.forward;
        }


		Vector3 targetVel = targetGroundDir * downwardForce;		

		Vector3 finalDir = Vector3.Lerp(currentVelocity, targetVel, 8.0f * Time.deltaTime);
		playerController.velocity -= targetVel;

		transform.Translate(moveInput.x * _speed * speedMult * Time.deltaTime,
					0,
					moveInput.y * _speed * speedMult * Time.deltaTime);
		//_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
		//playerController.velocity = _speed * inputDirection.normalized;
	}

	private void ApplyGravity()
	{
		if (Grounded)
		{
			print("gound");
			// reset the fall timeout timer
			//_fallTimeoutDelta = FallTimeout;

			// stop our velocity dropping infinitely when grounded
			//if (_verticalVelocity < 0.0f)
			//{
			//	_verticalVelocity = -2f;
			//}
			//playerController.AddForce((transform.position - _groundDir).normalized * GravityForce);
		}
		else
		{
			print("neing");
			// fall timeout
			//if (_fallTimeoutDelta >= 0.0f)
			//{
				//_fallTimeoutDelta -= Time.deltaTime;
			//}
		}

		//// apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
		//if (_verticalVelocity < _terminalVelocity)
		//{
		//	_verticalVelocity += GravityForce * Time.deltaTime;
		//}
	}

	private Vector3 FloorAngle()
	{
		Debug.DrawRay(groundChecks[0].position, -groundChecks[0].transform.up * rayLength);
		Debug.DrawRay(groundChecks[1].position, -groundChecks[1].transform.up * rayLength);
		Debug.DrawRay(groundChecks[2].position, -groundChecks[2].transform.up * rayLength);
		//Physics.Raycast(groundChecks[0].position, -groundChecks[0].transform.up, out frontHit, rayLength, GroundLayers);
		//Physics.Raycast(groundChecks[1].position, -groundChecks[1].transform.up, out midHit, rayLength, GroundLayers);
		//Physics.Raycast(groundChecks[2].position, -groundChecks[2].transform.up, out backHit, rayLength, GroundLayers);		

		Vector3 hitDir = transform.up;
		for(int i = 0; i < groundChecks.Length; i++)
        {
			RaycastHit hit;
			Physics.Raycast(groundChecks[i].position, -groundChecks[i].transform.up, out hit, rayLength, GroundLayers);
			if (hit.transform != null)
				hitDir += hit.normal;
        }

		Debug.DrawLine(transform.position, transform.position + (hitDir.normalized * rayLength/2));

		return hitDir.normalized;
	}

	private void RotatePlayer(Vector3 dir, float gravitySpeed)
	{
		Vector3 LerpDir = Vector3.Lerp(transform.up, dir, gravitySpeed * Time.deltaTime);
		transform.rotation = Quaternion.FromToRotation(transform.up, LerpDir) * transform.rotation;
	}

	private void RotateMesh(Vector3 lookDir, float speed)
	{
		Quaternion SlerpRot = Quaternion.LookRotation(lookDir, transform.up);
		transform.rotation = Quaternion.Slerp(transform.rotation, SlerpRot, speed * Time.deltaTime);
	}

	private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
	{
		if (lfAngle < -360f) lfAngle += 360f;
		if (lfAngle > 360f) lfAngle -= 360f;
		return Mathf.Clamp(lfAngle, lfMin, lfMax);
	}

	private void OnDrawGizmosSelected()
	{
		Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
		Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

		if (Grounded) Gizmos.color = transparentGreen;
		else Gizmos.color = transparentRed;

		// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
		//Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
		Gizmos.DrawSphere(transform.position + (_groundDir * GroundedOffset), collisionRadius);
	}
}
