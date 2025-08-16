using UnityEngine;
using Cinemachine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	[RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM
	[RequireComponent(typeof(PlayerInput))]
#endif
	public class HotasFirstPersonController : MonoBehaviour
	{
		[Header("Player")]
		public float MoveSpeed = 4.0f;
		public float SprintSpeed = 6.0f;
		public float RotationSpeed = 1.0f;
		public float SpeedChangeRate = 10.0f;
		public float JumpHeight = 1.2f;
		public float Gravity = -15.0f;
		public float JumpTimeout = 0.1f;
		public float FallTimeout = 0.15f;

		[Header("HOTAS Input")]
		public InputActionAsset hotasInputActions;
		private InputAction hotasThrottle;
		private InputAction hotasLook;
		private InputAction hotasJump;
		private InputAction hotasLookSensitivity;

		[Header("Player Grounded")]
		public bool Grounded = true;
		public float GroundedOffset = -0.14f;
		public float GroundedRadius = 0.5f;
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		public GameObject CinemachineCameraTarget;
		public float TopClamp = 90.0f;
		public float BottomClamp = -90.0f;

		private float _cinemachineTargetPitch;
		private float _speed;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		public CinemachineImpulseSource impulseSource;
		public CinemachineImpulseSource moveImpulseSource;
		private CinemachineVirtualCamera _virtualCamera;
		private CinemachineBasicMultiChannelPerlin _perlin;
		private Vector3 _previousPosition;
		
	    private float _previousSpeed;

		[Header("Audio")]
		public AudioSource footstepAudioSource;
		public AudioClip footstepClip;
		public float footstepInterval = 0.6f; // seconds between steps
		private float footstepTimer;

#if ENABLE_INPUT_SYSTEM
		private PlayerInput _playerInput;
#endif
		private CharacterController _controller;
		private StarterAssetsInputs _input;
		private GameObject _mainCamera;
		private const float _threshold = 0.01f;
		private float hotasThrottleInput;
		private Vector2 hotasLookInput;
		private float currentLookSensitivity = 1.0f;
		private bool _wasJumpPressedLastFrame = false;

		private bool IsCurrentDeviceMouse => _playerInput != null && _playerInput.currentControlScheme == "KeyboardMouse";
		private bool IsHotasJumpPressed => hotasJump != null && hotasJump.ReadValue<float>() > 0.5f;
		private bool IsJumpPressedThisFrame => (_input.jump || IsHotasJumpPressed) && !_wasJumpPressedLastFrame;

		private void Awake()
		{
			if (_mainCamera == null)
			{
				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}
		}

		private void Start()
		{
			_controller = GetComponent<CharacterController>();
			_input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM
			_playerInput = GetComponent<PlayerInput>();
#endif
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;

			if (hotasInputActions != null)
			{
				var map = hotasInputActions.FindActionMap("GOLEM Controls");
				if (map != null)
				{
					hotasThrottle = map.FindAction("throttle");
					hotasLook = map.FindAction("look_hotas");
					hotasLookSensitivity = map.FindAction("look_sensitivity");
					hotasJump = map.FindAction("jump");
					map.Enable();
				}
			}
			_virtualCamera = FindObjectOfType<CinemachineVirtualCamera>();
			if (_virtualCamera != null)
			{
				_perlin = _virtualCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
			}

			_previousPosition = transform.position;
		}

		private void Update()
		{
			ReadHotasInput();
			JumpAndGravity();
			GroundedCheck();
			Move();
			//Debug.Log($"[GroundedCheck] Is Grounded: {Grounded}");
			float movementSpeed = new Vector2(_controller.velocity.x, _controller.velocity.z).magnitude;

			// Trigger impulse when player starts moving
			/*
			if (movementSpeed > 0.1f && _previousSpeed <= 0.1f)
			{
				impulseSource.GenerateImpulse();
			}
			*/
			//Camera shake stuff
			_previousSpeed = movementSpeed;
			Vector3 velocity = (transform.position - _previousPosition) / Time.deltaTime;
			float horizontalSpeed = new Vector3(velocity.x, 0, velocity.z).magnitude;
			

			if (_perlin != null)
			{
			float targetAmplitude = horizontalSpeed > 0.1f ? 1.2f : 0f;
			_perlin.m_AmplitudeGain = Mathf.Lerp(_perlin.m_AmplitudeGain, targetAmplitude, Time.deltaTime * 5f);			
			}

			_previousPosition = transform.position;
			//stomping audio
			

			if (horizontalSpeed > 0.1f && Grounded)
			{
				footstepTimer -= Time.deltaTime;
				if (footstepTimer <= 0f)
				{
					PlayFootstep();
					footstepTimer = footstepInterval;
				}
			}
			else
			{
				footstepTimer = 0f; // reset when not moving
			}

		}

		private void LateUpdate()
		{
			CameraRotation();
		}

		private void ReadHotasInput()
		{
			if (hotasThrottle != null) hotasThrottleInput = hotasThrottle.ReadValue<float>();
			if (hotasLook != null) hotasLookInput = hotasLook.ReadValue<Vector2>();
			if (hotasLookSensitivity != null)
			{
				float rawSensitivityInput = hotasLookSensitivity.ReadValue<float>();
				currentLookSensitivity = Mathf.Lerp(0.5f, 2.0f, (rawSensitivityInput + 1f) / 2f);
			}
		}

		private void PlayFootstep()
		{
			if (footstepAudioSource && footstepClip)
			{
				footstepAudioSource.PlayOneShot(footstepClip);
			}
		}

		private void GroundedCheck()
		{
			Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
			Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
		}
		private static float ClampAngle(float angle, float min, float max)
		{
			if (angle < -360f) angle += 360f;
			if (angle > 360f) angle -= 360f;
			return Mathf.Clamp(angle, min, max);
		}
		private void CameraRotation()
		{
			Vector2 look = hotasLookInput.magnitude > 0.01f ? hotasLookInput : _input.look;
			if (look.sqrMagnitude >= _threshold)
			{
				float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
				float lookMagnitude = Mathf.Clamp01(look.magnitude);
				_cinemachineTargetPitch += look.y * RotationSpeed * deltaTimeMultiplier * lookMagnitude * currentLookSensitivity;
				_rotationVelocity = look.x * RotationSpeed * deltaTimeMultiplier * lookMagnitude * currentLookSensitivity;
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
				CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
				transform.Rotate(Vector3.up * _rotationVelocity);
			}
		}

		private void Move()
		{
			float targetSpeed = hotasThrottleInput != 0 ? MoveSpeed : (_input.sprint ? SprintSpeed : MoveSpeed);
			if (hotasThrottleInput == 0 && _input.move == Vector2.zero) targetSpeed = 0.0f;
			float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
			float speedOffset = 0.1f;
			float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

			if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
			{
				float analogSpeedMultiplier = hotasThrottleInput != 0 ? Mathf.Clamp01(Mathf.Abs(hotasThrottleInput)) : inputMagnitude;
				_speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * analogSpeedMultiplier, Time.deltaTime * SpeedChangeRate);
				_speed = Mathf.Round(_speed * 1000f) / 1000f;
			}
			else
			{
				_speed = targetSpeed;
			}

			Vector3 inputDirection = hotasThrottleInput != 0
				? transform.forward * -hotasThrottleInput
				: new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

			if (_input.move != Vector2.zero)
			{
				inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
			}
			//Debug.Log($"[Move] VerticalVelocity: {_verticalVelocity}");
			_controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
			
			if (moveImpulseSource != null && _controller.velocity.magnitude > 0.1f && Grounded)
			{
				float moveIntensity = Mathf.Clamp(_controller.velocity.magnitude / SprintSpeed, 0f, 1f);
				moveImpulseSource.GenerateImpulse(moveIntensity * 0.3f);
			}
			if (_controller.velocity.magnitude > 0.1f && Grounded && !footstepAudioSource.isPlaying)
			{
				footstepAudioSource.PlayOneShot(footstepClip);
			}
		}

		private void JumpAndGravity()
		{
			if (Grounded)
			{
				_fallTimeoutDelta = FallTimeout;

				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				//Debug.Log($"[JumpCheck] jump={_input.jump}, IsHotasJump={IsHotasJumpPressed}, jumpTimeoutDelta={_jumpTimeoutDelta}, Grounded={Grounded}");

				if (IsJumpPressedThisFrame && _jumpTimeoutDelta <= 0.0f)
				{
					_verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
					Debug.Log($"[Jump] JUMPING! VerticalVelocity set to {_verticalVelocity}");

					_jumpTimeoutDelta = JumpTimeout;

					// Reset jump input after applying
					//_input.jump = false;
				}

				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Time.deltaTime;
				}
			}
			else
			{
				_jumpTimeoutDelta = JumpTimeout;

				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Time.deltaTime;
				}
			}

			// Apply gravity
			if (_verticalVelocity < _terminalVelocity)
			{
				_verticalVelocity += Gravity * Time.deltaTime;
			}

			// Track previous jump press state
			_wasJumpPressedLastFrame = _input.jump || IsHotasJumpPressed;
		}
		
	}
}
