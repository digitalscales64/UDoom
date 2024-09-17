using System;
using Game.Attribute;
using Game.Input.Core;
using UnityEngine;

namespace Game.Controller 
{
    [RequireComponent(typeof(CapsuleCollider), typeof(IMovementInput))]
    public class MovementController : MonoBehaviour, Core.IMovement
    {
        private const int MAX_NEIGHBOURS = 16;
        private const float GRAVITY = 9.8f;
        private const float EPSILON = 0.001f;
        private const int MAX_ITERATTIONS = 18;

        #region Serialized Fields

        [SerializeField] private MovementAttributes _attributes;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private LayerMask _layerMask;
        [SerializeField] private bool _useGravity;

        #endregion

        private Action<GameObject> _onCollision;
        private Action<bool> _onFalling;

        // Desired Movement
        private Vector3 _moveDirection;
        private float _moveMagnitude;

        // Desired Rotation
        private Vector3 _turnDirection;
        private float _turnMagnitude;

        // Collisions
        private Collider[] _neighbours;

        // Falling
        private float _gravity;

        void Start() 
        {
            IMovementInput movementInput = GetComponent<IMovementInput>();
            movementInput.AddMovementListener(SetMovement);

            IRotationInput rotationInput = GetComponent<IRotationInput>();
            rotationInput.AddRotationListener(SetRotation); 

            if(!_collider) 
            {
                _collider = GetComponent<CapsuleCollider>(); 
                string msg = "Capsule collider not set for MovementController";
                Debug.LogWarning(msg);
            }

            _neighbours = new Collider[MAX_NEIGHBOURS];
        }

        void FixedUpdate() 
        {
            Rotation();
            Movement();
        }

        protected virtual void Movement() 
        {
            // Desired Movement
            float impulse = _moveMagnitude * _attributes.MovementSpeed * Time.fixedDeltaTime;
            Vector3 advance = _moveDirection * impulse;

            // Gravity
            Vector3 gravity = Vector3.down * _gravity;

            int count = Physics.OverlapCapsuleNonAlloc
            (
                transform.position + Vector3.up * _collider.height/2.0f,
                transform.position - Vector3.up * _collider.height/2.0f,
                _collider.radius + 2 * EPSILON, _neighbours,
                _layerMask, QueryTriggerInteraction.Ignore 
            );

            bool isGrounded = false;
            Vector3 movement = transform.position + advance + gravity;

            for(int iteration = 0; iteration < MAX_ITERATTIONS; iteration++) 
            {
                // Depenetration
                Vector3 resolve = Vector3.zero;

                for(int i = 0; i < count; i++) 
                {
                    Collider other = _neighbours[i];
                    if(_collider == other) continue;

                    Vector3 otherPosition = other.gameObject.transform.position;
                    Quaternion otherRotation = other.gameObject.transform.rotation; 

                    _onCollision?.Invoke(other.gameObject);

                    Vector3 direction;
                    float distance;

                    bool overlapped = Physics.ComputePenetration
                    (
                    _collider, movement, transform.rotation,
                    other, otherPosition, otherRotation,
                    out direction, out distance
                    );

                    if(!overlapped) continue;

                    resolve += direction * (distance - EPSILON);

                    float dot = Vector3.Dot(Vector3.up, direction);
                    if(dot < 0.5f) continue;

                    RaycastHit hit;
                    if(Physics.Raycast(movement, -direction, out hit)) 
                    {
                        isGrounded = true;
                    }
                }

                movement += resolve;
            }

            // Grounding
            _onFalling?.Invoke(!isGrounded);

            if(!isGrounded) 
            {
                _gravity += GRAVITY * Time.fixedDeltaTime * Time.fixedDeltaTime;
            }
            else 
            {
                _gravity = 0.0f;
            }

            // Advance is the desired movement, and resolve is a vector stopping the advance
            transform.position = movement;
        }

        protected virtual void Rotation() 
        {
            float speed = _turnDirection.x * _turnMagnitude * _attributes.TurnSpeed * Time.deltaTime;
            Vector3 rot = new Vector3(0.0f, speed, 0.0f);
            
            transform.Rotate(rot);
        }

        public void AddFallingListener(Action<bool> eventHandler) 
        {
            _onFalling += eventHandler;
        }

        public void RemoveFallingListener(Action<bool> eventHandler) 
        {
            _onFalling -= eventHandler;
        }

        #region IMovement Implementation

        public void AddCollisionListener(Action<GameObject> eventHandler) 
        {
            _onCollision += eventHandler;
        }

        public void RemoveCollisionListener(Action<GameObject> eventHander) 
        {
            _onCollision -= eventHander;
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

        #endregion
    }
}
