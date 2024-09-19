using System;
using Game.Attribute;
using Game.Input.Core;
using UnityEngine;

namespace Game.Controller 
{
    public class MovementController : MonoBehaviour
    {
        private const float GRAVITY = 9.8f;
        private const float GROUND_TOLERANCE = 0.1f;

        #region Serialized Fields

        [SerializeField] private MovementAttributes _attributes;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private bool _useGravity;

        #endregion

        // Desired Movement
        private Vector2 _moveDirection;
        private float _moveMagnitude;

        // Desired Rotation
        private Vector2 _turnDirection;
        private float _turnMagnitude;

        // gravity
        private float _gravity;
        private bool _isFalling;

        void Start() 
        {
            Cursor.lockState = CursorLockMode.Locked;

            if(!_controller) 
            {
                _controller = GetComponent<CharacterController>(); 
                string msg = "No CharacterController present";
                Debug.LogWarning(msg);
            }
        }

        void FixedUpdate() 
        {
            // Fixed direction
            Vector3 fix = transform.right * _moveDirection.x + transform.forward * _moveDirection.y; 
            fix = new Vector3(fix.x, 0.0f, fix.z).normalized;

            // Desired Rotation
            float speed = _turnDirection.x * _turnMagnitude * _attributes.TurnSpeed * Time.deltaTime;
            Vector3 rot = new Vector3(0.0f, speed, 0.0f);
            
            transform.Rotate(rot);

            // Desired Movement
            Vector3 advance = fix * (_moveMagnitude * _attributes.MovementSpeed * Time.fixedDeltaTime) + Vector3.down * _gravity;

            if(_controller.isGrounded) 
            {
                _isFalling = false;
            }
            else 
            {
                RaycastHit hit;
                if(Physics.SphereCast(transform.position, 0.2f, Vector3.down, out hit)) 
                {
                    float difference = transform.position.y - hit.point.y;
                    bool flag = difference < _controller.height/2.0f + GROUND_TOLERANCE;

                    if(!flag) 
                    {
                        Vector3 grounding = Vector3.down * difference;
                        advance += grounding;
                    }

                    _isFalling = flag;
                }

                _isFalling = true;
            }

            if(_isFalling) 
            {
                _gravity += GRAVITY * Time.fixedDeltaTime * Time.fixedDeltaTime;
            }   
            else 
            {
                _gravity = 0.0f;
            }

            _controller.Move(advance);
        }

        #region Callable Methods

        public void SetMovement(Vector2 dir, float mag)
        {
            _moveDirection = dir;
            _moveMagnitude = mag;
        }

        public void SetRotation(Vector2 dir, float mag) 
        {
            _turnDirection = dir;
            _turnMagnitude = mag;
        }

        #endregion
    }
}
