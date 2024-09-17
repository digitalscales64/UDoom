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
        private const float EPSILON = 0.3f;

        #region Serialized Fields

        [SerializeField] private MovementAttributes _attributes;
        [SerializeField] private CapsuleCollider _collider;
        [SerializeField] private float _groundSphereRadius = 0.2f;
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
        private bool _onGround;
        private float _gravity;
        private float _grounding;

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

        void Movement() 
        {
            // Desired Movement
            float impulse = _moveMagnitude * _attributes.MovementSpeed * Time.fixedDeltaTime;
            Vector3 advance = _moveDirection * impulse + Vector3.down * _gravity;

            // Check gravity
            if(_useGravity) 
            {
                // Check grounding
                RaycastHit hit;
                Ray ray = new Ray(transform.position + advance, Vector3.down); 
                _onGround = Physics.SphereCast(ray, _groundSphereRadius, out hit, _collider.height/2.0f + EPSILON);

                if(!_onGround) 
                {
                    _grounding = 0.0f;
                    _gravity += GRAVITY * Time.fixedDeltaTime * Time.fixedDeltaTime;
                } else 
                {
                    _grounding = Mathf.Abs(transform.position.y - hit.point.y);
                    _gravity = 0.0f;
                }
            }
            else 
            {
                _gravity = 0.0f;
                _grounding = 0.0f;
            }

            //_falling = !groundFlag;
            _onFalling?.Invoke(!_onGround);

            int count = Physics.OverlapCapsuleNonAlloc
            (
                transform.position + Vector3.up * _collider.height/2.0f,
                transform.position - Vector3.up * _collider.height/2.0f,
                _collider.radius + EPSILON, _neighbours
            );

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
                _collider, transform.position + advance, transform.rotation,
                other, otherPosition, otherRotation,
                out direction, out distance
                );

                if(!overlapped) continue;
                resolve += direction * distance;
            }

            // Advance is the desired movement, and resolve is a vector stopping the advance
            transform.position += advance + resolve;
        }

        void Rotation() 
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
