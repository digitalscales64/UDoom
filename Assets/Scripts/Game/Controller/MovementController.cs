using Game.Attribute;
using UnityEngine;

namespace Game.Controller 
{
    public class MovementController : MonoBehaviour
    {
        private const float GRAVITY = -9.8f;
        private const float GROUND_TOLERANCE = 0.1f;
        private const float VERTICAL_TOLERANCE = 0.1f;
        private const float COYOTE_TIME = 0.1f;

        #region Serialized Fields

        [SerializeField] private MovementAttributes _attributes;
        [SerializeField] private CharacterController _controller;
        [SerializeField] private bool _useGravity;
        [SerializeField] private float vertical = 0.5f;

        #endregion

        // Coyote time
        private float _timeSinceLeftGround;

        // Desired Movement
        private Vector2 _moveDirection;
        private float _moveMagnitude;

        // Desired Rotation
        private Vector2 _turnDirection;
        private float _turnMagnitude;

        // gravity
        private float _vertical;
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
            Vector3 advance = fix * (_moveMagnitude * _attributes.MovementSpeed * Time.fixedDeltaTime) + Vector3.up * _vertical;

            Debug.Log(_controller.isGrounded);

            //
            RaycastHit hit; 
            if(Physics.Raycast(transform.position + advance, Vector3.down, out hit)) 
            {
                float distance = transform.position.y - hit.point.y;
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
                _timeSinceLeftGround += Time.fixedDeltaTime;
                _vertical += GRAVITY * Time.fixedDeltaTime * Time.fixedDeltaTime;
            }
            else 
            {
                _timeSinceLeftGround = 0.0f;
                _vertical = 0.0f;
            }

            _controller.Move(advance);
        }

        #region Callable Methods

        public void TriggerJump(bool value) 
        {
            if(value && (!_isFalling || _timeSinceLeftGround <= COYOTE_TIME)) 
            {
                _vertical += vertical;
            }
        }

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
