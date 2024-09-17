using System;
using Game.Controller.Core;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Game.Controller
{
    public class PlayerController : MonoBehaviour, IInput
    {
        #region Serialized Fields

        [SerializeField] Transform _cam;

        #endregion

        private Action<Vector3, float> OnMovement;

        void Start() 
        {
            if(!_cam) 
            {
                string msg = "Camera not set for player controller";
                Debug.LogWarning(msg);

                _cam = Camera.main.transform;
            } 
        }

        public void MovementInput(InputAction.CallbackContext context) 
        {
            Vector2 input = context.ReadValue<Vector2>();

            Vector3 fix = _cam.right * input.x + _cam.forward * input.y;
            Vector3 dir = new Vector3(fix.x, 0.0f, fix.z).normalized;

            #if UNITY_EDITOR
            _direction = dir;
            #endif

            OnMovement?.Invoke(dir, input.magnitude);
        }

        #region IListener<Action<Vector3, float>> Implementation

        public void AddMovementListener(Action<Vector3, float> eventHandler)
        {
            OnMovement += eventHandler;
        }

        public void RemoveMovementListener(Action<Vector3, float> eventHandler)
        {
            OnMovement -= eventHandler;
        }

        #endregion

        #region Debug

        #if UNITY_EDITOR
        private Vector3 _direction;
        void OnDrawGizmos() 
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(transform.position, _direction);
        }
        #endif

        #endregion
    }
}
