using System;
using Game.Attribute;
using Game.Input.Core;
using UnityEngine;

namespace Game.Controller 
{
    public class MovementController : MonoBehaviour
    {
        private const float GRAVITY = 9.8f;
        private const float TOLERANCE = 0.01f;

        #region Serialized Fields

        [SerializeField] private MovementAttributes _attributes;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private bool _useGravity;

        #endregion

        // Desired Movement
        private Vector3 _moveDirection;
        private float _moveMagnitude;

        // Desired Rotation
        private Vector3 _turnDirection;
        private float _turnMagnitude;

        // gravity
        private float _gravity;
        private bool _falling;

        void Start() 
        {
            IMovementInput movementInput = GetComponent<IMovementInput>();
            movementInput.AddMovementListener(SetMovement);

            IRotationInput rotationInput = GetComponent<IRotationInput>();
            rotationInput.AddRotationListener(SetRotation); 

            if(!_controller) 
            {
                _controller = GetComponent<CharacterController>(); 
                string msg = "No CharacterController present";
                Debug.LogWarning(msg);
            }
        }

        void FixedUpdate() 
        {
            Rotation();
            Movement();
        }

        void Movement() 
        {
            // Desired Movement
            Vector3 advance = _moveDirection * (_moveMagnitude * _attributes.MovementSpeed * Time.fixedDeltaTime) + Vector3.down * _gravity;
            
            if(_controller.isGrounded) 
            {
                _falling = false;
            }
            else 
            {
                RaycastHit hit;
                Vector3 next = transform.position + advance;
                if(Physics.SphereCast(next, 2.0f, Vector3.down, out hit)) 
                {
                    float distance = next.y - hit.point.y;
                    
                    if(distance <= _controller.height/2.0f + TOLERANCE) 
                    {
                        Vector3 down = Vector3.down * distance;
                        advance += down;

                        _falling = false;
                    }
                }
                else 
                {
                    _falling = true;
                }
            }

            if(_falling) 
            {
                _gravity += GRAVITY * Time.fixedDeltaTime * Time.fixedDeltaTime;
            }
            else 
            {   
                _gravity = 0.0f;
            }

            _controller.Move(advance);
        }

        void Rotation() 
        {
            float speed = _turnDirection.x * _turnMagnitude * _attributes.TurnSpeed * Time.deltaTime;
            Vector3 rot = new Vector3(0.0f, speed, 0.0f);
            
            transform.Rotate(rot);
        }

        public void SetMovement(Vector3 dir, float mag)
        {
            _moveDirection = dir;
            _moveMagnitude = mag;
        }

        public void SetRotation(Vector3 dir, float mag) 
        {
            _turnDirection = dir;
            _turnMagnitude = mag;
        }
    }
}
