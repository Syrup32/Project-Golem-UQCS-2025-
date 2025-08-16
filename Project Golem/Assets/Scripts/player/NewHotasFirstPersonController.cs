using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class NewHotasFirstPersonController : MonoBehaviour
{
    [Header("References")]
    public Transform cameraTarget;
    public Transform cameraFollowProxy;
    public PlayerInputHandler inputHandler;

    [Header("Movement")]
    public float moveSpeed = 4f;
    public float rotationSpeed = 1f;
    public float jumpHeight = 1.2f;
    public float gravity = -15f;

    [Header("Ground Check")]
    public float groundedOffset = -0.14f;
    public float groundedRadius = 0.5f;
    public LayerMask groundLayers;

    [Header("Camera Shake")]
    public float shakeAmplitude = 2f;
    public float minShakeFrequency = 0.1f;
    public float maxShakeFrequency = 1.5f;
    public float shakeLerpSpeed = 5f;

    [Header("Audio")]
    public AudioSource stompAudioSource;
    public float stompInterval = 0.6f;
    private float stompTimer;

    private CharacterController controller;
    private float verticalVelocity;
    private bool isGrounded;
    private float terminalVelocity = 53.0f;
    private float pitch;

    private CinemachineVirtualCamera _virtualCam;
    private CinemachineBasicMultiChannelPerlin _perlin;
    private Vector3 _lastPosition;
    private float smoothedSpeed;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
    }

    private void Start()
    {
        _virtualCam = Object.FindFirstObjectByType<CinemachineVirtualCamera>();
        if (_virtualCam != null)
        {
            _perlin = _virtualCam.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();
        }

        if (cameraFollowProxy != null && cameraTarget != null)
        {
            cameraFollowProxy.position = cameraTarget.position;
            cameraFollowProxy.rotation = cameraTarget.rotation;
        }

        _lastPosition = transform.position;
    }

    private void Update()
    {
        GroundedCheck();
        HandleJumpAndGravity();
        Move();
        if (isGrounded && Mathf.Abs(inputHandler.throttleValue) > 0.1f)
        {
            stompTimer -= Time.deltaTime;
            if (stompTimer <= 0f)
            {
                stompAudioSource.Play();
                stompTimer = stompInterval;
            }
        }
        else
        {
            stompTimer = 0f; // reset when not moving
        }
        RotateCamera();
    }

    private void LateUpdate()
    {
        if (cameraFollowProxy != null && cameraTarget != null)
        {
            cameraFollowProxy.position = Vector3.Lerp(
                cameraFollowProxy.position,
                cameraTarget.position,
                Time.deltaTime * 20f
            );

            cameraFollowProxy.rotation = Quaternion.Lerp(
                cameraFollowProxy.rotation,
                cameraTarget.rotation,
                Time.deltaTime * 20f
            );
        }

        HandleCameraShake();
    }

    private void GroundedCheck()
    {
        Vector3 spherePos = new Vector3(transform.position.x, transform.position.y + groundedOffset, transform.position.z);
        isGrounded = Physics.CheckSphere(spherePos, groundedRadius, groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void HandleJumpAndGravity()
    {
        if (isGrounded && verticalVelocity < 0f)
        {
            verticalVelocity = -2f;
        }

        if (isGrounded && inputHandler.ConsumeJumpPressed())
        {
            verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        if (verticalVelocity < terminalVelocity)
        {
            verticalVelocity += gravity * Time.deltaTime;
        }
    }

    private void Move()
    {
        Vector3 move = transform.forward * -inputHandler.throttleValue * moveSpeed;
        Vector3 gravityMove = new Vector3(0, verticalVelocity, 0);
        controller.Move((move + gravityMove) * Time.deltaTime);
    }

    private void RotateCamera()
    {
        Vector2 look = inputHandler.lookInput;
        pitch += look.y * rotationSpeed;
        pitch = Mathf.Clamp(pitch, -90f, 90f);
        cameraTarget.localRotation = Quaternion.Euler(pitch, 0, 0);
        transform.Rotate(Vector3.up * look.x * rotationSpeed);
    }

    private void HandleCameraShake()
    {
        Vector3 horizontalDelta = transform.position - _lastPosition;
        horizontalDelta.y = 0f;
        float horizontalSpeed = horizontalDelta.magnitude / Time.deltaTime;
        smoothedSpeed = Mathf.Lerp(smoothedSpeed, horizontalSpeed, Time.deltaTime * shakeLerpSpeed);
        _lastPosition = transform.position;

        if (_perlin != null && isGrounded)
        {
            float targetAmplitude = smoothedSpeed > 0.1f ? shakeAmplitude : 0f;
            float targetFrequency = Mathf.Lerp(minShakeFrequency, maxShakeFrequency, smoothedSpeed / moveSpeed);

            _perlin.m_AmplitudeGain = Mathf.Lerp(_perlin.m_AmplitudeGain, targetAmplitude, Time.deltaTime * shakeLerpSpeed);
            _perlin.m_FrequencyGain = Mathf.Lerp(_perlin.m_FrequencyGain, targetFrequency, Time.deltaTime * shakeLerpSpeed);
        }
    }
}
