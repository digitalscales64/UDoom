using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

namespace Game.Input
{
    public class PlayerInput : MonoBehaviour, Core.IInput
    {
        #region Serialized Fields

        [Header("Input Settings")]
        [SerializeField] Game.Setting.InputSettings _settings;

        #endregion

        [Header("Input Events")]
        [SerializeField] UnityEvent<Vector2, float> _onMovement;
        [SerializeField] UnityEvent<Vector2, float> _onTurning;
        [SerializeField] UnityEvent<bool> _onJumping;

        void Update() 
        {
            SmoothRotation();
            SmoothMovement();
        }

        #region IInput Implementation
        
        public UnityEvent<Vector2, float> OnMovement => _onMovement;
        public UnityEvent<Vector2, float> OnTurning => _onTurning;
        public UnityEvent<bool> OnJumping => _onJumping;

        #endregion

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

            _onMovement?.Invoke(_smoothMovementDir, _smoothMovementMagnitude);
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

            _onTurning?.Invoke(_smoothTurningDir, _smoothTurningMagnitude);
        }

        #endregion

        #region Jumping

        public void JumpingInput(InputAction.CallbackContext context) 
        {
            bool pressed = context.phase == InputActionPhase.Performed? true : false;
            OnJumping?.Invoke(pressed);
        }

        #endregion
    }
}
