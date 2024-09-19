using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Controller
{
    public class PlayerInput : MonoBehaviour
    {
        #region Serialized Fields

        [Header("Input Settings")]
        [SerializeField] Game.Setting.InputSettings _settings;

        #endregion

        #region Public Fields

        [Header("Input Events")]
        [SerializeField] public UnityEvent<Vector2, float> OnMovementInput;  
        [SerializeField] public UnityEvent<Vector2, float> OnTurningInput;

        #endregion

        void Update() 
        {
            SmoothRotation();
            SmoothMovement();
        }

        #region Movement

        // Movement from Input calls
        private Vector2 _movementDesiredDir = Vector2.zero;
        private float _movementDesiredMagnitude = 0.0f;

        // Processed smooth input
        private Vector2 _smoothMovementDir = Vector2.zero;
        private float _smoothMovementMagnitude = 0.0f;

        // Callable from input system
        public void MovementInput(InputAction.CallbackContext context) 
        {
            Vector2 input = context.ReadValue<Vector2>();

            _movementDesiredDir = input;
            _movementDesiredMagnitude = input.magnitude;
        }

        void SmoothMovement() 
        {
            // Movement smoothing
            _smoothMovementDir = Vector2.Lerp(_smoothMovementDir, _movementDesiredDir, Time.deltaTime * _settings.MovementInputSmoothing);
            _smoothMovementMagnitude = Mathf.Lerp(_smoothMovementMagnitude, _movementDesiredMagnitude, Time.deltaTime * _settings.MovementInputSmoothing);

            OnMovementInput?.Invoke(_smoothMovementDir, _smoothMovementMagnitude);
        }

        #endregion

        #region Turning

        // Movement from Input calls
        private Vector2 _turningDesiredDir = Vector2.zero;
        private float _turningDesiredMagnitude = 0.0f;

        // Processed smooth input
        private Vector2 _smoothTurningDir = Vector2.zero;
        private float _smoothTurningMagnitude = 0.0f;

        // Callable from input system
        public void TurningInput(InputAction.CallbackContext context) 
        {
            Vector2 input = context.ReadValue<Vector2>();

            _turningDesiredDir = input.normalized;
            _turningDesiredMagnitude = input.magnitude;
        }

        void SmoothRotation() 
        {
            // Camera rotation smoothing
            _smoothTurningDir = Vector3.Lerp(_smoothTurningDir, _turningDesiredDir, Time.deltaTime * _settings.TurningInputSmoothing);
            _smoothTurningMagnitude = Mathf.Lerp(_smoothTurningMagnitude, _turningDesiredMagnitude, Time.deltaTime * _settings.TurningInputSmoothing);

            OnTurningInput?.Invoke(_smoothTurningDir, _smoothTurningMagnitude);
        }

        #endregion
    }
}
