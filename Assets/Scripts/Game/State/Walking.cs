using System;
using System.Collections;
using System.Collections.Generic;
using Game.Attribute;
using UnityEngine;

namespace Game.State 
{
    public class WalkingStateNullException : ArgumentNullException 
    {
        public WalkingStateNullException(string arg) : base(arg) {}
    }

    public class Walking : Design.FSM.State
    {

        #region Constants

        private const float GRAVITY = -9.8f;
        private const float GROUND_TOLERANCE = 0.1f;
        private const float VERTICAL_TOLERANCE = 0.1f;

        #endregion

        // Dependencies
        private readonly MovementAttributes _attributes;
        private readonly CharacterController _controller;

        // Desired Movement
        private Vector2 _moveDirection;
        private float _moveMagnitude;

        // Desired Rotation
        private Vector2 _turnDirection;
        private float _turnMagnitude;

        // gravity
        private float _vertical;
        private bool _isFalling;

        #region Base State implementation

        public Walking(GameObject agent, MovementAttributes attributes) : base(agent)
        {
            _controller = agent.GetComponent<CharacterController>();
            if(!_controller) 
            {
                string msg = $"Error: Controller component is null";
                throw new WalkingStateNullException(msg);
            }

            _attributes = attributes;
            if(!_attributes) 
            {
                string msg = $"Error: Attributes is null";
                throw new WalkingStateNullException(msg);
            }

            // Remove, Should call a GameManager to request locking or not instead
            Cursor.lockState = CursorLockMode.Locked;
        }

        public override void Update()
        {
            Vector3 fix = Agent.transform.right * _moveDirection.x + Agent.transform.forward * _moveDirection.y;
            fix = new Vector3(fix.x, 0.0f, fix.z);

            // Turning 
            float speed = _turnDirection.x * _turnMagnitude * _attributes.TurnSpeed * Time.fixedDeltaTime;
            Vector3 rot = new Vector3(0.0f, speed, 0.0f);

            Agent.transform.Rotate(rot);

            // Movement
            Vector3 advance = fix * (_moveMagnitude * _attributes.MovementSpeed * Time.fixedDeltaTime) + Vector3.up * _vertical;

            // Grounding
            RaycastHit hit; 
            if(Physics.Raycast(Agent.transform.position + advance, Vector3.down, out hit)) 
            {
                float distance = Agent.transform.position.y - hit.point.y;
                if(distance <= _controller.height + GROUND_TOLERANCE) 
                {
                    if(_vertical <= VERTICAL_TOLERANCE) 
                    {
                        _isFalling = false;
                        advance += Vector3.down * (distance - _controller.height/2.0f);
                    }
                    else 
                    {
                        _isFalling = true;
                    }
                }
                else 
                {
                    _isFalling = true;
                }
            }
            else 
            {
                _isFalling = true;
            }

            // falling
            if(_isFalling) 
            {
                _vertical += GRAVITY * Time.fixedDeltaTime * Time.fixedDeltaTime;
            }
            else 
            {
                _vertical = 0.0f;
            }

            _controller.Move(advance);
        }

        public override void Exit()
        {
            _vertical = 0.0f;
        }

        #endregion

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
