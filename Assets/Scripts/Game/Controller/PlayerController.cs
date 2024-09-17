using System;
using Game.Input.Core;
using Game.Setting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Controller
{
    public class PlayerController : MonoBehaviour, IMovementInput, IRotationInput
    {
        #region Serialized Fields

        [SerializeField] private CameraSettings _camSettings;

        #endregion

        private Action<Vector3, float> OnMovementInput;
        private Action<Vector3, float> OnRotationInput;

        void Start() 
        {
            Cursor.lockState = CursorLockMode.Locked;
        }

        void Update() 
        {
            SmoothRotation();
            SmoothMovement();
        }

        #region Movement

        [Header("Desired Movement")]
        private Vector2 _movementDesiredDir = Vector2.zero;
        private float _movementDesiredMagnitude = 0.0f;

        public void MovementInput(InputAction.CallbackContext context) 
        {
            Vector2 input = context.ReadValue<Vector2>();

            _movementDesiredDir = input;
            _movementDesiredMagnitude = input.magnitude;
        }

        private const float SMOOTH_MOVEMENT_FACTOR = 10.0f;

        [Header("Smooth Movement")]
        private Vector3 _smoothMovementDir = Vector3.zero;
        private float _smoothMovementMagnitude = 0.0f;

        void SmoothMovement() 
        {
            Vector3 fix = transform.right * _movementDesiredDir.x + transform.forward * _movementDesiredDir.y;
            fix = new Vector3(fix.x, 0.0f, fix.z).normalized;

            // Movement smoothing
            _smoothMovementDir = Vector3.Lerp(_smoothMovementDir, fix, Time.deltaTime * SMOOTH_MOVEMENT_FACTOR);
            _smoothMovementMagnitude = Mathf.Lerp(_smoothMovementMagnitude, _movementDesiredMagnitude, Time.deltaTime * SMOOTH_MOVEMENT_FACTOR);

            OnMovementInput?.Invoke(_smoothMovementDir, _smoothMovementMagnitude);
        }

        #endregion

        #region Camera

        [Header("Desired Rotation")]
        private Vector2 _cameraDesiredDir = Vector2.zero;
        private float _cameraDesiredMagnitude = 0.0f;

        public void CameraInput(InputAction.CallbackContext context) 
        {
            Vector2 input = context.ReadValue<Vector2>();

            _cameraDesiredDir = input.normalized;
            _cameraDesiredMagnitude = input.magnitude;
        }

        [Header("Smooth Rotation")]
        private Vector3 _smoothCameraDir = Vector3.zero;
        private float _smoothCameraMagnitude = 0.0f;

        void SmoothRotation() 
        {
            // Camera rotation smoothing
            _smoothCameraDir = Vector3.Lerp(_smoothCameraDir, _cameraDesiredDir, Time.deltaTime * _camSettings.HorizontalSensitivity);
            _smoothCameraMagnitude = Mathf.Lerp(_smoothCameraMagnitude, _cameraDesiredMagnitude, Time.deltaTime * _camSettings.HorizontalSensitivity);

            OnRotationInput?.Invoke(_smoothCameraDir, _smoothCameraMagnitude);            
        }

        #endregion

        #region IRotationInput<Action<Vector3, float>> Implementation

        public void AddRotationListener(Action<Vector3, float> eventHandler)
        {
            OnRotationInput += eventHandler;
        }

        public void RemoveRotationListener(Action<Vector3, float> eventHandler)
        {
            OnRotationInput -= eventHandler;
        }

        #endregion 

        #region IMovementInput<Action<Vector3, float>> Implementation

        public void AddMovementListener(Action<Vector3, float> eventHandler)
        {
            OnMovementInput += eventHandler;
        }

        public void RemoveMovementListener(Action<Vector3, float> eventHandler)
        {
            OnMovementInput -= eventHandler;
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        void OnDrawGizmos() 
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, _smoothMovementDir);

            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(transform.position, transform.forward);
        }
        #endif

        #endregion
    }
}
