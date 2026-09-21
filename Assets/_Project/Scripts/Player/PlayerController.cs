using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;

namespace TPShooter.Player
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float crouchSpeed = 2.5f;
        [SerializeField] private float rotationSpeed = 12f;

        [Header("Jump")]
        [SerializeField] private float jumpHeight = 1.5f;
        [SerializeField] private float gravity = -20f;

        [Header("Crouch")]
        [SerializeField] private float standingHeight = 2f;
        [SerializeField] private float crouchingHeight = 1.2f;
        [SerializeField] private float crouchTransitionSpeed = 8f;

        private CharacterController characterController;
        private Transform cameraTransform;

        private Vector2 moveInput;
        private float verticalVelocity;
        private Vector3 currentVelocity;
        private IInputService inputService;

        private bool isCrouching;

        public Vector3 Velocity => currentVelocity;
        public bool IsCrouching => isCrouching;

        [Inject]
        private void Construct(IInputService inputService)
        {
            this.inputService = inputService;
        }

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();

            inputService =
                Zenject.ProjectContext.Instance.Container
                    .Resolve<IInputService>();

            standingHeight = characterController.height;
        }

        private void Start()
        {
            if (Camera.main != null)
                cameraTransform = Camera.main.transform;
        }

        private void Update()
        {
            ReadMovementInput();
            ReadCrouchInput();
            HandleJump();
            UpdateCharacterHeight();
            Move();
        }

        private void ReadMovementInput()
        {
            moveInput = inputService.Move;
        }

        private void ReadCrouchInput()
        {
            isCrouching = inputService.Crouch;
        }

        private void HandleJump()
        {
            if (characterController.isGrounded)
            {
                if (verticalVelocity < 0f)
                    verticalVelocity = -2f;

                if (Keyboard.current != null &&
                    Keyboard.current.spaceKey.wasPressedThisFrame &&
                    !isCrouching)
                {
                    verticalVelocity =
                        Mathf.Sqrt(jumpHeight * -2f * gravity);
                }
            }

            verticalVelocity += gravity * Time.deltaTime;
        }

        private void UpdateCharacterHeight()
        {
            float targetHeight =
                isCrouching
                    ? crouchingHeight
                    : standingHeight;

            characterController.height = Mathf.Lerp(
                characterController.height,
                targetHeight,
                crouchTransitionSpeed * Time.deltaTime
            );

            Vector3 center = characterController.center;

            center.y = characterController.height * 0.5f;

            characterController.center = center;
        }

        private void Move()
        {
            if (cameraTransform == null && Camera.main != null)
                cameraTransform = Camera.main.transform;

            Vector3 forward = cameraTransform != null
                ? cameraTransform.forward
                : Vector3.forward;

            Vector3 right = cameraTransform != null
                ? cameraTransform.right
                : Vector3.right;

            forward.y = 0f;
            right.y = 0f;

            forward.Normalize();
            right.Normalize();

            Vector3 direction =
                forward * moveInput.y +
                right * moveInput.x;

            if (direction.sqrMagnitude > 0.001f)
            {
                Quaternion targetRotation =
                    Quaternion.LookRotation(direction);

                transform.rotation = Quaternion.Slerp(
                    transform.rotation,
                    targetRotation,
                    rotationSpeed * Time.deltaTime
                );
            }

            float currentMoveSpeed =
                isCrouching
                    ? crouchSpeed
                    : moveSpeed;

            currentVelocity =
                direction * currentMoveSpeed;

            Vector3 velocity = currentVelocity;
            velocity.y = verticalVelocity;

            characterController.Move(
                velocity * Time.deltaTime
            );
        }
    }
}