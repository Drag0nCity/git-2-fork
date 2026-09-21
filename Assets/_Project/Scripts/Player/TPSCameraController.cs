using UnityEngine;
using UnityEngine.InputSystem;

namespace TPShooter.Player
{
    public sealed class TPSCameraController : MonoBehaviour
    {
        [SerializeField] private Transform target;

        [Header("Camera")]
        [SerializeField] private float distance = 5f;
        [SerializeField] private float height = 2.5f;

        [Header("Mouse")]
        [SerializeField] private float sensitivity = 0.15f;
        [SerializeField] private float smoothTime = 0.05f;

        [Header("Vertical Limits")]
        [SerializeField] private float minPitch = -30f;
        [SerializeField] private float maxPitch = 65f;

        private float yaw;
        private float pitch = 15f;

        private Vector3 currentVelocity;

        private void Start()
        {
            if (target == null)
            {
                Debug.LogError("TPSCameraController: Target is not assigned.");
                enabled = false;
                return;
            }

            yaw = target.eulerAngles.y;

            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        private void Update()
        {
            ReadMouse();
        }

        private void LateUpdate()
        {
            if (target == null)
                return;

            Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);

            Vector3 targetPosition =
                target.position +
                Vector3.up * height -
                rotation * Vector3.forward * distance;

            transform.position = Vector3.SmoothDamp(
                transform.position,
                targetPosition,
                ref currentVelocity,
                smoothTime
            );

            transform.rotation = rotation;
        }

        private void ReadMouse()
        {
            if (Mouse.current == null)
                return;

            Vector2 mouseDelta = Mouse.current.delta.ReadValue();

            yaw += mouseDelta.x * sensitivity;
            pitch -= mouseDelta.y * sensitivity;

            pitch = Mathf.Clamp(pitch, minPitch, maxPitch);
        }

        private void OnDisable()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }
}